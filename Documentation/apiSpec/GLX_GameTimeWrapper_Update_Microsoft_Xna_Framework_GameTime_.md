---
uid: GLX.GameTimeWrapper.Update(Microsoft.Xna.Framework.GameTime)
---

Initially GameSpeed was reported at the speed the player set. However I ran into an issue with animations. If we moved more than one animation frame in an update we would have to skip frames. Displaying the right frame was not an issue. The real issue was the frame actions that were set on a frame. Say we had an animation with five frames. On frame 2 we tell the animation to start moving upwards 5 pixels. On frame 5 we tell the animation to stop moving upwards. On frames 2, 3, and 4 we would move upwards 5 pixels for a total of 15 pixels. If the game was running fast enough that we hit frame 2 and then skipped to frame 5 we would only move up 5 pixels. We would move up 15 pixels if we set frame 3 and 4 to also move up 5 pixels but it makes much more sense to have the sprite the animation is linked to handle moving upwards after we tell it to start moving upwards. If the sprite has no idea that we skipped frames then it would have no idea that it missed out on moving 10 pixels.

To fix this issue if the game should be running faster than 1.0 then we run the update method attached to this game time multiple times until we hit our actual game speed. If we want the game to run at 3.5 times its normal rate we run the update method three times at a speed of 1.0 then one time at a speed of 0.5. Now when we run animations we will never skip frames unless the frame time for a sprite sheet is shorter than the usual update rate of the game (16.6667 milliseconds). If we go back to our example we will hit frame 2, 3, and 4 and all other update methods in the game time will run their appropriate things making sure we don't miss out on things.

TotalGameTime and ElapsedGameTime are then modified to reflect how much time has passed in the game world. If the game is running at 0.5 then ElapsedGameTime will be 83333 ticks instead of 166667 ticks.

Some things in a game may still need to know the real speed the game is running at so ActualGameSpeed will display the real speed of the game even if we are running faster than 1.0 (or slower than -1.0)
