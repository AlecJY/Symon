using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symon.Client {
    partial class SymonClientService: ServiceBase {
        private string[] Args;
        private EventLog EventLog;

        public SymonClientService() {
            InitializeComponent();
            EventLog = new EventLog();
            EventLog.Source = "SymonClientSource";
            EventLog.Log = "SymonClientLog";
        }

        protected override void OnStart(string[] args) {
            Args = args;
            EventLog.WriteEntry(System.IO.Directory.GetCurrentDirectory(), EventLogEntryType.Error, 10);
            Thread thread = new Thread(StartClient);
            thread.Start();
        }

        protected override void OnStop() {

        }

        private void StartClient() {
            while (true) {
                new StartClient(Args);
            }
        }
    }
}
