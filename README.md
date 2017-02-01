# GLX

An XNA/MonoGame library

# Using on macOS
Go to where MonoGame keeps its assemblies, typically `/Library/Frameworks/MonoGame.framework/v3.0/Assemblies/DesktopGL` and make a symbolic link from MonoGame.Framework.dll to MonoGame.Framework.Unix.dll  
`sudo ln -s /Library/Frameworks/MonoGame.framework/v3.0/Assemblies/DesktopGL/MonoGame.Framework.dll /Library/Frameworks/MonoGame.framework/v3.0/Assemblies/DesktopGL/MonoGame.Framework.Unix.dll`

[Overview of classes](https://github.com/golf1052/GLX/blob/master/Documentation/Overview.md)

## Usage
1. Install Visual Studio 2010/2012/2013
2. Install [MonoGame](http://teamcity.monogame.net/repository/download/MonoGame_DevelopWin/latest.lastSuccessful/Windows/MonoGameInstaller-3.2.exe?guest=1)
3. Install [XNA for Visual Studio](https://msxna.codeplex.com/)
4. Download the library
5. Add GLX to solution
6. Add reference to GLX to MonoGame project
7. Add `using GLX;` to any relevant class files

After you create a new MonoGame solution MonoGame creates a Game1.cs that contains the main Update/Draw loop that the game runs on. This is the file you change to start using GLX. Add a using statement for GLX. First you can remove the field spriteBatch and the initialization call for spriteBatch in LoadContent(). GLX.World contains a sprite batch. Add `World world;` to the top of the file after `GraphicsDeviceManager graphics;`. GLX.World handles many things for the game. Now add `GameTimeWrapper mainGameTime;` after the world line. mainGameTime is the main game time the game will run in. You can have multiple game times in one game. For example the UI elements could run at 1.0 speed, the player could run at 2.0 speed, and enemies could run at 0.5 speed. You would need to create a new GameTimeWrapper for each one of the speeds so in this case you would have three GameTimeWrappers defined.

After Update() add a new method. Here is what it should look like.
```CSharp
public void MainUpdate(GameTimeWrapper gameTime)
{
    world.gameStates["game1"].UpdateCurrentCamera(gameTime);
    base.Update(gameTime);
}
```
This is the update method we will use to update things in the game. world.gameStates["game1"].UpdateCurrentCamera will update the camera we are using for the game. gameStates["game1"] is our main game state that we have not set up yet. base.Update will update any GameComponents we have.

In LoadContent() add `world = new World(graphics);`. This sets up the world. Next add `world.LoadSpriteBatch();`. This replaces the spriteBatch initialization call we had before. Next add `mainGameTime = new GameTimeWrapper(MainUpdate, this, 1.0m);`. This sets up the mainGameTime to call MainUpdate() at a speed of 1.0. Now to set up the game states. Game states and menu states make it easy to switch between menus and parts of a game. Add the line `world.AddGameState("game1", graphics);`. This creates a new game state with the name game1. If a game state is being updated the game state will run its update methods and draw methods. MainUpdate() is the update method for this state but we still need a draw method. After Draw() add a new method. This is what it should look like. Between BeginDraw() and EndDraw() draw code would be added.
```CSharp
public void MainDraw()
{
  world.BeginDraw();
  world.EndDraw();
}
```
Now back in LoadContent() add
```CSharp
world.gameStates["game1"].AddTime(mainGameTime);
world.gameStates["game1"].AddDraw(MainDraw);
world.ActivateGameState("game1");
```
Now every time the game state updates game1 will tell mainGameTime to update its time and call its method MainUpdate. game1 will also tell the draw method to run. The last line activates the game state so that it is updated every frame. If the game state is not active then it will not be updated or drawn at all. This is useful if you want to pause a state. `world.ClearStates();` clears all the active game and menu states.

In Update() remove `base.Update(gameTime);`. This means GameComponents will update at the speed MainUpdate runs. Now add `world.Update(gameTime);` to Update(). This means the game times added to the world will be updated. This call must be made in the original update method and not in any update methods using GameTimeWrapper.

The last thing that needs to be done is to make sure the active game state/menu state is drawn. Add `world.DrawWorld();` to original Draw() method after the line `GraphicsDevice.Clear(Color.CornflowerBlue);`.

Now you are ready to use GLX!

### Updating and drawing a sprite
First create a new sprite `Sprite sprite;`. In LoadContent() add
```CSharp
sprite = new Sprite(Content.Load<Texture2D>(*NAME OF TEXTURE*));
sprite.pos = new Vector2(100, 100);
```
In your game world update loop (MainUpdate() from the example above) add
```CSharp
KeyboardState keyboardState = Keyboard.GetState();
sprite.Move(keyboardState, 5.0f * (float)gameTime.GameSpeed, SpriteBase.MovementDirection.Right, Keys.Right);
sprite.Update(gameTime, graphics);
```
This gets the keyboard state and allows us to press the right arrow key to move the sprite. The sprite will move 5 pixels if the game is running at 1.0. If the game is running at 2.0 the sprite will move 10 pixels. In MainDraw() add `world.Draw(sprite.Draw);` between BeginDraw() and EndDraw().

This starts our sprite batch and also uses the camera for the world. GLX.World starts out with a default camera so you don't need to worry about it. Then we draw the sprite using its draw method. Then we end the sprite batch. If you did everything correctly you should see the center of your sprite at position 100, 100. If you press the right arrow key the sprite will move towards the right side of the screen. If you set the update method to update at 0.5 speed the sprite will move slower. If you set the update method to update at 2.0 speed the sprite will move faster.
