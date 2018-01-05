class ItemForSale {
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
    }

    handleItemShopEvent(eventName, args) {
        if (eventName == "EVENT_OPEN_ITEM_SHOP_MENU") {
            this.initializeItems(args[0], args[1], args[2], args[3], args[4]);
            this.createShopMenu();
        }
    }

    initializeItems(ids, names, descriptions, amounts, prices) {
        for (var i = 0; i < ids.Count; i++) {
            this.itemsForSale.push(new ItemForSale(ids[i], names[i], descriptions[i], amounts[i], prices[i]));
        }
    }

    createShopMenu() {
        let menu = API.createMenu("247 Supermarket", "Options", 0, 0, 6);
        let item1 = API.createMenuItem("Buy items", "");
        let item2 = API.createMenuItem("Sell items", "");

        menu.BindMenuToItem(this.createShopBuyMenu(), item1);

        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.Visible = true;
    }

    createShopBuyMenu() {
        let menu = API.createMenu("247 Supermarket", "Items on sale", 0, 0, 6);
        for (var i = 0; i < this.itemsForSale.length; i++) {
            let item = API.createMenuItem(this.itemsForSale[i].name, this.itemsForSale[i].description);
            item.SetRightLabel("$" + this.itemsForSale[i].price.toString());
            menu.AddItem(item);
        }
        return menu;
    }

    createShopSellMenu() {

    }
}

let itemShopManager = new ItemShopManager();
API.onServerEventTrigger.connect((eventName, args) => itemShopManager.handleItemShopEvent(eventName, args));