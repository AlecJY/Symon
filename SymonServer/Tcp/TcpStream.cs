using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Symon.Server {
    public class TcpStream {
        private X509Certificate2 cert;
        private ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private Dictionary<uint, ClientInfo> ClientList = new Dictionary<uint, ClientInfo>();
        private ConnectionManager ConnectionManager;
        private uint _lastClientId = 0;

        public TcpStream(X509Certificate2 cert) {
            this.cert = cert;
            ConnectionManager = new ConnectionManager(ClientList);
        }

        public Dictionary<uint, ClientInfo> GetClients() {
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
                client.ClientId = _lastClientId;
                _lastClientId++;
                ClientList.Add(client.ClientId, client);

                sslStream.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);
                byte[] buffer = new byte[1024];
                int recv;
                int length = 0;
                uint id;
                List<byte> recvList = new List<byte>();
                List<byte> message = new List<byte>();

                while (true) {
                    recv = sslStream.Read(buffer, 0, buffer.Length);
                    if (clientRequest.Connected == false || sslStream.IsAuthenticated == false ||
                        sslStream.IsEncrypted == false) {
                        break;
                    }
                    byte[] recvData = new byte[recv];
                    Array.Copy(buffer, recvData, recv);
                    recvList.AddRange(recvData);
                    while (true) {
                        if (length == 0 && recvList.Count >= 4) {
                            length = BitConverter.ToInt32(recvList.ToArray(), 0);
                            recvList.RemoveRange(0, 4);
                        }
                        else if (length + 4 <= recvList.Count) {
                            id = BitConverter.ToUInt32(recvList.ToArray(), 0);
                            recvList.RemoveRange(0, 4);
                            message.AddRange(recvList.GetRange(0, length));
                            recvList.RemoveRange(0, length);

                            ClientInfo.Message msg = new ClientInfo.Message();
                            msg.Id = id;
                            msg.MsgBytes = message.ToArray();
                            msg.ClientId = client.ClientId;
                            ConnectionManager.SetMessage(msg);
                            message.Clear();

                            length = 0;
                        } else {
                            ConnectionManager.CallReceiver();
                            break;
                        }
                    }
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
        public int PingLostTimes = 0;
        public uint ClientId;
        public bool IsAuth = false;

        public class Message {
            public uint Id;
            public byte[] MsgBytes;
            public uint ClientId;
        }
    }
}