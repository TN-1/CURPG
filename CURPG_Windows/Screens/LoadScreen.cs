using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using CURPG_Engine.Core;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Panel = GeonBit.UI.Entities.Panel;
using ProgressBar = GeonBit.UI.Entities.ProgressBar;

namespace CURPG_Windows.Screens
{
    internal class LoadScreen : GameScreen
    {
        private readonly string _exeLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private Task<World> _result;
        private Panel _panel;
        private ProgressBar _prog;

        public override void Initialize()
        {
            UserInterface.Initialize(ScreenManager.ContentMgr, BuiltinThemes.hd);
            UserInterface.ShowCursor = false;

            var tilesPath = Path.Combine(_exeLocation, @"DataFiles\Tiles.xml");
            var tileSet = WorldTools.TileSetBuilder(tilesPath);

            _panel = new Panel(new Vector2(ScreenManager.ScreenArea.Width, ScreenManager.ScreenArea.Height));
            UserInterface.AddEntity(_panel);

            // add title and text
            _panel.AddChild(new Header("Loading..."));
            _panel.AddChild(new HorizontalLine());

            _prog = new ProgressBar
            {
                Min = 0,
                Max = 500000,
                Draggable = false,
                StepsCount = 500000
            };
            _panel.AddChild(_prog);

            var progressHandler = new Progress<int>(value =>
            {
                if(value > 250000)
                    _prog.Value = value;
            });
            var progress = (IProgress<int>) progressHandler;

            _result = Task.Run(() =>
            {
                var world = WorldTools.GenerateWorld(0, 500, 500, tileSet, "World", 24, tilesPath, progress);
                return world;
            });

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (_result.IsCompleted)
                if (_result.Result != null)
                {
                    ScreenManager.WorldTrans = _result.Result;
                    ScreenManager.ChangeScreens("Load", "Play");
                }

            //TODO: Unhack this and actually fix it :)
            if (_prog.Value < 250000)
                _prog.Value = _prog.Value + 500;

            UserInterface.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDeviceMgr.GraphicsDevice.Clear(Color.CornflowerBlue);

            UserInterface.Draw(ScreenManager.Sprites);

            base.Draw(gameTime);

        }
    }
}
