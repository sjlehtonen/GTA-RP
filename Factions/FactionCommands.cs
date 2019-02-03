using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Factions
{
    class FactionCommands : Script
    {
        protected bool IsCharacterValid(Character character, FactionI faction)
        {
            if (character != null && character.factionID == faction)
            {
                return true;
            }
            return false;
        }

        protected bool IsCharacterValid(Client client, FactionI faction)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(client);
            return IsCharacterValid(character, faction);
        }

        protected bool IsCharacterValid(Character character)
        {
            if (character != null)
            {
                return true;
            }
            return false;
        }

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
