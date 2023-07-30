using System;
using System.Diagnostics;

namespace BoomLang
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input BOOM file: ");

            string boom = Console.ReadLine();
            Console.WriteLine("Input Output EXE location: ");

            string outputEXE = Console.ReadLine();
            AssemblyConverter.Clear();
            string assemblyOutput = AssemblyConverter.ConvertAssembly(boom);
            Process.Start("notepad.exe", assemblyOutput);

            string objLocation = Path.GetTempPath() + Guid.NewGuid().ToString() + ".obj";
            Process.Start("nasm.exe", $"-fwin32 {assemblyOutput} -o {objLocation}");
            Process.Start("gcc.exe", $"{objLocation} -o {outputEXE}");

            // shitty drawProgressBar(10, 100, "Done, you can now close this window.");
        }

        // copy pasta :)
        static void drawProgressBar(int max, int millisecs, string finishMsg)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(1, 1);

            for (int i = 0; i < max; i++)
            {
                for (int y = 0; y < i; y++)
                {
                    string bar = "#";
                    Console.WriteLine(bar);
                }
                Console.Write(i + " / " + max);
                Console.SetCursorPosition(1, 1);
                System.Threading.Thread.Sleep(millisecs);
            }

            Console.WriteLine(finishMsg);



            Console.ReadLine();
        }
    }
}