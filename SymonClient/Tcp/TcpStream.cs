using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using log4net;

namespace Symon.Client {
    public class TcpStream {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);
        private TcpClient client;
        private X509Certificate2 cert;
        private ServerInfo server;
        private ConnectionManager ConnectionManager;

        public TcpStream(X509Certificate2 cert) {
            this.cert = cert;
        }

        public void Start(String ip) {
            try {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), AppInfo.TcpStreamPort);
                client = new TcpClient();
                client.Connect(remoteEP);
                client.LingerState = new LingerOption(true, 60);
                NetworkStream stream = client.GetStream();
                SslStream sslStream = new SslStream(stream, false, CertificateValidationCallback);
                sslStream.AuthenticateAsClient("Symon");

                server = new ServerInfo();
                server.SslStream = sslStream;
                server.Client = client;


                ConnectionManager = new ConnectionManager(server);

                byte[] buffer = new byte[1024];
                int recv;
                int length = 0;
                uint id;
                List<byte> recvList = new List<byte>();
                List<byte> message = new List<byte>();

                while (true) {
                    recv = sslStream.Read(buffer, 0, buffer.Length);
                    if (client.Connected == false || sslStream.IsAuthenticated == false ||
                        sslStream.IsEncrypted == false) {
                        break;
                    }
                    byte[] recvData = new byte[recv];
                    Array.Copy(buffer, recvData, recv);
                    recvList.AddRange(recvData);
                    while (true) {
                        if (length == 0) {
                            length = BitConverter.ToInt32(recvList.ToArray(), 0);
                            recvList.RemoveRange(0, 4);
                        } else if (length + 4 <= recvList.Count) {
                            id = BitConverter.ToUInt32(recvList.ToArray(), 0);
                            recvList.RemoveRange(0, 4);
                            message.AddRange(recvList.GetRange(0, length));
                            recvList.RemoveRange(0, length);

                            ServerInfo.Message msg = new ServerInfo.Message();
                            msg.Id = id;
                            msg.MsgBytes = message.ToArray();
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
                Logger.Warn(e);
            }


        }

        private bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            bool certificated = false;
            try {
                certificated = true;
                if (!certificate.GetPublicKeyString().Equals(cert.GetPublicKeyString())) {
                    certificated = false;
                }
                if (!certificate.GetRawCertDataString().Equals(cert.GetRawCertDataString())) {
                    certificated = false;
                }
                if (!certificate.GetSerialNumberString().Equals(cert.GetSerialNumberString())) {
                    certificated = false;
                }
            }
            catch (Exception e) {
                certificated = false;
            }
            return certificated;
        }
    }

    public class ServerInfo {
        public SslStream SslStream;
        public TcpClient Client;
        public List<Message> Messages = new List<Message>();
        public ServerAuth Auth;

        public class Message {
            public uint Id;
            public byte[] MsgBytes;
        }
    }
}