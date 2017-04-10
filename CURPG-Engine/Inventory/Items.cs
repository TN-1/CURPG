using System;

namespace CURPG_Engine.Inventory
{
    [Serializable]
    public class Item
    {
        public int ID;
        public string Name;
        public int Weight;
        public string EntityName;
    }

    [Serializable]
    public class Weapon : Item
    {
        //Add combat modifyers 
    }

    [Serializable]
    public class Armor : Item
    {
        //Add combat modifyers 
    }

    [Serializable]
    public class Tool : Item
    {
        public int TerrainMod;

        public Tool(int id, string name, int weight, int terrmod, string entname)
        {
            ID = id;
            Name = name;
            Weight = weight;
            TerrainMod = terrmod;
            EntityName = entname;
        }
    }
}
