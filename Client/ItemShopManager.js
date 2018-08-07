class ItemForSale {
    constructor(id, name, description, amount, price) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.amount = amount;
        this.price = price;
    }
}

class BuyableItem {
    constructor(id, name, description, amount, price) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.amount = amount;
        this.price = price;
    }
}

/**
 * Class for managing the shop menus and such.
 * Everything related to the shop should be here.
 */
class ItemShopManager {
    constructor() {
        this.itemsForSale = [];
        this.canSellItems = [];
        this.shopId = null;
        this.shopName = null;
        this.sellItemsMenu = null;
        this.sellItemsMenuItems = [];

        this.mainShopMenu = null;
        this.sellMenuButton = null;
        this.modifiers = [];
    }

    handleItemShopEvent(eventName, args) {
        if (eventName == "EVENT_OPEN_ITEM_SHOP_MENU") {
            this.initializeItems(args[2], args[3], args[4], args[5], args[6]);
            this.initializeCanSellItems(args[7], args[8], args[9], args[10], args[11]);
            this.shopId = args[0];
            this.shopName = args[1];
            this.createShopMenu();
        } else if (eventName == "EVENT_UPDATE_SELL_ITEM_COUNT") {
            this.updateSellItemCount(args[0], args[1]);
        }
    }

    initializeItems(ids, names, descriptions, amounts, prices) {
        this.itemsForSale = [];
        for (var i = 0; i < ids.Count; i++) {
            this.itemsForSale.push(new ItemForSale(ids[i], names[i], descriptions[i], amounts[i], prices[i]));
        }
    }

    initializeCanSellItems(ids, names, descriptions, amounts, prices) {
        this.canSellItems = [];
        this.sellItemsMenuItems = [];
        for (var i = 0; i < ids.Count; i++) {
            this.canSellItems.push(new BuyableItem(ids[i], names[i], descriptions[i], amounts[i], prices[i]));
        }
    }


    updateSellItemCount(itemId, count) {
        for (var i = 0; i < this.canSellItems.length; i++) {
            let item = this.canSellItems[i];
            if (item.id == itemId) {
                this.canSellItems[i].amount = count;
                this.updateSellMenu();
                return;
            }
        }

        for (var i = 0; i < this.itemsForSale.length; i++) {
            let item = this.itemsForSale[i];
            if (itemId == item.id) {
                this.canSellItems.push(new BuyableItem(itemId, item.name, item.description, count, item.price / 2));
                this.updateSellMenu();
                return;
            }
        }
    }

    createShopMenu() {
        let menu = API.createMenu(this.shopName, "Options", 0, 0, 6);
        this.mainShopMenu = menu;
        let item1 = API.createMenuItem("Buy items", "");
        let item2 = API.createMenuItem("Sell items", "");
        this.sellMenuButton = item2;

        menu.BindMenuToItem(this.createShopBuyMenu(), item1);
        menu.BindMenuToItem(this.createShopSellMenu(), item2);

        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.Visible = true;
    }

    updateSellMenu() {
        this.mainShopMenu.ReleaseMenuFromItem(this.sellMenuButton);
        this.mainShopMenu.BindMenuToItem(this.createShopSellMenu(), this.sellMenuButton);
    }

    closeShopMenu() {
        //API.closeAllMenus();
    }

    createShopBuyMenu() {
        let menu = API.createMenu(this.shopName, "Items on sale", 0, 0, 6);
        for (var i = 0; i < this.itemsForSale.length; i++) {
            let item = API.createMenuItem(this.itemsForSale[i].name, this.itemsForSale[i].description);
            item.SetRightLabel("$" + this.itemsForSale[i].price.toString());
            let itemId = this.itemsForSale[i].id;
            item.Activated.connect(() => this.buyItem(this.shopId, itemId, 1));
            menu.AddItem(item);
        }
        return menu;
    }

    buyItem(shopId, itemId, count) {
        API.triggerServerEvent("EVENT_TRY_BUY_ITEM", shopId, itemId, count);
    }

    sellItem(shopId, itemId, count, name, description, position, menu, item) {

        var modifier = 0;
        for (var i = 0; i < this.modifiers.length; i++) {
            let mod = this.modifiers[i];
            if (mod < position) {
                modifier -= 1;
            }
        }

        API.triggerServerEvent("EVENT_TRY_SELL_ITEM", shopId, itemId, count);
        var oldCount = this.canSellItems[position+modifier].amount;
        var newCount = oldCount - count;


        if (newCount > 0) {
            item.Text = name + " (" + newCount.toString() + ")";
            this.canSellItems[position+modifier].amount = newCount;
        } else {
            menu.RemoveItemAt(position+modifier);
            this.canSellItems.splice(position+modifier, 1);
            this.modifiers.push(position);
        }
    }

    createShopSellMenu() {
        let menu = API.createMenu(this.shopName, "Sell items", 0, 0, 6);
        this.modifiers = [];
        this.sellItemsMenu = menu;
        for (var i = 0; i < this.canSellItems.length; i++) {
            let item = API.createMenuItem(this.canSellItems[i].name + " (" + this.canSellItems[i].amount.toString() + ")", this.canSellItems[i].description);
            item.SetRightLabel("$" + this.canSellItems[i].price.toString());
            let itemId = this.canSellItems[i].id;
            let amount = this.canSellItems[i].amount;
            let name = this.canSellItems[i].name;
            let desc = this.canSellItems[i].description;
            let position = i;
            item.Activated.connect(() => this.sellItem(this.shopId, itemId, 1, name, desc, position, menu, item));
            menu.AddItem(item);
        }
        return menu;
    }
}

let itemShopManager = new ItemShopManager();
API.onServerEventTrigger.connect((eventName, args) => itemShopManager.handleItemShopEvent(eventName, args));