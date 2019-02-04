using GrandTheftMultiplayer.Server.API;
using GTA_RP.Vehicles;
using GTA_RP.Weather;
using GTA_RP.Jobs;
using GTA_RP.Factions;
using GTA_RP.Items;
using GTA_RP.Map;

namespace GTA_RP
{
    /// <summary>
    /// Main body of the script
    /// </summary>
    public class Roleplay : Script
    {

        public Roleplay()
        {
            API.onResourceStart += StartScript;
        }

        /// <summary>
        /// Script startup function
        /// </summary>
        public void StartScript()
        {
            API.consoleOutput("#### Script started! ####");

            API.onVehicleDeath += VehicleManager.Instance().VehicleDestroyedEvent;
            API.onPlayerEnterVehicle += VehicleManager.Instance().VehicleEnterEvent;
            API.onPlayerExitVehicle += VehicleManager.Instance().VehicleExitEvent;


            API.onPlayerDisconnected += PlayerManager.Instance().HandlePlayerDisconnect;
            // When API.onPlayerConnected is fixed, change this
            API.onPlayerFinishedDownload += PlayerManager.Instance().HandlePlayerConnect;
            API.onPlayerDeath += PlayerManager.Instance().HandlePlayerDeath;


            PlayerManager.Instance().InitializePlayerManager();
            VehicleManager.Instance().InitializeVehicleManager();
            HouseManager.Instance().InitializeHouseManager();
            ItemManager.Instance().InitializeItemManager();
            MapManager.Instance().Initialize();
            JobManager.Instance().Initialize();
            FactionManager.Instance().InitializeFactions();

            API.consoleOutput("#### Script load complete! ####");
        }
    }
}
