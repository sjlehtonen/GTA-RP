using System.Collections.Generic;
using System.Linq;
using GTA_RP.Misc;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Items
{
    /// <summary>
    /// Class that is responsible for dealing with items
    /// </summary>
    class ItemManager : Singleton<ItemManager>
    {
        private List<ItemTemplate> itemTemplates = new List<ItemTemplate>();
        private List<ItemShop> itemShops = new List<ItemShop>();

        public ItemManager() { }

        /// <summary>
        /// Loads items from database
        /// </summary>
        public void InitializeItemManager()
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

        /// <summary>
        /// Initializes all item shops
        /// TODO: Move these to database.
        /// </summary>
        private void InitializeItemShops()
        {
            itemShops.Add(new ItemShop(0, "247 Supermarket", new Vector3(-3039.459f, 586.1381f, 6.97893f)));
            itemShops.Add(new ItemShop(1, "Rob's Liquor", new Vector3(-1487.462f, -379.331f, 39.18343f)));
            itemShops.Add(new ItemShop(2, "LTD Gasoline", new Vector3(-48.61258f, -1757.845f, 28.52101f)));
            itemShops.Add(new ItemShop(3, "Rob's Liquor", new Vector3(1136.012f, -982.1758f, 45.43584f)));
            itemShops.Add(new ItemShop(4, "LTD Gasoline", new Vector3(1163.478f, -324.059f, 68.20506f)));
            itemShops.Add(new ItemShop(5, "247 Supermarket", new Vector3(2557.196f, 382.482f, 107.7229f)));
            itemShops.Add(new ItemShop(6, "247 Supermarket", new Vector3(374.0887f, 325.9345f, 102.5664f)));
            itemShops.Add(new ItemShop(7, "LTD Gasoline", new Vector3(-707.8062f, -914.7662f, 18.31559f)));
            itemShops.Add(new ItemShop(8, "Rob's Liquor", new Vector3(-1223.017f, -906.9675f, 11.34636f)));
            itemShops.Add(new ItemShop(9, "LTD Gasoline", new Vector3(-1820.645f, 792.2414f, 137.1385f)));

            /// TODO: move these to database, should be easy
            /// Currently all shops have same items with same prices.
            foreach (ItemShop shop in itemShops)
            {
                shop.AddItemForSale(0, 10230);
                shop.AddItemForSale(1, 450);
                shop.AddItemForSale(2, 100);
                shop.AddItemForBuy(3, 25000);
                shop.AddItemForSale(6, 25);
                shop.AddItemForBuy(5, 75);
                shop.AddItemForBuy(7, 150);
            }
        }

        /// <summary>
        /// Gets an item shop with certain id
        /// </summary>
        /// <param name="shopId">Id of the shop</param>
        /// <returns>Item shop if found or else null</returns>
        private ItemShop GetItemShopForId(int shopId)
        {
            return itemShops.SingleOrDefault(x => x.id == shopId);
        }

        /// <summary>
        /// Loads inventure for character
        /// </summary>
        /// <param name="character">Character</param>
        public void LoadInventoryForCharacter(Character character)
        {
            DBManager.SelectQuery("SELECT * FROM items WHERE owner_id=@id", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                character.AddItemToInventory(ItemsFactory.CreateItemForId(reader.GetInt32(1), reader.GetInt32(2)), false, false);
            }).AddValue("@id", character.ID).Execute();
        }

        /// <summary>
        /// Gets item template of item with certain id
        /// </summary>
        /// <param name="id">Id of the item</param>
        /// <returns>Item</returns>
        public ItemTemplate GetItemTemplateForId(int id)
        {
            return itemTemplates.SingleOrDefault(x => x.id == id);
        }

        /// <summary>
        /// Uses an item for character (character has to have the item in inventory)
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="itemId">Item id</param>
        public void TryUseItemForCharacter(Character character, int itemId)
        {
            character.UseItemInInventory(itemId);
        }

        public Item CreateItemForId(int id)
        {
            return ItemsFactory.CreateItemForId(id);
        }

        /// <summary>
        /// Tries to buy an item for character
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="shopId">Id of shop</param>
        /// <param name="itemId">Id of item</param>
        /// <param name="count">Amount of item</param>
        public void TryBuyItemForCharacter(Character character, int shopId, int itemId, int count)
        {
            ItemShop shop = GetItemShopForId(shopId);
            if (shop != null)
            {
                shop.BuyItem(character, itemId, count);
            }
        }

        /// <summary>
        /// Attempts to sell an item
        /// </summary>
        /// <param name="character">Sekller</param>
        /// <param name="shopId">Shop id</param>
        /// <param name="itemId">Item id</param>
        /// <param name="count">Amount to sell</param>
        public void TrySellItemForCharacter(Character character, int shopId, int itemId, int count)
        {
            ItemShop shop = GetItemShopForId(shopId);
            if (shop != null)
            {
                shop.SellItem(character, itemId, count);
            }
        }
    }
}
