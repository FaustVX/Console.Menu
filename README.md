# Console.Menu
A Simple Console Menu

## Using the menu
(this menu will return the selected item)
``` csharp
char selected = Menu("My Menu", new[] {'e', 'r', 't', 'y'}, title: e => e.ToString(), visible: e => true);
```
or (thiw will execute the associated action)
``` csharp
Menu("My Menu", ("Menu 1", () => DoAction1(), visible: true), ("Menu 2", () => DoAction2(), visible: false));
```
or (an easy Yes/No menu)
``` csharp
bool selected = Menu("Continue ?", new[] {true, false}, title: e => e ? "Yes" : "No");
```
or (the simplest form, with an enum)
``` csharp
enum E { E1, E2, E3};
E selected = Menu<E>("Select value");
```
Not visible elements will be in red and not selectable.

The first menu will return the selected item, and the second will execute the appropriate method.

You can use the arrow keys, or the number keys, to highlight the item, and enter to select it.
