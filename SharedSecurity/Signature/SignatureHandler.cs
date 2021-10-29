using DAO.DataAccess.Security;
using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace SharedSecurity.Signature
{
    public class SignatureHandler
    {
        public static string CreateRSASignature(string parameters, string identification)
        {
            try { 
                //Check if user already possess key-pair. If not create new public key pair
                SharedBusiness.Security.BlSecurity blSecurity = new SharedBusiness.Security.BlSecurity();
                KeyPair keyPair = blSecurity.GetKeyPair(identification);
                var digitalSignature = new DigitalRSASignature();

                if (keyPair.privateKey == null || keyPair.publicKey == null)
                {
                    digitalSignature.AssignNewKey(identification);
                }
                else 
                {
                    digitalSignature.ImportPrivateKey(keyPair.privateKey);
                }



                var document = Encoding.UTF8.GetBytes(parameters);
                byte[] hashedDocument;

                using (var sha256 = SHA256.Create())
                {
                    hashedDocument = sha256.ComputeHash(document);
                }

                var signature = digitalSignature.SignData(hashedDocument);
                //var verified = digitalSignature.VerifySignature(hashedDocument, signature);
                
                return Convert.ToBase64String(signature);
            }
            catch (Exception ex) 
            {
                Byte[] array = new Byte[64];

                return array.ToString();
            } 
        }

        public static string CreateHMACSignature(string parameters, string identification) 
        {
            try
            {
                var digitalSignature = new DigitalHMACSignature();
                //Check if user already possess key. If not create new key
                SharedBusiness.Security.BlSecurity blSecurity = new SharedBusiness.Security.BlSecurity();
                string key = blSecurity.GetHMACKey(identification);

                if (key == null)
                {
                    key = digitalSignature.AssignNewKey(identification);
                }

                //IF KEY IS BASE64 STRING
                //return digitalSignature.HashHMACBase64(key,parameters);
                //IF KEY IS HEX STRING
                return digitalSignature.HashHMACHex(key, parameters);
            }
            catch (Exception ex)
            {
                Byte[] array = new Byte[64];

                return array.ToString();
            }
        }
    }
}
