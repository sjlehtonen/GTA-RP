using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA_RP.Misc;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Items
{
    class ItemManager : Singleton<ItemManager>
    {
        private List<ItemTemplate> itemTemplates = new List<ItemTemplate>();
        private List<ItemShop> itemShops = new List<ItemShop>();

        public ItemManager()
        {

        }

        /// <summary>
        /// Loads items from database
        /// </summary>
        public void InitializeItems()
        {
            DBManager.SelectQuery("SELECT * FROM item_templates", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                int field1 = 0, field2 = 0, field3 = 0;
                string field4 = "";
                if (!reader.IsDBNull(4)) field1 = reader.GetInt32(4);
                if (!reader.IsDBNull(5)) field2 = reader.GetInt32(5);
                if (!reader.IsDBNull(6)) field3 = reader.GetInt32(6);
                if (!reader.IsDBNull(7)) field4 = reader.GetString(7);
                itemTemplates.Add(new ItemTemplate(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), field1, field2, field3, field4));
            }).Execute();

            InitializeItemShops();
        }

        private void InitializeItemShops()
        {
            ItemShop shop = new ItemShop(0, new Vector3(-3039.459f, 586.1381f, 6.97893f));
            shop.AddItemForSale(0, 10230);
            itemShops.Add(shop);
        }

        public void LoadInventoryForCharacter(Character character)
        {
            // Fill inventory from database
            DBManager.SelectQuery("SELECT * FROM items WHERE owner_id=@id", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                character.AddItemToInventory(ItemsFactory.CreateItemForId(reader.GetInt32(1), reader.GetInt32(2)));
            }).AddValue("@id", character.ID)
            .Execute();
        }

        public ItemTemplate GetItemTemplateForId(int id)
        {
            return itemTemplates.SingleOrDefault(x => x.id == id);
        }

        public void TryUseItemForCharacter(Character character, int itemId)
        {
            character.UseItemInInventory(itemId);
        }
    }
}
