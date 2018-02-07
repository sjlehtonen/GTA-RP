using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Items
{
    enum ItemType
    {
        ITEM_STANDARD = 0,
        ITEM_WEAPON = 1,
        ITEM_USE_ANIMATE = 2,
        ITEM_FOOD = 3
    }

    class ItemsFactory
    {
        static private Item CreateWeapon(int id, int hash, string name, string description, int amount = 1)
        {
            return new Weapon(id, name, description, hash, 100, amount);
        }

        static private Item CreateUseAnimateItem(int id, string name, string description, int entityId, int bone, string other, int amount = 1)
        {
            return new UseAnimationItem(id, name, description, entityId, bone, other, amount);
        }

        static private Item CreateFoodItem(int id, string name, string description, int entityId, int bone, string other, int health, int amount = 1)
        {
            return new FoodItem(id, name, description, entityId, bone, other, health, amount);
        }

        static public Item CreateItemForId(int id, int amount = 1)
        {
            ItemTemplate template = ItemManager.Instance().GetItemTemplateForId(id);
            if (template == null) return null;
            return CreateItemForTemplate(template, amount);
        }

        static public Item CreateItemForTemplate(ItemTemplate template, int amount = 1)
        {
            if (template.type == ItemType.ITEM_WEAPON) return CreateWeapon(template.id, template.field1, template.name, template.description, amount);
            else if (template.type == ItemType.ITEM_USE_ANIMATE) return CreateUseAnimateItem(template.id, template.name, template.description, template.field1, template.field2, template.field4, amount);
            else if (template.type == ItemType.ITEM_FOOD) return CreateFoodItem(template.id, template.name, template.description, template.field1, template.field2, template.field4, template.field3, amount);
            return null;
        }
    }
}
