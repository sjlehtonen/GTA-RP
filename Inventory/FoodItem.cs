using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Items
{
    /// <summary>
    /// Class for food items that restore health or do other things when used.
    /// TODO: Implement animations
    /// </summary>
    class FoodItem : UseAnimationItem
    {
        private int healthToRestore;
        private int animationTime;


        public FoodItem(int id, string name, string description, int entityId, int bone, string other, int healthToRestore, int count = 1) : base(id, name, description, entityId, bone, other, count)
        {
            this.healthToRestore = healthToRestore;
        }

        public override void Use(Character user)
        {
            user.client.health += this.healthToRestore;
            user.PlayFrontendSound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            this.ConsumeItem(user);
            // TODO: Implement animation
        }
    }
}
