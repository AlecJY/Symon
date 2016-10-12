using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
                SslStream sslStream = new SslStream(stream, false, CertificateValidationCallback);
                sslStream.AuthenticateAsClient("Symon");

                byte[] buffer = Encoding.UTF8.GetBytes("Hello");
                while (true) {
                    sslStream.Write(buffer, 0, buffer.Length);
                    sslStream.Flush();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }


        }

        static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        }
    }
}