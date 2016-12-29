using log4net;

namespace Symon.Client {
    public class StartClient {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);

        public StartClient(string[] args) {
            Logger.Info("Starting Symon Client");
            Broadcast broadcast = new Broadcast();
            string ip = broadcast.Listen();
            TcpStream stream = new TcpStream(PublicKeyReader.Read("key.cer"));
            stream.Start(ip);
        }
    }
}