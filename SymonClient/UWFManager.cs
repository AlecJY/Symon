using System;
using System.Text;
using log4net;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Symon.Client;

namespace Symon {
    class UWFManager {
        private static readonly ILog Logger = LogManager.GetLogger(AppInfo.AppName);
        private Connection _connection;
        private CimInstance _cimInstance;
        private CimSession _cimSession;

        public UWFManager(ConnectionManager connectionManager) {
            _connection = connectionManager.NewConnection("UWFManager", Receive);
            try {
                if (_cimSession != null) {
                    _cimSession.Close();
                }
                var sessionOptions = new DComSessionOptions();
                sessionOptions.Timeout = new TimeSpan(0, 2, 0);
                _cimSession = CimSession.Create(@".", sessionOptions);
            }
            catch (CimException e) {
                Console.WriteLine(e);
                throw;
            }
            UpdateCimInstance();
        }

        private void Receive(byte[] msg) {
            string msgStr = Encoding.UTF8.GetString(msg).ToLower();
            if (msgStr.Equals("enable uwf")) {
                UWFEnable();
                if (!UWFNextEnabled()) {
                    string failed = "Enable failed";
                    _connection.Send(Encoding.UTF8.GetBytes(failed));
                }
            } else if (msgStr.Equals("disable uwf")) {
                UWFDisable();
                if (UWFNextEnabled()) {
                    string failed = "Disable failed";
                    _connection.Send(Encoding.UTF8.GetBytes(failed));
                }
            } else if (msgStr.Equals("uwf restart")) {
                UWFRestartSystem();
            }
        }

        private void UpdateCimInstance() {
            try
            {
                var enumerateInstances = _cimSession.EnumerateInstances(@"root\standardcimv2\embedded", "UWF_Filter");
                foreach (var enumerateInstance in enumerateInstances)
                {
                    _cimInstance = enumerateInstance;
                    break;
                }
            }
            catch (Exception e)
            {
                if (e.HResult == -2146233088)
                {
                    Console.Error.WriteLine(e);
                    Logger.Error(e);
                }
                else
                {
                    Console.Error.WriteLine(e);
                    Logger.Fatal(e);
                }
            }
        }

        public bool UWFCurrentEnabled() {
            UpdateCimInstance();
            return (bool)_cimInstance.CimInstanceProperties["CurrentEnabled"].Value;
        }

        public bool UWFNextEnabled() {
            UpdateCimInstance();
            return (bool)_cimInstance.CimInstanceProperties["NextEnabled"].Value;
        }

        public void UWFEnable() {
            CimMethodParametersCollection parametersCollection = new CimMethodParametersCollection();
            var result = _cimSession.InvokeMethod(_cimInstance, "Enable", parametersCollection);
            if ((uint)result.ReturnValue.Value != 0) {
                Logger.Error("Enable UWF Failed. Error code: " + result.ReturnValue.Value);
            }
        }

        public void UWFDisable()
        {
            CimMethodParametersCollection parametersCollection = new CimMethodParametersCollection();
            var result = _cimSession.InvokeMethod(_cimInstance, "Disable", parametersCollection);
            if ((uint)result.ReturnValue.Value != 0)
            {
                Logger.Error("Disable UWF Failed. Error code: " + result.ReturnValue.Value);
            }
        }

        public void UWFRestartSystem() {
            CimMethodParametersCollection parametersCollection = new CimMethodParametersCollection();
            var result = _cimSession.InvokeMethod(_cimInstance, "RestartSystem", parametersCollection);
            if ((uint)result.ReturnValue.Value != 0)
            {
                Logger.Error("Restart System Failed. Error code: " + result.ReturnValue.Value);
            }
        }
    }
}
