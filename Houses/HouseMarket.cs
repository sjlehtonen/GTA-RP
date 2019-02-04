using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GTA_RP.Misc;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Map;

namespace GTA_RP.Houses
{
    /// <summary>
    /// Structure that represents a house that is on sale.
    /// </summary>
    struct HouseForSale
    {
        public HouseForSale(int entryId, int price, string name, string sellerName, int sellerId, int templateId)
        {
            this.entryId = entryId;
            this.price = price;
            this.name = name;
            this.sellerName = sellerName;
            this.sellerId = sellerId;
            this.templateId = templateId;
        }

        public static readonly HouseForSale Empty = new HouseForSale();
        public int entryId;
        public int price;
        public string name;
        public string sellerName;
        public int sellerId;
        public int templateId;

        public static bool operator ==(HouseForSale houseA, HouseForSale houseB)
        {
            return houseA.entryId == houseB.entryId;
        }

        public static bool operator !=(HouseForSale houseA, HouseForSale houseB)
        {
            return !(houseA == houseB);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HouseForSale))
            {
                return false;
            }

            var sale = (HouseForSale)obj;
            return entryId == sale.entryId;
        }

        public override int GetHashCode()
        {
            return entryId.GetHashCode();
        }
    }

    struct RandomHouseTemplate
    {
        public RandomHouseTemplate(int minPrice, int maxPrice, int templateId)
        {
            this.minPrice = minPrice;
            this.maxPrice = maxPrice;
            this.templateId = templateId;
        }

        public int minPrice;
        public int maxPrice;
        public int templateId;
    }

    /// <summary>
    /// Class that handles the market for properties
    /// Handles listings, selling and buying of properties
    /// </summary>
    class HouseMarket
    {
        private int currentEntryId = 1;
        private const int maxNonPlayerHousesForSale = 6;
        private Random random = new Random();
        private List<HouseForSale> housesForSale = new List<HouseForSale>();
        private List<RandomHouseTemplate> randomHouseTemplates = new List<RandomHouseTemplate>();
        private Timer refreshHouseTimer = new Timer();
        private Checkpoint entryCheckpoint;

        public HouseMarket()
        {
            Vector3 position = new Vector3(-368.2728, -240.6654, 35.08997);
            entryCheckpoint = new Checkpoint(position, OnEnterMarket, OnExitMarket, 25, 2, 0);
            MapManager.Instance().AddBlipToMap(374, "Los Santos Properties", position.X, position.Y, position.Y);

            InitializeRandomHouseTemplates();
            InitializePlayerHouseSales();
            InitializeNonPlayerHouseSales();

            refreshHouseTimer.Interval = TimeSpan.FromHours(6).TotalMilliseconds;
            refreshHouseTimer.Elapsed += TimerEvent;
            refreshHouseTimer.Enabled = true;

        }

        /// <summary>
        /// Initializes random house templates from the database
        /// </summary>
        private void InitializeRandomHouseTemplates()
        {
            DBManager.SelectQuery("SELECT * FROM random_house_sell_templates", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                randomHouseTemplates.Add(new RandomHouseTemplate(reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(0)));
            }).Execute();
        }

        /// <summary>
        /// Gets names of buildings for houses on sale
        /// </summary>
        /// <param name="housesForSale"></param>
        /// <returns>List of building names</returns>
        private List<string> GetBuildingNamesForHousesOnSale(List<HouseForSale> housesForSale)
        {
            List<string> buildingNames = new List<string>();
            foreach (HouseForSale house in housesForSale)
            {
                buildingNames.Add(HouseManager.Instance().GetBuildingNameForHouseTemplateId(house.templateId));
            }
            return buildingNames;
        }

        /// <summary>
        /// Is ran when character steps into the market checkpoint
        /// </summary>
        /// <param name="checkpoint">Checkpoint(marker)</param>
        /// <param name="character">Character</param>
        private void OnEnterMarket(Checkpoint checkpoint, Character character)
        {
            List<HouseForSale> housesForSale = new List<HouseForSale>();
            housesForSale.AddRange(this.housesForSale);
            housesForSale.Sort((x, y) => x.price.CompareTo(y.price));
            character.TriggerEvent("EVENT_OPEN_HOUSE_MARKET_MENU", housesForSale.Select(x => x.entryId).ToList(), housesForSale.Select(x => x.name).ToList(), housesForSale.Select(x => x.price).ToList(), housesForSale.Select(x => x.sellerName).ToList(), GetBuildingNamesForHousesOnSale(housesForSale));
        }

        /// <summary>
        /// Closes the market menu for character
        /// </summary>
        /// <param name="character">Character</param>
        private void CloseHouseMarketMenuForCharacter(Character character)
        {
            character.TriggerEvent("EVENT_CLOSE_HOUSE_MARKET_MENU");
        }

        /// <summary>
        /// Is ran when character walks out of the checkpoint (marker)
        /// </summary>
        /// <param name="checkpoint">Checkpoint(marker)</param>
        /// <param name="character">Character</param>
        private void OnExitMarket(Checkpoint checkpoint, Character character)
        {
            this.CloseHouseMarketMenuForCharacter(character);
        }

        /// <summary>
        /// Refreshes house listings
        /// Ran on certain intervals
        /// </summary>
        /// <param name="source">Timer</param>
        /// <param name="eventArgs">Arguments</param>
        private void TimerEvent(System.Object source, ElapsedEventArgs eventArgs)
        {
            RefreshNonPlayerHouseListings();
            refreshHouseTimer.Enabled = true;
        }

        /// <summary>
        /// Returns house for certain entry id
        /// </summary>
        /// <param name="entryId">Entry id for house on sale</param>
        /// <returns>House with certain entry id</returns>
        private HouseForSale GetHouseForSaleForEntryId(int entryId)
        {
            return housesForSale.SingleOrDefault(x => x.entryId == entryId);
        }

        /// <summary>
        /// Removes a house that is on sale
        /// </summary>
        /// <param name="house">House</param>
        private void RemoveHouseFromSale(HouseForSale house)
        {
            this.housesForSale.Remove(house);
        }

        /// <summary>
        /// Adds a player owned house to sale
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="houseId">House id</param>
        public void AddPlayerHouseForSale(Character character, int houseId)
        {
            // TODO
        }

        /// <summary>
        /// Returns all houses that are currently on sale
        /// </summary>
        /// <returns></returns>
        public List<HouseForSale> GetAllHousesForSale()
        {
            return housesForSale;
        }

        /// <summary>
        /// Attemps to purchase a house for character
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="sellEntryId">Entry id of the house on sale</param>
        /// <param name="nameOfHouse">Name of the house</param>
        public void TryBuyHouseForCharacter(Character character, int sellEntryId, string nameOfHouse)
        {
            if (!entryCheckpoint.IsCharacterInsideCheckpoint(character))
            {
                this.CloseHouseMarketMenuForCharacter(character);
                return;
            }

            HouseForSale house = GetHouseForSaleForEntryId(sellEntryId);
            if (house != HouseForSale.Empty)
            {
                if (character.money >= house.price)
                {
                    HouseManager.Instance().AddHouseOwnershipForCharacter(character, house.templateId, nameOfHouse);
                    character.SetMoney(character.money - house.price);
                    if (IsSellerOfHousePlayer(house))
                    {
                        PlayerManager.Instance().AddMoneyForCharacterWithId(house.sellerId, house.price);
                        if (PlayerManager.Instance().IsCharacterWithIdOnline(house.sellerId))
                        {
                            PlayerManager.Instance().SendNotificationToCharacterWithid(sellEntryId, String.Format("Your house {0} was just sold for {1}!", house.name, house.price));
                        }
                    }
                    this.RemoveHouseFromSale(house);
                    this.CloseHouseMarketMenuForCharacter(character);
                    character.PlayFrontendSound("LOCAL_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
                    character.SendNotification(String.Format("Congratulations! You have bought a house at {0} for ${1}", HouseManager.Instance().GetBuildingNameForHouseTemplateId(house.templateId), house.price.ToString()));
                }
                else
                {
                    character.SendErrorNotification("You don't have enought money to buy this property!");
                }
            }
            else
            {
                character.SendErrorNotification("Property not found!");
            }
        }

        /// <summary>
        /// Tries to put a house on sale
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="houseId">House id</param>
        public void TryPutHouseForSaleForCharacter(Character character, int houseId)
        {
            // TODO
        }

        /// <summary>
        /// Refreshes non-player house sell listings
        /// Replaces a random amount of houses on sale, or if not enough, then adds new ones
        /// </summary>
        private void RefreshNonPlayerHouseListings()
        {
            int replaceAmount = random.Next(1, 8);
            for (int i = 0; i < housesForSale.Count; i++)
            {
                HouseForSale house = housesForSale.ElementAt(i);
                if (!IsSellerOfHousePlayer(house))
                {
                    housesForSale.Insert(i, GenerateNonPlayerHouse());
                    replaceAmount--;
                }

                if (replaceAmount == 0)
                {
                    break;
                }
            }

            for (int i = 0; i < replaceAmount; i++)
            {
                housesForSale.Add(GenerateNonPlayerHouse());
            }
        }

        /// <summary>
        /// Initializes non-player house sells
        /// </summary>
        private void InitializeNonPlayerHouseSales()
        {
            for (int i = 0; i < maxNonPlayerHousesForSale; i++)
            {
                housesForSale.Add(GenerateNonPlayerHouse());
            }
        }

        /// <summary>
        /// Initializes player house sells from the database
        /// </summary>
        private void InitializePlayerHouseSales()
        {
            // TODO
            // Set currentEntryId here to the max + 1 of the player house sell ids
        }

        /// <summary>
        /// Checks if a seller of a house is a player
        /// </summary>
        /// <param name="house">House</param>
        /// <returns>Yes if seller is player, otherwise no</returns>
        private bool IsSellerOfHousePlayer(HouseForSale house)
        {
            if (house.sellerId != -1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates a random house for sale based on template
        /// </summary>
        /// <returns>Random house for sale</returns>
        private HouseForSale GenerateNonPlayerHouse()
        {
            int position = random.Next(0, randomHouseTemplates.Count);
            RandomHouseTemplate template = randomHouseTemplates.ElementAt(position);
            int price = random.Next(template.minPrice, template.maxPrice + 1);
            HouseForSale house = new HouseForSale(currentEntryId, price, HouseManager.Instance().GetHouseNameForTemplateId(template.templateId), "Los Santos Government", -1, template.templateId);
            currentEntryId++;
            return house;
        }

        /// <summary>
        /// Adds a non-player house for sale
        /// </summary>
        private void AddRandomNonPlayerHouseForSale()
        {
            housesForSale.Add(GenerateNonPlayerHouse());
        }
    }
}
