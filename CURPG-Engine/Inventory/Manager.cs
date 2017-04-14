using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
// ReSharper disable UnusedMember.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local

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
        public List<Item> ItemDb;

        public Inventory(int capacity)
        {
            Items = new Item[capacity];
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Finds the index of the first available slot
        /// </summary>
        /// <returns>Index of first avail slot</returns>
        private int FirstAvailSlot(Item[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Clears the inventory. USE WITH CAUTION, THIS IS IRREVESIBLE
        /// </summary>
        public void Clear()
        {
            if (Items == null) return;
            for (var i = 0; i < Items.Length; i++)
                Items[i] = null;
            OnPropertyChanged("Items");
        }

        /// <summary>
        /// Adds item to the inventory
        /// </summary>
        /// <param name="item">Item class of item to add</param>
        public void AddItem(Item item)
        {
            Items[FirstAvailSlot(Items)] = item;
            OnPropertyChanged("Items");
        }

        /// <summary>
        /// Adds item to the inventory
        /// </summary>
        /// <param name="i">Item ID to add to inventory</param>
        public void AddItem(int i)
        {
            var j = FirstAvailSlot(Items);
            foreach (var item in ItemDb)
            {
                if (item.Id == i)
                    Items[j] = item;
            }
            OnPropertyChanged("Items");
        }

        /// <summary>
        /// Builds our Items database from the XML decleration file
        /// </summary>
        /// <param name="path">Path to items.xml</param>
        public void BuildDatabase(string path)
        {
            var items = new XmlDocument();
            var itemDb = new List<Item>();
            items.Load(path);

            //Go throught the XML file iterating through each of the nodes to make our items, Add them to a temp list
            //Then return the list to populate our actual itemDB.

            Debug.Assert(items.DocumentElement != null, "items.DocumentElement != null");
            var nodes = items.DocumentElement.SelectNodes("/items/tools/item");
            Debug.Assert(nodes != null, "nodes != null");
            foreach (XmlNode node in nodes)
            {
                Debug.Assert(node.Attributes != null, "node.Attributes != null");
                var id = Convert.ToInt32(node.Attributes.GetNamedItem("id").InnerText);
                var nameNode = node.SelectSingleNode("Name");
                if (nameNode == null) continue;
                var name = nameNode.InnerText;
                var weightNode = node.SelectSingleNode("Weight");
                if (weightNode == null) continue;
                var weight = Convert.ToInt32(weightNode.InnerText);
                var entName = node.SelectSingleNode("EntityName");
                if (entName == null) continue;
                var entname = entName.InnerText;
                var terrMod = node.SelectSingleNode("TerrainMod");
                if (terrMod == null) continue;
                var terrmod = Convert.ToInt32(terrMod.InnerText);
                var tool = new Tool(id, name, weight, terrmod, entname);
                itemDb.Add(tool);
            }

            nodes = items.DocumentElement.SelectNodes("/items/craftable/item");
            Debug.Assert(nodes != null, "nodes != null");
            foreach (XmlNode node in nodes)
            {
                Debug.Assert(node.Attributes != null, "node.Attributes != null");
                var id = Convert.ToInt32(node.Attributes.GetNamedItem("id").InnerText);
                var nameNode = node.SelectSingleNode("Name");
                if (nameNode == null) continue;
                var name = nameNode.InnerText;
                var weightNode = node.SelectSingleNode("Weight");
                if (weightNode == null) continue;
                var weight = Convert.ToInt32(weightNode.InnerText);
                var entNode = node.SelectSingleNode("EntityName");
                if (entNode == null) continue;
                var entname = entNode.InnerText;
                var stackNode = node.SelectSingleNode("MaxStackHeight");
                if (stackNode == null) continue;
                var maxStack = Convert.ToInt32(stackNode.InnerText);
                var craftable = new Craftable(id, name, entname, weight, maxStack);
                itemDb.Add(craftable);
            }

            ItemDb = itemDb;
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
                    if (args == null) throw new Exception("Args cant be null");
                    var tool = new Tool(id, name, weight, Convert.ToInt32(args[0]), args[1]);
                    return tool;
            }

            return null;
        }

        public bool Contains(int id)
        {
            return Items.Select(item => item.Id == id).FirstOrDefault();
        }

        public void MakeActive(int index)
        {
            Item iitem = Items[0];
            Item item = Items[index];
            Items[index] = iitem;
            Item[] temp = new Item[Items.Length];
            temp[0] = item;
            for (var i = 1; i <= (Items.Length - 1); i++)
            {
                if (Items[i] == null) continue;
                temp[FirstAvailSlot(temp)] = Items[i];
            }
            Items = temp;
            OnPropertyChanged("Items");
        }
    }
}
