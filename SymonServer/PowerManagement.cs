using System.Collections.Generic;
using System.Text;

namespace Symon.Server {
    class PowerManagement {
        public static void PowerOff(SendMsg send, Dictionary<uint, bool> sendClient) {
            string poweroff = "320 Power Off";
            send(Encoding.UTF8.GetBytes(poweroff), sendClient);
        }

        public static void Reboot(SendMsg send, Dictionary<uint, bool> sendClient) {
            string reboot = "321 Reboot";
            send(Encoding.UTF8.GetBytes(reboot), sendClient);
        }

        public delegate void SendMsg(byte[] msg, Dictionary<uint, bool> sendClient);
    }
}