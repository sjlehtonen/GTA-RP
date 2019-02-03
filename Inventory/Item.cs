
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
            this.id = id;
            this.description = description;
            this.name = name;
            this.count = count;
        }

        public override bool Equals(object obj)
        {
            var second = obj as Item;

            if (second == null)
            {
                return false;
            }

            return this.id.Equals(second.id);
        }

        public override int GetHashCode()
        {
            return this.id;
        }

        /// <summary>
        /// Destroys the item and removes it from character's inventory
        /// </summary>
        protected void ConsumeItem(Character character)
        {
            character.RemoveItemFromInventory(this.id, 1, true, true);
        }

        /// <summary>
        /// This method uses the item
        /// </summary>
        /// <param name="user">Character that uses the item</param>
        public virtual void Use(Character user) { }

        /// <summary>
        /// Method that is ran when the item is added to the inventory of the owner
        /// </summary>
        /// <param name="owner">Character to whose inventory the item is added to</param>
        public virtual void AddedToInventory(Character owner) { }

        /// <summary>
        /// Method that is ran when the item is removed from the inventory of the owner
        /// </summary>
        /// <param name="owner">Character from whose inventory the item is removed from</param>
        public virtual void RemovedFromInventory(Character owner) { }

    }
}
