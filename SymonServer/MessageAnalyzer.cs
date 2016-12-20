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
        }

        private void Run() {
            Running = true;
            while (_msgList.Count > 0) {
                string msg = Encoding.UTF8.GetString(_msgList[0].Msg);
                _msgList.RemoveAt(0);
                if (msg.StartsWith("200")) {
                    ClientAuth clientAuth = new ClientAuth(_connection.getClient(_msgList[0].Id), _connection.Send);
                    clientAuth.GetAuth(msg);
                }
            }
            Running = false;
        }

        public void Analyze(byte[] msg, uint id) {
            _msgList.Add(new Message(msg, id));
            if (!Running) {
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