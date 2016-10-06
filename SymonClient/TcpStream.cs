using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Symon.Client {
    public class TcpStream {
        private TcpClient client;

        public void Start(String ip) {
            try {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), AppInfo.TcpStreamPort);
                client = new TcpClient();
                client.Connect(remoteEP);
                client.LingerState = new LingerOption(true, 60);
                NetworkStream stream = client.GetStream();
                NegotiateStream authStream = new NegotiateStream(stream, false);
                authStream.AuthenticateAsClient();

                byte[] buffer = Encoding.UTF8.GetBytes("Test message.");
                while (true) {
                    authStream.Write(buffer, 0, buffer.Length);
                    authStream.Flush();
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }


        }
    }
}