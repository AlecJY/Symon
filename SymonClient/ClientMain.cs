using System;

namespace Symon.Client {
    class ClientMain {
        public ClientMain(string[] args) {
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
