using System;
using System.Diagnostics;
using System.Threading;

namespace Symon.Server {
    public class ServerMain {
        private ConfigManager settings;
        private ObjectManager objectManager = new ObjectManager();

        public ServerMain(string[] args) {
            PrivateKeyReader keyReader = new PrivateKeyReader("private.key");
            keyReader.GetKeyString();
            
            settings = new ConfigManager();
            settings.Load("settings.json");
            StartBroadcast("169.254.255.255");
            StartTcpStream();
        }

        private static void Main(string[] args) {
            new ServerMain(args);
        }

        private void StartBroadcast(string ip) {
            if (settings.EnabledBroadcast()) {
                Broadcast broadcast = new Broadcast(ip);
                Thread broadcastThread = new Thread(broadcast.Send);
                objectManager.SetBroadcastThread(broadcastThread);
                broadcastThread.Start();
            }
        }

        private void StartTcpStream() {
            TcpStream tcpStream = new TcpStream();
            Thread tcpStreamThread = new Thread(tcpStream.Start);
            tcpStreamThread.Start();
        }
    }
}