﻿using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Symon.Client {
    class ClientMain {
        static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ClientMain(string[] args) {
            Logger.Info("Starting Symon Client");
            Broadcast broadcast = new Broadcast();
            string ip = broadcast.Listen();
            TcpStream stream = new TcpStream(PublicKeyReader.Read("key.cer"));
            stream.Start(ip);
        }

        static void Main(string[] args) {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            while (true) {
                new ClientMain(args);
            }
        }
    }
}
