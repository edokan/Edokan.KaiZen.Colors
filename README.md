Edokan.KaiZen.Colors
====================

A simple rewrite of colors module of node in C#

Includes a dummy escape sequence parser for interpreting ANSI escape codes (colors only)

Here is a basic example

````csharp
    using Edokan.Kaizen.Colors;

    namespace MyAwesomeConsoleApplication
    {
      public class Program
      {
        static void Main()
        {
          EscapeSequencer.Install(); // Install sequence parse
          EscapeSequencer.Bold = true; // I like bright colors
    
          Console.WriteLine("INFO: ".Yellow() + "Hello".Red() + ", " + "World.".Blue());
          Console.WriteLine("This is zebra".Zebra());
          Console.WriteLine("This is rainbow".Rainbow());
          Console.ReadLine();
        }
      }
    }
````
