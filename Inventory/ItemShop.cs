using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.API;
using GTA_RP.Items;
using GTA_RP.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA_RP.Map;

namespace GTA_RP.Items
{
    struct ItemForSale
    {
        public ItemForSale(ItemTemplate template, int price, int amount)
        {
            this.template = template;
            this.price = price;
            this.amount = amount;
        }

        public ItemTemplate template;
        public int price;
        public int amount;
    }

    struct BuyableItem
    {
        public int id;
        public int price;

        public BuyableItem(int id, int price)
        {
            this.id = id;
            this.price = price;
        }
    }

    /// <summary>
    /// Class responsible for shops where items are sold
    /// </summary>
    public class ItemShop
    {
        private int id;
        private List<ItemForSale> stock = new List<ItemForSale>();
        private List<Item> sellableItems = new List<Item>();
        private Checkpoint storeCheckPoint;
        public ItemShop(int id, Vector3 position, bool addBlip = true)
        {
            this.storeCheckPoint = new Checkpoint(position, OnEnterShop, OnExitShop, 25, 1.6f, 0);
            if (addBlip) MapManager.Instance().AddBlipToMap(52, "247 Supermarket", position.X, position.Y, position.Z);
        }

        private void OnEnterShop(Checkpoint checkpoint, Character character)
        {
            // Open shop menu
            API.shared.triggerClientEvent(character.client, "EVENT_OPEN_ITEM_SHOP_MENU", stock.Select(x => x.template.id).ToList(), stock.Select(x => x.template.name).ToList(), stock.Select(x => x.template.description).ToList(), stock.Select(x => x.amount).ToList(), stock.Select(x => x.price).ToList());
        }

        private void OnExitShop(Checkpoint checkpoint, Character character)
        {
            // Close shop menu
            API.shared.triggerClientEvent(character.client, "EVENT_CLOSE_ITEM_SHOP_MENU");
        }

        /// <summary>
        /// Adds an item for sale to the shop
        /// </summary>
        /// <param name="id">ID of the item</param>
        /// <param name="price">Price of the item</param>
        public void AddItemForSale(int id, int price)
        {
            stock.Add(new ItemForSale(ItemManager.Instance().GetItemTemplateForId(id), price, 1));
        }

        /// <summary>
        /// Buys an item from the shop for character
        /// </summary>
        /// <param name="character">Buyer</param>
        /// <param name="itemId">Id of the item</param>
        /// <param name="count">How many to buy</param>
        public void BuyItem(Character character, int itemId, int count = 1)
        {
            
        }

        /// <summary>
        /// Sells an item to the shop
        /// </summary>
        /// <param name="character">Name of the character</param>
        /// <param name="id">Id of the item</param>
        /// <param name="count">How many items to buy</param>
        public void SellItem(Character character, int id, int count = 1)
        {

        }

    }
}
