using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleMenu;

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
                Console.Write($"{(i == selected ? Options.SelectedChar : "-")} {i}- ");
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
                case >= ConsoleKey.D0 and <= ConsoleKey.D9 and var num when (num - ConsoleKey.D0) <= list.Length:
                    return num - ConsoleKey.D0 + offset;
                case >= ConsoleKey.NumPad0 and <= ConsoleKey.NumPad9 and var num when (num - ConsoleKey.NumPad0) <= list.Length:
                    return num - ConsoleKey.NumPad0 + offset;
            }
        }
    }

    public static void Menu(string title, params (string title, Action action, bool visible)[] list)
        => list[Menu(title, list.Select(l => (l.title, l.visible)).ToArray())].action();

    public static T Menu<T>(string title, IList<T> elements, Func<T, string> toTitle, Func<T, bool> toVisible = null)
    {
        toVisible ??= (_ => true);
        return elements[Menu(title, elements.Select(elem => (toTitle(elem), toVisible(elem))).ToArray())];
    }

    public static T Menu<T>(string title, Func<T, string> toTitle = null!, Func<T, bool> toVisible = null!)
        where T : struct, Enum
        => Menu(title, (T[])Enum.GetValues(typeof(T)), toTitle ?? (t => t.ToString()), toVisible);
}
