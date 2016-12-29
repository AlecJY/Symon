using System.Net;
using MonoTorrent.Tracker;
using HttpListener = MonoTorrent.Tracker.Listeners.HttpListener;

namespace Symon.Server.BitTorrent {
    public class BitTorrent {
        public BitTorrent() {
            StartTracker();
        }

        private void StartTracker() {
            Tracker tracker = new Tracker();
            HttpListener httpListener = new HttpListener(IPAddress.Any, AppInfo.TrackerPort);
            tracker.RegisterListener(httpListener);
            httpListener.Start();

            tracker.AllowUnregisteredTorrents = false;
        }
    }
}