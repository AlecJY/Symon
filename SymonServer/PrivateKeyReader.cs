using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Symon.Server {
    public class PrivateKeyReader {
        public static X509Certificate2 GetCert(string filename) {
            X509Certificate2 cert;
            if (!File.Exists(filename)) {
                Console.Error.WriteLine("Cannot find cert \"" + filename + "\"");
                Environment.Exit(-1);
            }
            while (true) {
                try {
                    Console.Write("Password: ");
                    SecureString pwd = GetPasswd();
                    Console.WriteLine();
                    cert = new X509Certificate2("key.pfx", pwd);
                    break;
                }
                catch (FileNotFoundException e) {
                    Console.Error.WriteLine("Cannot find cert \"" + filename + "\"");
                    Environment.Exit(-1);
                }
                catch (CryptographicException e) {
                    if (e.HResult == unchecked ((int)0x80070056)) {
                        Console.WriteLine("Wrong password. Please try again");
                    }
                    else {
                        Console.Error.WriteLine("Certificate broken or other error.");
                        Console.Error.WriteLine(e);
                        Environment.Exit(-1);
                    }
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