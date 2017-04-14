using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    class MenuScreen : GameScreen
    {
        private KeyboardState _oldState;

        public override void Initialize()
        {
            BackgroundColor = Color.CornflowerBlue;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var newState = Keyboard.GetState();  // get the newest state

            if (_oldState.IsKeyUp(Keys.OemTilde) && newState.IsKeyDown(Keys.OemTilde))
                ScreenManager.Console.ToggleOpenClose();

            _oldState = newState;

            base.Update(gameTime);
        }
    }
}
