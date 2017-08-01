using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Symon.Server;

namespace Symon {
    class UWFManager {
        private Connection _connection;

        public UWFManager(ConnectionManager connectionManager) {
            _connection = connectionManager.NewConnection("UWFManager", Receive);
            Thread startUiThread = new Thread(startUI);
            startUiThread.Start();
        }

        private void startUI() {
            Console.Write("Press any key...");
            Console.ReadKey();
            DisableUWF();
            Console.WriteLine("Send Disable");
        }

        private void Receive(byte[] msg, uint id) {
            string msgStr = Encoding.UTF8.GetString(msg).ToLower();
            if (msgStr.Equals("failed")) {
                Console.WriteLine(_connection.GetClientInfos()[id].ClientId + " failed");
            }
        }

        public void EnableUWF() {
            var clientInfos = _connection.GetClientInfos();
            Dictionary<uint, bool> sendClients = new Dictionary<uint, bool>();
            foreach (var clientInfo in clientInfos) {
                sendClients.Add(clientInfo.Key, true);
            }
            String enableUWF = "Enable UWF";
            _connection.Send(Encoding.UTF8.GetBytes(enableUWF), sendClients);
        }

        public void DisableUWF() {
            var clientInfos = _connection.GetClientInfos();
            Dictionary<uint, bool> sendClients = new Dictionary<uint, bool>();
            foreach (var clientInfo in clientInfos)
            {
                sendClients.Add(clientInfo.Key, true);
            }
            String disableUWF = "Disable UWF";
            _connection.Send(Encoding.UTF8.GetBytes(disableUWF), sendClients);
        }
    }
}
