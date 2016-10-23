using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Pluralsight.Crypto;

namespace Symon.SslKeygen {
    class GenerateKeys {
        public GenerateKeys() {

            SecureString pwd;

            // input private key password
            while (true) {
                Console.Write("Password: ");
                pwd = GetPasswd();
                Console.WriteLine();
                if (pwd.Length < 6) {
                    Console.WriteLine("Password too short. Please type again");
                    continue;
                }
                Console.Write("Retype password: ");
                SecureString pwdCheck = GetPasswd();
                Console.WriteLine();
                if (SecureStringIsEqual(pwd, pwdCheck)) {
                    break;
                }
                Console.WriteLine("Password not match. Please type again.");
            }

            // Generate ssl cert
            Console.WriteLine("Generating cert...");
            X509Certificate2 cert = SslCertificate.GenerateCert();

            byte[] certData = cert.Export(X509ContentType.Cert);
            byte[] pfxData = cert.Export(X509ContentType.Pfx, pwd);
            try {
                File.WriteAllBytes("key.cer", certData);
                File.WriteAllBytes("key.pfx", pfxData);
                Console.WriteLine("Keys generate to \"" + Directory.GetCurrentDirectory() + "\"");
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
            }


        }

        private SecureString GetPasswd() {
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

        private bool SecureStringIsEqual(SecureString str1, SecureString str2) {
            // compare two secure string
            // reference: http://stackoverflow.com/questions/4502676/c-sharp-compare-two-securestrings-for-equality

            IntPtr str1Ptr = IntPtr.Zero;
            IntPtr str2Ptr = IntPtr.Zero;

            try {
                str1Ptr = Marshal.SecureStringToBSTR(str1);
                str2Ptr = Marshal.SecureStringToBSTR(str2);
                int str1Len = Marshal.ReadByte(str1Ptr, -4);
                int str2Len = Marshal.ReadByte(str2Ptr, -4);
                if (str1Len == str2Len) {
                    for (int i = 0; i < str1Len; i++) {
                        byte b1 = Marshal.ReadByte(str1Ptr, i);
                        byte b2 = Marshal.ReadByte(str2Ptr, i);
                        if (b1 != b2) {
                            return false;
                        }
                    }
                }
                else {
                    return false;
                }
                return true;
            }
            finally {
                if (str1Ptr != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(str1Ptr);
                }
                if (str2Ptr != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(str2Ptr);
                }
            }
        }

        static void Main(string[] args) {
            new GenerateKeys();
        }
    }


    public class SslCertificate {
        public static X509Certificate2 GenerateCert() {
            X509Certificate2 cert;
            using (CryptContext ctx = new CryptContext()) {
                ctx.Open();
                cert = ctx.CreateSelfSignedCertificate(new SelfSignedCertProperties {
                    IsPrivateKeyExportable = true,
                    KeyBitLength = 4096,
                    Name = new X500DistinguishedName("CN=Symon"),
                    ValidFrom = DateTime.Now.AddDays(-1),
                    ValidTo = DateTime.Now.AddYears(10)
                });
            }
            return cert;
        }
    }
}
