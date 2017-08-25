class TextMessage {
    constructor(id, sender, message, time) {
        this.id = id;
        this.sender = sender;
        this.message = message;
        this.time = time;
    }
}

class Contact {
    constructor(name, number) {
        this.name = name;
        this.number = number;
    }
}

class Vehicle {
    constructor(id, licensePlate, spawned) {
        this.id = id;
        this.licensePlate = licensePlate;
        this.spawned = spawned;
    }
}

class House {
    constructor(id, name) {
        this.id = id;
        this.name = name;
    }
}

class HUDManager
{
    constructor() {
        this.hudActive = false;
        this.menuPool = null;
        this.characterName = "Player";
        this.phoneNumber = "1234";

        this.receivedTextMessages = [];
        this.contacts = [];
        this.textMessageReceiverNumber = "";
        this.textMessage = "";

        // Vehicle menu values
        this.vehicles = [];
        this.vehiclesMenu = null;
        this.vehicleMenuItems = [];
        this.selectedVehicle = null;
        //

        this.addContactNumber = "";
        this.addContactName = "";

        this.employmentText = "Unemployed";
        this.employmentTextColorR = 255;
        this.employmentTextColorG = 255;
        this.employmentTextColorB = 255;

        this.factionText = "Civilian";
        this.factionTextColorR = 50;
        this.factionTextColorG = 205;
        this.factionTextColorB = 50;

        this.moneyText = "$ 55002";

        // House menu values
        this.ownedHouseNames = null;
        this.ownedHouseIds = null;
        this.houses = [];

        // help values
        this.screenResolution = null;
        this.selectedTextMessageIndex = null;
        this.selectedContactIndex = null;
        this.contactModifier = 1; // Change when new buttons are added to contact menu!

        // controls
        this.helpMenu = null;
        this.houseButton = null;
        this.phoneMenu = null;
        this.phoneCallMenu = null;
        this.phoneCallMenuOn = false;
        this.textMessagesMenu = null;
        this.sendTextMessageMenu = null;
        this.addContactMenu = null;
        this.contactsMenu = null;
        this.textMessagesNeedUpdate = false;
        this.helpMenuActive = false;
    }

    handleHUDEvent(eventName, args)
    {
        switch (eventName)
        {
            case 'EVENT_INIT_HUD':
                this.initHUD(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15], args[16], args[17], args[18], args[19]);
                break;

            case 'EVENT_UPDATE_MONEY':
                this.updateMoney(args[0]);
                break;

            case 'EVENT_TOGGLE_HUD_ON':
                this.setHudOn();
                break;

            case 'EVENT_TOGGLE_HUD_OFF':
                this.setHudOff();
                break;

            case 'EVENT_SEND_OWNED_HOUSES':
                this.setOwnedHouses(args[0], args[1]);
                break;

            case 'EVENT_NEW_TEXT_MESSAGE_RECEIVED':
                this.handleNewTextMessageReceivedEvent(args[0], args[1], args[2], args[3]);
                break;

            case 'EVENT_RECEIVE_PHONE_CALL': // Other player is calling, show menu
                this.receivePhoneCall(args[0]);
                break;

            case 'EVENT_PHONE_CALL_STARTED': // You are calling someone, show menu
                this.startPhoneCalling(args[0]);
                break;

            case 'EVENT_PHONE_CALL_PICKED_UP': // You pressed accept button and took the call
                this.startPhoneCalling(args[0]);
                break;

            case 'EVENT_PHONE_CALL_ENDED':
                this.phoneCallEnded();
                break;

            case 'EVENT_NEW_VEHICLE_ADDED':
                this.addNewVehicle(args[0], args[1], args[2]);
                break;
        }
    }

    addNewVehicle(id, license, spawned) {
        this.vehicles.push(new Vehicle(id, license, spawned));
    }

    startPhoneCalling(number) {
        this.createPhoneCallMenu(number, true);
    }

    phoneCallEnded() {
        this.phoneCallMenuOn = false;
        this.phoneCallMenu.Visible = false;
    }

    endPhoneCalling() {

    }

    receivePhoneCall(number) {
        this.createPhoneCallMenu(number, false);
    }

    setOwnedHouses(houseNames, houseIds)
    {
        this.ownedHouseNames = houseNames;
        this.ownedHouseIds = houseIds;
        this.initHouses(houseIds, houseNames);
    }

    setHudOn()
    {
        this.hudActive = true;
        API.setHudVisible(true);
    }

    setHudOff()
    {
        this.hudActive = false;
        API.setHudVisible(false);
    }

    setEmployment(job, r, g, b)
    {
        this.employmentText = job;
        this.employmentTextColorR = r;
        this.employmentTextColorG = g;
        this.employmentTextColorB = b;
    }

    setFaction(faction, r, g, b)
    {
        this.factionText = faction;
        this.factionTextColorR = r;
        this.factionTextColorG = g;
        this.factionTextColorB = b;
    }

    updateMoney(newMoney)
    {
        this.moneyText = "$" + newMoney;
    }

    initTextMessages(ids, senders, times, messages)
    {
        for (var i = 0; i < messages.Count; i++) {
            this.receivedTextMessages.push(new TextMessage(ids[i], senders[i], messages[i], times[i]));
        }
    }

    initContacts(names, numbers)
    {
        for (var i = 0; i < names.Count; i++) {
            this.contacts.push(new Contact(names[i], numbers[i]));
        }
    }

    initVehicles(ids, licensePlates, spawneds)
    {
        for (var i = 0; i < ids.Count; i++) {
            this.vehicles.push(new Vehicle(ids[i], licensePlates[i], spawneds[i]));
        }
    }

    initHouses(ids, names)
    {
        for (var i = 0; i < ids.Count; i++) {
            this.houses.push(new House(ids[i], names[i]));
        }
    }

    initHUD(employment, r1, g1, b1, faction, r2, g2, b2, money, name, phoneNumber, textMessageIds, textMessageSenders, textMessageTimes, textMessages, contactNames, contactNumbers, vehicleIds, licensePlates, vehicleSpawneds)
    {
        this.hudActive = true;
        this.characterName = name;
        this.setFaction(faction, r2, g2, b2);
        this.setEmployment(employment, r1, g1, b1);
        this.updateMoney(money);
        this.phoneNumber = phoneNumber;
        this.initTextMessages(textMessageIds, textMessageSenders, textMessageTimes, textMessages);
        this.initContacts(contactNames, contactNumbers);
        this.initVehicles(vehicleIds, licensePlates, vehicleSpawneds);
    }

    draw()
    {
        if (this.hudActive) {

            if (this.screenResolution == null) {
                this.screenResolution = API.getScreenResolution();
            }

            let xPos = this.screenResolution.Width * 0.145;
            let yPos = this.screenResolution.Height * 0.82;
            let increment = yPos * 0.036;

            /*let xPos = 278.4;
            let yPos = 885.6;
            let increment = yPos * 0.036;*/

            API.drawText(this.characterName, xPos, yPos, 0.8, 255, 255, 255, 255, 1, 0, true, true, 1000);
            API.drawText(this.factionText, xPos, yPos + increment * 2.3, 0.65, this.factionTextColorR, this.factionTextColorG, this.factionTextColorB, 255, 6, 0, false, true, 1000);
            API.drawText(this.employmentText, xPos, yPos + increment * 3.4, 0.65, this.employmentTextColorR, this.employmentTextColorG, this.employmentTextColorB, 255, 6, 0, false, true, 1000);
            API.drawText(this.moneyText, xPos, yPos + increment * 4.7, 0.635, 34, 139, 34, 255, 7, 0, false, true, 1000);
        }
    }

    updateTextMessagesMenu()
    {
        for (var i = 0; i < this.receivedTextMessages.length - 1; i++) { 
            this.textMessagesMenu.RemoveItemAt(0); // Might be bugged
            this.textMessagesMenu.RefreshIndex();
        }

        for (var i = 0; i < this.receivedTextMessages.length; i++) {
            let item = API.createMenuItem(this.receivedTextMessages[i].sender, "Message: \n" + this.receivedTextMessages[i].message);
            item.SetRightLabel(this.receivedTextMessages[i].time);
            this.textMessagesMenu.AddItem(item);
        }

    }

    handleNewTextMessageReceivedEvent(id, number, text, time)
    {
        this.receivedTextMessages.unshift(new TextMessage(id, number, text, time));
        if (this.textMessagesMenu.Visible) { // test
            this.updateTextMessagesMenu();
        }
        else {
            this.textMessagesNeedUpdate = true;
        }
    }

    createHouseMenu()
    {
        this.menuPool = API.getMenuPool();
        let menu = API.createMenu("Player Menu", "Houses", 0, 0, 6);
        for (var i = 0; i < this.houses.length; i++)
        {
            let item = API.createMenuItem(this.houses[i].name, "");
            menu.AddItem(item);
        }

        return menu;
    }

    spawnVehicle(menu, sender) {
        API.triggerServerEvent("EVENT_TRY_SPAWN_VEHICLE", this.selectedVehicle);
    }

    parkVehicle(menu, sender) {
        API.triggerServerEvent("EVENT_TRY_PARK_VEHICLE", this.selectedVehicle);
    }

    lockVehicle(menu, sender) {
        API.triggerServerEvent("EVENT_TRY_LOCK_VEHICLE", this.selectedVehicle);
    }

    buyParkSpot(menu, sender) {
        API.triggerServerEvent("EVENT_TRY_BUY_PARKING_SPOT", this.selectedVehicle);
    }

    createVehicleDetailMenu(id, licensePlate, spawned) {
        let menu = API.createMenu("Player Menu", "Vehicle " + licensePlate, 0, 0, 6);
        let item1 = API.createMenuItem("Park vehicle", "You have to be inside the vehicle and at the park spot in order to park the vehicle");
        let item2 = API.createMenuItem("Spawn vehicle", "Spawns the vehicle at the spot where it was parked");
        let item3 = API.createMenuItem("Lock/Unlock vehicle", "");
        let item4 = API.createMenuItem("Purchase new parking spot ($10000)", "Updates the parking location for vehicle\nNote: You have to be inside the vehicle");

        item2.Activated.connect((menu, sender) => this.spawnVehicle(menu, sender));
        item1.Activated.connect((menu, sender) => this.parkVehicle(menu, sender));
        item3.Activated.connect((menu, sender) => this.lockVehicle(menu, sender));
        item4.Activated.connect((menu, sender) => this.buyParkSpot(menu, sender));

        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);
        menu.AddItem(item4);
        return menu;
    }

    vehicleSelected(sender, item, index)
    {
        this.selectedVehicle = this.vehicles[index].id;
    }

    createVehicleMenu()
    {
        let menu = API.createMenu("Player Menu", "Vehicles", 0, 0, 6);
        this.vehiclesMenu = menu;
        menu.OnItemSelect.connect((sender, item, index) => this.vehicleSelected(sender, item, index));

        for (var i = 0; i < this.vehicles.length; i++) {
            let item = API.createMenuItem(this.vehicles[i].licensePlate, "");

            if (this.vehicles[i].spawned) item.SetRightLabel("Active");
            else item.SetRightLabel("Inactive");

            menu.AddItem(item);

            // Create detail menus for every vehicle
            let detailMenu = this.createVehicleDetailMenu(this.vehicles[i].id, this.vehicles[i].licensePlate, this.vehicles[i].spawned);
            this.menuPool.Add(detailMenu);
            menu.BindMenuToItem(detailMenu, item);
        }

        return menu;
    }

    createActionsMenu()
    {
        this.menuPool = API.getMenuPool();
        let menu = API.createMenu("Player Menu", "Actions", 0, 0, 6);
        let item1 = API.createMenuItem("Play animation", "");
        let item2 = API.createMenuItem("Give money", "");

        menu.AddItem(item1);
        menu.AddItem(item2);

        return menu;
    }

    createItemTest(parent)
    {
        let item = API.createMenuItem("test", "");
        parent.AddItem(item);
        this.menuPool.RefreshIndex();
    }

    setPlayerUsingPhone(){
        API.triggerServerEvent("EVENT_SET_PLAYER_USING_PHONE");
    }

    setPlayerNotUsingPhone() {
        API.triggerServerEvent("EVENT_SET_PLAYER_NOT_USING_PHONE");
    }

    setPlayerCalling() {
        API.triggerServerEvent("EVENT_SET_PLAYER_CALLING");
    }

    menuChanged(sender, next, forward)
    {
        if (sender == this.helpMenu && next == this.phoneMenu && forward) {
            this.setPlayerUsingPhone();
        }
        else if (sender == this.phoneMenu && next == this.textMessagesMenu && this.textMessagesNeedUpdate) {
            this.updateTextMessagesMenu();
            this.textMessagesNeedUpdate = false;
        }
    }

    menuClosed(sender)
    {
        if (sender == this.phoneMenu) {
            this.setPlayerNotUsingPhone();
        }

        if (sender == this.helpMenu) {
            this.helpMenuActive = false;
        }

        if (sender == this.textMessagesMenu) {
            this.selectedTextMessageIndex = null;
        }

        if (sender == this.contactsMenu) {
            this.selectedContactIndex = null;
        }

        if (sender == this.phoneCallMenu) {
            if (this.phoneCallMenuOn) {
                this.phoneCallMenu.Visible = true;
            }
        }
    }

    getUserInputForReceiverNumber(menu, item) {
        var number = API.getUserInput("", 7);
        if (number != "") {
            item.Text = "Receiver phone number: " + number;
            this.textMessageReceiverNumber = number;
        }
        else {
            item.Text = "Receiver phone number";
            this.textMessageReceiverNumber = "Receiver phone number";
        }
    }

    getUserInputForMessage(menu, item) {
        var message = API.getUserInput("", 60);
        if (message != "") {
            item.Text = "Message: " + message;
            this.textMessage = message;
        }
        else {
            item.Text = "Message";
            this.textMessage = "Message";
        }
    }

    sendTextMessage(menu, item) {
        var isnum = /^\d+$/.test(this.textMessageReceiverNumber);
        if (this.textMessageReceiverNumber.length != 7) {
            API.sendNotification("Number has to be 7 digits long!");
        }
        else if (this.textMessage.length == "") {
            API.sendNotification("Text message can't be empty!");
        }
        else if (!isnum) {
            API.sendNotification("Number can only contains digits!");
        }
        else {
            API.triggerServerEvent("EVENT_SEND_TEXT_MESSAGE", this.textMessageReceiverNumber, this.textMessage);
            //this.closePhoneMenu(menu); // Remove for debug
            this.sendTextMessageMenu.GoBack(); // maybe bug
        }
    }

    closePhoneMenu(menu) {
        menu.Visible = false;
        this.helpMenuActive = false;
        this.setPlayerNotUsingPhone();
    }

    createSendTextMessageMenu()
    {
        let menu = API.createMenu("Player Menu", "Send text message", 0, 0, 6);
        this.sendTextMessageMenu = menu;

        let item1 = API.createMenuItem("Receiver phone number", "");
        let item2 = API.createMenuItem("Message", "");
        let item3 = API.createMenuItem("Send message", "");

        item1.Activated.connect((menu, sender) => this.getUserInputForReceiverNumber(menu, sender));
        item2.Activated.connect((menu, sender) => this.getUserInputForMessage(menu, sender));
        item3.Activated.connect((menu, sender) => this.sendTextMessage(menu, sender));

        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);

        return menu;
    }

    getUserInputForAddContactNumber(menu, item) {
        var message = API.getUserInput("", 7);
        if (message != "") {
            item.Text = "Phone number: " + message;
            this.addContactNumber = message;
        }
        else {
            item.Text = "Phone number";
            this.addContactNumber = "Phone number";
        }
    }

    getUserInputForAddContactName(menu, item) {
        var message = API.getUserInput("", 20);
        if (message != "") {
            item.Text = "Contact name: " + message;
            this.addContactName = message;
        }
        else {
            item.Text = "Contact name";
            this.addContactName = "Contact name";
        }
    }

    addContact(menu, sender) {
        if (this.addContactNumber.length != 7) {
            API.sendNotification("Phone number has to be 7 digits long!");
        }
        else if (this.addContactName.length > 12) {
            API.sendNotification("Contact name can't be more than 12 characters long!");
        }
        else if (this.addContactName.length == 0) {
            API.sendNotification("Contact name can't be empty!");
        }
        else {
            API.triggerServerEvent("EVENT_ADD_PHONE_CONTACT", this.addContactName, this.addContactNumber);
            this.contacts.push(new Contact(this.addContactName, this.addContactNumber));
            this.closePhoneMenu(this.addContactMenu);
        }
    }


    createAddContactMenu(parent, button)
    {
        let menu = API.createMenu("Player Menu", "Add new contact", 0, 0, 6);
        this.menuPool.Add(menu);
        parent.BindMenuToItem(menu, button);
        this.addContactMenu = menu;

        let item1 = API.createMenuItem("Phone number", "");
        let item2 = API.createMenuItem("Contact name", "");
        let item3 = API.createColoredItem("Add", "", "#009933", "#33cc33");

        item1.Activated.connect((menu, sender) => this.getUserInputForAddContactNumber(menu, sender)); // Maybe bug
        item2.Activated.connect((menu, sender) => this.getUserInputForAddContactName(menu, sender));
        item3.Activated.connect((menu, sender) => this.addContact(menu, sender));

        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);
    }

    messageSelected(sender, index)
    {
        this.selectedTextMessageIndex = index;
    }

    contactSelected(sender, index) {
        this.selectedContactIndex = index;
    }

    deleteSelectedTextMessage()
    {
        if (this.selectedTextMessageIndex != null) {
            API.triggerServerEvent("EVENT_REMOVE_TEXT_MESSAGE", this.receivedTextMessages[this.selectedTextMessageIndex].id);
            this.receivedTextMessages.splice(this.selectedTextMessageIndex, 1);
            this.textMessagesMenu.RemoveItemAt(this.selectedTextMessageIndex);
            this.selectedTextMessageIndex = null;
        }
    }

    deleteSelectedContact()
    {
        if (this.selectedContactIndex != null) {
            if (this.selectedContactIndex != 0) {
                API.triggerServerEvent("EVENT_REMOVE_PHONE_CONTACT", this.contacts[this.selectedContactIndex - this.contactModifier].number);
                this.contacts.splice(this.selectedContactIndex - this.contactModifier, 1);
                this.contactsMenu.RemoveItemAt(this.selectedContactIndex);
                this.selectedContactIndex = null;
            }
        }
    }

    createMessagesMenu()
    {
        let menu = API.createMenu("Player Menu", "Messages", 0, 0, 6);
        menu.OnIndexChange.connect((sender, index) => this.messageSelected(sender, index));
        menu.OnMenuClose.connect((sender) => this.menuClosed(sender));
        this.textMessagesMenu = menu;

        if (this.receivedTextMessages.length != 0) {
            for (var i = 0; i < this.receivedTextMessages.length; i++) {
                let item = API.createMenuItem(this.receivedTextMessages[i].sender, "Message: \n" + this.receivedTextMessages[i].message);
                item.SetRightLabel(this.receivedTextMessages[i].time);
                menu.AddItem(item);
            }
        }

        return menu;
    }

    contactSelectedEvent(sender, selectedItem, newIndex) {
        if (newIndex != 0) {
            var number = this.contacts[newIndex - this.contactModifier].number;
            this.makePhoneCall(number);
        }
    }

    makePhoneCall(number) {
        API.triggerServerEvent("EVENT_START_PHONE_CALL", number);
    }

    createContactsMenu(parent, button)
    {
        let menu = API.createMenu("Player Menu", "Address book", 0, 0, 6);
        this.menuPool.Add(menu);
        parent.BindMenuToItem(menu, button);

        this.contactsMenu = menu;

        menu.OnIndexChange.connect((sender, index) => this.contactSelected(sender, index));
        menu.OnMenuClose.connect((sender) => this.menuClosed(sender));
        menu.OnItemSelect.connect((sender, selectedItem, newIndex) => this.contactSelectedEvent(sender, selectedItem, newIndex));

        let item1 = API.createColoredItem("Add new contact", "Add new contact to your address book", "#009933", "#33cc33");
        menu.AddItem(item1);

        for (var i = 0; i < this.contacts.length; i++) {
            let item = API.createMenuItem(this.contacts[i].name, "");
            item.SetRightLabel(this.contacts[i].number);
            menu.AddItem(item);
        }

        this.createAddContactMenu(menu, item1);
        return menu;
    }

    createPhoneMenu()
    {
        let menu = API.createMenu("Player Menu", "Phone - " + this.phoneNumber, 0, 0, 6);
        this.phoneMenu = menu;

        menu.OnMenuClose.connect((sender) => this.menuClosed(sender));
        menu.OnMenuChange.connect((sender, next, forward) => this.menuChanged(sender, next, forward));

        let item1 = API.createMenuItem("Address book", "");
        let item2 = API.createMenuItem("Call (not implemented yet, use address book", "");
        let item4 = API.createMenuItem("Messages", "");
        let item3 = API.createMenuItem("Send text message", "");


        menu.AddItem(item1);
        menu.AddItem(item4);
        menu.AddItem(item2);
        menu.AddItem(item3);

        let menu1 = this.createSendTextMessageMenu();
        let menu2 = this.createContactsMenu(menu, item1);
        let menu3 = this.createMessagesMenu();

        this.menuPool.Add(menu1);
        this.menuPool.Add(menu2);
        this.menuPool.Add(menu3);

        //menu.BindMenuToItem(menu2, item1);
        menu.BindMenuToItem(menu1, item3);
        menu.BindMenuToItem(menu3, item4);

        return menu;
    }

    createHelpMenu()
    {
        this.menuPool = API.getMenuPool();
        let menu = API.createMenu("Player Menu", "Main menu", 0, 0, 6);
        this.helpMenu = menu;

        menu.OnMenuChange.connect((sender, next, forward) => this.menuChanged(sender, next, forward));
        menu.OnMenuClose.connect((sender) => this.menuClosed(sender));

        let item = API.createMenuItem("Inventory (Not yet implemented)", "");
        let item2 = API.createMenuItem("Vehicles", "");
        if (this.vehicles.length == 0) {
            item2.Enabled = false;
            item2.Description = "You don't own any vehicles";
        }


        let item3 = API.createMenuItem("Houses", "");
        if (this.ownedHouseNames.Count == 0) {
            item3.Enabled = false;
            item3.Description = "You don't own or rent any properties";
        }
        let item4 = API.createMenuItem("Actions", "");
        let item5 = API.createMenuItem("Phone", "");

        this.houseButton = item3;

        menu.AddItem(item4);
        menu.AddItem(item5);
        menu.AddItem(item);
        menu.AddItem(item2);
        menu.AddItem(item3);

        var menu1 = this.createHouseMenu();
        var menu2 = this.createActionsMenu();
        var menu4 = this.createVehicleMenu();
        var menu3 = this.createPhoneMenu();

        this.menuPool.Add(this.helpMenu);
        this.menuPool.Add(menu2);
        this.menuPool.Add(menu3);
        this.menuPool.Add(menu4);
        this.menuPool.Add(menu1);
        this.menuPool.RefreshIndex();

        menu.BindMenuToItem(menu1, item3);
        menu.BindMenuToItem(menu2, item4);
        menu.BindMenuToItem(menu3, item5);
        menu.BindMenuToItem(menu4, item2);

        menu.Visible = true;
    }

    acceptPhoneCall(menu, sender) {
        API.triggerServerEvent("EVENT_ACCEPT_PHONE_CALL");
    }

    endPhoneCall(menu, sender) {
        API.triggerServerEvent("EVENT_END_PHONE_CALL");
    }

    createPhoneCallMenu(caller, isCaller)
    {
        let menu = API.createMenu("Phone call", "Number: " + caller, 0, 0, 3);
        menu.OnMenuClose.connect((sender) => this.menuClosed(sender));
        this.phoneCallMenu = menu;

        if (!isCaller) {
            let item2 = API.createColoredItem("Accept Call", "", "#009933", "#33cc33");
            item2.Activated.connect((menu, sender) => this.acceptPhoneCall(menu, sender));
            menu.AddItem(item2);
        }

        let item1 = API.createColoredItem("End call", "", "#ff0000", "#ff3333");
        item1.Activated.connect((menu, sender) => this.endPhoneCall(menu, sender));
        menu.AddItem(item1);

        this.menuPool.Add(menu);
        menu.Visible = true;
        this.helpMenuActive = false;
        this.phoneCallMenuOn = true;
    }

    openHelpMenu()
    {
        if (this.hudActive) {
            this.helpMenuActive = true;
            this.createHelpMenu();
        }
    }

    processMenus() {
        if (this.menuPool != null) {
            this.menuPool.ProcessMenus();
        }
    }
}

let hudManager = new HUDManager();

API.onServerEventTrigger.connect((eventName, args) => hudManager.handleHUDEvent(eventName, args));
API.onUpdate.connect(() => hudManager.draw());

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.F1 && !API.isChatOpen() && hudManager.hudActive && !hudManager.helpMenuActive && !hudManager.phoneCallMenuOn) {
        hudManager.openHelpMenu();
    }

    if (args.KeyCode == Keys.Delete && !API.isChatOpen() && hudManager.hudActive && hudManager.textMessagesMenu != null) {
        if (hudManager.textMessagesMenu.Visible) {
            hudManager.deleteSelectedTextMessage();
        }
    }

    if (args.KeyCode == Keys.Delete && !API.isChatOpen() && hudManager.hudActive && hudManager.contactsMenu != null) {
        if (hudManager.contactsMenu.Visible) {
            hudManager.deleteSelectedContact();
        }
    }
});

API.onUpdate.connect(() => hudManager.processMenus());

