
namespace GTA_RP.Items
{
    /// <summary>
    /// Class for representing an item template.
    /// </summary>
    class ItemTemplate
    {
        public int id { get; private set; }
        public ItemType type { get; private set; }
        public string name { get; private set; }

        public string description { get; private set; }
        public int field1 { get; private set; }
        public int field2 { get; private set; }
        public int field3 { get; private set; }

        public string field4 { get; private set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemTemplate))
            {
                return false;
            }

            var item = (ItemTemplate)obj;
            return id == item.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public ItemTemplate(int id, int type, string name, string description, int field1, int field2, int field3, string field4)
        {
            this.description = description;
            this.id = id;
            this.type = (ItemType)type;
            this.name = name;
            this.field1 = field1;
            this.field2 = field2;
            this.field3 = field3;
            this.field4 = field4;
        }
    }
}
