using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Symon.Client {
    public class MessageAnalyzer {
        private SuperviseConnection connection;
        private bool Running = false;
        private SystemCall SystemCall;
        private List<string> msgs = new List<string>();

        public MessageAnalyzer(SuperviseConnection connection) {
            this.connection = connection;
        }

        private void Run() {
            while (msgs.Count > 0) {
                string msg = msgs[0];
                msgs.RemoveAt(0);
                if (msg.StartsWith("201")) {
                    connection.GetServer().Auth.GetAuth(msg);
                }
                if (msg.StartsWith("30")) {
                    if (msg.StartsWith("300")) {
                        SystemCall = new SystemCall(msg);
                        if (SystemCall == null) {
                            Console.WriteLine("Object create failed");
                        }
                    } else {
                        if (SystemCall == null) {
                            Console.Error.WriteLine("Wrong argument");
                        } else {
                            SystemCall.SetMsg(msg);
                        }
                    }
                }
                if (msg.StartsWith("32")) {
                    PowerManagement powerManagement = new PowerManagement();
                    if (msg.StartsWith("320")) {
                        powerManagement.PowerOff();
                    } else if (msg.StartsWith("321")) {
                        powerManagement.Reboot();
                    }
                }
            }
            Running = false;
        }

        public void Analyze(byte[] msg) {
            msgs.Add(Encoding.UTF8.GetString(msg));
            if (!Running) {
                Running = true;
                Thread runMessageAnalyze = new Thread(Run);
                runMessageAnalyze.Start();
            }
        }
    }
}