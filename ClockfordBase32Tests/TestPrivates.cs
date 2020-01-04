using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClockfordBase32;
namespace ClockfordBase32Tests
{
    [TestClass]
    public class TestPrivates
    {

        [TestMethod]
        public void Test_ByteArrayToBase32String()
        {
            byte[] test_phase1 = new byte[256];
            byte[] test_phase2 = new byte[256];
            for (int i = 0; i <= 255; i++)
            {
                test_phase1[i] = (byte)i;
                test_phase2[255 - i] = (byte)i;
            }

            string exp_test_phase1 = @"000G40R40M30E209185GR38E1W8124GK2GAHC5RR34D1P70X3RFJ08924CJ2A9H750MJMASC5MQ2YC1H68SK8D9P6WW3JEHV7GYKWFT085146H258S3MGJAA9D64TKJFA18N4MTMANB5EP2SB9DNRQAYBXG62RK3CHJPCSV8D5N6PV3DDSQQ0WBJEDT7AXKQF1WQMYVWFNZ7Z041GA1R91C6GY48K2MBHJ6RX3WGJ699754NJTBSH6CTKEE9V7MZM2GT58X4MPKAFA59NANTSBDENYRB3CNKPJTVDDXRQ6XBQF5XQTZW1GE2RF2CBHP7S34WNJYCSQ7CZM6HTB9X9NEPTZCDKPPVVKEXXQZ0W7HE7S75WVKYHTFAXFPEVVQFY3RZ5WZMYQVFFY7SZBXZSZFYZW";
            string exp_test_phase2 = @"ZZZFVZ7VZBWZHXZPYQTF7WQHY3QYXVFCXFNEKT77WVJY9RZ2W7GDZQPXVKDXNPERTZBDBN6KTB8X1KYESQ6CQJP9S33WDHE4RF1C3G5ZQTYVSEXTQ6WBFDNNPJSV5CDGNYQAVB5BNAMTH9X6MPJA78N1M2FSX7CWKED9K64QJTAS94WJJ688Z3MDHJ5RN2C8GY38B143GA0R0ZVYFNY7PYKSF1VQCXBMEDS72W3FDSPPRTVAD5M6ESK5CHHP4RB0BXF5TQ2VB9CNGNTPANA56MJHA17MWKAC9D54JJ278S2M8GT28503YFHX7GXKME9R6WV3AD1K68RK0BSE5MP2PAH950KJC9944CH2280Z3REHR6RT34C1E5GN2G9H448G1W70T30B184GG1R60M2060G100";

            Assert.AreEqual("91JPRV3F5GG7EVVJDHJ0", CFBase32.ToBase32String(System.Text.Encoding.UTF8.GetBytes("Hello, world")));
            Assert.AreEqual("RARG", CFBase32.ToBase32String(System.Text.Encoding.UTF8.GetBytes("±")));
            Assert.AreEqual(exp_test_phase1, CFBase32.ToBase32String(test_phase1));
            Assert.AreEqual(exp_test_phase2, CFBase32.ToBase32String(test_phase2));
        }

        [TestMethod]
        public void Test_Guid()
        {
            for (int i = 1000; i < 0; i--)
            {
                Guid g = Guid.NewGuid();
                Assert.AreEqual(g, CFBase32.GuidFromBase32String(CFBase32.ToBase32String(inGuid: g)));
            }
        }

        [TestMethod]
        public void Test_AlternativeChars()
        {
            // Here we test alternative chars.
            // In Clockford's base32 encoded string, 
            // * char '0' can be replaced with 'o' and 'O' chars
            // * char '1' can be replaced with 'i', 'I', 'l' and 'L' chars
            // Also any alpha char can be lowercase or uppercase. Case does not affects decoding result.

            // We should have those bytes on every iteration
            // "hj" is encoded to D1N0.
            byte[] correct_s = System.Text.Encoding.UTF8.GetBytes("hj");

            // D1N0 is direct encoding
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("D1N0")), "Direct decoding failed");

            // d1n0 is lowercase encoding
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("d1n0")), "Lowercase decoding failed");

            // D1NO is encoding with 0 (zero) replaced with O (letter O capital)
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("D1NO")), "0->O decoding failed");

            // D1No is encoding with 0 (zero) replaced with o (letter O small)
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("D1No")), "0->o decoding failed");

            // DiN0 is encoding with 1 (one) replaced with i (letter i small)
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("DiN0")), "1->i decoding failed");

            // DIN0 is encoding with 1 (one) replaced with i (letter i capital)
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("DIN0")), "1->I decoding failed");

            // DlN0 is encoding with 1 (one) replaced with l (letter L small)
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("DlN0")), "1->l decoding failed");

            // DLN0 is encoding with 1 (one) replaced with L (letter L capital)
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("DLN0")), "1->L decoding failed");

            // Dino is combination of previous methods
            Assert.IsTrue(AreByteArraysEqual(correct_s, CFBase32.FromBase32String("Dino")), "Dino decoding failed");


            // Test lowercase
            string lc_test_src;
            byte[] lc_test_bytes;
            for (int i = 0; i <= 255; i++)
            {
                lc_test_src = new string((char)i, 100);
                lc_test_bytes = System.Text.Encoding.UTF8.GetBytes(lc_test_src);
                Assert.IsTrue(AreByteArraysEqual(lc_test_bytes, CFBase32.FromBase32String(CFBase32.ToBase32String(lc_test_bytes).ToLower())));
            }
        }


        [TestMethod]
        public void Test_FromBase32StringToString()
        {
            byte[] test_phase1 = new byte[256];
            byte[] test_phase2 = new byte[256];
            for (int i = 0; i <= 255; i++)
            {
                test_phase1[i] = (byte)i;
                test_phase2[255 - i] = (byte)i;
            }

            string exp_test_phase1 = @"000G40R40M30E209185GR38E1W8124GK2GAHC5RR34D1P70X3RFJ08924CJ2A9H750MJMASC5MQ2YC1H68SK8D9P6WW3JEHV7GYKWFT085146H258S3MGJAA9D64TKJFA18N4MTMANB5EP2SB9DNRQAYBXG62RK3CHJPCSV8D5N6PV3DDSQQ0WBJEDT7AXKQF1WQMYVWFNZ7Z041GA1R91C6GY48K2MBHJ6RX3WGJ699754NJTBSH6CTKEE9V7MZM2GT58X4MPKAFA59NANTSBDENYRB3CNKPJTVDDXRQ6XBQF5XQTZW1GE2RF2CBHP7S34WNJYCSQ7CZM6HTB9X9NEPTZCDKPPVVKEXXQZ0W7HE7S75WVKYHTFAXFPEVVQFY3RZ5WZMYQVFFY7SZBXZSZFYZW";
            string exp_test_phase2 = @"ZZZFVZ7VZBWZHXZPYQTF7WQHY3QYXVFCXFNEKT77WVJY9RZ2W7GDZQPXVKDXNPERTZBDBN6KTB8X1KYESQ6CQJP9S33WDHE4RF1C3G5ZQTYVSEXTQ6WBFDNNPJSV5CDGNYQAVB5BNAMTH9X6MPJA78N1M2FSX7CWKED9K64QJTAS94WJJ688Z3MDHJ5RN2C8GY38B143GA0R0ZVYFNY7PYKSF1VQCXBMEDS72W3FDSPPRTVAD5M6ESK5CHHP4RB0BXF5TQ2VB9CNGNTPANA56MJHA17MWKAC9D54JJ278S2M8GT28503YFHX7GXKME9R6WV3AD1K68RK0BSE5MP2PAH950KJC9944CH2280Z3REHR6RT34C1E5GN2G9H448G1W70T30B184GG1R60M2060G100";

            Assert.IsTrue(AreByteArraysEqual(test_phase1, CFBase32.FromBase32String(exp_test_phase1)));
            Assert.IsTrue(AreByteArraysEqual(test_phase2, CFBase32.FromBase32String(exp_test_phase2)));
            Assert.IsTrue(AreByteArraysEqual(System.Text.Encoding.UTF8.GetBytes("Hello, world"), CFBase32.FromBase32String("91JPRV3F5GG7EVVJDHJ0")));
            Assert.IsTrue(AreByteArraysEqual(System.Text.Encoding.UTF8.GetBytes("±"), CFBase32.FromBase32String("RARG")));
        }

        private bool AreByteArraysEqual(byte[] expected, byte[] actual)
        {
            if (expected == null)
            {
                throw new AssertFailedException("Expected is null");
            }
            if (actual == null)
            {
                throw new AssertFailedException("Actual is null");
            }
            if (expected.Length != actual.Length)
            {
                throw new AssertFailedException($"Expected's array length ({expected.Length}) is not equal to Actuial's array length ({actual.Length})");
            }
            for (int i = 0; i < expected.Length; i++)
            {
                if (!expected[i].Equals(actual[i]))
                {
                    throw new AssertFailedException($"Expected and Actual arrays differ at position {i} : Expected[{i}]={expected[i]}; Actual[{i}]={actual[i]}");
                }
            }
            return true;
        }
    }
}
