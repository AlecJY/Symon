using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Symon.Server {
    public class Broadcast {
        private string ip;
        private int timeout = 20000;
        private Socket server;
        private IPEndPoint remoteIP;

        public Broadcast(string ip) {
            this.ip = ip;
            Initialize();
        }

        public Broadcast(string ip, int timeout) {
            this.ip = ip;
            this.timeout = timeout;
            Initialize();
        }

        public void Send() {
            while (true) {
                byte[] buffer = Encoding.UTF8.GetBytes("100 " + AppInfo.AppName + " " + AppInfo.AppVersion);
                server.SendTo(buffer, remoteIP);
                buffer = Encoding.UTF8.GetBytes("101 " + AppInfo.ProtocolVersion);
                server.SendTo(buffer, remoteIP);
                Thread.Sleep(timeout);
            }
        }

        private void Initialize() {
            remoteIP = new IPEndPoint(IPAddress.Parse(ip), AppInfo.BroadcastPort);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.EnableBroadcast = true;
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        }
    }
}