using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA_RP.Misc;
using GTA_RP.Map;
using IniParser;
using IniParser.Model;


namespace GTA_RP.Database
{
    class ConfigManager : Singleton<ConfigManager>
    {
        private FileIniDataParser parser;
        private IniData data;
        private const string location = "resources/GTA-RP/Config/Config.ini";
        public ConfigManager()
        {
            parser = new FileIniDataParser();
            data = parser.ReadFile(location);
        }

        /// <summary>
        /// Reads a string value from a section with a key
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public string ReadStringValue(string section, string key)
        {
            return data[section][key];
        }
    }
}
