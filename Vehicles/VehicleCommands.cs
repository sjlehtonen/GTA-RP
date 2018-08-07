using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

namespace GTA_RP.Vehicles
{
    class VehicleCommands : Script
    {
        /// <summary>
        /// Command to lock a vehicle
        /// </summary>
        /// <param name="client"></param>
        [Command("lock")]
        public void LockVehicle(Client client)
        {
            VehicleManager.Instance().LockVehicle(client);
        }

    }
}
