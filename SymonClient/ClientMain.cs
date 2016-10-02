using System;

namespace Symon.Client {
    class ClientMain {
        public ClientMain(string[] args) {
            Broadcast broadcast = new Broadcast();
            string IP =  broadcast.Listen();
            Console.WriteLine(IP);
            args[0] += args[1];
        }

        static void Main(string[] args) {
            new ClientMain(args);
        }
    }
}
