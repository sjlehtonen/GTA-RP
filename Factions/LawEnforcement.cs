using System;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;

namespace GTA_RP.Factions
{

    class LawEnforcement : RankedFaction
    {

        public LawEnforcement(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB)
        {
        }

        private void InitializeRanks()
        {
            AddRank(0, "Police Officer I");
            AddRank(1, "Police Officer II");
            AddRank(2, "Traffic Police");
            AddRank(3, "Detective");
            AddRank(4, "Chief of Police");
        }

        private void LoadCharacterInfoFromDB()
        {
            DBManager.SelectQuery("SELECT * FROM police_ranks", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                AddCharacterToFaction(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2));
            }).Execute();
        }

        public override void Initialize()
        {
            API.shared.consoleOutput("Init police faction");
            InitializeRanks();
            LoadCharacterInfoFromDB();
        }

        public override string GetRankText(Character character)
        {
            if (IsCharacterPartOfFaction(character))
            {
                Rank rank = GetRankForCharacter(character);
                return rank.name;
            }

            return "Police Officer";
        }

        /// <summary>
        /// Returns chat color for the law enforcement
        /// </summary>
        /// <returns>Color to use in chat for law enforcement players' names</returns>
        public override string GetChatColor()
        {
            return "~b~";
        }

        public void ArrestCharacter(Character arrester, int CharacterToArrest)
        {
            API.shared.sendChatMessageToPlayer(arrester.owner.client, "adsda");
        }

        /// <summary>
        /// Handles on duty command for law enforcement players
        /// </summary>
        /// <param name="c">Character object</param>
        override public void HandleOnDutyCommand(Character c)
        {
            if (IsCharacterPartOfFaction(c))
            {
                if (!c.onDuty)
                {
                    CharacterInfo info = GetInfoForCharacter(c);
                    Rank rank = GetRank(info.rank);

                    c.onDuty = true;
                    c.SetModel(info.onDutyModel);

                    // Determine what to do based on rank
                    // Add weapons, items etc

                    //c.UpdateFactionRankText(rank.name, 255, 255, 255);
                } else
                {
                    c.onDuty = false;
                    c.SetModel(c.model);
                }
            }
        }
    }
}
