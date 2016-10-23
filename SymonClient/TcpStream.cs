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
        private X509Certificate2 cert;

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
}