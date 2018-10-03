# Console.Menu
A Simple Console Menu

## Using the menu
``` csharp
char selected = Menu("My Menu", new[] {'e', 'r', 't', 'y'}, title: e => e.ToString(), visible: e => true)
```
or
``` csharp
Menu("My Menu", ("Menu 1", () => DoAction1(), visible: true), ("Menu 2", () => DoAction2(), visible: false));
```
Not visible elements will be in red and not selectable.

The first menu will return the selected item, and the second will execute the appropriate method.

You can use the arrow keys to highlight the item, and enter to select it.
