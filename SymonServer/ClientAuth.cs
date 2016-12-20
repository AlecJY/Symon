using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Symon.Server {
    public class ClientAuth {
        private ClientInfo client;
        private Send send;

        public ClientAuth(ClientInfo client, Send send) {
            this.client = client;
            this.send = send;
        }

        public void GetAuth(string msg) {
            if (msg.ToLower().Contains("hello") && !client.IsAuth) {
                client.IsAuth = true;
                string hello = "201 Hello From Server";
                byte[] buffer = Encoding.UTF8.GetBytes(hello);
                Console.WriteLine("Client " + client.TcpClient.Client.RemoteEndPoint + " Connected.");
                try {
                    Dictionary<uint, bool> sendClient = new Dictionary<uint, bool>();
                    sendClient.Add(client.ClientId, true);
                    send(buffer, sendClient);
                }
                catch (IOException e) {
                    Console.Error.WriteLine(e);
                }
            }
        }

        public delegate void Send(byte[] msg, Dictionary<uint, bool> sendClient);
    }
}