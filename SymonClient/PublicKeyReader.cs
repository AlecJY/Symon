using System;
using System.IO;

namespace Symon.Client {
    public class PublicKeyReader {
        public static string Read(string filename) {
            try {
                return File.ReadAllText(filename);
            } catch (FileNotFoundException e) {
                Console.Error.WriteLine("Cannot found private key file \"" + filename + "\"");
                Environment.Exit(-1);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
            return null;
        }
    }
}