using IniParser;
using IniParser.Model;


namespace GTA_RP.Misc
{
    /// <summary>
    /// Class for accessing config file
    /// </summary>
    class ConfigManager
    {
        static private FileIniDataParser parser = new FileIniDataParser();
        static private IniData data = null;
        private const string location = "resources/GTA-RP/Config/Config.ini";


        /// <summary>
        /// Reads a string value from a section with a key
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        static public string ReadStringValue(string section, string key)
        {
            if (data == null)
            {
                data = parser.ReadFile(location);
            }
            return data[section][key];
        }
    }
}
