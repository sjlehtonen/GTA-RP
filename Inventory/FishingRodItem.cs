using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA_RP.Map;

namespace GTA_RP.Items
{
    class FishingRodItem : UseAnimationItem
    {
        public FishingRodItem(int id, string name, string description, int entityId, int bone, string other, int amount = 1) : base(id, name, description, entityId, bone, other, amount) { }

        protected override bool EquipItem(Character user)
        {
            // Check that player is in fishing area etc
            return MapManager.Instance().StartFishing(user);
        }

        protected override bool UnEquipItem(Character user)
        {
            MapManager.Instance().StopFishing(user);
            return true;
        }

        public void StopFishing(Character user)
        {
            this.ForceUnequip(user);
        }
    }
}
