using System;
using System.Collections.Generic;
using System.Threading;

namespace Symon.Server {
    public class ServerMain {
        private ConfigManager settings;
        private ObjectManager objectManager = new ObjectManager();

        public ServerMain(string[] args) {
            settings = new ConfigManager();
            settings.Load("settings.json");
            TcpStream tcpStream = StartTcpStream();
            StartBroadcast("127.0.0.1");
            while (true) {
                string cmd = Console.ReadLine();
                string arguments = Console.ReadLine();
                foreach (KeyValuePair<uint, ClientInfo> keyValuePair in tcpStream.GetClients()) {
                    SystemCall systemCall = new SystemCall(keyValuePair.Value);
                    systemCall.Send(cmd, arguments);
                }
            }

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

        private TcpStream StartTcpStream() {
            TcpStream tcpStream = new TcpStream(PrivateKeyReader.GetCert("key.pfx"));
            Thread tcpStreamThread = new Thread(tcpStream.Start);
            tcpStreamThread.Start();
            return tcpStream;
        }
    }
}