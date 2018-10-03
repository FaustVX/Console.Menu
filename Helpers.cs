using System;
using System.Linq;

namespace ConsoleMenu
{
    public static class Helpers
    {
        public static (string SelectedChar, ConsoleColor NormalColor, ConsoleColor NotVisibleColor) Options
            = (">", ConsoleColor.Blue, ConsoleColor.Red);

        public static void Write(string text, ConsoleColor foregroundColor)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.Write(text);
            Console.ForegroundColor = old;

        }

        public static int Menu(string title, params (string title, bool visible)[] list)
        {
            var selected = 0;
            var offset = list.TakeWhile(l => !l.visible).Count();
            list = list.Skip(offset).Reverse().SkipWhile(l => !l.visible).Reverse().ToArray();

            while (true)
            {
                Console.Clear();

                Console.WriteLine(title);
                for (var i = 0; i < list.Length; i++)
                {
                    Console.Write($"{(i == selected ? Options.SelectedChar : "")} {i}- ");
                    Write(list[i].title, list[i].visible ? Options.NormalColor : Options.NotVisibleColor);
                    Console.WriteLine();
                }

                switch (Console.ReadKey().Key)
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
                        Console.Clear();
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
