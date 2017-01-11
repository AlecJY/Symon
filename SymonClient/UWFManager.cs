using System;
using System.Threading;
using Microsoft.Management.Infrastructure;

namespace Symon.Client {
    class UWFManager {
        public UWFManager() {
            CimSession cimSession = null;
            CimInstance cimInstance;

            CimInstance cimInstanceClass;

            try {
                if (cimSession != null) {
                    cimSession.Close();
                }
                cimSession = CimSession.Create("127.0.0.1");
                Thread.Sleep(2000);
            }
            catch (CimException e) {
                Console.WriteLine(e);
                throw;
            }

            try {
                cimInstanceClass = new CimInstance("UWF_Overlay");

                cimInstance = cimSession.GetInstance(@"root\standardcimv2\embedded", cimInstanceClass);


            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
