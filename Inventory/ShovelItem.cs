using System.Timers;

namespace GTA_RP.Items
{
    /// <summary>
    /// Item class for shovel
    /// Planned for treasure hunter job
    /// TODO
    /// </summary>
    class ShovelItem : UseAnimationItem
    {
        private int diggingTime;
        private Timer diggingTimer;
        public ShovelItem(int id, string name, string description, int entityId, int bone, string other, int diggingTime, int amount = 1) : base(id, name, description, entityId, bone, other, amount)
        {
            this.diggingTime = diggingTime;
            this.diggingTimer = new Timer();
        }

        private void StartDigging()
        {

        }

        protected override bool EquipItem(Character user)
        {
            // Code here
            // 1. Check job and location
            return true;
        }

        protected override bool UnEquipItem(Character user)
        {
            return false;
        }
    }
}
