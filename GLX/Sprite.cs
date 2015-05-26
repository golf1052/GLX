using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GLX
{
    /// <summary>
    /// Sprite class
    /// </summary>
    public class Sprite : SpriteBase
    {
        /// <summary>
        /// Sprite texture
        /// </summary>
        public Texture2D tex;

        /// <summary>
        /// Sprite draw rectangle
        /// </summary>
        public Rectangle drawRect;

        /// <summary>
        /// Texture color data
        /// </summary>
        public ColorData colorData;

        /// <summary>
        /// Sprite transform. Used when sprite is rotated and/or scaled
        /// </summary>
        public Matrix spriteTransform;

        /// <summary>
        /// Sprite animation data
        /// </summary>
        public Animations animations;

        /// <summary>
        /// Does the sprite have animations
        /// </summary>
        private bool isAnimated;

        /// <summary>
        /// If the sprite should have animations have they been loaded in and are
        /// we ready to use them?
        /// </summary>
        private bool ready;

        /// <summary>
        /// Creates a new sprite with the loaded texture
        /// </summary>
        /// <param name="loadedTex">The texture loaded from the Content manager</param>
        public Sprite(Texture2D loadedTex)
        {
            tex = loadedTex;
            drawRect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), 0, 0);
            rect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), tex.Width, tex.Height);
            colorData = new ColorData(tex);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            isAnimated = false;
            ready = true;
        }

        /// <summary>
        /// Creates a new sprite that has a 1 by 1 white pixel as its texture
        /// </summary>
        /// <remarks>This pixel can be resized using <see cref="drawRect"/></remarks>
        /// <param name="graphics">Graphics device manager for game</param>
        public Sprite(GraphicsDeviceManager graphics)
        {
            tex = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            tex.SetData(new[] { color });
            drawRect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), 0, 0);
            rect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), tex.Width, tex.Height);
            colorData = new ColorData(tex);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            isAnimated = false;
            ready = true;
        }

        /// <summary>
        /// Creates an animatable Sprite that is not ready to use
        /// In order to use this sprite you need to add sprite sheets
        /// Then call <code>Ready</code>
        /// </summary>
        /// <param name="spriteSheetInfo">The sprite sheet info</param>
        /// <param name="gameTime">The game time the sprite will exist in</param>
        public Sprite(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime)
        {
            isAnimated = true;
            drawRect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), 0, 0);
            animations = new Animations(spriteSheetInfo, gameTime);
            ready = false;
        }

        /// <summary>
        /// Sets up an animated Sprite so that it is ready to be used
        /// </summary>
        /// <remarks>Sets the texture as the first </remarks>
        /// <param name="graphics">The graphics device manager</param>
        public void Ready(GraphicsDeviceManager graphics)
        {
            tex = new Texture2D(graphics.GraphicsDevice, animations.spriteSheetInfo.frameWidth, animations.spriteSheetInfo.frameHeight);
            if (animations.currentAnimation == "" || animations.currentAnimation == null)
            {
                animations.currentSpriteSheet = animations.spriteSheets.First().Value;
            }
            UpdateTexAndColorData(animations.currentSpriteSheet, graphics.GraphicsDevice);
            rect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), tex.Width, tex.Height);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            ready = true;
        }

        /// <summary>
        /// Updates the sprite
        /// </summary>
        /// <param name="gameTime">The game time the sprite exists in</param>
        /// <param name="graphics">The graphics device manager</param>
        public virtual void Update(GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            if (isAnimated)
            {
                UpdateAnimation(gameTime, graphics);
            }
            pos += vel * (float)gameTime.GameSpeed;
            drawRect.X = (int)Math.Round(pos.X);
            drawRect.Y = (int)Math.Round(pos.Y);
            rect = new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), tex.Width, tex.Height);
            spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateScale(scale) * Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(pos, 0.0f));
            rect = CalculateBoundingRectangle(new Rectangle(0, 0, tex.Width, tex.Height), spriteTransform);
        }

        /// <summary>
        /// Updates the sprite animation
        /// </summary>
        /// <param name="gameTime">The game time the sprite exists in</param>
        /// <param name="graphics">The graphics device manager</param>
        private void UpdateAnimation(GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            // If the animation is active
            if (animations.active)
            {
                // Determine how much time has passed since we last updated
                animations.elapsedTime += gameTime.ElapsedGameTime.Ticks;
                if (gameTime.GameSpeed > 0)
                {
                    // If time is moving forwards then check to see if our elapsed time is greater than
                    // time we should spend on a frame
                    if (animations.elapsedTime > animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed)
                    {
                        // If it is determine how many frames we should have moved
                        long framesMoved = animations.elapsedTime / (long)(animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed);
                        for (int i = 0; i < framesMoved; i++)
                        {
                            // For every frame we moved add one to our current frame
                            animations.currentFrame++;
                            if (animations.runOneFrame)
                            {
                                animations.runOneFrame = false;
                                animations.active = false;
                            }
                            if (animations.currentFrame == animations.currentSpriteSheet.frameCount)
                            {
                                // Then check if we hit the frame count, if we did and the animation should not
                                // loop then stop the animation
                                if (!animations.currentSpriteSheet.loop)
                                {
                                    animations.active = false;
                                    animations.elapsedTime = 0;
                                    animations.currentFrame = 0;
                                    break;
                                }
                                else
                                {
                                    // If we should loop just go back to the first frame
                                    animations.currentFrame = 0;
                                }
                            }
                            if (animations.currentFrame >= 0 && animations.currentFrame < animations.currentSpriteSheet.frameActions.Count)
                            {
                                // and then run all the animation actions we have for this frame
                                foreach (Action action in animations.currentSpriteSheet.frameActions[animations.currentFrame])
                                {
                                    action.Invoke();
                                }
                            }
                        }
                        // then set the elapsed time to the left over time we had
                        animations.elapsedTime = animations.elapsedTime % (long)(animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed);
                    }
                }
                else if (gameTime.GameSpeed < 0)
                {
                    // everything in here is the same as above
                    if (animations.elapsedTime < animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed)
                    {
                        long framesMoved = animations.elapsedTime / (long)(animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed);
                        for (int i = 0; i < framesMoved; i++)
                        {
                            // except we go backwards in frames
                            animations.currentFrame--;
                            if (animations.runOneFrame)
                            {
                                animations.runOneFrame = false;
                                animations.active = false;
                            }
                            if (animations.currentFrame == -1)
                            {
                                // and if we hit the first frame then check to see if we should loop
                                if (!animations.currentSpriteSheet.loop)
                                {
                                    animations.active = false;
                                    animations.elapsedTime = 0;
                                    animations.currentFrame = 0;
                                    break;
                                }
                                else
                                {
                                    animations.currentFrame = animations.currentSpriteSheet.frameCount - 1;
                                }
                            }
                            if (animations.currentFrame >= 0 && animations.currentFrame < animations.currentSpriteSheet.reverseFrameActions.Count)
                            {
                                foreach (Action action in animations.currentSpriteSheet.reverseFrameActions[animations.currentFrame])
                                {
                                    action.Invoke();
                                }
                            }
                        }
                        animations.elapsedTime = animations.elapsedTime % (long)(animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed);
                    }
                }
            }
            UpdateTexture(graphics);
        }

        internal void UpdateTexture(GraphicsDeviceManager graphics)
        {
            Rectangle oldSourceRect = animations.sourceRect;
            // Then move our source rectangle to the right spot and update our texture and color data
            if (animations.currentSpriteSheet.direction == SpriteSheet.Direction.LeftToRight)
            {
                animations.sourceRect = new Rectangle((animations.currentFrame % animations.currentSpriteSheet.columns) * animations.spriteSheetInfo.frameWidth,
                    (animations.currentFrame / animations.currentSpriteSheet.columns) * animations.spriteSheetInfo.frameHeight,
                    animations.spriteSheetInfo.frameWidth,
                    animations.spriteSheetInfo.frameHeight);
            }
            else if (animations.currentSpriteSheet.direction == SpriteSheet.Direction.TopToBottom)
            {
                animations.sourceRect = new Rectangle((animations.currentFrame / animations.currentSpriteSheet.rows) * animations.spriteSheetInfo.frameWidth,
                    (animations.currentFrame % animations.currentSpriteSheet.rows) * animations.spriteSheetInfo.frameHeight,
                    animations.spriteSheetInfo.frameWidth,
                    animations.spriteSheetInfo.frameHeight);
            }
            if (animations.sourceRect != oldSourceRect)
            {
                UpdateTexAndColorData(animations.currentSpriteSheet, graphics.GraphicsDevice);
            }
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, pos, null, color * alpha, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws the sprite using its draw rectangle
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public void DrawRect(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, drawRect, null, color * alpha, MathHelper.ToRadians(rotation), origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Updates the texture field and color data information with the frame we are on in the animation
        /// </summary>
        /// <param name="spriteSheet">The sprite sheet we are using</param>
        /// <param name="graphicsDevice">The graphics device</param>
        private void UpdateTexAndColorData(SpriteSheet spriteSheet, GraphicsDevice graphicsDevice)
        {
            // Unset the graphics device before setting new graphics data
            graphicsDevice.Textures[0] = null;
            // Setting color data is super taxing
            tex = new Texture2D(graphicsDevice, animations.spriteSheetInfo.frameWidth, animations.spriteSheetInfo.frameHeight);
            tex.SetData<Color>(spriteSheet.GetFrameColorData(animations.sourceRect).colorData1D);
            colorData = spriteSheet.GetFrameColorData(animations.sourceRect);
        }

        /// <summary>
        /// Used for pixel perfect collision.
        /// </summary>
        /// <param name="rectangle">The current bounding rectangle</param>
        /// <param name="transform">The current sprite transform matrix</param>
        /// <returns>Returns a new bounding rectangle</returns>
        private Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)Math.Round(min.X), (int)Math.Round(min.Y),
                                 (int)Math.Round(max.X - min.X), (int)Math.Round(max.Y - min.Y));
        }
    }
}
