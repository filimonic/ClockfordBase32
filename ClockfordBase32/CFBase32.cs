using System;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace ClockfordBase32
{

    public static class CFBase32
    {
        /*
         * Conversion of byte array to Base32 using Douglas Clockford's (https://www.crockford.com/) conversion table
         * Read more: https://en.wikipedia.org/wiki/Base32#Crockford's_Base32 
         * 
         * Each output character represents 5 bits of byte array passed as input.
         * This means, each 5 bytes are converted to 8 characters.
         * The output is ~160% longer than input
         * 
         * In this implementation, padding symbols are omitted.
         * Because of padding symbols are omitted, length of the encoded string should be
         * multiple of 8 symbols plus 2, 4, 5 or 7 symbols.

         * Table of bits per symbol while encoding
         *    0 1 2 3 4 5 6 7
         * 0: 0 0 0 A A A A A
         * 1: 0 0 0 A A A B B
         * 2: 0 0 0 B B B B B
         * 3: 0 0 0 B C C C C
         * 4: 0 0 0 C C C C D
         * 5: 0 0 0 D D D D D
         * 6: 0 0 0 D D E E E
         * 7: 0 0 0 E E E E E

         * Table of bits per symbol while decoding
         * letter represents one of five bits in a byte
         *    0 1 2 3 4 5 6 7
         * 0: A A A A A B B B 
         * 1: B B C C C C C D
         * 2: D D D D E E E E
         * 3: E F F F F F G G
         * 4: G G G H H H H H

         */


        internal static readonly char[] clockfordBase32Table_fwd = { '0', '1', '2', '3', '4', '5', '6', '7',
                                                                     '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
                                                                     'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q',
                                                                     'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z', '=' };

        internal static readonly byte[] clockfordBase32Table_rev = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,   // 0, 1, 2, 3, 4, 5, 6, 7, 
                                                                     0x08, 0x09, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // 8, 9, _, _, _, _, _, _, 
                                                                     0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10,   // _, A, B, C, D, E, F, G, 
                                                                     0x11, 0x01, 0x12, 0x13, 0x01, 0x14, 0x15, 0x00,   // H, I, J, K, L, M, N, O, 
                                                                     0x16, 0x17, 0x18, 0x19, 0x1a, 0xff, 0x1b, 0x1c,   // P, Q, R, S, T, _, V, W, 
                                                                     0x1d, 0x1e, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff,   // X, Y, Z, _, _, _, _, _, 
                                                                     0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10,   // _, a, b, c, d, e, f, g, 
                                                                     0x11, 0x01, 0x12, 0x13, 0x01, 0x14, 0x15, 0x00,   // h, i, j, k, l, m, n, o, 
                                                                     0x16, 0x17, 0x18, 0x19, 0x1a, 0xff, 0x1b, 0x1c,   // p, q, r, s, t, _, v, w, 
                                                                     0x1d, 0x1e, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff,   // x, y, z, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,   // _, _, _, _, _, _, _, _, 
                                                                     0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }; // _, _, _, _, _, _, _, _

        [System.Runtime.InteropServices.ComVisible(false)]
        public static String ToBase32String(byte[] inArray)
        {
            if (inArray == null)
            {
                throw new ArgumentNullException("inArray");
            }
            Contract.Ensures(Contract.Result<string>() != null);
            Contract.EndContractBlock();
            return ToBase32String(inArray, 0, inArray.Length);
        }


        /*
         * Special method for GUID for performance
         */
        [System.Runtime.InteropServices.ComVisible(false)]
        public unsafe static String ToBase32String(Guid inGuid)
        {
            if (inGuid == null)
            {
                throw new ArgumentNullException("inGuid");
            }
            Contract.Ensures(Contract.Result<string>() != null);
            Contract.EndContractBlock();

            string returnString = new string('\0', 26);
            fixed (char* outChars = returnString)
            {
                fixed (byte* inData = inGuid.ToByteArray())
                {
                    fixed (char* base32 = clockfordBase32Table_fwd)
                    {
                        outChars[00] = base32[(inData[00] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[01] = base32[((inData[00] & 0x07) << 2) | ((inData[01] & 0xC0) >> 6)]; // 0x07 == 0b0000_0111; 0xC0 == 0b1100_0000
                        outChars[02] = base32[(inData[01] & 0x3E) >> 1]; // 0x3E == 0b0011_1110
                        outChars[03] = base32[((inData[01] & 0x01) << 4) | ((inData[02] & 0xF0) >> 4)]; // 0x01 == 0b0000_0001; 0xF0 == 0b1111_0000
                        outChars[04] = base32[((inData[02] & 0x0F) << 1) | ((inData[03] & 0x80) >> 7)]; // 0x0F == 0b0000_1111; 0x80 == 0b1000_0000
                        outChars[05] = base32[(inData[03] & 0x7C) >> 2]; // 0x7C == 0b0111_1100
                        outChars[06] = base32[((inData[03] & 0x03) << 3) | ((inData[04] & 0xE0) >> 5)]; // 0x03 == 0b0000_0011; 0xE0 == 0b1110_0000
                        outChars[07] = base32[(inData[04] & 0x1F)]; // 0x1F == 0b0001_1111

                        outChars[08] = base32[(inData[05] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[09] = base32[((inData[05] & 0x07) << 2) | ((inData[06] & 0xC0) >> 6)]; // 0x07 == 0b0000_0111; 0xC0 == 0b1100_0000
                        outChars[10] = base32[(inData[06] & 0x3E) >> 1]; // 0x3E == 0b0011_1110
                        outChars[11] = base32[((inData[06] & 0x01) << 4) | ((inData[07] & 0xF0) >> 4)]; // 0x01 == 0b0000_0001; 0xF0 == 0b1111_0000
                        outChars[12] = base32[((inData[07] & 0x0F) << 1) | ((inData[08] & 0x80) >> 7)]; // 0x0F == 0b0000_1111; 0x80 == 0b1000_0000
                        outChars[13] = base32[(inData[08] & 0x7C) >> 2]; // 0x7C == 0b0111_1100
                        outChars[14] = base32[((inData[08] & 0x03) << 3) | ((inData[09] & 0xE0) >> 5)]; // 0x03 == 0b0000_0011; 0xE0 == 0b1110_0000
                        outChars[15] = base32[(inData[09] & 0x1F)]; // 0x1F == 0b0001_1111

                        outChars[16] = base32[(inData[10] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[17] = base32[((inData[10] & 0x07) << 2) | ((inData[11] & 0xC0) >> 6)]; // 0x07 == 0b0000_0111; 0xC0 == 0b1100_0000
                        outChars[18] = base32[(inData[11] & 0x3E) >> 1]; // 0x3E == 0b0011_1110
                        outChars[19] = base32[((inData[11] & 0x01) << 4) | ((inData[12] & 0xF0) >> 4)]; // 0x01 == 0b0000_0001; 0xF0 == 0b1111_0000
                        outChars[20] = base32[((inData[12] & 0x0F) << 1) | ((inData[13] & 0x80) >> 7)]; // 0x0F == 0b0000_1111; 0x80 == 0b1000_0000
                        outChars[21] = base32[(inData[13] & 0x7C) >> 2]; // 0x7C == 0b0111_1100
                        outChars[22] = base32[((inData[13] & 0x03) << 3) | ((inData[14] & 0xE0) >> 5)]; // 0x03 == 0b0000_0011; 0xE0 == 0b1110_0000
                        outChars[23] = base32[(inData[14] & 0x1F)]; // 0x1F == 0b0001_1111

                        outChars[24] = base32[(inData[15] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[25] = base32[((inData[15] & 0x07) << 2)]; // 0x07 == 0b0000_0111
                    }
                }
            }
            return returnString;
        }


        [System.Security.SecuritySafeCritical]  // auto-generated
        [System.Runtime.InteropServices.ComVisible(false)]
        public static unsafe String ToBase32String(byte[] inArray, int offset, int length)
        {
            //Do data verfication
            if (inArray == null)
                throw new ArgumentNullException("inArray");
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            Contract.Ensures(Contract.Result<string>() != null);
            Contract.EndContractBlock();

            int inArrayLength;
            int stringLength;

            inArrayLength = inArray.Length;
            if (offset > (inArrayLength - length))
                throw new ArgumentOutOfRangeException("offset");

            if (inArrayLength == 0)
                return String.Empty;

            //Create the new string.  This is the maximally required length.
            stringLength = ToBase32_CalculateAndValidateOutputLength(length);

            // TODO : Make usage of internal FastAllocateString(length)
            string returnString = new string('\0', stringLength);
            fixed (char* outChars = returnString)
            {
                fixed (byte* inData = inArray)
                {
                    int j = ConvertToBase32Array(outChars, inData, offset, length);
                    return returnString;
                }
            }
        }


        [System.Security.SecuritySafeCritical]  // auto-generated
        [System.Runtime.InteropServices.ComVisible(false)]
        public static unsafe int ToBase32CharArray(byte[] inArray, int offsetIn, int length, char[] outArray, int offsetOut)
        {
            //Do data verfication
            if (inArray == null)
                throw new ArgumentNullException("inArray");
            if (outArray == null)
                throw new ArgumentNullException("outArray");
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            if (offsetIn < 0)
                throw new ArgumentOutOfRangeException("offsetIn");
            if (offsetOut < 0)
                throw new ArgumentOutOfRangeException("offsetOut");

            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() <= outArray.Length);
            Contract.EndContractBlock();

            int retVal;

            int inArrayLength;
            int outArrayLength;
            int numElementsToCopy;

            inArrayLength = inArray.Length;

            if (offsetIn > (int)(inArrayLength - length))
                throw new ArgumentOutOfRangeException("offsetIn");

            if (inArrayLength == 0)
                return 0;

            //This is the maximally required length that must be available in the char array
            outArrayLength = outArray.Length;

            // Length of the char buffer required
            numElementsToCopy = ToBase32_CalculateAndValidateOutputLength(length);

            if (offsetOut > (int)(outArrayLength - numElementsToCopy))
                throw new ArgumentOutOfRangeException("offsetOut");

            fixed (char* outChars = &outArray[offsetOut])
            {
                fixed (byte* inData = inArray)
                {
                    retVal = ConvertToBase32Array(outChars, inData, offsetIn, length);
                }
            }

            return retVal;
        }

        /*
         * Calculates number of Base32 symbols needed to represent inputLength bytes.
         * Calculations: length is 
         */
        private unsafe static int ToBase32_CalculateAndValidateOutputLength(int inputLength)
        {
            if (inputLength == 0) return 0;

            // calculate number of chars required to encode bytes.
            long outlen = ((long)inputLength) / 5 * 8;
            switch (inputLength % 5)
            {
                case 1:
                    outlen += 2; break;
                case 2:
                    outlen += 4; break;
                case 3:
                    outlen += 5; break;
                case 4:
                    outlen += 7; break;
            }

            // If we overflow an int then we cannot allocate enough
            // memory to output the value so throw
            if (outlen > int.MaxValue)
                throw new OutOfMemoryException();

            return (int)outlen;
        }


        [System.Security.SecurityCritical]  // auto-generated
        private static unsafe int ConvertToBase32Array(char* outChars, byte* inData, int offset, int length)
        {
            int lengthmod5 = length % 5;
            int calcLength = offset + (length - lengthmod5);
            int j = 0;

            //Convert five bytes at a time to base32 notation.  This will consume 8 chars.
            int i;
            Debug.WriteLine("calcLength", calcLength);
            Debug.WriteLine("lengthmod5", lengthmod5);
            fixed (char* base32 = clockfordBase32Table_fwd)
            {
                for (i = offset; i < calcLength; i += 5)
                {

                    outChars[j + 0] = base32[(inData[i + 0] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                    outChars[j + 1] = base32[((inData[i + 0] & 0x07) << 2) | ((inData[i + 1] & 0xC0) >> 6)]; // 0x07 == 0b0000_0111; 0xC0 == 0b1100_0000
                    outChars[j + 2] = base32[(inData[i + 1] & 0x3E) >> 1]; // 0x3E == 0b0011_1110
                    outChars[j + 3] = base32[((inData[i + 1] & 0x01) << 4) | ((inData[i + 2] & 0xF0) >> 4)]; // 0x01 == 0b0000_0001; 0xF0 == 0b1111_0000
                    outChars[j + 4] = base32[((inData[i + 2] & 0x0F) << 1) | ((inData[i + 3] & 0x80) >> 7)]; // 0x0F == 0b0000_1111; 0x80 == 0b1000_0000
                    outChars[j + 5] = base32[(inData[i + 3] & 0x7C) >> 2]; // 0x7C == 0b0111_1100
                    outChars[j + 6] = base32[((inData[i + 3] & 0x03) << 3) | ((inData[i + 4] & 0xE0) >> 5)]; // 0x03 == 0b0000_0011; 0xE0 == 0b1110_0000
                    outChars[j + 7] = base32[(inData[i + 4] & 0x1F)]; // 0x1F == 0b0001_1111
                    j += 8;
                }

                switch (lengthmod5)
                {
                    case 4:
                        outChars[j + 0] = base32[(inData[i + 0] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[j + 1] = base32[((inData[i + 0] & 0x07) << 2) | ((inData[i + 1] & 0xC0) >> 6)]; // 0x07 == 0b0000_0111; 0xC0 == 0b1100_0000
                        outChars[j + 2] = base32[(inData[i + 1] & 0x3E) >> 1]; // 0x3E == 0b0011_1110
                        outChars[j + 3] = base32[((inData[i + 1] & 0x01) << 4) | ((inData[i + 2] & 0xF0) >> 4)]; // 0x01 == 0b0000_0001; 0xF0 == 0b1111_0000
                        outChars[j + 4] = base32[((inData[i + 2] & 0x0F) << 1) | ((inData[i + 3] & 0x80) >> 7)]; // 0x0F == 0b0000_1111; 0x80 == 0b1000_0000
                        outChars[j + 5] = base32[(inData[i + 3] & 0x7C) >> 2]; // 0x7C == 0b0111_1100
                        outChars[j + 6] = base32[((inData[i + 3] & 0x03) << 3)]; // 0x03 == 0b0000_0011; 0xE0 == 0b1110_0000
                        //outChars[j + 7] = base32[32]; // Pad
                        j += 7;
                        break;
                    case 3: // three padding characters needed
                        outChars[j + 0] = base32[(inData[i + 0] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[j + 1] = base32[((inData[i + 0] & 0x07) << 2) | ((inData[i + 1] & 0xC0) >> 6)]; // 0x07 == 0b0000_0111; 0xC0 == 0b1100_0000
                        outChars[j + 2] = base32[(inData[i + 1] & 0x3E) >> 1]; // 0x3E == 0b0011_1110
                        outChars[j + 3] = base32[((inData[i + 1] & 0x01) << 4) | ((inData[i + 2] & 0xF0) >> 4)]; // 0x01 == 0b0000_0001; 0xF0 == 0b1111_0000
                        outChars[j + 4] = base32[((inData[i + 2] & 0x0F) << 1)]; // 0x0F == 0b0000_1111; 0x80 == 0b1000_0000
                        //outChars[j + 5] = base32[32]; // Pad
                        //outChars[j + 6] = base32[32]; // Pad
                        //outChars[j + 7] = base32[32]; // Pad
                        j += 5;
                        break;
                    case 2: // four padding characters needed
                        outChars[j + 0] = base32[(inData[i + 0] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[j + 1] = base32[((inData[i + 0] & 0x07) << 2) | ((inData[i + 1] & 0xC0) >> 6)]; // 0x07 == 0b0000_0111; 0xC0 == 0b1100_0000
                        outChars[j + 2] = base32[(inData[i + 1] & 0x3E) >> 1]; // 0x3E == 0b0011_1110
                        outChars[j + 3] = base32[((inData[i + 1] & 0x01) << 4)]; // 0x01 == 0b0000_0001
                        //outChars[j + 4] = base32[32]; // Pad
                        //outChars[j + 5] = base32[32]; // Pad
                        //outChars[j + 6] = base32[32]; // Pad
                        //outChars[j + 7] = base32[32]; // Pad
                        j += 4;
                        break;
                    case 1: // six padding characters needed
                        outChars[j + 0] = base32[(inData[i + 0] & 0xF8) >> 3]; // 0xf8 == 0b1111_1000
                        outChars[j + 1] = base32[((inData[i + 0] & 0x07) << 2)]; // 0x07 == 0b0000_0111
                        //outChars[j + 2] = base32[32]; // Pad
                        //outChars[j + 3] = base32[32]; // Pad
                        //outChars[j + 4] = base32[32]; // Pad
                        //outChars[j + 5] = base32[32]; // Pad
                        //outChars[j + 6] = base32[32]; // Pad
                        //outChars[j + 7] = base32[32]; // Pad
                        j += 2;
                        break;
                }
            }
            return j;
        }

        public static Guid GuidFromBase32String(String encodedGuid)
        {
            return new Guid(FromBase32String(encodedGuid));
        }

        [System.Security.SecuritySafeCritical]
        public static Byte[] FromBase32String(String s)
        {

            // "s" is an unfortunate parameter name, but we need to keep it for backward compat.

            if (s == null)
                throw new ArgumentNullException("s");

            Contract.EndContractBlock();

            unsafe
            {
                fixed (Char* sPtr = s)
                {

                    return FromBase32CharPtr(sPtr, s.Length);
                }
            }
        }

        [System.Security.SecurityCritical]
        private static unsafe Byte[] FromBase32CharPtr(Char* inputPtr, Int32 inputLength)
        {

            // The validity of parameters much be checked by callers, thus we are Critical here.

            Contract.Assert(0 <= inputLength);

            // We need to get rid of any trailing white spaces.
            // Otherwise we would be rejecting input such as "abc= ":
            while (inputLength > 0)
            {
                Int32 lastChar = inputPtr[inputLength - 1];
                if (lastChar != (Int32)' ' && lastChar != (Int32)'\n' && lastChar != (Int32)'\r' && lastChar != (Int32)'\t' && lastChar != (Int32)'=')
                    break;
                inputLength--;
            }

            // Compute the output length:
            Int32 resultLength = ( inputLength / 8  )* 5;
            switch (inputLength % 8)
            {
                case 7:
                    resultLength += 4;
                    break;
                case 5:
                    resultLength += 3;
                    break;
                case 4:
                    resultLength += 2;
                    break;
                case 2:
                    resultLength += 1;
                    break;
                case 0:
                    break;
                default:
                    throw new ArgumentException("inputPtr");
            }

            Contract.Assert(0 <= resultLength);

            // resultLength can be zero. We will still enter FromBase64_Decode and process the input.
            // It may either simply write no bytes (e.g. input = " ") or throw (e.g. input = "ab").

            // Create result byte blob:
            Byte[] decodedBytes = new Byte[resultLength];

            // Convert Base64 chars into bytes:
            Int32 actualResultLength;
            fixed (Byte* decodedBytesPtr = decodedBytes)
                actualResultLength = FromBase32_Decode(inputPtr, inputLength, decodedBytesPtr, resultLength);

            // Note that actualResultLength can differ from resultLength if the caller is modifying the array
            // as it is being converted. Silently ignore the failure.
            // Consider throwing exception in an non in-place release.

            // We are done:
            return decodedBytes;
        }

        [System.Security.SecurityCritical]
        private static unsafe Int32 FromBase32_Decode(Char* startInputPtr, Int32 inputLength, Byte* startDestPtr, Int32 destLength)
        {
            int loopCnt = inputLength / 8;
            int tailSize = inputLength % 8;
            //UInt64 curBlockIn = 0x00;
            //UInt64 blockTest = 16204198715729174752; // 1110 0000 1110 0000 1110 0000 1110 0000 1110 0000 1110 0000 1110 0000 1110 0000
            byte[] curBlockOut = new byte[8];
            Char* inputPtr = startInputPtr;
            byte* destPtr = startDestPtr;
            int j = 0;


            fixed (byte* cbOut = curBlockOut)
            {
                fixed (byte* base32rev = clockfordBase32Table_rev)
                {
                    for (int i = 0; i < loopCnt; i++)
                    {
                        /*
                        curBlockIn = (UInt64)(*inputPtr);
                        if (((UInt64)cbOut & blockTest) > 0)
                        {
                            throw new ArgumentException("inputB");
                        }
                        */
                        for (j = 0; j < 8; j++)
                        {
                            cbOut[j] = base32rev[(byte)(*(inputPtr + j))];
                            // 0xE0 is 1110 0000. 
                            // If we have any symbol greater that hits one of first 3 bits, we have unsupported symbol.
                            if ((cbOut[j] & 0xE0) != 0)
                            {
                                throw new ArgumentException("input_symbol");
                            }
                        }

                        // BYTE0 : 000AAAAA | 000BBBbb. 
                        // A shifted left 3 bits.
                        // B shifted right 2 bits.
                        // AAAAA000 | 00000BBB
                        *(destPtr + 0) = (byte)((byte)(cbOut[0] << 3) | (byte)(cbOut[1] >> 2));
                        // BYTE1 : 000bbbBB | 000CCCCC | 000Ddddd
                        // B shifted left 6 bits
                        // C shifted left 1 bit
                        // D shifted right 4 bits
                        // BB000000 | 00CCCCC0 | 0000000D
                        *(destPtr + 1) = (byte)((byte)(cbOut[1] << 6) | (byte)(cbOut[2] << 1) | (byte)(cbOut[3] >> 4));
                        // BYTE2 : 000dDDDD | 000EEEEe
                        // D shifted left 4 buts
                        // E shifted right 1 bit
                        // DDDD0000 | 0000EEEE
                        *(destPtr + 2) = (byte)((byte)(cbOut[3] << 4) | (cbOut[4] >> 1));
                        // BYTE3 : 000eeeeE | 000FFFFF | 000GGggg
                        // E shifted left 7 bits
                        // F shifted left 2 bits
                        // G shifted right 3 bits
                        // E0000000 | 0FFFFF00 | 000000GG
                        *(destPtr + 3) = (byte)((byte)(cbOut[4] << 7) | (byte)(cbOut[5] << 2) | (byte)(cbOut[6] >> 3));
                        // BYTE3 : 000ggGGG | 000HHHHH
                        // G shifted left 5 bits
                        // H used as-is
                        // GGG00000 | 000HHHHH
                        *(destPtr + 4) = (byte)((byte)(cbOut[6] << 5) | (byte)(cbOut[7]));
                        destPtr += 5;
                        inputPtr += 8;
                    }

                    for (j = 0; j < tailSize; j++)
                    {
                        cbOut[j] = base32rev[(byte)(*(inputPtr + j))];
                        // 0xE0 is 1110 0000. 
                        // If we have any symbol greater that hits one of first 3 bits, we have unsupported symbol.
                        if ((cbOut[j] & 0xE0) != 0)
                        {
                            throw new ArgumentException("input_symbol");
                        }
                    }

                    switch (tailSize)
                    {
                        case 7: // When tail is 7, we decode 4 bytes
                            *(destPtr + 0) = (byte)((byte)(cbOut[0] << 3) | (byte)(cbOut[1] >> 2));
                            *(destPtr + 1) = (byte)((byte)(cbOut[1] << 6) | (byte)(cbOut[2] << 1) | (byte)(cbOut[3] >> 4));
                            *(destPtr + 2) = (byte)((byte)(cbOut[3] << 4) | (cbOut[4] >> 1));
                            *(destPtr + 3) = (byte)((byte)(cbOut[4] << 7) | (byte)(cbOut[5] << 2) | (byte)(cbOut[6] >> 3));
                            destPtr += 4;
                            inputPtr += 7;
                            break;
                        case 5: // When tail is 5, we decode 3 bytes
                            *(destPtr + 0) = (byte)((byte)(cbOut[0] << 3) | (byte)(cbOut[1] >> 2));
                            *(destPtr + 1) = (byte)((byte)(cbOut[1] << 6) | (byte)(cbOut[2] << 1) | (byte)(cbOut[3] >> 4));
                            *(destPtr + 2) = (byte)((byte)(cbOut[3] << 4) | (cbOut[4] >> 1));
                            destPtr += 3;
                            inputPtr += 5;
                            break;
                        case 4: // When tail is 4, we decode 2 bytes
                            *(destPtr + 0) = (byte)((byte)(cbOut[0] << 3) | (byte)(cbOut[1] >> 2));
                            *(destPtr + 1) = (byte)((byte)(cbOut[1] << 6) | (byte)(cbOut[2] << 1) | (byte)(cbOut[3] >> 4));
                            destPtr += 2;
                            inputPtr += 4;
                            break;
                        case 2: // When tail is 2, we decode 1 byte
                            *(destPtr + 0) = (byte)((byte)(cbOut[0] << 3) | (byte)(cbOut[1] >> 2));
                            destPtr += 1;
                            inputPtr += 2;
                            break;
                        case 0: // When tail is 0, we decode 0 bytes
                            break;

                        default: // When tail is not 7, 5, 4 or 2, we have unexpected length.
                            throw new ArgumentException("inputA");
                    }
                    return (Int32)(destPtr - startDestPtr);
                }
            }
        }
    }
}
