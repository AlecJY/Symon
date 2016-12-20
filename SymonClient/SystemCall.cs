using System;
using System.Diagnostics;
using System.Security;
using log4net;

namespace Symon.Client {
    public class SystemCall {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);

        private int mode;
        private ServerInfo server;
        private string[] Msgs = new string[4];

        public SystemCall(string msg) {
            if (msg.EndsWith("MODE_1")) {
                mode = 1;
            }
            else if (msg.EndsWith("MODE_2")) {
                mode = 2;
            }
            else if (msg.EndsWith("MODE_4")) {
                mode = 4;
            }
        }

        public void SetMsg(string msg) {
            if (msg.StartsWith("301 ")) {
                Msgs[0] = msg.Substring(4);
            }
            else if (msg.StartsWith("302 ")) {
                Msgs[1] = msg.Substring(4);
            }
            else if (msg.StartsWith("303 ")) {
                Msgs[3] = msg.Substring(4);
            }
            else if (msg.StartsWith("304 ")) {
                Msgs[4] = msg.Substring(4);
            }
            for (int i = 0; i < mode; i++) {
                if (Msgs[i] == null) {
                    return;
                }
            }
            if (mode == 1) {
                StartMode1();
            }
            else if (mode == 2) {
                StartMode2();
            }
            else if (mode == 4) {
                StartMode4();
            }
        }

        private void StartMode1() {
            try {
                Process.Start(Msgs[0]);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Logger.Error(e);
            }
        }

        private void StartMode2() {
            try {
                Process.Start(Msgs[0], Msgs[1]);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Logger.Error(e);

            }
        }

        private void StartMode4() {
            try {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = Msgs[0];
                info.Arguments = Msgs[1];
                info.UserName = Msgs[2];
                info.Password = StringToSecureString(Msgs[3]);
                Process.Start(info);
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                Logger.Error(e);
            }

        }

        private SecureString StringToSecureString(string msg) {
            SecureString str = new SecureString();
            char[] msgArray = msg.ToCharArray();
            foreach (char c in msgArray) {
                str.AppendChar(c);
            }
            return str;
        }
    }
}