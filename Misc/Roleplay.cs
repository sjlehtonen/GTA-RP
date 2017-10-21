using GrandTheftMultiplayer.Server.API;
using GTA_RP.Vehicles;
using GTA_RP.Weather;

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
            API.consoleOutput("####Script started!####");
            WeatherManager weatherManager = new WeatherManager();

            API.onVehicleDeath += VehicleManager.Instance().VehicleDestroyedEvent;
            API.onPlayerEnterVehicle += VehicleManager.Instance().VehicleEnterEvent;
            API.onPlayerExitVehicle += VehicleManager.Instance().VehicleExitEvent;


            //API.onPlayerConnected += PlayerManager.Instance().HandlePlayerConnect;
            API.onPlayerDisconnected += PlayerManager.Instance().HandlePlayerDisconnect;
            API.onPlayerFinishedDownload += PlayerManager.Instance().HandlePlayerConnect;

            PlayerManager.Instance().InitAccountCreationId();
            PlayerManager.Instance().InitTextMessagesId();
            PlayerManager.Instance().InitCharacterCreationId();
            PlayerManager.Instance().InitCharacterSelectorModels();
            PlayerManager.Instance().InitPhoneNumbers();
            PlayerManager.Instance().InitCharacterGenders();

            VehicleManager.Instance().LoadVehiclesFromDB();
            HouseManager.Instance().LoadHouseTemplates();
            API.consoleOutput("####Script load complete!####");
        }
    }
}
