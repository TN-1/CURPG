using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CURPG_Windows.Screens
{
    internal class MenuScreen : GameScreen
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

            _oldState = newState;

            base.Update(gameTime);
        }
    }
}
