using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using GeonBit.UI.DataTypes;
using System.Diagnostics;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    partial class PlayScreen
    {
        [NonSerialized] private Panel _panel;
        private void Ui()
        {
            UserInterface.Initialize(ScreenManager.ContentMgr, BuiltinThemes.hd);

            //Right Panel
            _rightPanel = new Panel(new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) - 16), Convert.ToInt32(ScreenManager.ScreenArea.Height - 70)), PanelSkin.Default, Anchor.TopRight, new Vector2(0, 70));
            _tabs = new PanelTabs();
            _invTab = _tabs.AddTab("Inventory");
            _statTab = _tabs.AddTab("Skills");


            _invTab.panel.AddChild(new Header("Hello inventory!"));
            _statTab.panel.AddChild(new Header("Hello stats!"));

            if(new StackTrace().GetFrame(1).GetMethod().Name != "DrawInv")
                DrawInv();

            _rightPanel.AddChild(_tabs);
            UserInterface.AddEntity(_rightPanel);

            //Bottom Panel
            _bottomPanel = new Panel(new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) + (16 - Convert.ToInt32((ScreenManager.ScreenArea.Height * .3) - 16))), Convert.ToInt32((ScreenManager.ScreenArea.Height * .3) - 16)), PanelSkin.Default, Anchor.BottomRight, new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) - 16), 0));
            UserInterface.AddEntity(_bottomPanel);

        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void DrawInv()
        {
            //BUG: Optimise this!
            if (_rightPanel == null)
                Ui();

            var width = _rightPanel.Size.X;
            var rowSize = (float)Math.Floor(width / 65);
            _invTab.panel.ClearChildren();

            var icons = new Dictionary<string, IconType> {{"axe", IconType.Axe}, {"log", IconType.Apple}};
            for (var i = 0; i < Player.Inventory.Items.Length; i++)
            {
                var item = Player.Inventory.Items[i];
                if (item == null) continue;
                var icon = new IconI(i, icons[item.EntityName], Anchor.TopLeft, 1.2f, true, new Vector2(10, 10))
                {
                    OnClick = (Entity icn) =>
                    {
                        if(icn is IconI icnI)
                        Player.Inventory.MakeActive(icnI.Index);
                    },
                    OnMouseEnter = (Entity icn) =>
                    {
                        _panel = new Panel(new Vector2(100, 100), PanelSkin.Default, Anchor.TopRight);
                        if (!(icn is IconI icni)) return;
                        var items = Player.Inventory.Items[icni.Index];
                        if (items is CURPG_Engine.Inventory.Craftable craftable)
                            _panel.AddChild(new Paragraph(craftable.StackHeight.ToString()));
                        UserInterface.AddEntity(_panel);
                    },
                    OnMouseLeave = (Entity icn) =>
                    {
                        UserInterface.RemoveEntity(_panel);
                    }
                };
                //TODO: Finish off this.
                if(i == 0)
                    icon.SetPosition(Anchor.TopCenter, new Vector2(0));
                else if (i - 1 < rowSize)
                    icon.SetPosition(Anchor.TopLeft, new Vector2(65 * (i - 1), 65));
                else if (i - (1 + rowSize) < rowSize)
                    icon.SetPosition(Anchor.TopLeft, new Vector2(65 * (i - (1 + rowSize)), 65 * 2));

                _invTab.panel.AddChild(icon);
            }
        }
    }

    class IconI : Icon
    {
        public int Index;

        public IconI(int index, IconType icon, Anchor anchor = Anchor.Auto, float scale = 1, bool background = false, Vector2? offset = default(Vector2?)) : base(icon, anchor, scale, background, offset)
        {
            Index = index;
            Scale = scale;
            DrawBackground = background;
            Texture = Resources.IconTextures[(int)icon];

            // set default background color
            SetStyleProperty("BackgroundColor", new StyleProperty(Color.White));

            // if have background, add default space-after
            if (background)
            {
                SpaceAfter = Vector2.One * BackgroundSize;
            }

            // update default style
            UpdateStyle(DefaultStyle);

        }
    }
}
