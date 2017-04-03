namespace CURPG_Engine.Core
{
    public class Camera
    {
        int X;
        int Y;
        System.Drawing.Rectangle ViewPort;
        int MaxX;
        int MaxY;
        World World;
        Player Player;

        public Camera(int x, int y, System.Drawing.Rectangle viewPort, World world, Player player)
        {
            X = x;
            Y = y;
            ViewPort = viewPort;
            World = world;
            Player = player;
            MaxX = World.Grid.GetLength(0) - ViewPort.Width;
            MaxY = World.Grid.GetLength(1) - ViewPort.Height;
        }

        public World GetDrawArea()
        {
            World DrawArea = new World(1, ViewPort.Width + 1, ViewPort.Height + 1, World.TileSet, "DrawArea", World.TileSize);
            X = Player.locationX - (ViewPort.Width / 2);
            Y = Player.locationY - (ViewPort.Height / 2);
            var maxX = Player.locationX + (ViewPort.Width / 2);
            var maxY = Player.locationY + (ViewPort.Height / 2);

            for (int x = X; x <= maxX; x++)
            {
                for(int y = Y; y <= maxY; y++)
                {
                    DrawArea.Grid[x - X, y - Y] = World.Grid[x, y];
                }
            }
            return DrawArea;
        }
        private bool NeedUpdate()
        {
            bool NeedUpdate = false;
            return NeedUpdate;
        }
    }
}
