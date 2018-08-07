class VehicleInfo {
    constructor(model, price) {
        this.model = model;
        this.price = price;
    }
}

/**
 * Class for managing the vehicle shops.
 */
class VehicleShopManager {
    constructor() {
        this.id = null;
        this.vehicleInfos = [];
        this.vehicleMenu = null;
        this.buyVehicleMenu = null;
        this.customiseVehicleMenu = null;
        this.vehiclePosition = null;
        this.vehicleRotation = null;
        this.currentVehicle = null;
        this.colors = [];

        for (var i = 0; i < 160; i++)
            this.colors.push(i);

        this.currentVehicleModel = null;
        this.currentColor1 = 1;
        this.currentColor2 = 1;
    }

    onMenuClose(sender) {
        if (sender == this.vehicleMenu) {
            this.vehicleMenu.Visible = true;
        }
    }

    handleVehicleShopEvent(eventName, args) {
        if (eventName == "EVENT_CHARACTER_ENTER_VEHICLE_SHOP") {
            this.id = args[0];
            this.createVehicleMenu(args[1], args[2]);
            this.initVehicleModel(args[3], args[4]);
        }
        else if (eventName == "EVENT_CHARACTER_EXIT_VEHICLE_SHOP") {
            this.exitShop(null, null);
        }
    }

    initVehicles(models, prices) {
        this.vehicleInfos.length = 0;
        for (var i = 0; i < models.Count; i++) {
            this.vehicleInfos.push(new VehicleInfo(models[i], prices[i]));
        }
        this.currentVehicleModel = this.vehicleInfos[0].model;
    }

    setVehicle(model, color1, color2) {
        if (this.currentVehicle != null)
            API.deleteEntity(this.currentVehicle);
        this.currentVehicle = API.createVehicle(API.vehicleNameToModel(model), this.vehiclePosition, this.vehicleRotation);
        this.setVehicleColor(color1, color2);
        this.currentVehicleModel = model;
    }

    setVehicleColor(color1, color2) {
        API.setVehiclePrimaryColor(this.currentVehicle, color1);
        API.setVehicleSecondaryColor(this.currentVehicle, color2);
    }

    setVehicleColor1(color) {
        API.setVehiclePrimaryColor(this.currentVehicle, color);
        this.currentColor1 = color;
    }

    setVehicleColor2(color) {
        API.setVehicleSecondaryColor(this.currentVehicle, color);
        this.currentColor2 = color;
    }

    initVehicleModel(pos, rot) {
        this.vehiclePosition = pos;
        this.vehicleRotation = rot;
        this.setVehicle(this.vehicleInfos[0].model, 1, 1);
    }

    exitShop(menu, sender) {
        API.triggerServerEvent("EVENT_EXIT_VEHICLE_SHOP", this.id);
        this.vehicleMenu.Visible = false;
        this.customiseVehicleMenu.Visible = false;
    }

    createVehicleMenu(models, prices) {
        this.initVehicles(models, prices);

        let menu = API.createMenu("Vehicle dealership", "Options", 0, 0, 6);
        this.vehicleMenu = menu;
        let item1 = API.createMenuItem("Buy vehicle", "");
        let item2 = API.createMenuItem("Exit shop", "");
        menu.AddItem(item1);
        menu.AddItem(item2);

        menu.OnMenuClose.connect((sender) => this.onMenuClose(sender));
        item2.Activated.connect((menu, sender) => this.exitShop(menu, sender));

        var buyMenu = this.createBuyVehicleMenu();

        menu.BindMenuToItem(buyMenu, item1);
        menu.Visible = true;
    }


    onIndexChange(sender, newindex) {
        if (sender == this.buyVehicleMenu) {
            this.setVehicle(this.vehicleInfos[newindex].model, this.currentColor1, this.currentColor2);
        }
    }

    onListChange(sender, list, newindex) {
        if (list == this.color1item) {
            this.setVehicleColor1(parseInt(this.colors[newindex]));
        }

        if (list == this.color2item) {
            this.setVehicleColor2(parseInt(this.colors[newindex]));
        }
    }

    onItemSelect(sender, selectedItem, index) {
        if (sender == this.buyVehicleMenu) {
            this.setVehicleColor(this.currentColor1, this.currentColor2);
        }
    }

    buyVehicle() {
        API.triggerServerEvent("EVENT_BUY_VEHICLE", this.id, this.currentVehicleModel, this.currentColor1, this.currentColor2);
    }

    createVehicleCustomiseMenu(parent, buttonList) {
        let menu = API.createMenu("Vehicle dealership", "Customise vehicle", 0, 0, 6);
        this.customiseVehicleMenu = menu;
        menu.OnListChange.connect((sender, list, newindex) => this.onListChange(sender, list, newindex));

        var list = new List(String);
        for (var i = 0; i < 160; i++) {
            list.Add(i.toString());
        }

        let item1 = API.createListItem("Primary color", "", list, 0);
        let item2 = API.createListItem("Secondary color", "", list, 0);

        this.color1item = item1;
        this.color2item = item2;

        let item3 = API.createMenuItem("Buy vehicle", "Purchase the vehicle");
        item3.Activated.connect((menu, sender) => this.buyVehicle());
        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);

        for (var i = 0; i < buttonList.length; i++) {
            parent.BindMenuToItem(menu, buttonList[i]);
        }
    }

    createBuyVehicleMenu() {
        let menu = API.createMenu("Vehicle dealership", "Vehicles", 0, 0, 6);
        menu.OnIndexChange.connect((sender, newindex) => this.onIndexChange(sender, newindex));
        menu.OnItemSelect.connect((sender, selectedItem, index) => this.onItemSelect(sender, selectedItem, index));
        this.buyVehicleMenu = menu;
        var itemArray = [];

        for (var i = 0; i < this.vehicleInfos.length; i++) {
            let item = API.createMenuItem(this.vehicleInfos[i].model, "Select to customise and purchase vehicle");
            item.SetRightLabel("$" + this.vehicleInfos[i].price.toString());
            menu.AddItem(item);
            itemArray.push(item);
        }

        this.createVehicleCustomiseMenu(menu, itemArray);
        return menu;
    }
}

let vehicleShopManager = new VehicleShopManager();
API.onServerEventTrigger.connect((eventName, args) => vehicleShopManager.handleVehicleShopEvent(eventName, args));