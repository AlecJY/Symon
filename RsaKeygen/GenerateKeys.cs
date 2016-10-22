using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Symon.RsaKeygen {
    class GenerateKeys {
        public GenerateKeys() {
            // Generate rsa keypair
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            var publicKey = rsa.ToXmlString(false);
            var privateKey = rsa.ToXmlString(true);
            SecureString pwd;
            List<byte> privateKeyFile = new List<byte>();

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

            // rfc2898 crypt
            byte[] salt = new byte[8];

            // randomly generate salt, not use now
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider()) {
                rngCsp.GetBytes(salt);
            }

            try {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SecureStringToBytes(pwd), salt, 1000);
                TripleDES encAlg = TripleDES.Create();
                encAlg.Key = key.GetBytes(16);
                MemoryStream encryptionStream = new MemoryStream();
                CryptoStream encrypt = new CryptoStream(encryptionStream, encAlg.CreateEncryptor(),
                    CryptoStreamMode.Write);
                byte[] privetKeyBytes = Encoding.UTF8.GetBytes(privateKey);
                encrypt.Write(privetKeyBytes, 0, privetKeyBytes.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                privateKeyFile.AddRange(salt);
                privateKeyFile.AddRange(encAlg.IV);
                privateKeyFile.AddRange(encryptionStream.ToArray());
                key.Reset();
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Environment.Exit(-1);
            }
            
            try {
                File.WriteAllText("public.key", publicKey);
                File.WriteAllBytes("private.key", privateKeyFile.ToArray());
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

        private byte[] SecureStringToBytes(SecureString str) {
            IntPtr strPtr = IntPtr.Zero;
            byte[] pwd;
            try {
                strPtr = Marshal.SecureStringToBSTR(str);
                int strLen = Marshal.ReadByte(strPtr, -4);
                pwd = new byte[strLen];
                for (int i = 0; i < strLen; i++) {
                    pwd[i] = Marshal.ReadByte(strPtr, i);
                }
            }
            finally {
                if (strPtr != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(strPtr);
                }
            }
            return pwd;
        }

        static void Main(string[] args) {
            new GenerateKeys();
        }
    }
}
