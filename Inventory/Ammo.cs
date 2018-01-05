using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Items
{
    class Ammo : Item
    {
        private int weaponId;

        public Ammo(int id, string name, int weaponId, string description, int count = 0) : base(id, name, description, count)
        {
            this.weaponId = weaponId;
        }

        public override void Use(Character user)
        {
            // Put ammo to weapon if weapon equipped
        }

        public override void AddedToInventory(Character owner)
        {

        }

        public override void RemovedFromInventory(Character owner)
        {

        }
    }
}
