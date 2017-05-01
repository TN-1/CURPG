using System;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global

namespace CURPG_Engine.Inventory
{
    /// <summary>
    /// Defines our Item classes
    /// Items are divide into seperate classes for ease of use
    /// </summary>
    [Serializable]
    public class Item
    {
        public int Id;
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
        public readonly int TerrainMod;

        public Tool(int id, string name, int weight, int terrmod, string entname)
        {
            Id = id;
            Name = name;
            Weight = weight;
            TerrainMod = terrmod;
            EntityName = entname;
        }
    }

    [Serializable]
    public class Craftable : Item
    {
        public int StackHeight;
        private readonly int _maxStackHeight;

        public Craftable(int id, string name, string entname, int weight, int maxStack, int quantity = 0)
        {
            Id = id;
            Name = name;
            EntityName = entname;
            Weight = weight;
            _maxStackHeight = maxStack;
            if (quantity != 0)
                StackHeight = quantity;
        }

        public bool AddQuantity(int quantity)
        {
            if (StackHeight + quantity > _maxStackHeight)
            {
                return false;
            }
            else
            {
                StackHeight = StackHeight + quantity;
                return true;
            }
        }

        public int HowManyMore()
        {
            return _maxStackHeight - StackHeight;
        }
    }
}
