using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

namespace GTA_RP.Factions
{
    class FactionCommands : Script
    {
        /// <summary>
        /// Handles /duty command and directs it to the right class
        /// </summary>
        /// <param name="player"></param>
        [Command("duty")]
        public void toggleDuty(Client player)
        {
            if (PlayerManager.Instance().IsClientUsingCharacter(player))
            {
                Character character = PlayerManager.Instance().GetActiveCharacterForClient(player);
                FactionManager.Instance().HandleDutyCommand(character);
            }
        }
    }
}
