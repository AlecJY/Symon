using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using log4net;
using log4net.Config;

namespace Symon.Client {
    class ClientMain {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);

        public ClientMain(string[] args) {
            Logger.Info("Starting Symon Client");
            Broadcast broadcast = new Broadcast();
            string ip = broadcast.Listen();
            TcpStream stream = new TcpStream(PublicKeyReader.Read(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "key.cer"));
            stream.Start(ip);
        }

        static void Main(string[] args) {
            XmlConfigurator.Configure(new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "log.config"));
            if (args.Length > 0) {
                if (args.Length == 1 && args[0].ToLower().Equals("/install")) {
                    Process.Start("cmd", "/c schtasks /Create /SC ONSTART /TN \"Symon Client\" /TR \"'" + Assembly.GetExecutingAssembly().Location + "'\" /RU " + "SYSTEM" + " /RL HIGHEST & pause");
                } else if (args.Length == 1 && args[0].ToLower().Equals("/uninstall")) {
                    Process.Start("cmd", "/c schtasks /Delete /F /TN \"Symon Client\"");
                } else {
                    Console.Error.WriteLine("Unknown arguments!");
                }
                return;
            }
            while (true) {
                new ClientMain(args);
            }
        }
    }
}
