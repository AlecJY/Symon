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

                MessageAnalyzer messageAnalyzer = new MessageAnalyzer(server);

                ServerAuth auth = new ServerAuth(server);
                server.Auth = auth;
                Thread authThread = new Thread(auth.Auth);
                authThread.Start();

                byte[] buffer = new byte[1024];
                int recv;
                string str = "";

                while (true) {
                    recv = sslStream.Read(buffer, 0, buffer.Length);
                    str += Encoding.UTF8.GetString(buffer, 0, recv);
                    while (true) {
                        if (!str.Contains("\r\n")) {
                            break;
                        }
                        int p = str.IndexOf("\r\n");
                        server.Messages.Add(str.Substring(0, p));
                        str = str.Substring(p + 2);
                    }
                    messageAnalyzer.Analyze();
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
        public bool isAuth = false;
        public SslStream SslStream;
        public TcpClient Client;
        public List<string> Messages = new List<string>();
        public ServerAuth Auth;
    }
}