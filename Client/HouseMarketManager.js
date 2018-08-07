class HouseForSale {
    constructor(id, name, price, seller, buildingName) {
        this.id = id;
        this.name = name;
        this.price = price;
        this.seller = seller;
        this.buildingName = buildingName;
    }
}

/**
 * Class for managing the housing market.
 * In the future I plan to move this to a website
 * so it's more intuitive than the ingame UI.
 */
class HouseMarketManager {
    constructor() {
        this.housesForSale = [];
    }

    initializeHouses(houseIds, houseNames, housePrices, houseSellers, houseBuildingNames) {
        this.housesForSale = [];
        for (var i = 0; i < houseIds.Count; i++) {
            this.housesForSale.push(new HouseForSale(houseIds[i], houseNames[i], housePrices[i], houseSellers[i], houseBuildingNames[i]));
        }
    }

    closeHouseMenu() {
        API.closeAllMenus();
    }

    showHouseBuySellSelectMenu(houseIds, houseNames, housePrices, houseSellers, houseBuildingNames) {
        API.closeAllMenus();
        this.initializeHouses(houseIds, houseNames, housePrices, houseSellers, houseBuildingNames);

        let menu = API.createMenu("LS Properties", "Options", 0, 0, 6);
        let item1 = API.createMenuItem("Buy properties", "");
        let item2 = API.createMenuItem("Sell properties", "");

        menu.BindMenuToItem(this.getMarketMenu(), item1);

        menu.AddItem(item1);
        menu.AddItem(item2);

        menu.Visible = true;

    }

    getUserInputForHouseName(menu, item) {
        var name = API.getUserInput("", 20);
        item.Text = "Name: " + name;
    }

    tryBuyPropertyWithId(id, name) {
        API.triggerServerEvent("EVENT_TRY_BUY_PROPERTY", id, name);
    }

    getBuyHouseDetailMenu(id, name, price, seller, buildingName) {
        let menu = API.createMenu("LS Properties", "Building: " + buildingName, 0, 0, 6);
        let item1 = API.createMenuItem("Seller: " + seller, "");
        let item2 = API.createMenuItem("Price: $" + price.toString(), "");
        let item5 = API.createMenuItem("Name: " + name, "Press to rename house");
        let item4 = API.createColoredItem("Buy property", "", "#53a828", "#69d831");
        item4.Activated.connect(() => this.tryBuyPropertyWithId(id, item5.Text.substring(6)));
        item5.Activated.connect(() => this.getUserInputForHouseName(menu, item5));
        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item5);
        menu.AddItem(item4);
        return menu;
    }

    getMarketMenu() {
        let menu = API.createMenu("LS Properties", "Properties for sale", 0, 0, 6);
        for (var i = 0; i < this.housesForSale.length; i++) {
            let item = API.createMenuItem(this.housesForSale[i].name, "Seller: " + this.housesForSale[i].seller);
            item.SetRightLabel("$ " + this.housesForSale[i].price.toString());
            menu.AddItem(item);
            menu.BindMenuToItem(this.getBuyHouseDetailMenu(this.housesForSale[i].id, this.housesForSale[i].name, this.housesForSale[i].price, this.housesForSale[i].seller, this.housesForSale[i].buildingName), item);
        }
        return menu;
    }

    handleHouseMarketEvent(eventName, args) {
        if (eventName == "EVENT_OPEN_HOUSE_MARKET_MENU") {
            this.showHouseBuySellSelectMenu(args[0], args[1], args[2], args[3], args[4]);
        }
        else if (eventName == "EVENT_CLOSE_HOUSE_MARKET_MENU") {
            this.closeHouseMenu();
        }
    }
}

let houseMarketManager = new HouseMarketManager();
API.onServerEventTrigger.connect((eventName, args) => houseMarketManager.handleHouseMarketEvent(eventName, args));