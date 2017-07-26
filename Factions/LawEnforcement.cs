using System;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;

namespace GTA_RP.Factions
{
    class LawEnforcement : Faction
    {
        public LawEnforcement(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB)
        {

        }

        /// <summary>
        /// Returns chat color for the law enforcement
        /// </summary>
        /// <returns>Color to use in chat for law enforcement players' names</returns>
        public override string GetChatColor()
        {
            return "~b~";
        }

        /// <summary>
        /// Handles on duty command for law enforcement players
        /// </summary>
        /// <param name="c">Character object</param>
        override public void HandleOnDutyCommand(Character c)
        {
            c.onDuty = true;
            c.owner.client.setSkin(PedHash.Sheriff01SMY);

            // Determine what to do based on rank
            c.owner.client.giveWeapon(WeaponHash.Nightstick, 100, true, true);
            c.owner.client.giveWeapon(WeaponHash.StunGun, 100, true, true);
            c.owner.client.giveWeapon(WeaponHash.Pistol, 100, true, true);
        }
    }
}
