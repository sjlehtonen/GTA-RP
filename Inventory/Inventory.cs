using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Items
{
    /// <summary>
    /// Class for player inventories
    /// </summary>
    public class Inventory
    {
        private Character owner;
        private List<Item> items = new List<Item>();

        public Inventory(Character owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds an item to the inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItem(Item item)
        {
            foreach (Item i in items)
            {
                if (i.id == item.id)
                {
                    i.count += item.count;
                    return;
                }
            }

            item.AddedToInventory(owner);
            items.Add(item);
        }

        /// <summary>
        /// Removes an item from inventory with id
        /// </summary>
        /// <param name="id">Id of the item</param>
        /// <param name="count">Amount of items in the inventory</param>
        public void RemoveItemWithId(int id, int count)
        {

        }

        /// <summary>
        /// Uses item with certain id
        /// </summary>
        /// <param name="id">Id of the item</param>
        public void UseItemWithId(int id)
        {
            Item item = items.SingleOrDefault(x => x.id == id);
            if (item != null) item.Use(this.owner);
        }

        /// <summary>
        /// Return all items sorted in alphabetical order
        /// </summary>
        /// <returns>All items in inventory</returns>
        public List<Item> GetAllItems()
        {
            List<Item> items = new List<Item>();
            items.AddRange(this.items);
            items.Sort((x, y) => string.Compare(x.name, y.name));
            return items;
        }
    }
}
