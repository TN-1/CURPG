using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CURPG_Engine.Core
{
    public class Persistance
    {
        static public int SaveGame(World world, Player player)
        {
            var exepath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var path = Path.Combine(exepath, "Saves", world.Index.ToString());
            Directory.CreateDirectory(path);

            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Path.Combine(path, "World.bin"), FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, world);
                stream.Close();

                stream = new FileStream(Path.Combine(path, "Player.bin"), FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, player);
                stream.Close();

                return 1;
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Save failed");
                System.Diagnostics.Debug.WriteLine("Exception: " + e);
                return 0;
            }
        }

        static public bool CanLoad()
        {
            var exepath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var path = Path.Combine(exepath, "Saves");
            if (Directory.Exists(path))
                return true;
            else
                return false;
        }

        static public World LoadWorld()
        {
            var exepath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var path = Path.Combine(exepath, "Saves", "0".ToString());
            World load;

            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Path.Combine(path, "World.bin"), FileMode.Open, FileAccess.Read, FileShare.Read);
                load = (World)formatter.Deserialize(stream);
                return load;
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("World Load failed");
                System.Diagnostics.Debug.WriteLine("Exception: " + e);
                return null;
            }
        }

        static public Player LoadPlayer()
        {
            var exepath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var path = Path.Combine(exepath, "Saves", "0".ToString());
            Player load;

            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Path.Combine(path, "Player.bin"), FileMode.Open, FileAccess.Read, FileShare.Read);
                load = (Player)formatter.Deserialize(stream);
                return load;
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Player Load failed");
                System.Diagnostics.Debug.WriteLine("Exception: " + e);
                return null;
            }
        }
    }
}
