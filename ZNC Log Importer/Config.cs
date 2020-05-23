using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;

namespace ZNC_Log_Importer
{
    public sealed class Config
    {
        private static Config instance = null;
        private static readonly object padlock = new object();

        private IniData data;

        private Config()
        {

        }

        public static Config Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }
                    return instance;
                }
            }
        }

        public void readConfigFile(string confFile)
        {
            if (!File.Exists(confFile))
            {
                throw new FileNotFoundException("Configuration file '" + confFile + "' not found!");
            }

            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(confFile);
            this.data = data;
        }

        public string getValue(string section, string key, string fallback)
        {
            return data[section][key] != null ? data[section][key] : fallback;
        }
    }
}
