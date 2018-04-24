public static class Barcodes
    {
        #region EAN13Definitions
        public static string Ean13Start { get; set; }
        public static string Ean13End { get; set; }
        public static string Ean13Middle { get; set; }
        public static Dictionary<int, string> EncodeSet { get; set; }
        public static Dictionary<string, string> TypeA { get; set; }
        public static Dictionary<string, string> TypeB { get; set; }
        public static Dictionary<string, string> TypeC { get; set; }
        #endregion

        #region EAN128Definitions
        public static string Ean128StartString { get; set; }
        public static string Ean128EndString { get; set; }
        public static int Ean128SumStart { get; set; }
        #endregion

        static Barcodes()
        {
            InitEan13();
            InitEan128();
        }

        #region EAN13Methods
        private static void InitEan13()
        {
            Ean13Start = "*";
            Ean13End = "*";
            Ean13Middle = "#";
            TypeA = new Dictionary<string, string>
            {
                { "1", "1" },
                { "2", "2" },
                { "3", "3" },
                { "4", "4" },
                { "5", "5" },
                { "6", "6" },
                { "7", "7" },
                { "8", "8" },
                { "9", "9" },
                { "0", "0" }
            };

            TypeB = new Dictionary<string, string>
            {
                { "1", "a" },
                { "2", "s" },
                { "3", "d" },
                { "4", "f" },
                { "5", "g" },
                { "6", "h" },
                { "7", "j" },
                { "8", "k" },
                { "9", "l" },
                { "0", ";" }
            };

            TypeC = new Dictionary<string, string>
            {
                { "1", "q" },
                { "2", "w" },
                { "3", "e" },
                { "4", "r" },
                { "5", "t" },
                { "6", "y" },
                { "7", "u" },
                { "8", "i" },
                { "9", "o" },
                { "0", "p" }
            };

            EncodeSet = new Dictionary<int, string>
            {
                { 0, "AAAAAA" },
                { 1, "AABABB" },
                { 2, "AABBAB" },
                { 3, "AABBBA" },
                { 4, "ABAABB" },
                { 5, "ABBAAB" },
                { 6, "ABBBAA" },
                { 7, "ABABAB" },
                { 8, "ABABBA" },
                { 9, "ABBABA" }
            };


        }
        
        //Calculates the Checkdigit
        public static int Ean13CheckDigit(string barcode)
        {
            if (barcode.Length != 12 && barcode.Length != 13)
                return -1;

            int sum = 0;

            for (int i = 11; i >= 0; i--)
            {
                int digit = barcode[i] - 0x30;
                if (i % 2 == 0)
                    sum += digit;
                else
                    sum += digit * 3;
            }
            int mod = sum % 10;
            return mod == 0 ? 0 : 10 - mod;
        }

        //Returns the Code + Checkdigit
        public static string Ean13Complete(string barcode)
        {
            return barcode + Ean13CheckDigit(barcode).ToString();
        }

        //Encodes the left side of the code using Type A or B, depending on the control digit (First digit of the code)
        public static string EncodeLeft(int control, string data)
        {
            string encoded = "";
            for(int i = 0; i < data.Length; i++)
            {
                switch(EncodeSet[control][i])
                {
                    case 'A':
                        encoded += TypeA[data[i].ToString()];
                        break;
                    case 'B':
                        encoded += TypeB[data[i].ToString()];
                        break;
                }
            }
            return encoded;
        }

        //Encodes the right side of the code using Type C
        public static string EncodeRight(string data)
        {
            string encoded = "";
            for(int i = 0; i < data.Length; i++)
                encoded += TypeC[data[i].ToString()].ToString();
            
            return encoded;
        }

        //Returns a completely encoded EAN13 from a length 12 code
        public static string Ean13(string barcode)
        {
            string completeCode = Ean13Complete(barcode);
            int control = int.Parse(completeCode.Substring(0, 1));
            string ean = Ean13Start;
            ean += EncodeLeft(control, completeCode.Substring(1, 6));
            ean += Ean13Middle;
            ean += EncodeRight(completeCode.Substring(7));
            ean += Ean13End;
            return ean;
        }
        #endregion

        #region EAN128Methods
        private static void InitEan128()
        {
            Ean128StartString = ((char)204).ToString();
            Ean128EndString = ((char)206).ToString();
            Ean128SumStart = 104;
        }

        //Returns a completely encoded EAN128 using type A
        public static string Ean128(string data)
        {
            string ean = Ean128StartString;
            int cSum = Ean128SumStart;
            char Checksum = ' ';
            for(int i = 0; i < data.Length; i++)
            {
                var currChar = data[i];
                ean += currChar;
                var cVal = currChar - 32;
                cSum += cVal * (i + 1);
            }
            Checksum = (char) ((cSum % 103) + 32);
            ean += Checksum;
            ean += Ean128EndString;
            return ean;
        }
        #endregion  
    }
