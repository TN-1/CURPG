using System;

namespace CURPG_Engine.Inventory
{
    /// <summary>
    /// Defines our Item classes
    /// Items are divide into seperate classes for ease of use
    /// </summary>
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

    public class Craftable : Item
    {
        public int StackHeight;
        public int MaxStackHeight;

        public Craftable(int id, string name, string entname, int weight, int maxStack, int quantity = 0)
        {
            ID = id;
            Name = name;
            EntityName = entname;
            Weight = weight;
            MaxStackHeight = maxStack;
            if (quantity != 0)
                StackHeight = quantity;
        }

        public void AddQuantity(int quantity)
        {
            if ((StackHeight + quantity) > MaxStackHeight)
            {
                //Start another stack
            }
            else
            {
                StackHeight = +quantity;
            }
        }

        public int HowManyMore()
        {
            return MaxStackHeight - StackHeight;
        }
    }
}
