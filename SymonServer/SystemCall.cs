using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Symon.Server {
    class SystemCall {
        private ClientInfo client;

        public SystemCall(ClientInfo client) {
            this.client = client;
        }

        public int Send(string cmd) {
            string systemCall = "300 MODE_1\r\n";
            cmd = "301 " + cmd + "\r\n";
            try {
                client.SslStream.Write(Encoding.UTF8.GetBytes(systemCall));
                client.SslStream.Write(Encoding.UTF8.GetBytes(cmd));
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                return -1;
            }
            return 0;
        }

        public int Send(string cmd, string argument) {
            string systemCall = "300 MODE_2\r\n";
            cmd = "301 " + cmd + "\r\n";
            argument = "302 " + argument + "\r\n";
            try {
                client.SslStream.Write(Encoding.UTF8.GetBytes(systemCall));
                client.SslStream.Write(Encoding.UTF8.GetBytes(cmd));
                client.SslStream.Write(Encoding.UTF8.GetBytes(argument));
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                return -1;
            }
            return 0;
        }

        public int Send(string cmd, string argument, string user, SecureString pass) {
            string systemCall = "300 MODE_4\r\n";
            cmd = "301 " + cmd + "\r\n";
            argument = "302 " + argument + "\r\n";
            user = "303 " + argument + "\r\n";
            try {
                client.SslStream.Write(Encoding.UTF8.GetBytes(systemCall));
                client.SslStream.Write(Encoding.UTF8.GetBytes(cmd));
                client.SslStream.Write(Encoding.UTF8.GetBytes(argument));
                client.SslStream.Write(Encoding.UTF8.GetBytes(user));
                if (Encoding.UTF8.GetString(SecureStringToBytes(pass)).Contains("\r\n")) {
                    throw new InvalidDataException("Password Contains CRLF");
                }
                client.SslStream.Write(Encoding.UTF8.GetBytes("304 "));
                client.SslStream.Write(SecureStringToBytes(pass));
                client.SslStream.Write(Encoding.UTF8.GetBytes("\r\n"));
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                return -1;
            }
            return 0;
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
