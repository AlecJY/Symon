using System;
using log4net;

namespace Symon.Client {
    class BroadcastAnalyzer {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);
        private bool nameCheck;
        private bool protocolCheck;
        private string IP;

        public void AnalyzeMessage(string msg, string IP) {
            if (!IP.Equals(this.IP)) {
                nameCheck = false;
                protocolCheck = false;
                this.IP = IP;
            }

            string[] linedMsg = msg.Split("\n".ToCharArray());
            string[] splittedMsg;
            for (int i = 0; i < linedMsg.Length; i++) {
                splittedMsg = linedMsg[i].Split(" ".ToCharArray(), 2);
                try {
                    switch (splittedMsg[0]) {
                        case "100":
                            if (splittedMsg[1].ToLower().Contains("symon server")) {
                                nameCheck = true;
                            }
                            break;
                        case "101":
                            double protocolVer;
                            if (double.TryParse(splittedMsg[1], out protocolVer)) {
                                if (protocolVer >= AppInfo.MinProtocolVersion && protocolVer <= AppInfo.MaxProtocolVersion) {
                                    protocolCheck = true;
                                }
                            }
                            break;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    Logger.Error(e);
                }
            }
        }

        public bool LegalProtocol() {
            if (nameCheck && protocolCheck) {
                return true;
            }
            return false;
        }
    }
}
