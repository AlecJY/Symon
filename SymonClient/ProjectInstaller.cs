using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Symon.Client {
    [RunInstaller(true)]
    public partial class ProjectInstaller: System.Configuration.Install.Installer {
        public ProjectInstaller() {
            InitializeComponent();
        }

        private void clientServiceInstaller_AfterInstall(object sender, InstallEventArgs e) {
            if (!EventLog.SourceExists("SymonClientSource")) {
                EventLog.CreateEventSource("SymonClientSource", "SymonClientLog");
            }
        }

        private void clientServiceInstaller_AfterUninstall(object sender, InstallEventArgs e) {
            if (EventLog.SourceExists("SymonClientSource")) {
                EventLog.DeleteEventSource("SymonClientSource");
            }
        }
    }
}
