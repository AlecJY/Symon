using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Symon.Server {
    class SystemCall {
        private ClientInfo client;
        private SendMsg send;

        public SystemCall(SendMsg send) {
            this.send = send;
        }

        public int Send(string cmd, Dictionary<uint, bool> sendClient) {
            string systemCall = "300 MODE_1";
            cmd = "301 " + cmd;
            try {
                send(Encoding.UTF8.GetBytes(systemCall), sendClient);
                send(Encoding.UTF8.GetBytes(cmd), sendClient);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                return -1;
            }
            return 0;
        }

        public int Send(string cmd, string argument, Dictionary<uint, bool> sendClient) {
            string systemCall = "300 MODE_2";
            cmd = "301 " + cmd;
            argument = "302 " + argument;
            try {
                send(Encoding.UTF8.GetBytes(systemCall), sendClient);
                send(Encoding.UTF8.GetBytes(cmd), sendClient);
                send(Encoding.UTF8.GetBytes(argument), sendClient);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                return -1;
            }
            return 0;
        }

        public int Send(string cmd, string argument, string user, SecureString pass, Dictionary<uint, bool> sendClient) {
            string systemCall = "300 MODE_4";
            cmd = "301 " + cmd;
            argument = "302 " + argument;
            user = "303 " + argument;
            try {
                send(Encoding.UTF8.GetBytes(systemCall), sendClient);
                send(Encoding.UTF8.GetBytes(cmd), sendClient);
                send(Encoding.UTF8.GetBytes(argument), sendClient);
                send(Encoding.UTF8.GetBytes(user), sendClient);
                List<byte> buffer = new List<byte>();
                buffer.AddRange(Encoding.UTF8.GetBytes("304 "));
                buffer.AddRange(SecureStringToBytes(pass));
                send(buffer.ToArray(), sendClient);
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

        public delegate void SendMsg(byte[] msg, Dictionary<uint, bool> sendClient);
    }
}
