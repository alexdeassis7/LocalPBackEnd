using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharedSecurity.Signature
{
    public class DigitalHMACSignature
    {
        #region LP Functions
        public string AssignNewKey(string identification) 
        {
            byte[] byteKey = GenerateNewHMACKEY();

            //Decide if key is saved as hexstring or base64string
            //var base64string = Convert.ToBase64String(byteKey);
            var hexKey = HashEncode(byteKey);

            SharedBusiness.Security.BlSecurity blSecurity = new SharedBusiness.Security.BlSecurity();

            blSecurity.SetHMACKey(identification, hexKey);

            return hexKey;
        }

        public byte[] GenerateNewHMACKEY()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {

                byte[] data = new byte[32];
                rng.GetBytes(data);

                return data;
            }

        }
        #endregion

        #region Hash Base64 Functions
        public string HashHMACBase64(string keyBase64, string message) 
        {
            byte[] document = Encoding.UTF8.GetBytes(message);
            byte[] key = Convert.FromBase64String(keyBase64);
            var hash = new HMACSHA256(key);
            byte[] hashedDocument = hash.ComputeHash(document);

            return Convert.ToBase64String(hashedDocument);
        }
        #endregion

        #region Hash Hex Functions
        public string HashHMACHex(string keyHex, string message)
        {
            byte[] hash = HashHMAC(HexDecode(keyHex), StringEncode(message));
            return HashEncode(hash);
        }
        #endregion

        #region Hash Functions
        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }
        #endregion

        #region Encoding Helpers
        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static byte[] HexDecode(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }
        #endregion
    }
}
