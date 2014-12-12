# MenuState Class
Holds information about a menu state. Inherits from GameState

## Notes
There is some information about MenuState in the GameState documentation file. Typically menus have text that takes the user to a new place in the game whether that is another menu or actual game play. MenuState provides extra functionality to handle selecting menu items.

## Fields
- **menuItems** - List of menu text items
- **unselectedColor** - The color the menu item should be when it is not selected
- **selectedColor** - The color the menu item should be when it is selected
- **menuFont** - The font that should be used when drawing text
- **initialPosition** - The position where text should start to be drawn
- **spacing** - The spacing between menu items
- **menuDirection** - The direction the menu should go, either left to right or top to bottom.

## Constructor
Creates a new menu state. Unlike GameState menu states come with their own game time because menus typically run at normal speed.

## Methods
- **AddMenuItem** - Add a new menu item to the list of menu where the given string is what that menu item will display
- **SetMenuAction** - Set an action on a menu item. This is used so that menu items can cause actions to happen like switching to a new menu or launching into gameplay.
- **Update** - Updates the menu. If a direction is pressed like the left arrow key then the menu will update accordingly (if the menu is left to right directional). Supports arrow keys, left thumbstick, and directional pad input for selecting menu items.
