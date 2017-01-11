using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;

namespace Symon.Client {
    class ClientMain {
        static void Main(string[] args) {
            XmlConfigurator.Configure(new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "log.config"));
            ServiceBase[] serviceBases = {
                new SymonClientService()
            };
            ServiceBase.Run(serviceBases);
            /*
            while (true) {
                new StartClient(args);
            }
            */
        }
    }
}
