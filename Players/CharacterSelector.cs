using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Elements;
using System.Linq;
using System.Globalization;

namespace GTA_RP
{
    /// <summary>
    /// Class for handling the character selection process
    /// </summary>
    class CharacterSelector
    {
        private List<Player> players = new List<Player>();
        public int characterCreationId { get; set; }
        private Dictionary<int, List<Character>> playerCharacters = new Dictionary<int, List<Character>>();
        private List<string> allowedCharacterCreatorModels = new List<string>();
        private List<string> phoneNumbers = new List<string>();
        private Random random = new Random();

        private Vector3 characterSelectionPosition = new Vector3(-793.8248, 325.9066, 210.7967);
        private Vector3 characterSelectionRotation = new Vector3(0, 0, -49.91037);
        private Vector3 characterSelectionCameraPosition = new Vector3(-792.506, 327.5799, 210.7966);
        private Vector3 characterSelectionCameraRotation = new Vector3(0, 0, 133.3806);

        /// <summary>
        /// Is ran when player disconnects
        /// </summary>
        /// <param name="c">Client who disconnected</param>
        private void PlayerDisconnected(Character c)
        {
            RemovePlayerFromCharacterSelector(c);
        }

        /// <summary>
        /// Gets a character with name
        /// </summary>
        /// <param name="p">Player whose character is being searched</param>
        /// <param name="name">Character name</param>
        /// <returns>A player's character with a certain name</returns>
        private Character GetCharacterForName(Player p, string name)
        {
            List<Character> characters;
            playerCharacters.TryGetValue(p.id, out characters);
            foreach (Character c in characters)
            {
                if (c.fullName.Equals(name))
                {
                    return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a new character for player
        /// </summary>
        /// <param name="p">Player on who to add</param>
        /// <param name="c">New character</param>
        private void AddCharacterForPlayer(Player p, Character c)
        {
            List<Character> characters;
            playerCharacters.TryGetValue(p.id, out characters);
            characters.Add(c);
        }

        /// <summary>
        /// Generates a random phone number that doesn't exist already
        /// </summary>
        /// <returns>A phone number</returns>
        private String GenerateRandomPhoneNumber()
        {
            const string chars = "1234567890";
            const int length = 7;

            String number;
            do
            {
                number = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

            } while (phoneNumbers.Contains(number));

            return number;
        }

        // Public methods

        /// <summary>
        /// Initializes models that are allowed in character creation
        /// Ran on server start
        /// </summary>
        public void InitAllowedCharacterCreatorModels()
        {
            PlayerManager.Instance().SubscribeToPlayerDisconnectEvent(this.PlayerDisconnected);
            DBManager.SelectQuery("SELECT * FROM allowed_character_creator_models", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                allowedCharacterCreatorModels.Add(reader.GetString(0));
            }).Execute();
        }

        /// <summary>
        /// Initializes phone numbers
        /// Ran on server start
        /// </summary>
        public void InitPhoneNumbers()
        {
            DBManager.SelectQuery("SELECT phone_number FROM characters", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                phoneNumbers.Add(reader.GetString(0));
            }).Execute();
        }

        /// <summary>
        /// Adds a new player to character selection mode
        /// </summary>
        /// <param name="p">Player to add to character selection</param>
        /// <param name="characters">Player's characters</param>
        public void AddPlayerToCharacterSelector(Player p, List<Character> characters)
        {
            players.Add(p);
            playerCharacters.Add(p.id, characters);

            p.client.freezePosition = true;
            p.client.dimension = p.id;

            // Teleport to the selection room
            p.client.position = characterSelectionPosition;
            p.client.rotation = characterSelectionRotation;

            List<string> characterNames = new List<string>();
            List<string> characterModels = new List<string>();

            characters.ForEach(x => characterNames.Add(x.fullName));
            characters.ForEach(x => characterModels.Add(x.model));

            API.shared.triggerClientEvent(p.client, "EVENT_OPEN_CHARACTER_SELECT_MENU", characterNames, characterModels, characterSelectionCameraPosition, characterSelectionCameraRotation, allowedCharacterCreatorModels, characterSelectionPosition, characterSelectionRotation);
        }

        /// <summary>
        /// Removes player from character selection
        /// </summary>
        /// <param name="p">Player to remove</param>
        public void RemovePlayerFromCharacterSelector(Player p)
        {
            playerCharacters.Remove(p.id);
            p.client.dimension = 0;
            players.Remove(p);
        }

        /// <summary>
        /// Removes a player from the character selector
        /// </summary>
        /// <param name="c">Client to remove</param>
        public void RemovePlayerFromCharacterSelector(Client c)
        {
            Player p = PlayerManager.Instance().GetPlayerByClient(c);
            c.dimension = 0;
            RemovePlayerFromCharacterSelector(p);
        }

        public void RemovePlayerFromCharacterSelector(Character c)
        {
            RemovePlayerFromCharacterSelector(c.owner);
        }

        /// <summary>
        /// Checks if player is in character selection
        /// </summary>
        /// <param name="p">Player to check</param>
        /// <returns>True if player is in character selection, false if not</returns>
        public Boolean IsPlayerInCharacterSelection(Player p)
        {
            return players.Contains(p);
        }

        /// <summary>
        /// Checks if player is in character selection
        /// </summary>
        /// <param name="c">Client to check</param>
        /// <returns>True if player is in character selection, false if not</returns>
        public Boolean IsPlayerInCharacterSelection(Client c)
        {
            return players.Exists(p => p.client == c);
        }

        /// <summary>
        /// Opens character creation menu for player
        /// </summary>
        /// <param name="p">Player to open menu for</param>
        public void OpenCharacterCreationMenu(Player p)
        {
            if (IsPlayerInCharacterSelection(p))
            {
                API.shared.triggerClientEvent(p.client, "EVENT_OPEN_CHARACTER_CREATION_MENU");
            }
        }

        /// <summary>
        /// Checks if model is allowed for creating character
        /// </summary>
        /// <param name="model">Model string</param>
        /// <returns>True if allowed, otherwise false</returns>
        public Boolean IsModelAllowed(string model)
        {
            return allowedCharacterCreatorModels.Contains(model);
        }

        /// <summary>
        /// Creates a new character
        /// </summary>
        /// <param name="p">Player to create character for</param>
        /// <param name="firstName">Character's first name</param>
        /// <param name="lastName">Character's last name</param>
        /// <param name="model">Character's model</param>
        public void CreateCharacter(Player p, string firstName, string lastName, string model)
        {
            String phoneNumber = this.GenerateRandomPhoneNumber();

            lastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lastName);
            firstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(firstName);

            DBManager.InsertQuery("INSERT INTO characters VALUES (@id, @player_id, @first_name, @last_name, @faction_id, @player_model, @money, @job, @phone_number, @spawn_house_id)")
                .AddValue("@id", characterCreationId)
                .AddValue("@player_id", p.id)
                .AddValue("@first_name", firstName)
                .AddValue("@last_name", lastName)
                .AddValue("@faction_id", 0)
                .AddValue("@player_model", model)
                .AddValue("@money", 200)
                .AddValue("@job", 0)
                .AddValue("@phone_number", phoneNumber)
                .AddValue("@spawn_house_id", -1)
                .Execute();

            Character c = new Character(p, characterCreationId, firstName, lastName, 0, model, 200, 0, phoneNumber, -1);
            this.AddCharacterForPlayer(p, c);
            characterCreationId++;
            this.SelectCharacter(p, c.fullName);
        }

        /// <summary>
        /// Selects a character
        /// </summary>
        /// <param name="p">Player to select character for</param>
        /// <param name="characterName">Character's name</param>
        public void SelectCharacter(Player p, string characterName)
        {
            if (IsPlayerInCharacterSelection(p))
            {
                Character c = this.GetCharacterForName(p, characterName);

                if (c != null)
                {

                    p.client.dimension = 0;
                    p.client.freezePosition = false;

                    if (PlayerManager.Instance().TeleportPlayerToJailIfTimeLeft(c))
                    {
                        c.SendNotification("You have " + PlayerManager.Instance().GetTimeLeftInJailForCharacter(c) + " minutes of jail time left");
                    }
                    else if (c.spawnHouseId != -1)
                    {
                        HouseManager.Instance().AddCharacterToHouseWithId(c.spawnHouseId, c);
                        p.client.position = HouseManager.Instance().GetSpawnLocationOfHouseWithId(c.spawnHouseId);
                    }
                    else
                    {
                        // Default spawn
                        p.client.position = new Vector3(-692.194, 295.9935, 82.83133);
                    }

                    p.SetActiveCharacter(c);
                    this.RemovePlayerFromCharacterSelector(p);
                }
            }
        }
    }
}
