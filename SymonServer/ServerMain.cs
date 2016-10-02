using System;
using System.Diagnostics;
using System.Threading;

namespace Symon.Server {
    public class ServerMain {
        private ConfigManager settings;
        private ObjectManager objectManager = new ObjectManager();

        public ServerMain(string[] args) {
            settings = new ConfigManager();
            settings.Load(@"settings.json");
            startBroadcast("127.0.0.1");
        }

        private static void Main(string[] args) {
            new ServerMain(args);
            // Process.Start(@"cmd /c pause");
        }

        private void startBroadcast(string ip) {
            if (settings.EnabledBroadcast()) {
                Broadcast broadcast = new Broadcast(ip, 1000);
                Thread broadcastThread = new Thread(broadcast.Send);
                objectManager.SetBroadcastThread(broadcastThread);
                broadcastThread.Start();
            }
        }
    }
}