namespace Symon.Client {
    partial class ProjectInstaller {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.clientServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.clientServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // clientServiceProcessInstaller
            // 
            this.clientServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.clientServiceProcessInstaller.Password = null;
            this.clientServiceProcessInstaller.Username = null;
            // 
            // clientServiceInstaller
            // 
            this.clientServiceInstaller.ServiceName = "Symon Client Service";
            this.clientServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.clientServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.clientServiceInstaller_AfterInstall);
            this.clientServiceInstaller.AfterUninstall += new System.Configuration.Install.InstallEventHandler(this.clientServiceInstaller_AfterUninstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.clientServiceProcessInstaller,
            this.clientServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller clientServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller clientServiceInstaller;
    }
}