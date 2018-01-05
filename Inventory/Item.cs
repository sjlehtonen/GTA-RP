using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Items
{
    /// <summary>
    /// Base class for item in inventory
    /// </summary>
    public abstract class Item
    {
        public int count { get; set; }
        public int id { get; private set; }
        public string name { get; private set; }

        public string description { get; private set; }

        public Item(int id, string name, string description, int count = 1)
        {
            this.description = description;
            this.name = name;
            this.count = count;
        }

        /// <summary>
        /// This method uses the item
        /// </summary>
        /// <param name="user">Character that uses the item</param>
        public abstract void Use(Character user);

        /// <summary>
        /// Method that is ran when the item is added to the inventory of the owner
        /// </summary>
        /// <param name="owner">Character to whose inventory the item is added to</param>
        public abstract void AddedToInventory(Character owner);

        /// <summary>
        /// Method that is ran when the item is removed from the inventory of the owner
        /// </summary>
        /// <param name="owner">Character from whose inventory the item is removed from</param>
        public abstract void RemovedFromInventory(Character owner);
        
    }
}
