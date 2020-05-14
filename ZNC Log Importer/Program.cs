using System;
using System.IO;

namespace ZNC_Log_Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(@"C:\temp\", "*.log");
            foreach (var item in files)
            {
                FileParser fp = new FileParser(item);
                fp.parse();
            }
        }
    }
}
