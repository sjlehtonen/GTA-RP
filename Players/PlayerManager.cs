using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;
using System.Linq;
using GTA_RP.Factions;
using GTA_RP.Misc;
using System.Security.Cryptography;
using System.Text;
using GTA_RP.Items;

namespace GTA_RP
{
    public delegate void OnPlayerDisconnectDelegate(Character character);
    public delegate void OnCharacterDeathDelegate(Character character, NetHandle entityKiller, int weapon);

    struct Position
    {
        public Vector3 pos;
        public Vector3 rot;

        public Position(Vector3 pos, Vector3 rot)
        {
            this.pos = pos;
            this.rot = rot;
        }
    }

    /// <summary>
    /// Class for managing players
    /// Everything related to player management is contained in this class
    /// </summary>
    class PlayerManager : Singleton<PlayerManager>
    {
        private event OnPlayerDisconnectDelegate OnPlayerDisconnectEvent;
        private event OnCharacterDeathDelegate OnCharacterDeathEvent;

        private int accountCreationId = 0;
        public int textMessageId { get; private set; }

        private Position[] startCameraPositions =
        {
            new Position(new Vector3(-1136.09, -58.34853, 44.20825), new Vector3(0, 0, -134.8262)),
            new Position(new Vector3(3346.998, 5183.969, 15.35839), new Vector3(0, 0, -86.46388)),
            new Position(new Vector3(465.9247, 5594.497, 781.0376), new Vector3(0, 0, 13.70422)),
            new Position(new Vector3(-545.1541, 4471.583, 60.59504), new Vector3(0, 0, 112.1356))
        };

        private List<Player> players = new List<Player>();
        private Dictionary<string, int> characterGenderDictionary = new Dictionary<string, int>();
        private CharacterSelector characterSelector = new CharacterSelector();

        // Constants
        // At some point move these to server config files
        private const int firstNameMinLength = 3;
        private const int firstNameMaxLength = 8;
        private const int lastNameMinLength = 2;
        private const int lastNameMaxLength = 10;
        private const int maxCharactersCount = 5;

        private const int phoneNumberLength = 7;
        private const int phoneBookNameMaxLength = 12;

        private const int minPasswordLength = 6;
        private const int maxPasswordLength = 20;

        DBManager dbCon = DBManager.Instance();


        public PlayerManager()
        {
            textMessageId = 0;
        }

        private void SetLoginScreenCamera(Client player)
        {
            Position p = GetRandomStartCameraPosition();
            player.position = p.pos;
            player.rotation = p.rot;
            player.dimension = 1;
            player.transparency = 0;
            player.freezePosition = true;
            API.shared.triggerClientEvent(player, "EVENT_SET_LOGIN_SCREEN_CAMERA", p.pos, p.rot);
        }

        private void RemoveLoginScreenCamera(Client player)
        {
            player.dimension = 0;
            player.transparency = 255;
            player.freezePosition = false;
            API.shared.triggerClientEvent(player, "EVENT_REMOVE_CAMERA");
        }


        /// <summary>
        /// Gets random starting camera position
        /// </summary>
        /// <returns>Starting camera position</returns>
        private Position GetRandomStartCameraPosition()
        {
            Random rnd = new Random();
            return startCameraPositions.ElementAt(rnd.Next(0, startCameraPositions.Length));
        }

        /// <summary>
        /// Checks if server has any accounts
        /// </summary>
        /// <returns>True if server has accounts, otherwise false</returns>
        private Boolean HasAccounts()
        {
            return !DBManager.IsTableEmpty("players");
        }

        /// <summary>
        /// Checks if server has characters
        /// </summary>
        /// <returns>True if server has characters, otherwise false</returns>
        private Boolean HasCharacters()
        {
            return !DBManager.IsTableEmpty("characters");
        }

        /// <summary>
        /// Checks if server has text messages
        /// </summary>
        /// <returns>True if server has text messages, otherwise false</returns>
        private Boolean HasTextMessages()
        {
            return !DBManager.IsTableEmpty("text_messages");
        }

        /// <summary>
        /// Checks if a character exists with the given phone number
        /// </summary>
        /// <param name="number">Phone number</param>
        /// <returns>True if character exists, false otherwise</returns>
        private Boolean DoesCharacterExistWithPhoneNumber(String number)
        {
            int rows = 0;
            DBManager.SelectQuery("SELECT COUNT(*) FROM characters WHERE phone_number=@number", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                rows = reader.GetInt32(0);
            }).AddValue("@number", number).Execute();

            if (rows > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delivers text message to character who is not currently in use
        /// </summary>
        /// <param name="id">Text message id</param>
        /// <param name="receiver">Receiver phone number</param>
        /// <param name="message">Text message object</param>
        /// <returns>True if message was sent succesfully, otherwise false</returns>
        private Boolean DeliverOfflineTextMessage(int id, String receiver, TextMessage message)
        {
            if (DoesCharacterExistWithPhoneNumber(receiver))
            {
                DBManager.InsertQuery("INSERT INTO text_messages VALUES (@id, @sender_number, @receiver_number, @time, @message)")
                    .AddValue("@id", id)
                    .AddValue("@sender_number", message.senderNumber)
                    .AddValue("@receiver_number", receiver)
                    .AddValue("@time", message.time)
                    .AddValue("@message", message.message)
                    .Execute();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if character exists with given name
        /// </summary>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <returns>True if character exists, otherwise false</returns>
        private Boolean DoesCharacterExistWithName(string firstName, string lastName)
        {
            int rows = 0;
            DBManager.SelectQuery("SELECT COUNT(*) FROM characters WHERE first_name=@firstName AND last_name=@lastName", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                rows = reader.GetInt32(0);
            }).AddValue("@firstName", firstName).AddValue("@lastName", lastName).Execute();

            if (rows < 1)
            {
                return false;
            }
            return true;
        }

        public void SetMinimapForPlayer(Client player)
        {
            API.shared.triggerClientEvent(player, "EVENT_TOGGLE_MINIMAP", false);
        }

        public void SetMinimapForPlayer(Character character)
        {
            SetMinimapForPlayer(character.owner.client);
        }

        public void RemoveMinimapFromPlayer(Client player)
        {
            API.shared.triggerClientEvent(player, "EVENT_TOGGLE_MINIMAP", true);
        }

        public void RemoveMinimapFromPlayer(Character player)
        {
            RemoveMinimapFromPlayer(player.owner.client);
        }

        /// <summary>
        /// Handles player connect event
        /// </summary>
        /// <param name="player">Player who connected</param>
        public void HandlePlayerConnect(Client player)
        {
            RemoveMinimapFromPlayer(player);
            SetLoginScreenCamera(player);
            if (!DoesPlayerHaveAccount(player))
            {
                OpenCreateAccountMenu(player);
            }
        }

        /// <summary>
        /// Handles player disconnect event
        /// </summary>
        /// <param name="player">Player who disconnect</param>
        /// <param name="reason">Disconnect reason</param>
        public void HandlePlayerDisconnect(Client player, String reason)
        {
            if (OnPlayerDisconnectEvent != null && IsClientUsingCharacter(player))
            {
                OnPlayerDisconnectEvent.Invoke(GetActiveCharacterForClient(player));
            }

            Player p = GetPlayerByClient(player);

            if (p != null)
            {
                this.players.Remove(GetPlayerByClient(player));
            }
        }

        private void HandleCharacterDeath(Character character, NetHandle entityKiller, int weapon)
        {
            // Override the character death to block spawning to hospital
            // Create death timer
            // This is not yet implemented in GTA-MP so commented out meanwhile
            // characterDeathTimers[c.ID] = new GTRPTimer(HandleCharacterDeathTimerExpire, (int)TimeSpan.FromMinutes(3).TotalMilliseconds);

            OnCharacterDeathEvent.Invoke(character, entityKiller, weapon);
        }

        /// <summary>
        /// Is ran when player dies
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="entityKiller">Killer</param>
        /// <param name="weapon">Weapon</param>
        /// <param name="args">Cancel args</param>
        public void HandlePlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            if (OnCharacterDeathEvent != null && IsClientUsingCharacter(player))
            {
                HandleCharacterDeath(GetActiveCharacterForClient(player), entityKiller, weapon);
            }
        }

        /// <summary>
        /// Loads player from database for given client
        /// </summary>
        /// <param name="client"></param>
        /// <returns>The loaded player object</returns>
        private Player LoadPlayerForClient(Client client)
        {
            Player p = null;
            DBManager.SelectQuery("SELECT * FROM players WHERE name = @name", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                p = new Player(client, reader.GetInt32(0), reader.GetInt32(3));
            }).AddValue("@name", client.name).Execute();
            return p;
        }

        /// <summary>
        /// Loads text messages for given character
        /// </summary>
        /// <param name="character">Character for which to load text messages</param>
        /// <returns>Loaded text messages</returns>
        private List<TextMessage> LoadTextMessagesForCharacter(Character character)
        {
            List<TextMessage> messages = new List<TextMessage>();
            DBManager.SelectQuery("SELECT * FROM text_messages WHERE receiver_number = @number", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                TextMessage tm;
                tm.id = reader.GetInt32(0);
                tm.senderNumber = reader.GetString(1);
                tm.time = reader.GetString(3);
                tm.message = reader.GetString(4);
                messages.Add(tm);
            }).AddValue("@number", character.phone.phoneNumber).Execute();

            return messages;
        }

        /// <summary>
        /// Loads phone contacts for given character
        /// </summary>
        /// <param name="character">Character for which to load phone contacts</param>
        /// <returns>Loaded contacts</returns>
        private List<Address> LoadPhoneContactsForCharacter(Character character)
        {
            List<Address> contacts = new List<Address>();
            DBManager.SelectQuery("SELECT * FROM phone_contacts WHERE owner = @owner", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                Address address;
                address.name = reader.GetString(1);
                address.number = reader.GetString(2);
                contacts.Add(address);
            }).AddValue("@owner", character.ID).Execute();


            return contacts;
        }

        /// <summary>
        /// Loads characters for player
        /// </summary>
        /// <param name="player">Player for which to load characters</param>
        /// <returns>Loaded characters</returns>
        private List<Character> LoadCharactersForPlayer(Player player)
        {
            List<Character> characters = new List<Character>();
            DBManager.SelectQuery("SELECT * FROM characters WHERE player_id = @id", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                Character c = new Character(player, reader.GetInt32(0), reader.GetString(2), reader.GetString(3), (Factions.FactionEnums)reader.GetInt32(4), reader.GetString(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetString(8), reader.GetInt32(9));
                characters.Add(c);
            }).AddValue("@id", player.id).Execute();

            // Load phone contents and init inventory
            foreach (Character c in characters)
            {
                ItemManager.Instance().LoadInventoryForCharacter(c);
                c.phone.AddReceivedMessages(LoadTextMessagesForCharacter(c));
                c.phone.AddContacts(LoadPhoneContactsForCharacter(c));
            }

            return characters;
        }

        /// <summary>
        /// Encrypts the given password
        /// TODO: Code a better encryption method instead of using md5. For example bcrypt
        /// </summary>
        /// <param name="password">Password to encrypt</param>
        /// <returns>Encrypted version of the password</returns>
        private string EncryptPassword(string password)
        {
            using (MD5 hasher = MD5.Create())
            {
                byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Authenticates player
        /// </summary>
        /// <param name="player">Player to authenticate</param>
        /// <param name="password">Player password</param>
        /// <returns>True if authenticated, otherwise false</returns>
        private Boolean AuthenticatePlayer(Client player, String password)
        {
            string pass = null;
            DBManager.SelectQuery("SELECT password FROM players WHERE name = @name", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                pass = reader.GetString(0);
            }).AddValue("@name", player.name).Execute();

            if (pass != null)
            {
                if (pass.Equals(EncryptPassword(password)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if player is logged in
        /// </summary>
        /// <param name="player">Client</param>
        /// <returns>True if player is logged in, otherwise false</returns>
        private Boolean IsPlayerLoggedIn(Client player)
        {
            if (players.Where(p => player.Equals(p.client)).ToList().Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if player has an account
        /// </summary>
        /// <param name="player">Client</param>
        /// <returns>True if player has account, otherwise false</returns>
        private Boolean DoesPlayerHaveAccount(Client player)
        {
            int count = 0;
            DBManager.SelectQuery("SELECT COUNT(*) FROM players WHERE name = @name", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                count = reader.GetInt32(0);
            }).AddValue("@name", player.name).Execute();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handle the login command
        /// </summary>
        /// <param name="player">Player who sends the command</param>
        /// <param name="password">Inputted password</param>
        public void HandlePlayerLogin(Client player, string password)
        {
            if (IsPlayerLoggedIn(player))
            {
                API.shared.sendChatMessageToPlayer(player, "You are already logged in!");
                return;
            }

            if (!DoesPlayerHaveAccount(player))
            {
                return;
            }

            if (AuthenticatePlayer(player, password))
            {
                Player newPlayer = LoadPlayerForClient(player);
                List<Character> characters = LoadCharactersForPlayer(newPlayer);

                AddNewPlayerToPool(newPlayer);
                RemoveLoginScreenCamera(player);

                this.OpenCharacterSelectionForPlayer(newPlayer, characters);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(player, "Wrong password!");
            }
        }

        /// <summary>
        /// Creates an account with given info
        /// </summary>
        /// <param name="client">Client for which to create account</param>
        /// <param name="accountName">Account name</param>
        /// <param name="password">Account password</param>
        private void CreateAccount(Client client, String accountName, String password)
        {
            // Create player to database
            // Create player object
            // Close menu for player
            // Open character creation menu

            DBManager.InsertQuery("INSERT INTO players VALUES (@id, @name, @password, @admin_level)")
                .AddValue("@id", this.accountCreationId)
                .AddValue("@name", accountName)
                .AddValue("@password", this.EncryptPassword(password))
                .AddValue("@admin_level", 0)
                .Execute();

            Player createdPlayer = new Player(client, this.accountCreationId, 0);
            this.accountCreationId++;
            this.AddNewPlayerToPool(createdPlayer);

            API.shared.triggerClientEvent(client, "EVENT_CLOSE_CREATE_ACCOUNT_MENU");
            RemoveLoginScreenCamera(client);
            this.OpenCharacterSelectionForPlayer(createdPlayer, new List<Character>());
        }

        /// <summary>
        /// Validate text message
        /// Message length, receiver number etc
        /// </summary>
        /// <param name="client">Client who sends the message</param>
        /// <param name="receiver">Receiver phone number</param>
        /// <param name="message">Message text</param>
        /// <returns>True if validated, otherwise false</returns>
        private Boolean ValidateTextMessage(Client client, String receiver, String message)
        {
            if (receiver.Length != phoneNumberLength)
            {
                API.shared.sendNotificationToPlayer(client, String.Format("Phone number has to be {0} digits long", phoneNumberLength));
                return false;
            }

            if (message.Length == 0)
            {
                API.shared.sendNotificationToPlayer(client, "Text message content is not allowed to be empty");
                return false;
            }

            if (!receiver.All(char.IsDigit))
            {
                API.shared.sendNotificationToPlayer(client, "Phone number can contain only numbers");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate phone contact
        /// Contact name, number etc
        /// </summary>
        /// <param name="client">Client who adds the contact</param>
        /// <param name="name">Contact name</param>
        /// <param name="number">Contact number</param>
        /// <returns>True if validated, otherwise false</returns>
        private Boolean ValidateContact(Client client, String name, String number)
        {
            if (name.Length == 0)
            {
                API.shared.sendChatMessageToPlayer(client, "Contact name can't be empty!");
                return false;
            }

            if (number.Length != phoneNumberLength)
            {
                API.shared.sendChatMessageToPlayer(client, String.Format("Phone number has to {0} digits long!", phoneNumberLength));
                return false;
            }

            if (name.Length > phoneBookNameMaxLength)
            {
                API.shared.sendChatMessageToPlayer(client, String.Format("Name can't be longer than {0} characters!", phoneBookNameMaxLength));
                return false;
            }

            Character character = this.GetActiveCharacterForClient(client);
            if (character != null && character.phone.HasContactForNumber(number))
            {
                API.shared.sendNotificationToPlayer(client, "You already have a contact for number " + number);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates player spawn position to a certain house
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="houseId">House id</param>
        private void UpdateCharacterSpawnPosition(Character character, int houseId)
        {
            // NOTE: When selling a house that player currently lives in, the spawn position has to be reset
            DBManager.UpdateQuery("UPDATE characters SET spawn_house_id=@house_id WHERE id=@id")
                .AddValue("@house_id", houseId)
                .AddValue("@id", character.ID)
                .Execute();
        }






        // Public methods

        public Character GetCharacterWithId(int id)
        {
            return GetActiveCharacters().SingleOrDefault(x => x.ID == id);
        }

        public bool TeleportPlayerToJailIfTimeLeft(Character character)
        {
            return FactionManager.Instance().LawEnforcement().MoveCharacterToJailIfJailTimeLeft(character);
        }

        public int GetTimeLeftInJailForCharacter(Character character)
        {
            return FactionManager.Instance().LawEnforcement().GetJailTimeLeftForCharacter(character);
        }


        public bool IsCharacterWithIdOnline(int id)
        {
            if (GetCharacterWithId(id) == null) return false;
            return true;
        }

        public void SendNotificationToCharacterWithid(int id, string notification)
        {
            if (IsCharacterWithIdOnline(id))
            {
                Character character = GetCharacterWithId(id);
                API.shared.sendNotificationToPlayer(character.owner.client, notification);
            }
        }

        /// <summary>
        /// Gets character with given phone number
        /// </summary>
        /// <param name="number">Phone number</param>
        /// <returns>Character with given phone number, null if not found</returns>
        public Character GetCharacterWithPhoneNumber(String number)
        {
            return this.GetActiveCharacters().SingleOrDefault(x => x.phone.phoneNumber.Equals(number));
        }


        /// <summary>
        /// Sets a new spawn position for player inside a house that is owned by him/her
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="houseId">ID of the owned house</param>
        public void SetCharacterSpawnHouse(Client client, int houseId)
        {
            if (IsClientUsingCharacter(client))
            {
                Character character = GetActiveCharacterForClient(client);
                if (HouseManager.Instance().IsCharacterOwnerOrRenterOfHouse(character, houseId))
                {
                    // Update spawn position for character
                    UpdateCharacterSpawnPosition(character, houseId);
                }
            }
        }

        /// <summary>
        /// Subscribes delegate to player disconnected event
        /// </summary>
        /// <param name="delegateMethod">Delegate</param>
        public void SubscribeToPlayerDisconnectEvent(OnPlayerDisconnectDelegate delegateMethod)
        {
            OnPlayerDisconnectEvent += delegateMethod;
        }

        /// <summary>
        /// Unsubscribes delegate from player disconnected event
        /// </summary>
        /// <param name="delegateMethod">Delegate</param>
        public void UnsubscribeFromPlayerDisconnectEvent(OnPlayerDisconnectDelegate delegateMethod)
        {
            OnPlayerDisconnectEvent -= delegateMethod;
        }

        public void SubscribeToCharacterDeathEvent(OnCharacterDeathDelegate delegateMethod)
        {
            OnCharacterDeathEvent += delegateMethod;
        }

        public void UnsubcsriveFromCharacterDeathEvent(OnCharacterDeathDelegate delegateMethod)
        {
            OnCharacterDeathEvent -= delegateMethod;
        }

        /// <summary>
        /// Gets player client for nethandle
        /// </summary>
        /// <param name="handle">Handle</param>
        /// <returns>Client object</returns>
        public Client ClientForHandle(NetHandle handle)
        {
            Client client = API.shared.getPlayerFromHandle(handle);
            return client;
        }

        /// <summary>
        /// Gets player object for nethandle
        /// </summary>
        /// <param name="handle">Handle</param>
        /// <returns>Player object</returns>
        public Player PlayerForHandle(NetHandle handle)
        {
            Client client = ClientForHandle(handle);
            if (client == null)
            {
                return null;
            }

            return this.GetPlayerByClient(client);
        }

        /// <summary>
        /// Updates character money to database
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="newMoney">New money amount</param>
        public void UpdateCharacterMoneyToDatabase(Character character, int newMoney)
        {
            DBManager.UpdateQuery("UPDATE characters SET money=@money WHERE id=@id")
            .AddValue("@id", character.ID)
            .AddValue("@money", newMoney)
            .Execute();
        }

        /// <summary>
        /// Gets active character(currently used character) for client
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>Currently active character of client</returns>
        public Character GetActiveCharacterForClient(Client client)
        {
            return GetPlayerByClient(client).activeCharacter;
        }

        /// <summary>
        /// Gets player object for client
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>Player object for client, null if no player found</returns>
        public Player GetPlayerByClient(Client client)
        {
            return players.SingleOrDefault(x => x.client == client);
        }

        /// <summary>
        /// Opens account creation menu for client
        /// </summary>
        /// <param name="client">Client to which for open the menu</param>
        public void OpenCreateAccountMenu(Client client)
        {
            API.shared.triggerClientEvent(client, "EVENT_OPEN_CREATE_ACCOUNT_MENU", client.name);
        }

        /// <summary>
        /// Sets money for character with id
        /// </summary>
        /// <param name="id">ID of character</param>
        /// <param name="newMoney">Amount of money to set</param>
        public void UpdateMoneyForCharacterWithId(int id, int newMoney)
        {
            if (IsCharacterWithIdOnline(id))
            {
                Character character = GetCharacterWithId(id);
                character.SetMoney(newMoney);
            }
            else
            {
                DBManager.UpdateQuery("UPDATE characters SET money=@money WHERE id=@id").AddValue("@id", id).AddValue("@money", newMoney).Execute();
            }
        }

        /// <summary>
        /// Adds money for character with certain id
        /// </summary>
        /// <param name="id">Character id</param>
        /// <param name="addMount">Amount of money to add</param>
        public void AddMoneyForCharacterWithId(int id, int addAmount)
        {
            if (IsCharacterWithIdOnline(id))
            {
                Character character = GetCharacterWithId(id);
                character.SetMoney(character.money + addAmount);
            }
            else
            {
                int money = 0;

                DBManager.SelectQuery("SELECT money FROM characters WHERE id=@id", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
                {
                    money = reader.GetInt32(0);
                })
                .AddValue("@id", id)
                .Execute();

                DBManager.UpdateQuery("UPDATE characters SET money=@money WHERE id=@id").AddValue("@id", id).AddValue("@money", money + addAmount).Execute();
            }
        }

        /// <summary>
        /// Request creating an account
        /// </summary>
        /// <param name="client">Client for which to create the account</param>
        /// <param name="accountName">Account name</param>
        /// <param name="password">Account password</param>
        public void RequestCreateAccount(Client client, string accountName, string password)
        {
            // Check if account exists already

            if (DoesPlayerHaveAccount(client))
            {
                return;
            }

            if (!password.All(char.IsLetterOrDigit))
            {
                API.shared.sendNotificationToPlayer(client, "Password can only contain characters and numerals!");
                return;
            }

            if (password.Length > maxPasswordLength)
            {
                API.shared.sendNotificationToPlayer(client, String.Format("Password is not allowed to be longer than {0} characters!", maxPasswordLength));
                return;
            }

            if (password.Length < minPasswordLength)
            {
                API.shared.sendNotificationToPlayer(client, String.Format("Password has to be at least {0} characters long!", minPasswordLength));
                return;
            }

            this.CreateAccount(client, accountName, password);
        }

        /// <summary>
        /// Gets all characters in a certain radius from other character
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="radius">Radius</param>
        /// <returns>List of characters</returns>
        public List<Character> GetCharactersInRadiusOfCharacter(Character character, float radius)
        {
            return GetCharactersInDistance(character.position, radius);
        }

        /// <summary>
        /// Opens character selection menu for player
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="characters">Character to select from</param>
        public void OpenCharacterSelectionForPlayer(Player player, List<Character> characters)
        {
            this.characterSelector.AddPlayerToCharacterSelector(player, characters);
        }

        /// <summary>
        /// Request to select character
        /// </summary>
        /// <param name="client">Client who makes the request</param>
        /// <param name="characterName">Character to choose</param>
        public void RequestSelectCharacter(Client client, string characterName)
        {
            Player player = this.GetPlayerByClient(client);
            this.characterSelector.SelectCharacter(player, characterName);
        }

        /// <summary>
        /// Checks if client is using a character
        /// </summary>
        /// <param name="client">Client</param>
        /// <returns>True if client is using character, otherwise false</returns>
        public Boolean IsClientUsingCharacter(Client client)
        {
            if (this.IsPlayerLoggedIn(client))
            {
                Player player = this.GetPlayerByClient(client);
                if (player.activeCharacter != null)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Adds a new player to the player pool
        /// </summary>
        /// <param name="player">Player to add</param>
        public void AddNewPlayerToPool(Player player)
        {
            this.players.Add(player);
        }

        /// <summary>
        /// Gets all active characters in the server
        /// </summary>
        /// <returns>List of active characters</returns>
        public List<Character> GetActiveCharacters()
        {
            return players.Where(p => p.activeCharacter != null).Select(x => x.activeCharacter).ToList();
        }

        /// <summary>
        /// Gets character with a name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Character with given name</returns>
        public Character GetCharacterWithName(String name)
        {
            return GetActiveCharacters().Single(c => name.Equals(c.fullName));
        }

        /// <summary>
        /// Initializes models to use in character selection
        /// </summary>
        public void InitCharacterSelectorModels()
        {
            this.characterSelector.InitAllowedCharacterCreatorModels();
        }

        /// <summary>
        /// Initializes phone numbers
        /// </summary>
        public void InitPhoneNumbers()
        {
            this.characterSelector.InitPhoneNumbers();
        }

        /// <summary>
        /// Requests the menu for creating character
        /// </summary>
        /// <param name="player">Player which makes the request</param>
        public void RequestCreateCharacterMenu(Client player)
        {
            this.characterSelector.OpenCharacterCreationMenu(this.GetPlayerByClient(player));
        }

        /// <summary>
        /// Requests a creation of character
        /// </summary>
        /// <param name="player">Player who makes the request</param>
        /// <param name="firstName">First name of character</param>
        /// <param name="lastName">Last name of character</param>
        /// <param name="modelHash">Character model hash</param>
        public void RequestCreateCharacter(Client player, string firstName, string lastName, string modelHash)
        {
            if (firstName.Length < firstNameMinLength || firstName.Length > firstNameMaxLength)
            {
                API.shared.sendNotificationToPlayer(player, String.Format("First name has to be between {0} and {1} characters long!", firstNameMinLength, firstNameMaxLength));
                return;
            }

            if (lastName.Length < lastNameMinLength || lastName.Length > lastNameMaxLength)
            {
                API.shared.sendNotificationToPlayer(player, String.Format("Last name has to be between {0} and {1} characters long!", lastNameMinLength, lastNameMaxLength));
                return;
            }

            if (!characterSelector.IsModelAllowed(modelHash))
            {
                return;
            }

            if (this.DoesCharacterExistWithName(firstName, lastName))
            {
                API.shared.sendNotificationToPlayer(player, String.Format("Character with name {0} exists already", firstName + " " + lastName));
                return;
            }

            if (this.LoadCharactersForPlayer(this.GetPlayerByClient(player)).Count > maxCharactersCount)
            {
                API.shared.sendNotificationToPlayer(player, String.Format("Only {0} characters are allowed!", maxCharactersCount));
            }

            this.characterSelector.CreateCharacter(this.GetPlayerByClient(player), firstName, lastName, modelHash);
        }

        /// <summary>
        /// Initialize character creation ID
        /// </summary>
        public void InitAccountCreationId()
        {
            if (HasAccounts())
            {
                DBManager.SelectQuery("SELECT MAX(id) FROM players", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
                {
                    this.accountCreationId = reader.GetInt32(0) + 1;
                }).Execute();
            }
        }

        /// <summary>
        /// Initialize text messages ID
        /// </summary>
        public void InitTextMessagesId()
        {
            if (HasTextMessages())
            {
                DBManager.SelectQuery("SELECT MAX(id) FROM text_messages", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
                {
                    this.textMessageId = reader.GetInt32(0) + 1;
                }).Execute();
            }
            else
            {
                this.textMessageId = 0;
            }
        }

        /// <summary>
        /// Initialize character creation ID
        /// </summary>
        public void InitCharacterCreationId()
        {
            if (HasCharacters())
            {
                DBManager.SelectQuery("SELECT MAX(id) FROM characters", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
                {
                    this.characterSelector.characterCreationId = reader.GetInt32(0) + 1;
                }).Execute();
            }
            else
            {
                this.characterSelector.characterCreationId = 0;
            }
        }

        /// <summary>
        /// Gets characters in certain distance
        /// </summary>
        /// <param name="startPoint">Starting point</param>
        /// <param name="distance">Distance</param>
        /// <returns>Characters in distance</returns>
        public List<Character> GetCharactersInDistance(Vector3 startPoint, float distance)
        {
            return this.GetActiveCharacters().Where(c => c.owner.client.position.DistanceTo(startPoint) <= distance).ToList();
        }


        /// <summary>
        /// Initialize character genders
        /// </summary>
        public void InitCharacterGenders()
        {
            DBManager.SelectQuery("SELECT * FROM model_genders", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                characterGenderDictionary.Add(reader.GetString(0), reader.GetInt32(1));
            }).Execute();
        }

        /// <summary>
        /// Increments text messages ID
        /// </summary>
        public void IncrementTextMessageId()
        {
            this.textMessageId++;
        }

        /// <summary>
        /// Attempts to start a phone call
        /// </summary>
        /// <param name="client">Sender</param>
        /// <param name="number">Number to call to</param>
        public void TryStartPhoneCall(Client client, String number)
        {
            if (IsClientUsingCharacter(client))
            {
                Character caller = this.GetActiveCharacterForClient(client);

                if (caller != null)
                {
                    caller.phone.CallPhone(number);
                }
            }
        }

        /// <summary>
        /// Attemps to accept a phone call
        /// </summary>
        /// <param name="client">Client who accepts the phone call</param>
        public void TryAcceptPhoneCall(Client client)
        {
            if (IsClientUsingCharacter(client))
            {
                Character thisUser = this.GetActiveCharacterForClient(client);
                thisUser.phone.PickupCall();
            }
        }

        /// <summary>
        /// Attemps to hang up a phone call
        /// </summary>
        /// <param name="client">Client who tries to hang up the call</param>
        public void TryHangupPhoneCall(Client client)
        {
            if (IsClientUsingCharacter(client))
            {
                Character thisUser = this.GetActiveCharacterForClient(client);
                thisUser.phone.HangUpCall();
            }
        }

        /// <summary>
        /// Attempts to delete a contact
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="contactNumber">Contact number which to delete</param>
        public void TryDeleteContact(Client client, String contactNumber)
        {
            if (IsClientUsingCharacter(client))
            {
                Character character = this.GetActiveCharacterForClient(client);
                if (character.phone.HasContactForNumber(contactNumber))
                {
                    character.phone.RemoveContactFromAddressBook(contactNumber);
                }
            }
        }

        /// <summary>
        /// Attempts to delete text message
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="id">Text message id</param>
        public void TryDeleteTextMessage(Client client, int id)
        {
            if (IsClientUsingCharacter(client))
            {
                Character character = this.GetActiveCharacterForClient(client);
                if (character.phone.HasTextMessageWithId(id))
                {
                    character.phone.RemoveTextMessage(id);
                }
            }
        }

        /// <summary>
        /// Attempts to add a new contact
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="contactName">Contact name</param>
        /// <param name="contactNumber">Contact number</param>
        public void TryAddNewContact(Client client, String contactName, String contactNumber)
        {
            if (IsClientUsingCharacter(client))
            {
                if (ValidateContact(client, contactName, contactNumber))
                {
                    Character character = this.GetActiveCharacterForClient(client);
                    character.phone.AddNameToAddressBook(contactName, contactNumber);
                }
            }
        }

        /// <summary>
        /// Attempts to send a text message
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="receiverNumber">Number to send message to</param>
        /// <param name="message">Message text</param>
        public void TrySendTextMessage(Client client, String receiverNumber, String message)
        {
            if (IsClientUsingCharacter(client))
            {
                // Implement additional checks like number length and message length
                if (ValidateTextMessage(client, receiverNumber, message))
                {
                    Character character = this.GetActiveCharacterForClient(client);
                    character.phone.SendMessage(receiverNumber, message);
                    character.SendNotification("Message sent!");
                }
            }
        }

        /// <summary>
        /// Delivers sent text message to a number
        /// </summary>
        /// <param name="receiver">Receiver number</param>
        /// <param name="message">Message object</param>
        public void DeliverTextMessageToNumber(String receiver, TextMessage message)
        {
            Character deliverCharacter = this.GetCharacterWithPhoneNumber(receiver);

            if (deliverCharacter != null)
            {
                deliverCharacter.phone.ReceiveMessage(this.textMessageId, message.senderNumber, message.message, message.time);
            }
            else
            {
                this.DeliverOfflineTextMessage(this.textMessageId, receiver, message);
            }

            this.IncrementTextMessageId();
        }

        /// <summary>
        /// Gets gender for selected model
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>0 for male, 1 for female</returns>
        public int GetGenderForModel(String model)
        {
            if (!characterGenderDictionary.ContainsKey(model))
            {
                return 0;
            }

            return characterGenderDictionary.Get(model);
        }

        /// <summary>
        /// Sets player using phone
        /// Phone out in hand
        /// </summary>
        /// <param name="client">Player</param>
        public void SetPlayerUsingPhone(Client client)
        {
            if (IsClientUsingCharacter(client))
            {
                Character character = GetActiveCharacterForClient(client);
                character.phone.SetPhoneUsing();
            }
        }

        /// <summary>
        /// Sets player calling with phone
        /// Phone on ear
        /// </summary>
        /// <param name="client">Player</param>
        public void SetPlayerPhoneCalling(Client client)
        {
            if (IsClientUsingCharacter(client))
            {
                Character character = GetActiveCharacterForClient(client);
                character.phone.SetPhoneCalling();
            }
        }

        /// <summary>
        /// Sets phone out for player
        /// </summary>
        /// <param name="client">Player</param>
        public void SetPlayerPhoneOut(Client client)
        {
            if (IsClientUsingCharacter(client))
            {
                Character character = GetActiveCharacterForClient(client);
                character.phone.SetPhoneNotUsing();
            }
        }

        /// <summary>
        /// Gets characters nearby
        /// </summary>
        /// <param name="character">Character from which to take the distance</param>
        /// <param name="distance">Distance</param>
        /// <returns>List of characters in this distance</returns>
        public List<Character> GetNearbyCharacters(Character character, float distance)
        {
            return GetActiveCharacters().Where(x => x.position.DistanceTo(character.position) <= distance).ToList();
        }

    }
}
