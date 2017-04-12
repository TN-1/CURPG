using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    class MenuScreen : GameScreen
    {
        private KeyboardState oldState;

        public override void Initialize()
        {
            BackgroundColor = Color.CornflowerBlue;
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();  // get the newest state

            if (oldState.IsKeyUp(Keys.OemTilde) && newState.IsKeyDown(Keys.OemTilde))
                ScreenManager.console.ToggleOpenClose();

            base.Update(gameTime);
        }
    }
}
