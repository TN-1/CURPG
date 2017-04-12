using System;
using System.Reflection;
using GeonBit.UI.Entities;
using System.IO;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    public partial class PlayScreen : GameScreen
    {
        /// <summary>
        /// Clears the console
        /// </summary>
        public void Clear()
        {
            ScreenManager.console.Clear();
        }

        /// <summary>
        /// Sets our testing flag
        /// </summary>
        public void Testing()
        {
            if (!player.Testing)
                player.Testing = true;
            else if (player.Testing)
                player.Testing = false;
        }

        /// <summary>
        /// Strings a series of strings to the story panel
        /// </summary>
        /// <param name="s">Strings[]</param>
        public void PrintStory(string[] s)
        {
            foreach (String S in s)
            {
                Paragraph p = new Paragraph(S);
                bottomPanel.AddChild(p);
            }
        }

        /// <summary>
        /// Adds buttons to the story panel
        /// </summary>
        public void StoryButtons(Button button)
        {
            bottomPanel.AddChild(button);
        }

        /// <summary>
        /// Clears the story panel
        /// </summary>
        public void ClearStory()
        {
            bottomPanel.ClearChildren();
        }

        /// <summary>
        /// Creates a new button
        /// </summary>
        /// <param name="label">Label for button</param>
        /// <param name="action">Method to call onClick</param>
        /// <param name="param">(Optional) Paramters for the previous method</param>
        /// <returns>Button object</returns>
        public Button CreateButton(string label, string action, string[] param = null)
        {
            Button button = new Button(label);

            button.OnClick = (Entity btn) => {
                Type thisType = this.GetType();
                MethodInfo theMethod = thisType.GetMethod(action);
                theMethod.Invoke(this, param);
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
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Scripts\" + name)))
                return ("File doesnt exist");
            try
            {
                lua.DoFile(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Scripts\" + name));
            }
            catch (Exception e)
            {
                return ("Failed: " + e);
            }
            return ("Success");
        }
    }
}