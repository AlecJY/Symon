using System.IO;
using System.Reflection;
using log4net;

namespace Symon.Client {
    public class StartClient {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);

        public StartClient(string[] args) {
            Logger.Info("Starting Symon Client");
            Broadcast broadcast = new Broadcast();
            string ip = broadcast.Listen();
            string certPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "key.cer";
            TcpStream stream = new TcpStream(PublicKeyReader.Read(certPath));
            stream.Start(ip);
        }
    }
}