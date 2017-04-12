using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CURPG_Engine.Core;
using System;
using System.IO;
using System.Reflection;
using GeonBit.UI;
using GeonBit.UI.Entities;

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
            foreach (string S in s)
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

    }
}