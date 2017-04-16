using System;
using System.Reflection;
using GeonBit.UI.Entities;
using System.IO;
using CURPG_Engine.Scriptables;

// ReSharper disable UnusedMember.Global

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    public partial class PlayScreen
    {
        /// <summary>
        /// Clears the console
        /// </summary>
        public void Clear()
        {
            ScreenManager.Console.Clear();
        }

        /// <summary>
        /// Sets our testing flag
        /// </summary>
        public void Testing()
        {
            if (!Player.Testing)
                Player.Testing = true;
            else if (Player.Testing)
                Player.Testing = false;
        }

        /// <summary>
        /// Strings a series of strings to the story panel
        /// </summary>
        /// <param name="s">Strings[]</param>
        public void PrintStory(string[] s)
        {
            foreach (String ss in s)
            {
                Paragraph p = new Paragraph(ss);
                _bottomPanel.AddChild(p);
            }
        }

        public void PrintStory(string s)
        {
            Paragraph p = new Paragraph(s);
            _bottomPanel.AddChild(p);
        }


        /// <summary>
        /// Adds buttons to the story panel
        /// </summary>
        public void StoryButtons(Button button)
        {
            _bottomPanel.AddChild(button);
        }

        /// <summary>
        /// Clears the story panel
        /// </summary>
        public void ClearStory()
        {
            _bottomPanel.ClearChildren();
        }

        /// <summary>
        /// Creates a new button
        /// </summary>
        /// <param name="label">Label for button</param>
        /// <param name="action">Method to call onClick</param>
        /// <param name="param">(Optional) Paramters for the previous method</param>
        /// <returns>Button object</returns>
        public Button CreateButton(string label, string action, object[] param = null)
        {
            var button = new Button(label)
            {
                OnClick = btn =>
                {
                    var thisType = GetType();
                    var theMethod = thisType.GetMethod(action);
                    theMethod.Invoke(this, param);
                }
            };

            return button;
        }

        /// <summary>
        /// Run a lua script
        /// </summary>
        /// <param name="name">Name of script inc .lua in Scripts folder</param>
        /// <returns>Indicator of success</returns>
        public string RunScript(string name)
        {
            var loc = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if(loc == null) throw new Exception("Loc is null");
            if (!File.Exists(Path.Combine(loc, @"Scripts\" + name)))
                return ("File doesnt exist");
            try
            {
                _lua.DoFile(Path.Combine(loc, @"Scripts\" + name));
            }
            catch (Exception e)
            {
                return ("Failed: " + e);
            }
            return ("Success");
        }

        public void AddNpc(double index, string name, string gender, double age, double height, double weight, double x, double y, double maxx, double maxy)
        {
            _npcs.Add(new Npc((int)index, name, gender.ToCharArray()[0], (int)age, (int)height, (int)weight, (int)x, (int)y, (int)maxx, (int)maxy, World));
        }

    }
}