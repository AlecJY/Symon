using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Symon.Client {
    public class MessageAnalyzer {
        private SuperviseConnection connection;
        private bool Running = false;
        private SystemCall SystemCall;
        private List<string> msgs;

        public MessageAnalyzer(SuperviseConnection connection) {
            this.connection = connection;
        }

        private void Run() {
            Running = true;
            while (msgs.Count > 0) {
                string msg = msgs[0];
                msgs.RemoveAt(0);
                if (msg.StartsWith("201")) {
                    connection.GetServer().Auth.GetAuth(msg);
                }
                if (msg.StartsWith("30")) {
                    if (msg.StartsWith("300")) {
                        SystemCall = new SystemCall(msg);
                    } else { 
                        SystemCall.SetMsg(msg);
                    }
                }
            }
            Running = false;
        }

        public void Analyze(byte[] msg) {
            msgs.Add(Encoding.UTF8.GetString(msg));
            if (!Running) {
                Thread runMessageAnalyze = new Thread(Run);
                runMessageAnalyze.Start();
            }
        }
    }
}