using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Symon.Server {
    public class TcpStream {
        private X509Certificate2 cert;
        private ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private List<ClientInfo> ClientList = new List<ClientInfo>();

        public TcpStream(X509Certificate2 cert) {
            this.cert = cert;
        }

        public List<ClientInfo> GetClients() {
            return ClientList;
        }

        public void Start() {
            TcpListener listener = new TcpListener(IPAddress.Any, AppInfo.TcpStreamPort);
            try {
                listener.Start();
            }
            catch (Exception e) {
                Console.Error.WriteLine("Port " + AppInfo.BroadcastPort + " is used. Please check if other application use the port.");
                Console.Error.WriteLine(e);
                Environment.Exit(-1);
            }
            while (true) {
                try {
                    tcpClientConnected.Reset();
                    listener.BeginAcceptTcpClient(AcceptCallback, listener);
                    tcpClientConnected.WaitOne();
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }

        private void AcceptCallback(IAsyncResult ar) {
            try {
                TcpListener listener = (TcpListener) ar.AsyncState;
                TcpClient clientRequest = listener.EndAcceptTcpClient(ar);
                tcpClientConnected.Set();
                NetworkStream stream = clientRequest.GetStream();
                SslStream sslStream = new SslStream(stream, false);

                ClientInfo client = new ClientInfo();
                client.TcpClient = clientRequest;
                client.SslStream = sslStream;
                ClientList.Add(client);

                MessageAnalyzer messageAnalyzer = new MessageAnalyzer(client);

                sslStream.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);
                byte[] buffer = new byte[1024];
                int recv;
                string str = "";

                while (true) {
                    recv = sslStream.Read(buffer, 0, buffer.Length);
                    if (clientRequest.Connected == false || sslStream.IsAuthenticated == false ||
                        sslStream.IsEncrypted == false) {
                        break;
                    }
                    str += Encoding.UTF8.GetString(buffer, 0, recv);
                    while (true) {
                        if (!str.Contains("\r\n")) {
                            break;
                        }
                        int p = str.IndexOf("\r\n");
                        client.Messages.Add(str.Substring(0, p));
                        str = str.Substring(p+2);
                    }
                    messageAnalyzer.Analyze();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }

    public class ClientInfo {
        public TcpClient TcpClient;
        public SslStream SslStream;
        public List<string> Messages = new List<string>();
        public int PingLostTimes = 0;
        public bool isAuth = false;
    }
}