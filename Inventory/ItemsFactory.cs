
namespace GTA_RP.Items
{
    /// <summary>
    /// Item types
    /// Add new item types here if you want to create new kinds of items
    /// </summary>
    public enum ItemType
    {
        ITEM_NO_USE = 0,
        ITEM_WEAPON = 1,
        ITEM_USE_ANIMATE = 2,
        ITEM_FOOD = 3,
        ITEM_FISHING_ROD = 4,
        ITEM_SHOVEL = 5
    }

    /// <summary>
    /// Factory class for creating items.
    /// </summary>
    class ItemsFactory
    {
        static private Item CreateWeapon(int id, int hash, string name, string description, int amount = 1)
        {
            return new Weapon(id, name, description, hash, 100, amount);
        }

        static private Item CreateNoUseItem(int id, string name, string description, int amount = 1)
        {
            return new NoUseItem(id, name, description, amount);
        }

        static private Item CreateUseAnimateItem(int id, string name, string description, int entityId, int bone, string other, int amount = 1)
        {
            return new UseAnimationItem(id, name, description, entityId, bone, other, amount);
        }

        static private Item CreateFishingRodItem(int id, string name, string description, int entityId, int bone, string other, int amount = 1)
        {
            return new FishingRodItem(id, name, description, entityId, bone, other, amount);
        }

        static private Item CreateFoodItem(int id, string name, string description, int entityId, int bone, string other, int health, int amount = 1)
        {
            return new FoodItem(id, name, description, entityId, bone, other, health, amount);
        }

        static private Item CreateShovelItem(int id, string name, string description, int entityId, int bone, string other, int diggingTime, int amount = 1)
        {
            return new ShovelItem(id, name, description, entityId, bone, other, diggingTime, amount);
        }

        static public Item CreateItemForId(int id, int amount = 1)
        {
            ItemTemplate template = ItemManager.Instance().GetItemTemplateForId(id);
            if (template == null) {
                return null;
            }
            return CreateItemForTemplate(template, amount);
        }

        /// <summary>
        /// Creates an actual item object from item template
        /// </summary>
        /// <param name="template">Item template</param>
        /// <param name="amount">Amount</param>
        /// <returns>Item object</returns>
        static public Item CreateItemForTemplate(ItemTemplate template, int amount = 1)
        {
            if (template.type == ItemType.ITEM_WEAPON) return CreateWeapon(template.id, template.field1, template.name, template.description, amount);
            else if (template.type == ItemType.ITEM_USE_ANIMATE) return CreateUseAnimateItem(template.id, template.name, template.description, template.field1, template.field2, template.field4, amount);
            else if (template.type == ItemType.ITEM_FOOD) return CreateFoodItem(template.id, template.name, template.description, template.field1, template.field2, template.field4, template.field3, amount);
            else if (template.type == ItemType.ITEM_NO_USE) return CreateNoUseItem(template.id, template.name, template.description, amount);
            else if (template.type == ItemType.ITEM_FISHING_ROD) return CreateFishingRodItem(template.id, template.name, template.description, template.field1, template.field2, template.field4, amount);
            else if (template.type == ItemType.ITEM_SHOVEL) return CreateShovelItem(template.id, template.name, template.description, template.field1, template.field2, template.field4, template.field3, amount);
            return null;
        }
    }
}
