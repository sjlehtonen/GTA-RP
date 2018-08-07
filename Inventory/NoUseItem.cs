
namespace GTA_RP.Items
{
    /// <summary>
    /// Class that represents an item that has no use effect. Like some trash for example.
    /// </summary>
    class NoUseItem : Item
    {
        public NoUseItem(int id, string name, string description, int amount) : base(id, name, description, amount) { }
    }
}
