using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using log4net;
using log4net.Repository.Hierarchy;

namespace Symon.Client {
    public class ConnectionManager {
        private ServerInfo server;
        private uint lastID = 999;
        private Dictionary<uint, ConnectionInfo> ConnectionList = new Dictionary<uint, ConnectionInfo>();
        private List<ServerInfo.Message> _msgs = new List<ServerInfo.Message>();
        private bool _receiving = false;

        public ConnectionManager(ServerInfo server) {
            this.server = server;
            SuperviseConnection connection = new SuperviseConnection(0, server);
            MessageAnalyzer messageAnalyzer = new MessageAnalyzer(connection);
            ConnectionInfo connectionInfo = new ConnectionInfo(0, "Symon", "Base", messageAnalyzer.Analyze);
            ConnectionList.Add(0, connectionInfo);

            ServerAuth auth = new ServerAuth(connection.Send);
            server.Auth = auth;
            Thread authThread = new Thread(auth.Auth);
            authThread.Start();
        }

        public Connection NewConnection(string name, Connection.Receive receive) {
            lastID++;
            StackFrame frame = new StackFrame(1);
            Connection connection = new Connection(lastID, server);
            ConnectionInfo connectionInfo = new ConnectionInfo(lastID, frame.GetMethod().GetType().Namespace, name, receive);
            ConnectionList.Add(lastID, connectionInfo);
            return connection;
        }

        public void SetMessage(ServerInfo.Message msg) {
            _msgs.Add(msg);
        }

        public void CallReceiver() {
            if (!_receiving) {
                _receiving = true;
                while (_msgs.Count != 0) {
                    ServerInfo.Message msg = _msgs[0];
                    _msgs.RemoveAt(0);
                    if (!ConnectionList.ContainsKey(msg.Id)) {
                        Console.WriteLine("Error: Unknown connection ID " + msg.Id);
                    } else {
                        ConnectionInfo connectionInfo = ConnectionList[msg.Id];
                        connectionInfo.Receive(msg.MsgBytes);
                    }
                }
                _receiving = false;
            }
        }

        private class ConnectionInfo {
            private uint id;
            private string packageName;
            private string subName;
            private Connection.Receive receive;

            public ConnectionInfo(uint id, string packageName, string subName, Connection.Receive receive) {
                this.id = id;
                this.packageName = packageName;
                this.subName = subName;
                this.receive = receive;
            }

            public void Receive(byte[] msg) {
                receive(msg);
            }
        }
    }

    public class Connection {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);
        protected uint id;
        protected ServerInfo server;

        public Connection(uint id, ServerInfo server) {
            this.id = id;
            this.server = server;
        }

        public void Send(byte[] msg) {
            try {
                server.SslStream.Write(BitConverter.GetBytes(msg.Length));
                server.SslStream.Write(BitConverter.GetBytes(id));
                server.SslStream.Write(msg);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                Logger.Error(e);
            }
        }

        public delegate void Receive(byte[] msg);
    }

    public class SuperviseConnection: Connection {
        public SuperviseConnection(uint id, ServerInfo server) : base(id, server) {
        }

        public ServerInfo GetServer() {
            return server;
        }
    }
}