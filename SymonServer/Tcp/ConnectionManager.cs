﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Symon.Server {
    public class ConnectionManager {
        private Dictionary<uint, ClientInfo> clients;
        private Dictionary<uint, ConnectionInfo> ConnectionList = new Dictionary<uint, ConnectionInfo>();
        private uint lastID = 999;
        private List<ClientInfo.Message> _msgs = new List<ClientInfo.Message>();
        private bool _receiving = false;
        

        public ConnectionManager(Dictionary<uint, ClientInfo> clients) {
            this.clients = clients;
            SuperviseConnection connection = new SuperviseConnection(0, clients);
            MessageAnalyzer messageAnalyzer = new MessageAnalyzer(connection);
            ConnectionList[0] = new ConnectionInfo(0, "Symon", "Base", messageAnalyzer.Analyze);
        }

        public Connection NewConnection(string name, Connection.Receive receive) {
            lastID++;
            StackFrame frame = new StackFrame(1);
            Connection connection = new Connection(lastID, clients);
            ConnectionInfo connectionInfo = new ConnectionInfo(lastID, frame.GetMethod().GetType().Namespace, name, receive);
            ConnectionList.Add(lastID, connectionInfo);
            return connection;
        }

        public void SetMessage(ClientInfo.Message msg) {
            _msgs.Add(msg);
        }

        public void CallReceiver() {
            if (!_receiving) {
                _receiving = true;
                while (_msgs.Count != 0) {
                    ClientInfo.Message msg = _msgs[0];
                    _msgs.RemoveAt(0);
                    if (!ConnectionList.ContainsKey(msg.Id)) {
                        Console.Error.WriteLine("Error: Unknown connection ID " + msg.Id);
                    } else {
                        ConnectionInfo connectionInfo = ConnectionList[msg.Id];
                        try {
                            connectionInfo.Receive(msg.MsgBytes, msg.ClientId);
                        } catch (Exception e) {
                            Console.Error.WriteLine(connectionInfo.PackageName + "." + connectionInfo.SubName + " receive data error:" + e);
                        }
                    }
                }
                _receiving = false;
            }
        }

        private class ConnectionInfo {
            private uint id;
            public string PackageName { get; }
            public string SubName { get; }
            private Connection.Receive receive;

            public ConnectionInfo(uint id, string packageName, string subName, Connection.Receive receive) {
                this.id = id;
                this.PackageName = packageName;
                this.SubName = subName;
                this.receive = receive;
            }

            public void Receive(byte[] msg, uint id) {
                receive(msg, id);
            }
        }
    }

    public class Connection {
        protected uint id;
        protected Dictionary<uint, ClientInfo> clients;

        public Connection(uint id, Dictionary<uint, ClientInfo> clients) {
            this.id = id;
            this.clients = clients;
        }

        public void Send(byte[] msg, Dictionary<uint, bool> sendClient) {
            foreach (KeyValuePair<uint, bool> keyValuePair in sendClient) {
                if (!clients.ContainsKey(keyValuePair.Key)) {
                    Console.Error.WriteLine("Cannot found client \"" + keyValuePair.Key + "\"!");
                } else if (keyValuePair.Value && clients[keyValuePair.Key].TcpClient.Connected) {
                    try {
                        clients[keyValuePair.Key].SslStream.Write(BitConverter.GetBytes(msg.Length));
                        clients[keyValuePair.Key].SslStream.Write(BitConverter.GetBytes(id));
                        clients[keyValuePair.Key].SslStream.Write(msg);
                    } catch (Exception e) {
                        Console.Error.WriteLine(e);
                    }
                }
            }
        }

        public Dictionary<uint, SimpleClientInfo> GetClientInfos() {
            Dictionary<uint, SimpleClientInfo> clientInfos = new Dictionary<uint, SimpleClientInfo>();
            foreach (KeyValuePair<uint, ClientInfo> keyValuePair in clients) {
                SimpleClientInfo simpleClientInfo = new SimpleClientInfo();
                simpleClientInfo.ClientId = keyValuePair.Value.ClientId;
                clientInfos.Add(keyValuePair.Key, simpleClientInfo);
            }
            return clientInfos;
        }

        public delegate void Receive(byte[] msg, uint id);

        public class SimpleClientInfo {
            public uint ClientId;
        }
    }

    public class SuperviseConnection : Connection {
        public SuperviseConnection(uint id, Dictionary<uint, ClientInfo> clients) : base(id, clients) {
        }

        public ClientInfo getClient(uint id) {
            return clients[id];
        }
    }
}