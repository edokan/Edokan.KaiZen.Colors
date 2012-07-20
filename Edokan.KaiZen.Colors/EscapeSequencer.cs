using System;
using System.IO;
using System.Text;

namespace Edokan.KaiZen.Colors
{
    /// <summary>
    /// Dummy escape sequence parser.
    /// Can only parse color codes. Anything other than color will probably will 
    /// Must be able to parse other stuff.
    /// 
    /// </summary>
    public class EscapeSequencer : TextWriter
    {
        private static EscapeSequencer Instance;

        private readonly TextWriter textWriter;
        private enum States
        {
            Text,
            Signaled,
            Started
        }

        private readonly ConsoleColor defaultForegroundColor;
        private readonly ConsoleColor defaultBackgroundColor;
        private EscapeSequencer(TextWriter textWriter)
        {
            Instance = this;
            this.textWriter = textWriter;
            defaultForegroundColor = Console.ForegroundColor;
            defaultBackgroundColor = Console.BackgroundColor;
        }

        private States state = States.Text;
        private string escapeBuffer;
        private byte intense;
        private const char ESC = '\x1b';

        public override void Write(char value)
        {
            switch (state)
            {
                case States.Text:
                    if (value == ESC)
                    {
                        state = States.Signaled;
                        escapeBuffer = "";
                    }
                    else
                        textWriter.Write(value);
                    break;
                case States.Signaled:
                    if (value != '[')
                    {
                        textWriter.Write(ESC);
                        textWriter.Write(value);
                        state = States.Text;
                    }
                    else
                    {
                        state = States.Started;
                    }
                    break;
                case States.Started:
                    if (value != 'm')
                        escapeBuffer += value;
                    else
                    {
                        byte val;
                        if (byte.TryParse(escapeBuffer, out val))
                        {
                            if (val >= 30 && val <= 37)
                                SetForeColor(val);
                            else if (val == 39)
                                SetDefaultForeColor();
                            else if (val == 1)
                                SetBold();
                            else if (val == 22)
                                RemoveBold();
                            else if (val == 7 || val == 27)
                                SetInverse();
                            else if (val >= 40 && val <= 47)
                                SetBackColor(val);
                            else if (val == 49)
                                SetDefaultBackColor();
                        }
                        state = States.Text;
                    }
                    break;
            }

        }

        private bool isInverted;
        private void SetInverse()
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = c;
            isInverted = !isInverted;
        }

        private void RemoveBold()
        {
            intense--;
        }

        private void SetBold()
        {
            intense++;
        }

        public static bool Bold
        {
            get
            {
                return (Instance.intense > 0);
            }
            set
            {
                if (value)
                    Instance.SetBold();
                else
                    Instance.RemoveBold();
            }
        }

        private void SetDefaultBackColor()
        {
            if (isInverted)
                Console.BackgroundColor = defaultBackgroundColor;
            else
                Console.ForegroundColor = defaultBackgroundColor;
        }

        private void SetDefaultForeColor()
        {
            if (isInverted)
                Console.BackgroundColor = defaultForegroundColor;
            else
                Console.ForegroundColor = defaultForegroundColor;
        }

        private readonly ConsoleColor[] ColorMap = {
            ConsoleColor.Black,
            ConsoleColor.DarkRed, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.DarkYellow, 
            ConsoleColor.DarkBlue, 
            ConsoleColor.DarkMagenta, 
            ConsoleColor.DarkCyan, 
            ConsoleColor.Gray,

            ConsoleColor.Black,
            ConsoleColor.Red,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Blue,
            ConsoleColor.Magenta,
            ConsoleColor.Cyan,
            ConsoleColor.White
        };

        private void SetBackColor(byte val)
        {
            if (isInverted)
                Console.ForegroundColor = ColorMap[val - 40 + (intense > 0 ? 8 : 0)];
            else
                Console.BackgroundColor = ColorMap[val - 40];
        }

        private void SetForeColor(byte val)
        {
            if (isInverted)
                Console.BackgroundColor = ColorMap[val - 30];
            else
                Console.ForegroundColor = ColorMap[val - 30 + (intense > 0 ? 8 : 0)];
        }

        public static void Install()
        {
            Console.SetOut(new EscapeSequencer(Console.Out));
        }

        public override Encoding Encoding
        {
            get { return textWriter.Encoding; }
        }
    }
}