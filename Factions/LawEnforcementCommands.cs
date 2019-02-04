using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

namespace GTA_RP.Factions
{
    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    }

    class LawEnforcementCommands : FactionCommands
    {

        /// <summary>
        /// Command to arrest a character.
        /// </summary>
        /// <param name="client">Sender of the command</param>
        /// <param name="characterId">Character ID of the person to arrest</param>
        /// <param name="time">Time to arrest(in minutes)</param>
        /// <param name="reason">Reason for the arrest</param>
        [Command("arrest")]
        public void ArrestCharacter(Client client, int characterId, int time, string reason)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(client);
            if (IsCharacterValid(character, FactionEnums.LAW_ENFORCEMENT))
            {
                FactionManager.Instance().LawEnforcement().ArrestCharacter(character, characterId, time, reason);
            }
        }

        /// <summary>
        /// Command for giving a fine to character.
        /// </summary>
        /// <param name="client">Sender of the command</param>
        /// <param name="characterId">Character ID of the person to give fine to</param>
        /// <param name="fee">Fine amount</param>
        /// <param name="reason">Reason for the fine</param>
        [Command("givefine")]
        public void GiveTicketToCharacter(Client client, int characterId, int fee, string reason)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(client);
            if (IsCharacterValid(character, FactionEnums.LAW_ENFORCEMENT))
            {
                FactionManager.Instance().LawEnforcement().FineCharacter(character, characterId, reason, fee);
            }
        }
    }
}
