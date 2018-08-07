/**
* This class is responsible for all the account related actions on
* the client side.
*/
class AccountManager {

    constructor() {
        this.characterNameToModelDict = new Map();

        this.lastMenu = null;

        this.accountCreationMenu = null;
        this.characterSelectionMenu = null;
        this.characterCreationMenu = null;

        this.characterFirstName = null;
        this.characterLastName = null;

        this.characterCreationModels = null;
        this.currentModelName = null;
        this.currentModelSpot = 0;
        this.characterModelSelectItem = null;


        this.accountName = null;
        this.accountPassword = null;
        this.accountPassword2 = null;
        this.acceptButton = null;

        this.isSelectingCharacter = false;
        this.isCreatingCharacter = false;
        this.characterSelectionMenuItems = [];

        this.isAllowedToCloseSelectCharacterMenu = false;
        this.isAllowedToCloseCreateAccountMenu = false;
    }


    handleAccountEvent(eventName, args) {
        switch (eventName) {
            case 'EVENT_OPEN_CREATE_ACCOUNT_MENU':
                this.handleOpenAccountCreationMenu(args);
                break;

            case 'EVENT_CLOSE_CREATE_ACCOUNT_MENU':
                this.handleCloseAccountCreationMenu();
                break;

            case 'EVENT_OPEN_CHARACTER_SELECT_MENU':
                this.handleOpenCharacterSelectMenu(args);
                break;

            case 'EVENT_CLOSE_CHARACTER_SELECT_MENU':
                this.handleCloseCharacterSelectMenu(args);
                break;

            default:
                break;
        }
    }

    getUserInputForFirstName(menu, item) {
        var name = API.getUserInput("", 20);
        item.Text = name;
        this.characterFirstName = name;
    }

    getUserInputForLastName(menu, item) {
        var name = API.getUserInput("", 20);
        item.Text = name;
        this.characterLastName = name;
    }

    requestCreateCharacter(menu, item) {
        API.triggerServerEvent("EVENT_REQUEST_CREATE_CHARACTER", this.characterFirstName, this.characterLastName, this.currentModelName);
    }

    handleOpenCharacterCreationMenu(parentMenu, button) {

        let menu = API.createMenu("Character creation", "New character", 0, 0, 6);
        menu.OnListChange.connect((sender, list, newIndex) => this.characterModelChanged(sender, list, newIndex));
        this.characterCreationMenu = menu;
        let item = API.createMenuItem("First name", "Your character's first name");
        let item2 = API.createMenuItem("Last name", "Your character's last name");
        var list = new List(String);


        for (var i = 0; i < this.characterCreationModels.Count; i++) {
            list.Add(this.characterCreationModels[i]);
        }

        let item3 = API.createListItem("Character model", "Character model", list, 0);
        let item4 = API.createColoredItem("Accept", "Finish creating character", "#009933", "#33cc33");
        this.characterModelSelectItem = item3;
        item.Activated.connect(() => this.getUserInputForFirstName(menu, item));
        item2.Activated.connect(() => this.getUserInputForLastName(menu, item2));
        item4.Activated.connect(() => this.requestCreateCharacter(menu, item));

        menu.AddItem(item);
        menu.AddItem(item2);
        menu.AddItem(item3);
        menu.AddItem(item4);
        parentMenu.BindMenuToItem(menu, button);
    }

    characterModelChanged(sender, list, newIndex) {
        this.currentModelSpot = newIndex;
        this.currentModelName = this.characterCreationModels[newIndex];
        API.setPlayerSkin(API.pedNameToModel(this.currentModelName));
    }

    getUserInputForPassword(menu, item) {
        var password = API.getUserInput("", 20);
        this.accountPassword = password;
        item.Text = "";
        for (var i = 0; i < password.length; i++)
        {
            item.Text = item.Text += "*";
        }
    }

    getUserInputForPassword2(menu, item) {
        var password = API.getUserInput("", 20);
        this.accountPassword2 = password;
        item.Text = "";
        for (var i = 0; i < password.length; i++) {
            item.Text = item.Text += "*";
        }
    }

    selectCharacter(menu, item) {
        API.triggerServerEvent("EVENT_REQUEST_SELECT_CHARACTER", item.Text);
    }

    hoverCharacter(menu, item) {
        API.setPlayerSkin(this.characterNameToModelDict.get(item.Text));
    }

    createNewCharacter(menu, item) {
        API.triggerServerEvent("EVENT_REQUEST_CREATE_CHARACTER_MENU");
    }

    handleCloseCharacterSelectMenu(args) {
        this.isAllowedToCloseSelectCharacterMenu = true;
        this.isSelectingCharacter = false;
        this.characterSelectionMenuItems = [];
        this.characterSelectionMenu.Visible = false;
        this.characterCreationMenu.Visible = false;
        API.setActiveCamera(null);
    }

    characterSelectionMenuCloseEvent(sender) {
        if (sender == this.characterSelectionMenu)
        {
            if (!this.isAllowedToCloseSelectCharacterMenu) {
                sender.Visible = true;
            }
        }
        else if (sender == this.accountCreationMenu)
        {
            if (!this.isAllowedToCloseCreateAccountMenu)
            {
                sender.Visible = true;
            }
        }
    }

    handleOpenCharacterSelectMenu(args) {
        this.characterCreationModels = args[4];

        this.isSelectingCharacter = true;
        this.isCreatingCharacter = false;

        // Map the character names to models
        this.nameList = args[0];
        this.modelList = args[1];
        this.cameraPos = args[2];
        this.cameraRot = args[3];

        for (var a = 0; a < this.nameList.Count; a++) {
            this.characterNameToModelDict.set(this.nameList[a], this.modelList[a]);
        }

        // Create camera
        let newCamera = API.createCamera(args[2], args[3]);
        API.setActiveCamera(newCamera);

        // Set the initial model for the player
        if (this.nameList.Count > 0) {
            let item = this.nameList[0];
            API.setPlayerSkin(API.pedNameToModel(this.characterNameToModelDict.get(item)));
        } else {
            API.setPlayerSkin(API.pedNameToModel("Abigail"));
        }

        let menu = API.createMenu("Characters", "Maximum characters: 4", 0, 0, 6);
        this.characterSelectionMenu = menu;

        menu.OnIndexChange.connect((sender, index) => this.selectionChanged(sender, index));
        menu.OnMenuClose.connect((sender) => this.characterSelectionMenuCloseEvent(sender));

        for (var i = 0; i < this.nameList.Count; i++) {
            let item = API.createMenuItem(this.nameList[i], "");
            item.Activated.connect(() => this.selectCharacter(menu, item));
            menu.AddItem(item);
            this.characterSelectionMenuItems.push(item);
        }

        let newItem = API.createColoredItem("Create new character", "", "#357df2", "#1c6def");
        if (this.nameList.Count > 4)
        {
            newItem.Enabled = false;
        }
        menu.AddItem(newItem);
        this.handleOpenCharacterCreationMenu(menu, newItem);
        menu.Visible = true;
    }

    sendAccountCreationRequest(menu, item) {
        if (this.accountPassword == this.accountPassword2) {
            API.triggerServerEvent("EVENT_REQUEST_CREATE_ACCOUNT", this.accountName, this.accountPassword);
        }
        else {
            API.sendNotification("Passwords have to match!");
        }
    }

    handleCloseAccountCreationMenu() {
        this.isAllowedToCloseCreateAccountMenu = true;
        this.accountCreationMenu.Visible = false;
    }

    handleOpenAccountCreationMenu(args) {
        let menu = API.createMenu("Account creation", "Please enter your password twice", -210, 150, 4);
        this.accountCreationMenu = menu;
        let name = args[0];
        this.accountName = name;

        let item = API.createColoredItem(args[0], "Note:\nAccount name is your GT-MP name and can't be changed.", "#666666", "#737373");
        let item2 = API.createMenuItem("Enter your password", "Press enter to write your password. \nNote:\nPassword may only contain letters and numericals");
        let item4 = API.createMenuItem("Enter your password again", "Press enter to write your password. \nNote:\nPassword may only contain letters and numericals");
        let item3 = API.createColoredItem("Accept", "Finish account creation", "#009933", "#33cc33");
        this.acceptButton = item3;

        item2.Activated.connect(() => this.getUserInputForPassword(menu, item2));
        item4.Activated.connect(() => this.getUserInputForPassword2(menu, item4));
        item3.Activated.connect(() => this.sendAccountCreationRequest(menu, item3));

        menu.AddItem(item);
        menu.AddItem(item2);
        menu.AddItem(item4);
        menu.AddItem(item3);

        menu.Visible = true;
    }

    selectionChanged(menu, index) {
        if (menu == this.characterSelectionMenu && index < this.characterSelectionMenuItems.length) {
            let item = this.characterSelectionMenuItems[index];
            API.setPlayerSkin(API.pedNameToModel(this.characterNameToModelDict.get(item.Text)));
        }
        else if (menu == this.characterSelectionMenu && index == this.characterSelectionMenuItems.length)
        {
            this.currentModelName = this.characterCreationModels[this.currentModelSpot];
            API.setPlayerSkin(API.pedNameToModel(this.currentModelName));
        }
    }

}

let accountManager = new AccountManager();

API.onServerEventTrigger.connect(function (eventName, args) {
    accountManager.handleAccountEvent(eventName, args);
});