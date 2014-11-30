# GameTimeWrapper Class
Allows you to speed up or slow down the rate at which things happen in your game.

## Notes
This is probably the most important class in the engine. This class helps you control the rate of the game. In XNA, games are usually set to run at 60 frames per second. If you want to slow down the game you would need to do everything in the game half as fast, 30 frames per second. You could just change the rate that frames are drawn but that just makes the game feel strange due to the low draw rate. This class allows you to manipulate the update rate of a game while keeping the draw rate at 60 FPS (or whatever rate your game runs at).

## Fields
- **systemSpeed** - The time between frames from XNA. Usually this is 60 FPS or 166667 ticks.
- **gameSpeed** - This time world's time between frames. Stored in ticks
- **gameSpeedDecimal** - This time world's time between frames specified as a ratio. gameSpeed / systemSpeed. If the are the same value (166667 / 166667) that means the game is running at a speed of 1.0. If gameSpeed is larger than systemSpeed then the game will run faster. (333334 / 166667) means the game is running at a speed of 2.0. If gameSpeed is smaller than systemSpeed then the game will run slower. (83333 / 166667) means the game is running at a speed of 0.5. The slowest the game can run is (1 / 166667) or a speed of 0.00000599998. The fastest the game can run is (9223372036854775807 / 16667) which is 5.534 * 10^13. The gameSpeed value is the max value of a long. I have not tried running stuff at this speed. I have no idea why you would want a game to run that fast.
- **GameSpeed** - The time of the world specified as a ratio. After setting a value (for example 0.5) we then recalculate the gameSpeedDecimal value by multiplying the systemSpeed by the value given then divide the gameSpeed by the systemSpeed. With our example value if we are running at 60 FPS we would have a systemSpeed of 166667. We would get a gameSpeed of 83333 (due to truncation) and then get a recalculated gameSpeedDecimal of 0.499997000006.
- **UpdateMethod** - The update method that should be run
- **ActualGameSpeed** - The speed the game is actually running at. GameSpeed will always be between -1 and 1 due to the way update is handled. For more information read up on the update method.
- **TotalGameTime** - From GameTime. How much time has passed since we started the game.
- **ElapsedGameTime** - From GameTime. How much time has passed since we last called update. Should always be the same if running a fixed update rate game.
- **IsRunningSlowly** - From GameTime. If we are running a fixed update rate game are we calling update slower than we should be.

## Constructor
Create a new game time with a given update method, game class, and speed this time should run at

## Methods
**Update** - Initially GameSpeed was reported at the speed the player set. However I ran into an issue with animations. If we moved more than one animation frame in an update we would have to skip frames. Displaying the right frame was not an issue. The real issue was the frame actions that were set on a frame. Say we had an animation with five frames. On frame 2 we tell the animation to start moving upwards 5 pixels. On frame 5 we tell the animation to stop moving upwards. On frames 2, 3, and 4 we would move upwards 5 pixels for a total of 15 pixels. If the game was running fast enough that we hit frame 2 and then skipped to frame 5 we would only move up 5 pixels. We would move up 15 pixels if we set frame 3 and 4 to also move up 5 pixels but it makes much more sense to have the sprite the animation is linked to handle moving upwards after we tell it to start moving upwards. If the sprite has no idea that we skipped frames then it would have no idea that it missed out on moving 10 pixels.

To fix this issue if the game should be running faster than 1.0 then we run the update method attached to this game time multiple times until we hit our actual game speed. If we want the game to run at 3.5 times its normal rate we run the update method three times at a speed of 1.0 then one time at a speed of 0.5. Now when we run animations we will never skip frames unless the frame time for a sprite sheet is shorter than the usual update rate of the game (16.6667 milliseconds). If we go back to our example we will hit frame 2, 3, and 4 and all other update methods in the game time will run their appropriate things making sure we don't miss out on things.

TotalGameTime and ElapsedGameTime are then modified to reflect how much time has passed in the game world. If the game is running at 0.5 then ElapsedGameTime will be 83333 ticks instead of 166667 ticks.

Some things in a game may still need to know the real speed the game is running at so ActualGameSpeed will display the real speed of the game even if we are running faster than 1.0 (or slower than -1.0)
