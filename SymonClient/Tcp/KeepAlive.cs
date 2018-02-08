using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symon.Client.Tcp {
    class KeepAlive {
        private ConnectionManager _connectionManager;
        private Connection _connection;

        public KeepAlive(ConnectionManager connectionManager) {
            _connectionManager = connectionManager;
            _connection = _connectionManager.NewConnection("KeepAlive", Receive);
            Thread keepAliveThread = new Thread(SendPing);
            keepAliveThread.Start();
        }

        private void SendPing() {
            while (true) {
                bool result = _connection.Send(Encoding.UTF8.GetBytes("202 PING"));
                if (!result) {
                    break;
                }
            }
        }

        private void Receive(byte[] msg) {
        }
    }
}