using System;
using System.Net;

namespace Symon.Client {
    class ClientMain {
        public ClientMain(string[] args) {
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            Broadcast broadcast = new Broadcast();
            string ip = broadcast.Listen();
            TcpStream stream = new TcpStream();
            stream.Start(ip);
        }

        static void Main(string[] args) {
            while (true) {
                new ClientMain(args);
            }
        }
    }
}
