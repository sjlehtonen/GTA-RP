using System;
using GTA_RP.Factions;
using GrandTheftMultiplayer.Server;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.PedManager
{
    class PedManager : Script
    {
        Ped ped;

        public PedManager()
        {

        }

        [Command("createnpc", Alias = "npcspawn")]
        public void CreatePed(Client player, String model)
        {
            Ped ped = API.createPed(API.pedNameToModel(model), player.position, 0);
            API.setEntityPositionFrozen(ped, false);
            API.setEntityInvincible(ped, false);

            API.sendNativeToAllPlayers(0x8E06A6FE76C9EFF4, ped, true);
            API.sendNativeToAllPlayers(0x304AE42E357B8C7E, ped, player, 1, 1, 0, 1, -1, 5.0, true);
        }
    }
}
