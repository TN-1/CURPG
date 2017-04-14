using System;
using Microsoft.Xna.Framework;
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    [Serializable]
    public class GameScreen
    {
        [NonSerialized] public static bool IsActive = true;
        [NonSerialized] public static bool IsPopup = false;
        [NonSerialized] public Color BackgroundColor = Color.White;

        public virtual void Initialize() { }
        public virtual void LoadAssets() { }
        public virtual void Update(GameTime gameTime) { }
        // ReSharper disable once UnusedParameter.Global
        public virtual void Draw(GameTime gameTime) { }
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        public virtual void UnloadAssets() { }
    }
}
