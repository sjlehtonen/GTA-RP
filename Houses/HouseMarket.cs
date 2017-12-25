using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Houses
{
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

        public int entryId;
        public int price;
        public string name;
        public string sellerName;
        public int sellerId;
        public int templateId;
    }

    struct RandomHouseTemplate
    {
        public int minPrice;
        public int maxPrice;
        public int templateId;
    }

    class HouseMarket
    {
        private int currentEntryId = 0;
        private Random random = new Random();
        private List<HouseForSale> housesForSale = new List<HouseForSale>();
        private List<RandomHouseTemplate> randomHouseTemplates = new List<RandomHouseTemplate>();

        public HouseMarket()
        {

        }

        public void AddPlayerHouseForSale(Character character, int houseId)
        {

        }

        public List<HouseForSale> GetAllHousesForSale()
        {
            return housesForSale;
        }

        private HouseForSale GenerateNonPlayerHouse()
        {
            int position = random.Next(0, randomHouseTemplates.Count);
            RandomHouseTemplate template = randomHouseTemplates.ElementAt(position);
            int price = random.Next(template.minPrice, template.maxPrice + 1);
            HouseForSale house = new HouseForSale(currentEntryId, price, HouseManager.Instance().GetHouseNameForTemplateId(template.templateId), "Los Santos Government", -1, template.templateId);
            currentEntryId++;
            return house;
        }

        private void AddRandomNonPlayerHouseForSale()
        {
            housesForSale.Add(GenerateNonPlayerHouse());
        }
    }
}
