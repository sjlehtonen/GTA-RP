using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Misc;

namespace GTA_RP.IPLManager
{
    /// <summary>
    /// A class that manages loading of interiors
    /// </summary>
    class IPLManager : Singleton<IPLManager>
    {
        private static IPLManager _instance = null;
        private List<String> loadedIpls = new List<String>();

        public IPLManager()
        {
            /*if (_instance == null)
            {
                _instance = this;
            }*/
        }

        /// <summary>
        /// Gets the instance of the IPLmanager
        /// </summary>
        /// <returns>Instance of IPLManager</returns>
        /*public static IPLManager Instance()
        {
            if (_instance == null)
                _instance = new IPLManager();
            return _instance;
        }*/

        /// <summary>
        /// Checks if certain IPL is loaded
        /// </summary>
        /// <param name="iplName">Name of the IPL to check for</param>
        /// <returns>True if IPL is loaded, false otherwise</returns>
        public Boolean IsIplLoaded(String iplName)
        {
            return loadedIpls.Contains(iplName);
        }

        /// <summary>
        /// Loads an IPL with specified name
        /// </summary>
        /// <param name="iplName">IPL to load</param>
        public void LoadIPL(String iplName)
        {
            if (!IsIplLoaded(iplName))
            {
                API.shared.requestIpl(iplName);
                loadedIpls.Add(iplName);
            }
        }

        /// <summary>
        /// Unloads an IPL
        /// </summary>
        /// <param name="iplName">IPL to unload</param>
        public void UnloadIPL(String iplName)
        {
            if (IsIplLoaded(iplName))
            {
                API.shared.removeIpl(iplName);
                loadedIpls.Remove(iplName);
            }
        }

    }
}
