using System.Collections.Generic;
using System.Linq;

namespace GTA_RP.Items
{
    /// <summary>
    /// Class for player inventories
    /// </summary>
    public class Inventory
    {
        private Character owner;
        private List<Item> items = new List<Item>();
        private int maxInventorySize = 30;

        public Inventory(Character owner)
        {
            this.owner = owner;
        }

        private int GetInventorySize()
        {
            int len = 0;
            this.items.ForEach(x => len += x.count);
            return len;
        }


        /// <summary>
        /// Adds an item to the inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        public bool AddItem(Item item, bool updateDB = true)
        {
            if (GetInventorySize() + 1 >= this.maxInventorySize) return false;

            foreach (Item i in items)
            {
                if (i.id == item.id)
                {
                    i.count += item.count;
                    if (updateDB)
                    {
                        DBManager.UpdateQuery("UPDATE items SET amount=@amount WHERE owner_id=@id AND item_id=@item_id")
                            .AddValue("@amount", i.count)
                            .AddValue("@id", owner.ID)
                            .AddValue("@item_id", i.id)
                            .Execute();
                    }
                    return true;
                }
            }

            item.AddedToInventory(owner);
            items.Add(item);
            if (updateDB)
            {
                DBManager.InsertQuery("INSERT INTO items VALUES (@owner_id, @item_id, @amount)")
                    .AddValue("@owner_id", this.owner.ID)
                    .AddValue("@item_id", item.id)
                    .AddValue("@amount", item.count)
                    .Execute();
            }
            return true;
        }

        public bool DoesContainItemWithIdAndCount(int id, int count)
        {
            Item item = items.SingleOrDefault(x => x.id == id);
            if (item != null)
            {
                if (item.count >= count)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public int GetItemCount(int itemId)
        {
            Item item = items.SingleOrDefault(x => x.id == itemId);
            if (item == null)
            {
                return 0;
            }
            return item.count;
        }

        /// <summary>
        /// Removes an item from inventory with id
        /// </summary>
        /// <param name="id">Id of the item</param>
        /// <param name="count">Amount of items in the inventory</param>
        public void RemoveItemWithId(int id, int count)
        {
            Item item = items.SingleOrDefault(x => x.id == id);
            if (item == null)
            {
                return;
            }

            if (item.count <= count)
            {
                items.Remove(item);
                DBManager.DeleteQuery("DELETE FROM items WHERE owner_id=@owner_id AND item_id=@item_id")
                    .AddValue("@owner_id", this.owner.ID)
                    .AddValue("@item_id", id)
                    .Execute();
            }
            else
            {
                item.count -= count;
                DBManager.UpdateQuery("UPDATE items SET amount=@amount WHERE owner_id=@owner_id AND item_id=@item_id")
                    .AddValue("@amount", item.count)
                    .AddValue("@owner_id", this.owner.ID)
                    .AddValue("@item_id", id)
                    .Execute();
            }
        }

        /// <summary>
        /// Uses item with certain id
        /// </summary>
        /// <param name="id">Id of the item</param>
        public void UseItemWithId(int id)
        {
            Item item = items.SingleOrDefault(x => x.id == id);
            if (item != null)
            {
                item.Use(this.owner);
            }
        }

        /// <summary>
        /// Return all items sorted in alphabetical order
        /// </summary>
        /// <returns>All items in inventory</returns>
        public List<Item> GetAlItems()
        {
            List<Item> items = new List<Item>();
            items.AddRange(this.items);
            items.Sort((x, y) => string.Compare(x.name, y.name));
            return items;
        }
    }
}
