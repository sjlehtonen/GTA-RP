using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Items
{
    /// <summary>
    /// Base class for weapon in inventory
    /// </summary>
    class Weapon : Item
    {
        private int ammo;
        private int hash;
        private bool equipped = false;

        public Weapon(int id, string name, string description, int hash, int ammo, int count = 1) : base(id, name, description, count)
        {
            this.hash = hash;
            this.ammo = ammo;
        }

        /// <summary>
        /// Loads ammo to the weapon
        /// </summary>
        /// <param name="amount">Amount of ammo to load</param>
        public void LoadAmmo(int amount)
        {
            this.ammo += amount;
        }

        /// <summary>
        /// Equips / Unequips the weapon
        /// </summary>
        /// <param name="user">Character who has the weapon</param>
        public override void Use(Character user)
        {
            if (!equipped)
            {
                user.client.giveWeapon((WeaponHash)this.hash, 100, true, true);
                equipped = true;
            }
            else
            {
                user.client.removeWeapon((WeaponHash)this.hash);
                equipped = false;
            }
        }

        public override void AddedToInventory(Character owner)
        {

        }

        public override void RemovedFromInventory(Character owner)
        {

        }
    }
}
