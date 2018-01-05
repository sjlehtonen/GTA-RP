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
        ITEM_WEAPON = 1
    }

    class ItemsFactory
    {
        static private Item CreateWeapon(int id, int hash, string name, string description)
        {
            return new Weapon(id, name, description, hash, 1);
        }

        static public Item CreateItemForId(int id, int amount = 1)
        {
            ItemTemplate template = ItemManager.Instance().GetItemTemplateForId(id);
            if (template == null) return null;
            return CreateItemForTemplate(template, amount);
        }

        static public Item CreateItemForTemplate(ItemTemplate template, int amount = 1)
        {
            if (template.type == ItemType.ITEM_WEAPON) return CreateWeapon(template.id, template.field1, template.name, template.description);
            return null;
        }
    }
}
