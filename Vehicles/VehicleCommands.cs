using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

namespace GTA_RP.Vehicles
{
    class VehicleCommands : Script
    {

        [Command("lock")]
        public void LockVehicle(Client client)
        {
            VehicleManager.Instance().LockVehicle(client);
        }

    }
}
