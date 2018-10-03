using System;
using System.Linq;
using CLI = System.Console;

namespace ConsoleMenu
{
    public static class Helpers
    {
        public static void Write(string text, ConsoleColor foregroundColor)
        {
            var old = CLI.ForegroundColor;
            CLI.ForegroundColor = foregroundColor;
            CLI.Write(text);
            CLI.ForegroundColor = old;

        }

        public static int Menu(string title, params (string title, bool visible)[] list)
        {
            var selected = 0;
            var offset = list.TakeWhile(l => !l.visible).Count();
            list = list.Skip(offset).Reverse().SkipWhile(l => !l.visible).Reverse().ToArray();

            while (true)
            {
                CLI.Clear();

                CLI.WriteLine(title);
                for (var i = 0; i < list.Length; i++)
                {
                    CLI.Write($"{(i == selected ? ">" : "")} {i}- ");
                    Write(list[i].title, list[i].visible ? ConsoleColor.Blue : ConsoleColor.Red);
                    CLI.WriteLine();
                }

                switch (CLI.ReadKey().Key)
                {
                    case ConsoleKey.PageUp:
                        selected = 0;
                        break;
                    case ConsoleKey.PageDown:
                        selected = list.Length - 1;
                        break;
                    case ConsoleKey.UpArrow:
                        do
                        {
                            if (selected > 0)
                                selected--;
                            else
                                selected = list.Length - 1;
                        } while (!list[selected].visible);
                        break;
                    case ConsoleKey.DownArrow:
                        do
                        {
                            if (selected < list.Length - 1)
                                selected++;
                            else
                                selected = 0;
                        } while (!list[selected].visible);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.Enter:
                        if (!list[selected].visible)
                            continue;
                        CLI.Clear();
                        return selected + offset;
                }
            }
        }

        public static void Menu(string title, params (string title, Action action, bool visible)[] list)
            => list[Menu(title, list.Select(l => (l.title, l.visible)).ToArray())].action();

        public static T Menu<T>(string title, System.Collections.Generic.IList<T> elements, Func<T, string> toTitle, Func<T, bool> toVisible = null)
        {
            toVisible = toVisible ?? (_ => true);
            return elements[Menu(title, elements.Select(elem => (toTitle(elem), toVisible(elem))).ToArray())];
        }
    }
}
