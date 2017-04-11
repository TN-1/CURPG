using System;
using System.Runtime.InteropServices;
using System.Text;
using WindowsInput;
using System.Threading;
using System.Diagnostics;

namespace CURPG_TestBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Begin:");
            InputSimulator input = new InputSimulator();
            Random r = new Random();
            Process proc = new Process();

            Console.WriteLine("Starting CURPG");
            proc.StartInfo.FileName = @"H:\visual studio 2015\CURPG-Engine\CURPG_Graphical_MonoGame_Windows\bin\Windows\x86\Debug\CURPG_Graphical_MonoGame_Windows.exe";
            proc.Start();
            Thread.Sleep(5000);

            if(GetActiveWindowTitle() == "CURPG")
            {
                Console.WriteLine("CURPG Active. Begining test...");
                for (int i = 0; i <= 50; i++)
                {
                    switch(r.Next(0, 3))
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
                    }
                    Thread.Sleep(200);
                }
                proc.Kill();
                TimeSpan duration = proc.StartTime - proc.ExitTime;

                Console.WriteLine("Test end.");
                Console.WriteLine("Execution time(Seconds): " + duration.TotalSeconds);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        static private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

    }
}
