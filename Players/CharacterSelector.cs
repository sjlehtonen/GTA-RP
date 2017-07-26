using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Jobs;
using GTA_RP.Factions;
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
        private void PlayerDisconnected(Client c)
        {
            if (IsPlayerInCharacterSelection(c))
            {
                RemovePlayerFromCharacterSelector(c);
            }
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
            var cmd = DBManager.SimpleQuery("SELECT * FROM allowed_character_creator_models");
            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                allowedCharacterCreatorModels.Add(reader.GetString(0));
            }
            reader.Close();
        }

        /// <summary>
        /// Initializes phone numbers
        /// Ran on server start
        /// </summary>
        public void InitPhoneNumbers()
        {
            var cmd = DBManager.SimpleQuery("SELECT phone_number FROM characters");
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                phoneNumbers.Add(reader.GetString(0));
            }
            reader.Close();
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

            API.shared.consoleOutput("Player added to character selector!");
            API.shared.consoleOutput("There are " + allowedCharacterCreatorModels.Count.ToString() + " models");
            API.shared.triggerClientEvent(p.client, "EVENT_OPEN_CHARACTER_SELECT_MENU", characterNames, characterModels, characterSelectionCameraPosition, characterSelectionCameraRotation, allowedCharacterCreatorModels);
        }

        /// <summary>
        /// Removes player from character selection
        /// </summary>
        /// <param name="p">Player to remove</param>
        public void RemovePlayerFromCharacterSelector(Player p)
        {
            playerCharacters.Remove(p.id);
            players.Remove(p);
        }

        /// <summary>
        /// Removes a player from the character selector
        /// </summary>
        /// <param name="c">Client to remove</param>
        public void RemovePlayerFromCharacterSelector(Client c)
        {
            Player p = PlayerManager.Instance().GetPlayerByClient(c);
            RemovePlayerFromCharacterSelector(p);
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
            var cmd = DBManager.SimpleQuery("INSERT INTO characters VALUES (@id, @player_id, @first_name, @last_name, @faction_id, @player_model, @money, @job, @phone_number)");
            String phoneNumber = this.GenerateRandomPhoneNumber();

            lastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lastName);
            firstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(firstName);

            cmd.Parameters.AddWithValue("@id", characterCreationId);
            cmd.Parameters.AddWithValue("@player_id", p.id);
            cmd.Parameters.AddWithValue("@first_name", firstName);
            cmd.Parameters.AddWithValue("@last_name", lastName);
            cmd.Parameters.AddWithValue("@faction_id", 0);
            cmd.Parameters.AddWithValue("@player_model", model);
            cmd.Parameters.AddWithValue("@money", 200);
            cmd.Parameters.AddWithValue("@job", 0);
            cmd.Parameters.AddWithValue("@phone_number", phoneNumber);

            cmd.ExecuteNonQuery();

            Character c = new Character(p, characterCreationId, firstName, lastName, 0, model, 200, 0, phoneNumber);
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
                    p.client.position = new Vector3(-772.716, 311.984, 85.6981);
                    p.SetActiveCharacter(c);

                    this.RemovePlayerFromCharacterSelector(p);
                }
            }
        }
    }
}
