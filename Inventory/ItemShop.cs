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
        public int id { private set; get; }
        public string name { private set; get; }
        private List<ItemForSale> stock = new List<ItemForSale>();
        private List<BuyableItem> sellableItems = new List<BuyableItem>();
        private Checkpoint storeCheckPoint;
        public ItemShop(int id, string name, Vector3 position, bool addBlip = true)
        {
            this.id = id;
            this.name = name;
            this.storeCheckPoint = new Checkpoint(position, OnEnterShop, OnExitShop, 25, 1.6f, 0);
            if (addBlip) MapManager.Instance().AddBlipToMap(52, name, position.X, position.Y, position.Z);
        }

        private List<Item> GetItemsThatCharacterHasAndShopBuys(Character character)
        {
            List<Item> items = character.GetAllItemsFromInventory();
            List<Item> both = new List<Item>();
            foreach(BuyableItem item in sellableItems)
            {
                if (items.Count(x => x.id == item.id) > 0)
                {
                    both.Add(items.Single(x => x.id == item.id));
                }
                    
            }
            return both;
        }

        private void OnEnterShop(Checkpoint checkpoint, Character character)
        {
            // Open shop menu
            List<Item> sellableItemsForCharacter = GetItemsThatCharacterHasAndShopBuys(character);
            List<int> sellableItemPrices = new List<int>();
            foreach (Item item in sellableItemsForCharacter) sellableItemPrices.Add(GetItemBuyPrice(item.id));

            API.shared.triggerClientEvent(character.client, "EVENT_OPEN_ITEM_SHOP_MENU", id, name, stock.Select(x => x.template.id).ToList(), stock.Select(x => x.template.name).ToList(), stock.Select(x => x.template.description).ToList(), stock.Select(x => x.amount).ToList(), stock.Select(x => x.price).ToList(), sellableItemsForCharacter.Select(x => x.id).ToList(), sellableItemsForCharacter.Select(x=> x.name).ToList(), sellableItemsForCharacter.Select(x => x.description).ToList(), sellableItemsForCharacter.Select(x => x.count).ToList(), sellableItemPrices);
        }

        private void OnExitShop(Checkpoint checkpoint, Character character)
        {
            // Close shop menu
            API.shared.triggerClientEvent(character.client, "EVENT_CLOSE_ITEM_SHOP_MENU");
        }

        private bool DoesSellItem(int itemId)
        {
            foreach (ItemForSale item in stock)
            {
                if (item.template.id == itemId)
                    return true;
            }
            return false;
        }

        private bool DoesBuyitem(int itemId)
        {
            foreach (BuyableItem item in sellableItems)
            {
                if (item.id == itemId)
                    return true;
            }
            return false;
        }

        private int GetItemBuyPrice(int itemId)
        {
            foreach (BuyableItem item in sellableItems)
            {
                if (item.id == itemId)
                    return item.price;
            }
            return -1;
        }

        private int GetItemSellPrice(int itemId)
        {
            foreach (ItemForSale item in stock)
            {
                if (item.template.id == itemId)
                    return item.price;
            }
            return -1;
        }

        /// <summary>
        /// Adds an item for sale to the shop
        /// </summary>
        /// <param name="id">ID of the item</param>
        /// <param name="price">Price of the item</param>
        public void AddItemForSale(int id, int price, bool canAlsoBuy = true)
        {
            stock.Add(new ItemForSale(ItemManager.Instance().GetItemTemplateForId(id), price, 1));
            if (canAlsoBuy)
                sellableItems.Add(new BuyableItem(id, (int)price / 2));
        }

        /// <summary>
        /// Buys an item from the shop for character
        /// </summary>
        /// <param name="character">Buyer</param>
        /// <param name="itemId">Id of the item</param>
        /// <param name="count">How many to buy</param>
        public void BuyItem(Character character, int itemId, int count = 1)
        {
            if (DoesSellItem(itemId) && storeCheckPoint.IsCharacterInsideCheckpoint(character))
            {
                int sellPrice = GetItemSellPrice(itemId) * count;
                if (character.money >= sellPrice)
                {
                    Item item = ItemsFactory.CreateItemForId(itemId, count);
                    character.AddItemToInventory(item, true, true);
                    character.SetMoney(character.money - sellPrice);
                    character.SendNotification("Item purchased!");
                    character.PlayFrontendSound("PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET");
                    character.TriggerEvent("EVENT_UPDATE_SELL_ITEM_COUNT", itemId, character.GetAmountOfItems(itemId));
                }
                else
                {
                    character.SendNotification("You don't have enough money!");
                    character.PlayFrontendSound("ERROR", "HUD_LIQUOR_STORE_SOUNDSET");
                }
            }
        }

        /// <summary>
        /// Sells an item to the shop
        /// </summary>
        /// <param name="character">Name of the character</param>
        /// <param name="id">Id of the item</param>
        /// <param name="count">How many items to buy</param>
        public void SellItem(Character character, int id, int count = 1)
        {
            if (!character.HasItemWithid(id, count)) return;
            if (!DoesBuyitem(id)) return;
            if (!storeCheckPoint.IsCharacterInsideCheckpoint(character)) return;

            character.RemoveItemFromInventory(id, count, true);
            character.SetMoney(character.money + GetItemBuyPrice(id) * count);
            //character.TriggerEvent("EVENT_UPDATE_SELL_ITEM_COUNT", id, character.GetAmountOfItems(id));
            character.PlayFrontendSound("PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET");
        }

    }
}
