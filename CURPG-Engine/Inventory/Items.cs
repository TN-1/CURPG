namespace CURPG_Engine.Inventory
{
    public class Item
    {
        public int ID;
        public string Name;
        public int Weight;
    }

    public class Weapon : Item
    {
        //Add combat modifyers 
    }

    public class Armor : Item
    {
        //Add combat modifyers 
    }

    public class Tool : Item
    {
        public int TerrainMod;
    }
}
