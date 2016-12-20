using System;
using System.IO;
using System.Text;
using System.Threading;
using log4net;

namespace Symon.Client {
    public class ServerAuth {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);
        private Send send;
        private bool isAuth = false;

        public ServerAuth(Send send) {
            this.send = send;
        }

        public void Auth() {
            string hello = "200 Hello From Client";
            byte[] buffer = Encoding.UTF8.GetBytes(hello);
            while (!isAuth) {
                try {
                    send(buffer);
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
                isAuth = true;
                Console.WriteLine("Connected to server.");
                Logger.Info("Connected to server.");
            }
        }

        public delegate void Send(byte[] msg);
    }
}