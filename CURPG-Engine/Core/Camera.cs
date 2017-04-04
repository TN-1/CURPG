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
            //Are we trying to draw outside the lower bounds of the map?
            if (X <= 0)
            {
                X = 0;
                maxX = ViewPort.Width;
            }
            if(Y <= 0)
            {
                Y = 0;
                //Y Is height dummy, Not width. CHECK YO VARIABLES FOOL!
                maxY = ViewPort.Height;
            } 
            //Are we trying to draw outside the upper bounds of the map?
            if(X >= World.Grid.GetLength(0) - ViewPort.Width)
            {
                maxX = World.Grid.GetLength(0) - 1;
                X = (World.Grid.GetLength(0) - 1)- ViewPort.Width;
            }
            if (Y >= World.Grid.GetLength(1) - ViewPort.Height)
            {
                maxY = World.Grid.GetLength(1) - 1;
                Y = (World.Grid.GetLength(1) - 1) - ViewPort.Height;
            }
            
            //Now we gotta pipe a signal to Draw() to allow the player character to move around these extreme bounds

            //Work out what to draw
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
