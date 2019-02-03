using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Globalization;
using GTA_RP.Factions;

namespace GTA_RP.Items
{
    /// <summary>
    /// Item that has an animation on use
    /// </summary>
    class UseAnimationItem : Item
    {
        protected Vector3 usePosMan;
        protected Vector3 useRotMan;
        protected Vector3 usePosFemale;
        protected Vector3 useRotFemale;

        protected string animDict;
        protected string animDictFemale;
        protected string animName;
        protected int entityId;
        protected int bone;
        protected NetHandle entity;
        protected bool equipped = false;

        public UseAnimationItem(int id, string name, string description, int entityId, int bone, string other, int count = 1) : base(id, name, description, count)
        {
            this.entityId = entityId;
            this.bone = bone;

            string[] splitString = other.Split(';');
            this.animDict = splitString[0];

            if (splitString[1] == "null")
            {
                animDictFemale = null;
            }
            else
            {
                animDictFemale = splitString[1];
            }

            this.animName = splitString[2];

            string[] posSplit = splitString[3].Split(',');
            string[] rotSplit = splitString[4].Split(',');

            string[] posSplit2 = splitString[5].Split(',');
            string[] rotSplit2 = splitString[6].Split(',');

            this.usePosMan = new Vector3(float.Parse(posSplit[0], CultureInfo.InvariantCulture), float.Parse(posSplit[1], CultureInfo.InvariantCulture), float.Parse(posSplit[2], CultureInfo.InvariantCulture));
            this.useRotMan = new Vector3(float.Parse(rotSplit[0], CultureInfo.InvariantCulture), float.Parse(rotSplit[1], CultureInfo.InvariantCulture), float.Parse(rotSplit[2], CultureInfo.InvariantCulture));

            this.usePosFemale = new Vector3(float.Parse(posSplit2[0], CultureInfo.InvariantCulture), float.Parse(posSplit2[1], CultureInfo.InvariantCulture), float.Parse(posSplit2[2], CultureInfo.InvariantCulture));
            this.useRotFemale = new Vector3(float.Parse(rotSplit2[0], CultureInfo.InvariantCulture), float.Parse(rotSplit2[1], CultureInfo.InvariantCulture), float.Parse(rotSplit2[2], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Method that is ran before equipping item
        /// If returns yes, then item will be equipped
        /// </summary>
        /// <returns>Yes if equipping can continue, otherwise false</returns>
        protected virtual bool EquipItem(Character user)
        {
            return true;
        }

        /// <summary>
        /// Method that is ran before unequipping item
        /// If returns yes, then item will be unequipped
        /// </summary>
        /// <returns>Yes if unequipping can continue, otherwise false</returns>
        protected virtual bool UnEquipItem(Character user)
        {
            return true;
        }

        protected void StopAnimationAndDetachItem(Character user)
        {
            user.SetEquippedItem(null);
            user.StopAnimation();
            equipped = false;
            API.shared.deleteEntity(entity);
        }

        public bool IsEquipped()
        {
            return equipped;
        }

        public void ForceUnequip(Character user)
        {
            if (equipped)
            {
                this.StopAnimationAndDetachItem(user);
            }
        }

        public override void Use(Character user)
        {
            if (!user.isInVehicle && !user.IsUsingPhone() && user.IsAvailable())
            {
                if (!equipped)
                {
                    if (this.EquipItem(user))
                    {
                        user.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), this.animDict, this.animName);
                        entity = API.shared.createObject(entityId, user.position, new Vector3(0, 0, 0));
                        if (user.gender == 0)
                        {
                            user.AttachObject(entity, bone.ToString(), this.usePosMan, this.useRotMan);
                        }
                        else
                        {
                            user.AttachObject(entity, bone.ToString(), this.usePosFemale, this.useRotFemale);
                        }
                        equipped = true;
                        user.SetEquippedItem(this);
                    }
                }
                else
                {
                    if (this.UnEquipItem(user))
                    {
                        this.StopAnimationAndDetachItem(user);
                    }
                }
            }
        }
    }
}
