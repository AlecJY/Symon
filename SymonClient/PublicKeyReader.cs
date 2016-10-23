using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Symon.Client {
    public class PublicKeyReader {
        public static X509Certificate2 Read(string filename) {
            try {
                X509Certificate2 cert = new X509Certificate2(filename);
                return cert;
            } catch (CryptographicException e) {
                Console.Error.WriteLine("Cannot found public key file \"" + filename + "\"");
                Environment.Exit(-1);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
            return null;
        }
    }
}