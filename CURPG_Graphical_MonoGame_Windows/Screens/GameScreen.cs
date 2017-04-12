using Microsoft.Xna.Framework;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    public class GameScreen
    {
        public bool IsActive = true;
        public bool IsPopup = false;
        public Color BackgroundColor = Color.White;

        public virtual void Initialize() { }
        public virtual void LoadAssets() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void UnloadAssets() { }
    }
}
