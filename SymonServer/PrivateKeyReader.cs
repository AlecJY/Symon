using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Symon.Server {
    public class PrivateKeyReader {
        public static X509Certificate2 GetCert(string filename) {
            X509Certificate2 cert;
            while (true) {
                try {
                    Console.Write("Password: ");
                    SecureString pwd = GetPasswd();
                    Console.WriteLine();
                    cert = new X509Certificate2("key.pfx", pwd);
                    break;
                }
                catch (FileNotFoundException e) {
                    Console.Error.WriteLine("Cannot found cert \"" + filename + "\"");
                    Environment.Exit(-1);
                }
                catch (CryptographicException e) {
                    Console.WriteLine("Wrong password. Please try again");
                }
                catch (Exception e) {
                    Console.Error.WriteLine(e);
                }
            }
            return cert;
        }

        private static SecureString GetPasswd() {
            SecureString pwd = new SecureString();
            while (true) {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter) {
                    break;
                }
                if (i.Key == ConsoleKey.Backspace) {
                    if (pwd.Length > 0) {
                        pwd.RemoveAt(pwd.Length - 1);
                    }
                }
                else {
                    pwd.AppendChar(i.KeyChar);
                }
            }
            return pwd;
        }
    }
}