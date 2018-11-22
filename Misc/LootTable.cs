using System;
using GTA_RP.Items;

namespace GTA_RP.Misc
{
    /// <summary>
    /// Class that represents loot from loot table
    /// </summary>
    public class Loot
    {
        public int chance, itemId; // chance is a number from 1 to 1000 which is the drop chance, -1 always drop
        public Loot(int itemId, int chance)
        {
            this.chance = chance;
            this.itemId = itemId;
        }
    }

    /// <summary>
    /// Class that represents a loot table
    /// For example used on fishing spots, could also be used on trasure hunting
    /// </summary>
    class LootTable
    {
        private int[] lootTable = new int[1000];
        private Random random = new Random();
        public LootTable(Loot[] loots)
        {
            int spot = 0;
            foreach (Loot loot in loots)
            {
                int i = 0;
                for (; i < loot.chance; i++) lootTable[i + spot] = loot.itemId;
                spot += i + 1;
            }
            for (; spot < 1000; spot++)
            {
                lootTable[spot] = -1;
            }
        }

        /// <summary>
        /// Creates an item for ID
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns>Item</returns>
        private Item CreateItemForId(int id)
        {
            if (id == -1)
            {
                return null;
            }
            return ItemManager.Instance().CreateItemForId(id);
        }

        /// <summary>
        /// Returns the item received for 1 roll
        /// </summary>
        /// <returns>Null if no item, otherwise return item</returns>
        public Item GetLoot()
        {
            int rdn = random.Next(0, 1000);
            int itemId = lootTable[rdn];
            return CreateItemForId(itemId);
        }
    }
}
