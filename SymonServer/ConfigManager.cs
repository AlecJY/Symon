using System.IO;
using Newtonsoft.Json;

namespace Symon.Server {
    class ConfigManager {
        private Config config = new Config();

        public bool Load(string jsonPath) {

            if (!File.Exists(jsonPath)) {
                return false;
            }
            using (StreamReader jsonReader = new StreamReader(jsonPath)) {
                string rawJson = jsonReader.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(rawJson);
            }
            return true;
        }

        public bool EnabledBroadcast() {
            return config.Broadcast;
        }
    }

    class Config {
        public bool Broadcast { get; set; } = true;
    }
}
