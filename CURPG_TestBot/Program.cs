using System;
using System.Runtime.InteropServices;
using System.Text;
using WindowsInput;
using System.Threading;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace CURPG_TestBot
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Begin:");
            var input = new InputSimulator();
            var r = new Random();
            var proc = new Process();

            Console.WriteLine("Starting CURPG");
            proc.StartInfo.FileName = @"H:\visual studio 2015\CURPG-Engine\CURPG_Graphical_MonoGame_Windows\bin\Windows\x86\Debug\CURPG_Graphical_MonoGame_Windows.exe";
            proc.Start();
            Thread.Sleep(5000);

            if (GetActiveWindowTitle() != "CURPG") return;
            Console.WriteLine("CURPG Active. Begining test...");
            for (var i = 0; i <= 50; i++)
            {
                switch (r.Next(0, 3))
                {
                    case 0:
                        input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.UP);
                        Thread.Sleep(200);
                        input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.UP);
                        Console.WriteLine("UP");
                        break;
                    case 1:
                        input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.DOWN);
                        Thread.Sleep(200);
                        input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.DOWN);
                        Console.WriteLine("DOWN");
                        break;
                    case 2:
                        input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LEFT);
                        Thread.Sleep(200);
                        input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LEFT);
                        Console.WriteLine("LEFT");
                        break;
                    case 3:
                        input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.RIGHT);
                        Thread.Sleep(200);
                        input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.RIGHT);
                        Console.WriteLine("RIGHT");
                        break;
                    // ReSharper disable once RedundantEmptySwitchSection
                    default:
                        break;
                }
                Thread.Sleep(200);
            }
            proc.Kill();
            var duration = proc.StartTime - proc.ExitTime;

            Console.WriteLine("Test end.");
            Console.WriteLine("Execution time(Seconds): " + duration.TotalSeconds);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, buff, nChars) > 0)
            {
                return buff.ToString();
            }
            return null;
        }

    }
}
