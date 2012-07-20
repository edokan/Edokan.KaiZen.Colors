using System;
using System.Linq;

namespace Edokan.KaiZen.Colors.Demo
{
    public static class Program
    {
        static void Main(string[] args)
        {
            EscapeSequencer.Install();
            EscapeSequencer.Bold = true; //set default bold to On. Actually this will use bright colors. Easier to read

            // simple test
            Console.WriteLine("INFO: ".Yellow() + "Hello".Red() + ", " + "World.".Blue());

            Console.WriteLine("This is rainbow".Rainbow()); // all we need is unicorn :)
            Console.WriteLine("This is zebra".Zebra());     // no worries. A zebra will do

            const string RandomColorLine = "This line will be randomly colored";

            for (int n = 0; n < 5; n++)
            {
                Console.WriteLine(RandomColorLine.CustomSequencer());
            }

            //Console.WriteLine("What color is this?".Reset());

            Console.ReadLine();
        }

        /// <summary>
        /// A simple color sequencer
        /// </summary>
        /// <param name="line">string to be colorized</param>
        /// <returns>Randomly colored string</returns>
        public static string CustomSequencer(this string line)
        {
            var colors = ColorsExtension.DefaultTheme.Keys.ToArray();
            var rnd = new Random(DateTime.Now.Millisecond);

            return ColorsExtension.RunSequencer(
                line,
                (s, i, c) => c.ToString()
                                 .Color(colors[rnd.Next(colors.Length)])
                                 .On()
                                 .Color(colors[rnd.Next(colors.Length)])
                );
        }
    }
}
