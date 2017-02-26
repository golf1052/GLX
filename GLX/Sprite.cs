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
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Sprite texture
        /// </summary>
        public Texture2D tex;

        /// <summary>
        /// Sprite draw rectangle
        /// </summary>
        private Rectangle drawRect;

        private bool usingDrawRect;

        private Size drawSize;
        /// <summary>
        /// Sprite draw size
        /// </summary>
        public Size DrawSize
        {
            get
            {
                return drawSize;
            }
            set
            {
                drawSize = value;
                drawRect.Width = (int)drawSize.Width;
                drawRect.Height = (int)drawSize.Height;
            }
        }

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
        /// SpriteEffects
        /// </summary>
        public SpriteEffects spriteEffects;

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
        public Sprite(Texture2D loadedTex) : base()
        {
            tex = loadedTex;
            drawRect = new Rectangle((int)position.X, (int)position.Y, 0, 0);
            usingDrawRect = false;
            drawSize = Size.Zero;
            rectangle = new Rectangle((int)position.X, (int)position.Y, tex.Width, tex.Height);
            colorData = new ColorData(tex);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            spriteEffects = SpriteEffects.None;
            isAnimated = false;
            ready = true;
        }

        /// <summary>
        /// Creates a new sprite that has a 1 by 1 white pixel as its texture
        /// </summary>
        /// <remarks>This pixel can be resized using <see cref="DrawSize"/></remarks>
        /// <param name="graphics">Graphics device manager for game</param>
        public Sprite(GraphicsDeviceManager graphics) : base()
        {
            tex = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            tex.SetData(new[] { color });
            drawRect = new Rectangle((int)position.X, (int)position.Y, 0, 0);
            usingDrawRect = true;
            drawSize = Size.Zero;
            rectangle = new Rectangle((int)position.X, (int)position.Y, tex.Width, tex.Height);
            colorData = new ColorData(tex);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            spriteEffects = SpriteEffects.None;
            isAnimated = false;
            ready = true;
        }

        public Sprite(SpriteSheetInfo spriteSheetInfo, GraphicsDeviceManager graphics) : base()
        {
            this.graphics = graphics;
            isAnimated = true;
            drawRect = new Rectangle((int)position.X, (int)position.Y, 0, 0);
            usingDrawRect = false;
            drawSize = Size.Zero;
            spriteEffects = SpriteEffects.None;
            animations = new Animations(spriteSheetInfo);
            ready = false;
        }

        /// <summary>
        /// Creates an animatable Sprite that is not ready to use
        /// In order to use this sprite you need to add sprite sheets
        /// Then call <code>Ready</code>
        /// </summary>
        /// <param name="spriteSheetInfo">The sprite sheet info</param>
        /// <param name="gameTime">The game time the sprite will exist in</param>
        public Sprite(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime, GraphicsDeviceManager graphics) : base()
        {
            this.graphics = graphics;
            isAnimated = true;
            drawRect = new Rectangle((int)position.X, (int)position.Y, 0, 0);
            usingDrawRect = false;
            drawSize = Size.Zero;
            spriteEffects = SpriteEffects.None;
            animations = new Animations(spriteSheetInfo, gameTime);
            ready = false;
        }

        /// <summary>
        /// Sets up an animated Sprite so that it is ready to be used
        /// </summary>
        /// <remarks>Sets the texture as the first </remarks>
        public void Ready()
        {
            tex = new Texture2D(graphics.GraphicsDevice, animations.spriteSheetInfo.frameWidth, animations.spriteSheetInfo.frameHeight);
            if (animations.CurrentAnimationName == "" || animations.CurrentAnimationName == null)
            {
                animations.currentSpriteSheet = animations.spriteSheets.First().Value;
            }
            UpdateTexAndColorData(animations.currentSpriteSheet, graphics.GraphicsDevice);
            rectangle = new Rectangle((int)position.X, (int)position.Y, tex.Width, tex.Height);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            ready = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (isAnimated)
            {
                UpdateAnimation(gameTime);
            }
            velocity += acceleration;
            position += velocity;
            drawRect.X = (int)position.X;
            drawRect.Y = (int)position.Y;
            if (!usingDrawRect)
            {
                rectangle = new Rectangle((int)position.X, (int)position.Y, tex.Width, tex.Height);
                spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                    Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)) *
                    Matrix.CreateTranslation(new Vector3(position, 0.0f));
                rectangle = CalculateBoundingRectangle(new Rectangle(0, 0, tex.Width, tex.Height), spriteTransform);
            }
            else
            {
                rectangle = new Rectangle((int)position.X, (int)position.Y, drawRect.Width, drawRect.Height);
                spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                    Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)) *
                    Matrix.CreateTranslation(new Vector3(position, 0.0f));
                rectangle = CalculateBoundingRectangle(new Rectangle(0, 0, drawRect.Width, drawRect.Height), spriteTransform);
            }
        }

        /// <summary>
        /// Updates the sprite
        /// </summary>
        /// <param name="gameTime">The game time the sprite exists in</param>
        public virtual void Update(GameTimeWrapper gameTime)
        {
            if (isAnimated)
            {
                UpdateAnimation(gameTime);
            }
            velocity += acceleration * (float)gameTime.GameSpeed;
            position += velocity * (float)gameTime.GameSpeed;
            drawRect.X = (int)position.X;
            drawRect.Y = (int)position.Y;
            if (!usingDrawRect)
            {
                rectangle = new Rectangle((int)position.X, (int)position.Y, tex.Width, tex.Height);
                spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                    Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)) *
                    Matrix.CreateTranslation(new Vector3(position, 0.0f));
                rectangle = CalculateBoundingRectangle(new Rectangle(0, 0, tex.Width, tex.Height), spriteTransform);
            }
            else
            {
                rectangle = new Rectangle((int)position.X, (int)position.Y, drawRect.Width, drawRect.Height);
                spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                    Matrix.CreateScale(scale) * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)) *
                    Matrix.CreateTranslation(new Vector3(position, 0.0f));
                rectangle = CalculateBoundingRectangle(new Rectangle(0, 0, drawRect.Width, drawRect.Height), spriteTransform);
            }
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            if (animations.active)
            {
                animations.elapsedTime += gameTime.ElapsedGameTime.Ticks;
                if (animations.elapsedTime > animations.currentSpriteSheet.frameTime)
                {
                    long framesMoved = animations.elapsedTime / animations.currentSpriteSheet.frameTime;
                    for (int i = 0; i < framesMoved; i++)
                    {
                        animations.currentFrame++;
                        if (animations.currentFrame == animations.currentSpriteSheet.frameCount)
                        {
                            if (!animations.currentSpriteSheet.loop)
                            {
                                animations.active = false;
                                animations.elapsedTime = 0;
                                animations.currentFrame = 0;
                                break;
                            }
                            else
                            {
                                animations.currentFrame = 0;
                            }
                        }
                        if (animations.currentFrame >= 0 && animations.currentFrame < animations.currentSpriteSheet.frameActions.Count)
                        {
                            foreach (Action action in animations.currentSpriteSheet.frameActions[animations.currentFrame])
                            {
                                action.Invoke();
                            }
                        }
                    }
                }
                animations.elapsedTime = animations.elapsedTime % animations.currentSpriteSheet.frameTime;
            }
            if (animations.CurrentAnimationName != null)
            {
                UpdateTexture();
            }
        }

        /// <summary>
        /// Updates the sprite animation
        /// </summary>
        /// <param name="gameTime">The game time the sprite exists in</param>
        private void UpdateAnimation(GameTimeWrapper gameTime)
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
            UpdateTexture();
        }

        internal void UpdateTexture()
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

            if (graphics != null)
            {
                if (animations.sourceRect != oldSourceRect)
                {
                    UpdateTexAndColorData(animations.currentSpriteSheet, graphics.GraphicsDevice);
                }
            }
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAnimated && animations.active)
            {
                DrawAnimation(spriteBatch);
            }
            else
            {
                if (!usingDrawRect)
                {
                    spriteBatch.Draw(tex, position, null, color * alpha, MathHelper.ToRadians(rotation), origin, scale, spriteEffects, 0);
                }
                else
                {
                    DrawRect(spriteBatch);
                }
            }
        }

        public void DrawAnimation(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(animations.CurrentAnimation.tex, position, animations.sourceRect, color * alpha, MathHelper.ToRadians(rotation), origin, scale, spriteEffects, 0);
        }

        /// <summary>
        /// Draws the sprite using its draw rectangle
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public void DrawRect(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, drawRect, null, color * alpha, MathHelper.ToRadians(rotation), origin, spriteEffects, 0);
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
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
