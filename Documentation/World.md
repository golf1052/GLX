# World Class
Holds information and controls the world for a game

## Fields
- Random number generator
- **gameTimes** - List of game times in the world
- **cameras** - List of cameras in the world
- **currentCamera** - The name of the camera we are using
- **camera1** - The default camera

## Constructor
- Create a new world

## Methods
- **LoadSpriteBatch** - Sets up the sprite batch for the game. Should be called at the beginning of LoadContent
- **AddTime** - Adds a game time to the list of game times
- **AddCamera** - Adds a camera to the list of cameras
- **RemoveCamera** - Removes a camera from the list of cameras
- **UpdateCurrentCamera** - Updates the current camera. Must be called by user because we don't know what time the camera exists in.
- **Update** - Updates the world
- **BeginDraw** - Begin the sprite batch
- **Draw** - Draw using the specified draw method
- **EndDraw** - End the sprite batch
