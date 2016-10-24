using System;
using System.Threading;

namespace Symon.Server {
    public class MessageAnalyzer {
        private ClientInfo client;
        private bool Running = false;

        public MessageAnalyzer(ClientInfo client) {
            this.client = client;
        }

        private void Run() {
            Running = true;
            while (client.Messages.Count > 0) {
                string msg = client.Messages[0];
                client.Messages.RemoveAt(0);
                if (msg.StartsWith("200")) {
                    ClientAuth clientAuth = new ClientAuth(client);
                    clientAuth.GetAuth(msg);
                }
            }
            Running = false;
        }

        public void Analyze() {
            if (!Running) {
                Thread runMessageAnalyzer = new Thread(Run);
                runMessageAnalyzer.Start();
            }
        }
    }
}