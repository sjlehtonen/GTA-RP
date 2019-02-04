using System;
using System.Collections.Generic;
using System.Linq;

namespace GTA_RP.Factions
{
    /// <summary>
    /// Structure to represent a rank
    /// </summary>
    struct Rank
    {
        public Rank(string name, int salary, params int[] itemIds)
        {
            this.name = name;
            this.itemIds = new List<int>();
            this.salary = salary;
            foreach (int itemId in itemIds) {
                this.itemIds.Add(itemId);
            }
        }

        public string name;
        List<int> itemIds;
        public int salary;
    }


    struct CharacterInfo
    {
        public CharacterInfo(int rank, string onDutyModel)
        {
            this.rank = rank;
            this.onDutyModel = onDutyModel;
        }

        public int rank;
        public string onDutyModel;
    }

    /// <summary>
    /// Faction subclass that represents a faction that
    /// has different ranks.
    /// </summary>
    abstract class RankedFaction : Faction
    {
        private Dictionary<int, CharacterInfo> characterInfos = new Dictionary<int, CharacterInfo>();
        private Dictionary<int, Rank> ranks = new Dictionary<int, Rank>();
        protected string paySalaryString = "You were paid ${0} by the Los Santos Government";


        public RankedFaction(FactionEnums id, String name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        /// <summary>
        /// Adds a new rank to the faction
        /// </summary>
        /// <param name="id">Rank id</param>
        /// <param name="name">Rank name</param>
        /// <param name="salary">Salary</param>
        /// <param name="items">Items on duty(TODO)</param>
        protected void AddRank(int id, string name, int salary, params int[] items)
        {
            ranks.Add(id, new Rank(name, salary, items));
        }

        /// <summary>
        /// Adds a character to faction.
        /// </summary>
        /// <param name="characterId">Character ID</param>
        /// <param name="rank">Rank to give to character</param>
        /// <param name="model">Character model name, applied on duty</param>
        protected void AddCharacterToFaction(int characterId, int rank, string model)
        {
            characterInfos.Add(characterId, new CharacterInfo(rank, model));
        }

        /// <summary>
        /// Checks if character is part of the faction.
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>True if part of the faction, otherwise false</returns>
        protected bool IsCharacterPartOfFaction(Character character)
        {
            return characterInfos.ContainsKey(character.ID);
        }

        /// <summary>
        /// Gets character info
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>Character info</returns>
        protected CharacterInfo GetInfoForCharacter(Character character)
        {
            return characterInfos[character.ID];
        }

        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns></returns>
        protected Rank GetRank(int rankId)
        {
            return ranks[rankId];
        }

        /// <summary>
        /// Gets the rank of a character
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>Character rank within the faction</returns>
        protected Rank GetRankForCharacter(Character character)
        {
            CharacterInfo info = GetInfoForCharacter(character);
            return GetRank(info.rank);
        }

        /// <summary>
        /// Gets all characters in a faction.
        /// </summary>
        /// <returns>All characters in a faction</returns>
        protected List<int> GetIdOfAllCharactersInFaction()
        {
            return this.characterInfos.Keys.ToList();
        }

        /// <summary>
        /// Pays salary for all the characters in the faction.
        /// </summary>
        public override void PaySalary()
        {
            foreach (int characterId in this.GetIdOfAllCharactersInFaction())
            {
                if (PlayerManager.Instance().IsCharacterWithIdOnline(characterId))
                {
                    PaySalaryForCharacterWithId(characterId, paySalaryString);
                }
            }
        }

        /// <summary>
        /// Pays salary for a character with id
        /// </summary>
        /// <param name="id">Character id</param>
        /// <param name="message">Message to give to player</param>
        protected void PaySalaryForCharacterWithId(int id, string message)
        {
            Character character = PlayerManager.Instance().GetCharacterWithId(id);
            if (character != null)
            {
                Rank rank = GetRankForCharacter(character);
                character.SetMoney(character.money + rank.salary);
                character.SendNotification(String.Format(message, rank.salary));
            }
        }

        /// <summary>
        /// Gets rank text for character.
        /// For example: Fire officer
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>Rank text</returns>
        public override string GetRankText(Character character)
        {
            if (IsCharacterPartOfFaction(character))
            {
                Rank rank = GetRankForCharacter(character);
                return rank.name;
            }

            // Return empty text because no rank
            return "";
        }

        /// <summary>
        /// Loads character info from the database.
        /// </summary>
        private void LoadCharacterInfoFromDB()
        {
            DBManager.SelectQuery("SELECT * FROM faction_ranks", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                if (reader.GetInt32(3) == (int)this.id)
                {
                    AddCharacterToFaction(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2));
                }
            }).Execute();
        }

        /// <summary>
        /// Initializes the faction.
        /// Called by the faction manager.
        /// </summary>
        public override void Initialize()
        {
            LoadCharacterInfoFromDB();
        }

    }
}
