﻿using System;

namespace CURPG_Engine.Inventory
{
    [Serializable]
    public class Item
    {
        public int ID;
        public string Name;
        public int Weight;
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
    }
}
