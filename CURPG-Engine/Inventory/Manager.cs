﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace CURPG_Engine.Inventory
{
    [Serializable]
    public class Inventory
    {
        public Item[] Items;
        List<Item> ItemDB;
        public int Capacity;
        public Inventory(int capacity)
        {
            Capacity = capacity;
            Items = new Item[Capacity];
        }

        int FirstAvailSlot()
        {
            if (Items != null)
            {
                for (int i = 0; i < Capacity; i++)
                {
                    if (Items[i] == null)
                        return i;
                }
            }
            return -1;
        }

        public void Clear()
        {
            if (Items != null)
            {
                for (int i = 0; i < Capacity; i++)
                {
                    if (Items[i] != null)
                        Items[i] = null;
                }
            }
        }

        public void AddItem(Item item)
        {
            Items[FirstAvailSlot()] = item;
        }

        public void BuildDatabase(string path)
        {
            XmlDocument items = new XmlDocument();
            List<Item> itemDB = new List<Item>();
            items.Load(path);
            XmlNodeList nodes = items.DocumentElement.SelectNodes("/items/item");

            foreach (XmlNode node in nodes)
            {
            }
            ItemDB = itemDB;
        }

        public string ListInventory()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach(Item item in Items)
            {
                if(item != null)
                    sb.Append(item.Name + ", ");
            }

            return sb.ToString();
        }
    }
}
