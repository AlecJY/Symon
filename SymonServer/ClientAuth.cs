using System;
using System.IO;
using System.Text;

namespace Symon.Server {
    public class ClientAuth {
        private ClientInfo client;

        public ClientAuth(ClientInfo client) {
            this.client = client;
        }

        public void GetAuth(string msg) {
            if (msg.ToLower().Contains("hello") && !client.isAuth) {
                client.isAuth = true;
                string hello = "201 Hello From Server\r\n";
                byte[] buffer = Encoding.UTF8.GetBytes(hello);
                Console.WriteLine("Client " + client.TcpClient.Client.RemoteEndPoint + " Connected.");
                try {
                    client.SslStream.Write(buffer);
                }
                catch (IOException e) {
                    Console.Error.WriteLine(e);
                }
            }
        }
    }
}