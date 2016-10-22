using System.Security.Cryptography;
using System.Text;

namespace Symon.Server {
    public class CryptMessage {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

        public CryptMessage(string privateKey) {
            rsa.FromXmlString(privateKey);
        }

        public byte[] Encrypt(string msg) {
            return rsa.Encrypt(Encoding.UTF8.GetBytes(msg), true);
        }

        public string Decrypt(byte[] encryptedData) {
            return Encoding.UTF8.GetString(rsa.Decrypt(encryptedData, true));
        }
    }
}