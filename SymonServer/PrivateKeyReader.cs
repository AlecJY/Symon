using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Symon.Server {
    public class PrivateKeyReader {
        private byte[] encrytedPrivateKey = null;
        private byte[] salt = new byte[8];
        private byte[] iv = new byte[8];
        private string filename;

        public PrivateKeyReader(string filename) {
            this.filename = filename;
            try {
                byte[] privateKeyFile = File.ReadAllBytes(filename);
                encrytedPrivateKey = new byte[privateKeyFile.Length - 16];
                Array.Copy(privateKeyFile, 0, salt, 0, 8);
                Array.Copy(privateKeyFile, 8, iv, 0, 8);
                Array.Copy(privateKeyFile, 16, encrytedPrivateKey, 0, privateKeyFile.Length - 16);
            } catch (FileNotFoundException e) {
                Console.Error.WriteLine("Cannot found private key file \"" + filename + "\"");
                Environment.Exit(-1);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }

        public string GetKeyString() {
            while (true) {
                Console.Write("Password: ");
                SecureString pwd = GetPasswd();
                Console.WriteLine();

                try {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SecureStringToBytes(pwd), salt, 1000);
                    TripleDES decAlg = TripleDES.Create();
                    decAlg.Key = key.GetBytes(16);
                    decAlg.IV = iv;
                    MemoryStream decryptionStream = new MemoryStream();
                    CryptoStream decrypt = new CryptoStream(decryptionStream, decAlg.CreateDecryptor(),
                        CryptoStreamMode.Write);
                    decrypt.Write(encrytedPrivateKey, 0, encrytedPrivateKey.Length);
                    decrypt.Flush();
                    decrypt.Close();
                    key.Reset();
                    return Encoding.UTF8.GetString(decryptionStream.ToArray());
                } catch (CryptographicException e) {
                    Console.WriteLine("Wrong password. Please try again");
                } catch (Exception e) {
                    Console.Error.WriteLine(e);
                }
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
                } else {
                    pwd.AppendChar(i.KeyChar);
                }
            }
            return pwd;
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
            } finally {
                if (strPtr != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(strPtr);
                }
            }
            return pwd;
        }
    }
}