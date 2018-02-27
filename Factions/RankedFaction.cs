using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Factions
{
    struct Rank
    {
        public Rank(string name, params int[] itemIds)
        {
            this.name = name;
            this.itemIds = new List<int>();
            foreach (int itemId in itemIds)
            {
                this.itemIds.Add(itemId);
            }
        }

        public string name;
        List<int> itemIds;
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

    abstract class RankedFaction : Faction
    {
        private Dictionary<int, CharacterInfo> characterInfos = new Dictionary<int, CharacterInfo>();
        private Dictionary<int, Rank> ranks = new Dictionary<int, Rank>();


        public RankedFaction(FactionI id, String name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB)
        {
        }

        protected void AddRank(int id, string name, params int[] items)
        {
            ranks.Add(id, new Rank(name, items));
        }

        protected void AddCharacterToFaction(int characterId, int rank, string model)
        {
            characterInfos.Add(characterId, new CharacterInfo(rank, model));
        }

        protected bool IsCharacterPartOfFaction(Character character)
        {
            return characterInfos.ContainsKey(character.ID);
        }

        protected CharacterInfo GetInfoForCharacter(Character character)
        {
            return characterInfos[character.ID];
        }

        protected Rank GetRank(int rankId)
        {
            return ranks[rankId];
        }

        protected Rank GetRankForCharacter(Character character)
        {
            CharacterInfo info = GetInfoForCharacter(character);
            return GetRank(info.rank);
        }

        public override string GetRankText(Character character)
        {
            if (IsCharacterPartOfFaction(character))
            {
                Rank rank = GetRankForCharacter(character);
                return rank.name;
            }

            return "";
        }

        private void LoadCharacterInfoFromDB()
        {
            DBManager.SelectQuery("SELECT * FROM faction_ranks", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                if (reader.GetInt32(3) == (int)this.id)
                    AddCharacterToFaction(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2));
            }).Execute();
        }

        public override void Initialize()
        {
            LoadCharacterInfoFromDB();
        }

    }
}
