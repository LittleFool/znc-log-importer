using System;
using System.IO;

namespace ZNC_Log_Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = Config.Instance;
            config.readConfigFile(args[0]);

            string[] files = Directory.GetFiles(config.getValue("Logs", "folder", ""), "*.log");
            foreach (var item in files)
            {
                FileParser fp = new FileParser(item);
                fp.parse();
            }
        }
    }
}
