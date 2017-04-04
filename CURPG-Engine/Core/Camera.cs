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
        System.Drawing.Point PlayerCoord;

        public System.Drawing.Point playerCoord { get { return PlayerCoord; } }

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
            bool ExtremeBound = false;
            bool XL = false;
            bool YL = false;
            bool XH = false;
            bool YH = false;
            X = Player.locationX - (ViewPort.Width / 2);
            Y = Player.locationY - (ViewPort.Height / 2);
            var maxX = Player.locationX + (ViewPort.Width / 2);
            var maxY = Player.locationY + (ViewPort.Height / 2);
            //Are we trying to draw outside the lower bounds of the map?
            if (X <= 0)
            {
                X = 0;
                maxX = ViewPort.Width;
                ExtremeBound = true;
                XL = true;
            }
            if(Y <= 0)
            {
                Y = 0;
                //Y Is height dummy, Not width. CHECK YO VARIABLES FOOL!
                maxY = ViewPort.Height;
                ExtremeBound = true;
                YL = true;
            } 
            //Are we trying to draw outside the upper bounds of the map?
            if(X >= World.Grid.GetLength(0) - ViewPort.Width)
            {
                maxX = World.Grid.GetLength(0) - 1;
                X = (World.Grid.GetLength(0) - 1)- ViewPort.Width;
                ExtremeBound = true;
                XH = true;
            }
            if (Y >= World.Grid.GetLength(1) - ViewPort.Height)
            {
                maxY = World.Grid.GetLength(1) - 1;
                Y = (World.Grid.GetLength(1) - 1) - ViewPort.Height;
                ExtremeBound = true;
                YH = true;
            }

            //Figure out where to draw the player relative to the viewport.
            if(!ExtremeBound)
            {
                PlayerCoord.X = ViewPort.Width / 2;
                PlayerCoord.Y = ViewPort.Height / 2;
            }
            else if(ExtremeBound)
            {
                if(XL == true)
                {
                    PlayerCoord.X = Player.locationX;
                    PlayerCoord.Y = ViewPort.Height / 2;
                }
                if(YL == true)
                {
                    PlayerCoord.X = ViewPort.Width / 2;
                    PlayerCoord.Y = Player.locationY;
                }
                if (XH == true)
                {
                    PlayerCoord.X = Player.locationX - (World.Grid.GetLength(0) - ViewPort.Width);
                    PlayerCoord.Y = ViewPort.Height / 2;
                }
                if (YH == true)
                {
                    PlayerCoord.X = ViewPort.Width / 2;
                    PlayerCoord.Y = Player.locationY - (World.Grid.GetLength(1) - ViewPort.Height);
                }
                if (XL == true && YL == true)
                {
                    //Top Left
                    PlayerCoord.X = Player.locationX;
                    PlayerCoord.Y = Player.locationY;

                }
                if (XL == true && YH == true)
                {
                    //Bottom Left
                    PlayerCoord.X = Player.locationX;
                    PlayerCoord.Y = Player.locationY - (World.Grid.GetLength(1) - ViewPort.Height);
                }
                if (XH == true && YL == true)
                {
                    //Top Right
                    PlayerCoord.X = Player.locationX - (World.Grid.GetLength(0) - ViewPort.Width);
                    PlayerCoord.Y = Player.locationY;

                }
                if (XH == true && YH == true)
                {
                    //Bottom Right
                    PlayerCoord.X = Player.locationX - (World.Grid.GetLength(0) - ViewPort.Width);
                    PlayerCoord.Y = Player.locationY - (World.Grid.GetLength(1) - ViewPort.Height);
                }

            }

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
    }
}
