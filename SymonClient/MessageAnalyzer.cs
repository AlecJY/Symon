using System.Threading;

namespace Symon.Client {
    public class MessageAnalyzer {
        private ServerInfo server;
        private bool Running = false;
        private SystemCall SystemCall;

        public MessageAnalyzer(ServerInfo server) {
            this.server = server;
        }

        private void Run() {
            Running = true;
            while (server.Messages.Count > 0) {
                string msg = server.Messages[0];
                server.Messages.RemoveAt(0);
                if (msg.StartsWith("201")) {
                    server.Auth.GetAuth(msg);
                }
                if (msg.StartsWith("30")) {
                    if (msg.StartsWith("300")) {
                        SystemCall = new SystemCall(server, msg);
                    } else { 
                        SystemCall.SetMsg(msg);
                    }
                }
            }
            Running = false;
        }

        public void Analyze() {
            if (!Running) {
                Thread runMessageAnalyze = new Thread(Run);
                runMessageAnalyze.Start();
            }
        }
    }
}