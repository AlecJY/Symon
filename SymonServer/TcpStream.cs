using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Pluralsight.Crypto;

namespace Symon.Server {
    public class TcpStream {
        private TcpListener listener;
        private X509Certificate2 cert = SslCertificate.GenerateCert();

        public void Start() {
            listener = new TcpListener(IPAddress.Any, AppInfo.TcpStreamPort);
            listener.Start();
            NewClient();
        }

        public void NewClient() {
            try {
                TcpClient clientRequest = listener.AcceptTcpClient();
                NetworkStream stream = clientRequest.GetStream();
                SslStream sslStream = new SslStream(stream, false);
                Thread newClienThread = new Thread(NewClient);
                newClienThread.Start();
                sslStream.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);
                byte[] buffer = new byte[1024];
                int recv;
                string str;

                while (true) {
                    recv = sslStream.Read(buffer, 0, buffer.Length);
                    if (clientRequest.Connected == false || sslStream.IsAuthenticated == false || sslStream.IsEncrypted == false) {
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

    public class SslCertificate {
        public static X509Certificate2 GenerateCert() {
            X509Certificate2 cert;
                // CngKey.Create(CngAlgorithm2.Rsa).CreateSelfSignedCertificate(new X500DistinguishedName("CN=*"));
            using (CryptContext ctx = new CryptContext()) {
                ctx.Open();
                cert = ctx.CreateSelfSignedCertificate(new SelfSignedCertProperties {
                    IsPrivateKeyExportable = true,
                    KeyBitLength = 4096,
                    Name = new X500DistinguishedName("CN=Symon"),
                    ValidFrom = DateTime.Now.AddDays(-1),
                    ValidTo = DateTime.Now.AddYears(10)
                });
            }
            return cert;
        }
    }
}