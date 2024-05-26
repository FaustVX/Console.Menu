using System;
using System.Collections.Generic;
using System.Linq;

#if NET8_0_OR_GREATER

using static Helpers;
using System.Threading.Tasks;

Menu("Hello", "1", "2");
_ = Menu("My Menu", ['e', 'r', 't', 'y'], toTitle: e => e.ToString(), toVisible: e => true);
Menu("My Menu", ("Menu 1", DoAction1), ("Menu 2", DoAction2, IsVisible: false));
_ = await Menu<Task<int>>("My Menu", ("Menu 1", () => Task.FromResult(DoFunc1()), IsVisible: false), ("Menu 2", DoFunc2));
_ = Menu("Continue ?", [true, false], toTitle: e => e ? "Yes" : "No");
_ = Menu<E>("Select value");

static void DoAction1() {}
static void DoAction2() {}

static int DoFunc1() => 1;
static Task<int> DoFunc2() => Task.FromResult(2);
enum E { E1, E2, E3};

#pragma warning disable CA1050 // Declare types in namespaces

#else

namespace ConsoleMenu;

#endif

public readonly record struct Item(string Title, bool IsVisible = true)
{
    public static implicit operator Item(string Title)
    => new(Title);

    public static implicit operator Item((string Title, bool IsVisible) tuple)
    => new(tuple.Title, tuple.IsVisible);
}

public readonly record struct ItemWithAction(string Title, Action Action, bool IsVisible = true)
{
    public static implicit operator ItemWithAction((string Title, Action Action, bool IsVisible) tuple)
    => new(tuple.Title, tuple.Action, tuple.IsVisible);

    public static implicit operator ItemWithAction((string Title, Action Action) tuple)
    => new(tuple.Title, tuple.Action);
}

public readonly record struct ItemWithFunc<T>(string Title, Func<T> Func, bool IsVisible = true)
{
    public static implicit operator ItemWithFunc<T>((string Title, Func<T> Func, bool IsVisible) tuple)
    => new(tuple.Title, tuple.Func, tuple.IsVisible);

    public static implicit operator ItemWithFunc<T>((string Title, Func<T> Func) tuple)
    => new(tuple.Title, tuple.Func);
}

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

    public static int Menu(string title, params Item[] list)
    {
        var selected = 0;
        var offset = list.TakeWhile(l => !l.IsVisible).Count();
        list = list.Skip(offset).Reverse().SkipWhile(l => !l.IsVisible).Reverse().ToArray();

        while (true)
        {
            Console.Clear();

            Console.WriteLine(title);
            for (var i = 0; i < list.Length; i++)
            {
                Console.Write($"{(i == selected ? Options.SelectedChar : "-")} {i}- ");
                Write(list[i].Title, list[i].IsVisible ? Options.NormalColor : Options.NotVisibleColor);
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
                    } while (!list[selected].IsVisible);
                    break;
                case ConsoleKey.DownArrow:
                    do
                    {
                        if (selected < list.Length - 1)
                            selected++;
                        else
                            selected = 0;
                    } while (!list[selected].IsVisible);
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.Enter:
                    if (!list[selected].IsVisible)
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

    public static void Menu(string title, params ItemWithAction[] list)
        => list[Menu(title, list.Select<ItemWithAction, Item>(l => (l.Title, l.IsVisible)).ToArray())].Action();

    public static T Menu<T>(string title, params ItemWithFunc<T>[] list)
        => list[Menu(title, list.Select<ItemWithFunc<T>, Item>(l => (l.Title, l.IsVisible)).ToArray())].Func();

    public static T Menu<T>(string title, IList<T> elements, Func<T, string> toTitle, Func<T, bool> toVisible = null)
    {
        toVisible ??= (_ => true);
        return elements[Menu(title, elements.Select<T, Item>(elem => (toTitle(elem), toVisible(elem))).ToArray())];
    }

    public static T Menu<T>(string title, Func<T, string> toTitle = null!, Func<T, bool> toVisible = null!)
        where T : struct, Enum
        => Menu(title, (T[])Enum.GetValues(typeof(T)), toTitle ?? (t => t.ToString()), toVisible);
}
