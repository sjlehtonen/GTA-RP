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

class ItemShopManager {
    constructor() {
        this.itemsForSale = [];
        this.canSellItems = [];
        this.shopId = null;
        this.shopName = null;
    }

    handleItemShopEvent(eventName, args) {
        if (eventName == "EVENT_OPEN_ITEM_SHOP_MENU") {
            this.initializeItems(args[2], args[3], args[4], args[5], args[6]);
            this.initializeCanSellItems(args[7], args[8], args[9], args[10], args[11]);
            this.shopId = args[0];
            this.shopName = args[1];
            this.createShopMenu();
        } else if (eventName = "EVENT_CLOSE_ITEM_SHOP_MENU") {
            this.closeShopMenu();
        } else if (eventName == "EVENT_UPDATE_ITEM_SHOP_ITEM_AMOUNT") {

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
        for (var i = 0; i < ids.Count; i++) {
            this.canSellItems.push(new BuyableItem(ids[i], names[i], descriptions[i], amounts[i], prices[i]));
        }
    }

    createShopMenu() {
        let menu = API.createMenu(this.shopName, "Options", 0, 0, 6);
        let item1 = API.createMenuItem("Buy items", "");
        let item2 = API.createMenuItem("Sell items", "");

        menu.BindMenuToItem(this.createShopBuyMenu(), item1);
        menu.BindMenuToItem(this.createShopSellMenu(), item2);

        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.Visible = true;
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
        API.triggerServerEvent("EVENT_TRY_SELL_ITEM", shopId, itemId, count);
        var oldCount = this.canSellItems[position].amount;
        var newCount = oldCount - count;
        if (newCount > 0) {
            item.Text = name + " (" + newCount.toString() + ")";
            this.canSellItems[position].amount = newCount;
        } else {
            menu.RemoveItemAt(position);
        }
    }

    createShopSellMenu() {
        let menu = API.createMenu(this.shopName, "Sell items", 0, 0, 6);
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