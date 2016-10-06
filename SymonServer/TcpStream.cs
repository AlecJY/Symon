using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Symon.Server {
    public class TcpStream {
        private TcpListener listener;

        public void Start() {
            listener = new TcpListener(IPAddress.Any, AppInfo.TcpStreamPort);
            listener.Start();
            NewClient();
        }

        public void NewClient() {
            try {
                TcpClient clientRequest = listener.AcceptTcpClient();
                NetworkStream stream = clientRequest.GetStream();
                NegotiateStream authStream = new NegotiateStream(stream, false);
                authStream.AuthenticateAsServer();
                Thread newClienThread = new Thread(NewClient);
                newClienThread.Start();
                byte[] buffer = new byte[1024];
                int recv;
                string str;

                while (true) {
                    recv = authStream.Read(buffer, 0, buffer.Length);
                    if (clientRequest.Connected == false || authStream.IsAuthenticated == false || authStream.IsEncrypted == false) {
                        break;
                    }
                    str = Encoding.UTF8.GetString(buffer, 0, recv);
                    Console.WriteLine(str);
                    Console.WriteLine(clientRequest.Client.RemoteEndPoint.ToString());
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}