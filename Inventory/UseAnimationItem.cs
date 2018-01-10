using GTA_RP.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Globalization;
using GTA_RP.Factions;

namespace GTA_RP.Items
{
    class UseAnimationItem : Item
    {
        private Vector3 usePosMan;
        private Vector3 useRotMan;
        private Vector3 usePosFemale;
        private Vector3 useRotFemale;

        private string animDict;
        private string animName;
        private int entityId;
        private int bone;
        private NetHandle entity;
        private bool equipped = false;

        public UseAnimationItem(int id, string name, string description, int entityId, int bone, string other, int count = 1) : base(id, name, description, count)
        {
            this.entityId = entityId;
            this.bone = bone;

            string[] splitString = other.Split(';');
            this.animDict = splitString[0];
            this.animName = splitString[1];

            string[] posSplit = splitString[2].Split(',');
            string[] rotSplit = splitString[3].Split(',');

            string[] posSplit2 = splitString[4].Split(',');
            string[] rotSplit2 = splitString[5].Split(',');

            this.usePosMan = new Vector3(float.Parse(posSplit[0], CultureInfo.InvariantCulture), float.Parse(posSplit[1], CultureInfo.InvariantCulture), float.Parse(posSplit[2], CultureInfo.InvariantCulture));
            this.useRotMan = new Vector3(float.Parse(rotSplit[0], CultureInfo.InvariantCulture), float.Parse(rotSplit[1], CultureInfo.InvariantCulture), float.Parse(rotSplit[2], CultureInfo.InvariantCulture));

            this.usePosFemale = new Vector3(float.Parse(posSplit2[0], CultureInfo.InvariantCulture), float.Parse(posSplit2[1], CultureInfo.InvariantCulture), float.Parse(posSplit2[2], CultureInfo.InvariantCulture));
            this.useRotFemale = new Vector3(float.Parse(rotSplit2[0], CultureInfo.InvariantCulture), float.Parse(rotSplit2[1], CultureInfo.InvariantCulture), float.Parse(rotSplit2[2], CultureInfo.InvariantCulture));
        }

        public override void AddedToInventory(Character owner)
        {

        }

        public override void RemovedFromInventory(Character owner)
        {

        }

        public override void Use(Character user)
        {
            if (!user.isInVehicle && !user.IsUsingPhone() && user.IsAvailable())
            {
                if (!equipped)
                {
                    user.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), this.animDict, this.animName);
                    entity = API.shared.createObject(entityId, user.position, new Vector3(0, 0, 0));
                    if (user.gender == 0) user.AttachObject(entity, bone.ToString(), this.usePosMan, this.useRotMan);
                    else user.AttachObject(entity, bone.ToString(), this.usePosFemale, this.useRotFemale);
                    equipped = true;
                }
                else
                {
                    user.StopAnimation();
                    equipped = false;
                    API.shared.deleteEntity(entity);
                }
            }
        }
    }
}
