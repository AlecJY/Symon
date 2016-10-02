using System;
using System.Linq;
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
            client.Bind(IPEnd);
            IP = IPEnd;
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
                    return IPOnly;
                }
            }
        }
    }
}