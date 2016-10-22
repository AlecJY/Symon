using System.Threading;

namespace Symon.Server {
    public class ObjectManager {
        private Thread _broadcastThread;

        public void SetBroadcastThread(Thread broadcastThread) {
            _broadcastThread = broadcastThread;
        }

        public void AbortBroadcastThread() {
            _broadcastThread.Abort();
        }
    }
}