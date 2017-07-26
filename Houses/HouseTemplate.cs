using System;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Houses
{
    /// <summary>
    /// Template for places
    /// </summary>
    class HouseTemplate
    {
        public int id { get; private set; }
        public int building_id { get; private set; }
        public String ipl { get; private set; }
        public String house_name { get; private set; }
        public Vector3 spawn_position { get; private set; }

        public HouseTemplate(int id, int building_id, String ipl, String house_name, Vector3 spawn_position)
        {
            this.id = id;
            this.building_id = building_id;
            this.ipl = ipl;
            this.house_name = house_name;
            this.spawn_position = spawn_position;
        }

    }
}
