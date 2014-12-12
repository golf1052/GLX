# World Class
Holds information and controls the world for a game

## Fields
- Random number generator
- Global keyboard, gamepad, and mouse states. Updated once a frame in World.Update
- **gameStates** - Dictionary of game states keyed by the name of the game state
- **activeGameStates** - List of active game states
- **menuStates** - Dictionary of menu states keyed by the name of the menu state
- **activeMenuStates** - List of active menu states

## Constructor
- Create a new world

## Methods
- **LoadSpriteBatch** - Sets up the sprite batch for the game. Should be called at the beginning of LoadContent
- **AddGameState** - Adds a game state to the list of game states
- **AddMenuState** - Adds a menu state to the list of menu states
- **ActivateGameState** - Activates a game state so that it gets updated and drawn when the world is updated
- **ActivateMenuState** - Activates a menu state so that it gets updated and drawn when the world is updated
- **ClearStates** - Clears both the activated game states and the activated menu states
- **Update** - Updates the world
- **BeginDraw** - Begin the sprite batch
- **Draw** - Draw using the specified draw method
- **EndDraw** - End the sprite batch
