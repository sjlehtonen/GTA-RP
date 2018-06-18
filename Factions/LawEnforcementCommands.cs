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

        [Command("arrest")]
        public void ArrestCharacter(Client client, int characterId, int time, string reason)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(client);
            if (IsCharacterValid(character, FactionI.LAW_ENFORCEMENT))
                FactionManager.Instance().LawEnforcement().ArrestCharacter(character, characterId, time, reason);
        }

        [Command("givefine")]
        public void GiveTicketToCharacter(Client client, int characterId, int fee, string reason)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(client);
            if (IsCharacterValid(character, FactionI.LAW_ENFORCEMENT))
                FactionManager.Instance().LawEnforcement().FineCharacter(character, characterId, reason, fee);
        }
    }
}
