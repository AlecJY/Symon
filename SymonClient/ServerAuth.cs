using System;
using System.IO;
using System.Text;
using System.Threading;
using log4net;

namespace Symon.Client {
    public class ServerAuth {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);
        private ServerInfo server;

        public ServerAuth(ServerInfo server) {
            this.server = server;
        }

        public void Auth() {
            string hello = "200 Hello From Client\r\n";
            byte[] buffer = Encoding.UTF8.GetBytes(hello);
            while (!server.isAuth) {
                try {
                    server.SslStream.Write(buffer);
                }
                catch (IOException e) {
                    Console.Error.WriteLine(e);
                    Logger.Error(e);
                    break;
                }
                Thread.Sleep(3000);
            }
        }

        public void GetAuth(string msg) {
            if (msg.ToLower().Contains("hello")) {
                server.isAuth = true;
                Console.WriteLine("Connected to server.");
                Logger.Info("Connected to server.");
            }
        }
    }
}