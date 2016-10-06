using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Symon.Client {
    public class Broadcast {
        private IPEndPoint IPEnd = new IPEndPoint(IPAddress.Any, AppInfo.BroadcastPort);
        private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint IP;
        private BroadcastAnalyzer broadcastAnalyzer = new BroadcastAnalyzer();

        public Broadcast() {
            try {
                client.Bind(IPEnd);
                IP = IPEnd;
            }
            catch (Exception e) {
                Console.Error.WriteLine("Port " + AppInfo.BroadcastPort + " is used. Please check if other application use the port.");
                Console.Error.WriteLine(e);
                Environment.Exit(-1);
            }
        }

        public string Listen() {
            byte[] buffer = new byte[1024];
            int recv;
            string msg;
            while (true) {
                recv = client.ReceiveFrom(buffer, ref IP);
                msg = Encoding.UTF8.GetString(buffer, 0, recv);
                broadcastAnalyzer.AnalyzeMessage(msg, IP.ToString());
                if (broadcastAnalyzer.LegalProtocol()) {
                    string IPOnly = IP.ToString().Substring(0, IP.ToString().LastIndexOf(':'));
                    client.Close();
                    return IPOnly;
                }
            }
        }
    }
}