using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Symon.Server {
    public class Broadcast {
        private string ip;
        private int timeout;
        private Socket server;
        private IPEndPoint remoteIP;

        public Broadcast(string ip, int timeout = 1000) {
            this.ip = ip;
            this.timeout = timeout;

            remoteIP = new IPEndPoint(IPAddress.Parse(ip), AppInfo.BroadcastPort);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.EnableBroadcast = true;
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
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
    }
}