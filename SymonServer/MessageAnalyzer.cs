using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Symon.Server {
    public class MessageAnalyzer {
        private SuperviseConnection _connection;
        private bool Running = false;
        private List<Message> _msgList = new List<Message>();

        public MessageAnalyzer(SuperviseConnection connection) {
            _connection = connection;
            Thread startUiThread = new Thread(StartUI);
            startUiThread.Start();
        }

        private void StartUI() {
            while (true) {
                string cmd = Console.ReadLine();
                string arguments = Console.ReadLine();
                Dictionary<uint, bool> sendClient = new Dictionary<uint, bool>();
                Dictionary<uint, Connection.SimpleClientInfo> clientInfos = _connection.GetClientInfos();
                foreach (KeyValuePair<uint, Connection.SimpleClientInfo> keyValuePair in clientInfos) {
                    sendClient.Add(keyValuePair.Value.ClientId, true);
                }
                SystemCall systemCall = new SystemCall(_connection.Send);
                systemCall.Send(cmd, arguments, sendClient);
            }
        }

        private void Run() {
            while (_msgList.Count > 0) {
                string msg = Encoding.UTF8.GetString(_msgList[0].Msg);
                uint id = _msgList[0].Id;
                _msgList.RemoveAt(0);
                if (msg.StartsWith("200")) {
                    ClientAuth clientAuth = new ClientAuth(_connection.getClient(id), _connection.Send);
                    clientAuth.GetAuth(msg);
                }
            }
            Running = false;
        }

        public void Analyze(byte[] msg, uint id) {
            _msgList.Add(new Message(msg, id));
            if (!Running) {
                Running = true;
                Thread runMessageAnalyzer = new Thread(Run);
                runMessageAnalyzer.Start();
            }
        }

        private class Message {
            public byte[] Msg;
            public uint Id;

            public Message(byte[] msg, uint id) {
                Msg = msg;
                Id = id;
            }
        }
    }
}