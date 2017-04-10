using System;
using System.Collections.Generic;
using System.Xml;

namespace CURPG_Engine.Inventory
{
    /// <summary>
    /// This is where the magic happens. We all need the shiney!
    /// All inventory related methods live in here to keep them centered
    /// </summary>
    [Serializable]
    public class Inventory
    {
        public Item[] Items;
        public List<Item> ItemDB;
        public int Capacity;
        public Inventory(int capacity)
        {
            Capacity = capacity;
            Items = new Item[Capacity];
        }

        /// <summary>
        /// Finds the index of the first available slot
        /// </summary>
        /// <returns>Index of first avail slot</returns>
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

        /// <summary>
        /// Clears the inventory. USE WITH CAUTION, THIS IS IRREVESIBLE
        /// </summary>
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

        /// <summary>
        /// Adds item to the inventory
        /// </summary>
        /// <param name="item">Item class of item to add</param>
        public void AddItem(Item item)
        {
            Items[FirstAvailSlot()] = item;
        }

        /// <summary>
        /// Adds item to the inventory
        /// </summary>
        /// <param name="i">Item ID to add to inventory</param>
        public void AddItem(int i)
        {
            foreach(Item item in ItemDB)
                if(item.ID == i)
                    Items[FirstAvailSlot()] = item;
        }

        /// <summary>
        /// Builds our Items database from the XML decleration file
        /// </summary>
        /// <param name="path">Path to items.xml</param>
        public void BuildDatabase(string path)
        {
            XmlDocument items = new XmlDocument();
            List<Item> itemDB = new List<Item>();
            XmlNodeList nodes;
            items.Load(path);

            //Go throught the XML file iterating through each of the nodes to make our items, Add them to a temp list
            //Then return the list to populate our actual itemDB.

            nodes = items.DocumentElement.SelectNodes("/items/tools/item");
            foreach (XmlNode node in nodes)
            {
                var id = Convert.ToInt32(node.Attributes.GetNamedItem("id").InnerText);
                var name = node.SelectSingleNode("Name").InnerText;
                var weight = Convert.ToInt32(node.SelectSingleNode("Weight").InnerText);
                var entname = node.SelectSingleNode("EntityName").InnerText;
                var terrmod = Convert.ToInt32(node.SelectSingleNode("TerrainMod").InnerText);
                Tool tool = new Tool(id, name, weight, terrmod, entname);
                ItemDB.Add(tool);
            }
            nodes = null; //Gotta null this shit for some sweet GC action BB!

            nodes = items.DocumentElement.SelectNodes("/items/craftable/item");
            foreach (XmlNode node in nodes)
            {
                var id = Convert.ToInt32(node.Attributes.GetNamedItem("id").InnerText);
                var name = node.SelectSingleNode("Name").InnerText;
                var weight = Convert.ToInt32(node.SelectSingleNode("Weight").InnerText);
                var entname = node.SelectSingleNode("EntityName").InnerText;
                var maxStack = Convert.ToInt32(node.SelectSingleNode("MaxStackHeight").InnerText);
                Craftable craftable = new Craftable(id, name, entname, weight, maxStack);
                ItemDB.Add(craftable);
            }
            nodes = null;

            ItemDB = itemDB;
        }

        /// <summary>
        /// Lists the contents of the inventory by name
        /// </summary>
        /// <returns>String of item names</returns>
        public string ListInventory()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach(Item item in Items)
            {
                if(item != null)
                    sb.Append(", " + item.Name);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <param name="s">Type of item</param>
        /// <param name="id">Item ID</param>
        /// <param name="name">Item name</param>
        /// <param name="weight">Item weight</param>
        /// <param name="args">Optional parameters as needed by classes</param>
        /// <returns></returns>
        public Item NewItem(string s, int id, string name, int weight, string[] args = null)
        {
            switch (s)
            {
                case "tool":
                    Tool tool = new Tool(id, name, weight, Convert.ToInt32(args[0]), args[1]);
                    return tool;
            }

            return null;
        }

        public bool Contains(int id)
        {
            foreach(Item item in Items)
                if (item.ID == id)
                    return true;
                else
                    return false;
            return false;
        }

    }
}
