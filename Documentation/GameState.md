# GameState Class
Holds information about a game state

## Notes
GameState and MenuState provide an easy way to switch between states in game. Typically a game can have different parts and also have different menus. GameState and MenuState help make managing those states easy. Most of the things that used to be in the World class are now in the GameState class.

## Fields
- **name** - Name of the game state
- **gameTimes** - List of game times in this state
- **drawMethods** - List of draw methods in this state
- **cameras** - List of cameras in this state
- **currentCamera** - The name of the camera that is being used
- **camera1** - The default camera for this state

## Constructor
Creates a new game state

## Methods
- **AddTime** - Adds a game time to the list of game times
- **AddCamera** - Adds a camera to the list of cameras
- **RemoveCamera** - Removes a camera from the list of cameras
- **UpdateCurrentCamera** - Updates the current camera. Must be called by the user because we don't know what time the camera exists in
- **Update** - Updates the game state
- **AddDraw** - Adds a draw method to the list of draw methods. The draw methods get called from the first draw method in the list to the last draw method in the list.
