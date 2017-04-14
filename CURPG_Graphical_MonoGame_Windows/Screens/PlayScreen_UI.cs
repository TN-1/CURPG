using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using System;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    partial class PlayScreen
    {
        private void Ui()
        {
            UserInterface.Initialize(ScreenManager.ContentMgr, BuiltinThemes.hd);

            //Right Panel
            _rightPanel = new Panel(new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) - 16), Convert.ToInt32(ScreenManager.ScreenArea.Height - 70)), PanelSkin.Default, Anchor.TopRight, offset: new Vector2(0, 70));
            var tabs = new PanelTabs();
            var invTab = tabs.AddTab("Inventory");
            var statTab = tabs.AddTab("Skills");


            invTab.panel.AddChild(new Header("Hello inventory!"));
            var icon = new Icon(IconType.Sword, anchor: Anchor.TopLeft, scale: 1.2f, background: true, offset: new Vector2(10, 10));
            invTab.panel.AddChild(icon);

            statTab.panel.AddChild(new Header("Hello stats!"));


            _rightPanel.AddChild(tabs);
            UserInterface.AddEntity(_rightPanel);

            //Bottom Panel
            _bottomPanel = new Panel(new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) + (16 - Convert.ToInt32((ScreenManager.ScreenArea.Height * .3) - 16))), Convert.ToInt32((ScreenManager.ScreenArea.Height * .3) - 16)), PanelSkin.Default, Anchor.BottomRight, new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) - 16), 0));
            UserInterface.AddEntity(_bottomPanel);

        }
    }
}
