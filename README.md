# GLX

An XNA/MonoGame library

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

Now in Initialize() add `world = new World(graphics);`. This sets up the world. Now after Update() add a new method. Here is what it should look like.
```CSharp
public void MainUpdate(GameTimeWrapper gameTime)
{
    world.UpdateCurrentCamera(gameTime);
    base.Update(gameTime);
}
```
This is the update method we will use to update things in the game. world.UpdateCurrentCamera will update the camera we are using for the game. base.Update will update any GameComponents we have. Back in Initialize() add `mainGameTime = new GameTimeWrapper(MainUpdate, this, 1.0m);`. This sets up the mainGameTime to call MainUpdate() at a speed of 1.0. The last line we need to add is `world.AddTime(mainGameTime);`. This adds mainGameTime to the list of game times the world will manage. Now everytime the game updates World will tell mainGameTime to update its time and call its method MainUpdate.

In LoadContent() add `world.LoadSpriteBatch();`. This replaces the spriteBatch initialization call we had before.

In Update() remove `base.Update(gameTime);`. This means GameComponents will update at the speed MainUpdate runs. Now add `world.Update(gameTime);` to Update(). This means the game times added to the world will be updated. This call must be made in the original update method and not in any update methods using GameTimeWrapper.

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
This gets the keyboard state and allows us to press the right arrow key to move the sprite. The sprite will move 5 pixels if the game is running at 1.0. If the game is running at 2.0 the sprite will move 10 pixels. In Draw() add
```CSharp
world.BeginDraw();
world.Draw(sprite.Draw);
world.EndDraw();
```
This starts our sprite batch and also uses the camera for the world. GLX.World starts out with a default camera so you don't need to worry about it. Then we draw the sprite using its draw method. Then we end the sprite batch. If you did everything correctly you should see the center of your sprite at position 100, 100. If you press the right arrow key the sprite will move towards the right side of the screen.
