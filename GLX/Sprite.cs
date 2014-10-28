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
    public class Sprite : SpriteBase
    {
        public Texture2D tex;
        public ColorData colorData;
        public Matrix spriteTransform;
        public bool isAnimated;
        public Animations animations;

        private bool ready;

        public Sprite(Texture2D loadedTex)
        {
            tex = loadedTex;
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            colorData = new ColorData(tex);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            isAnimated = false;
            ready = true;
        }

        /// <summary>
        /// Creates an animatable Sprite that is not ready to use
        /// In order to use this sprite you need to add sprite sheets
        /// Then call Ready
        /// </summary>
        /// <param name="spriteSheetInfo"></param>
        public Sprite(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime)
        {
            isAnimated = true;
            animations = new Animations(spriteSheetInfo, gameTime);
            ready = false;
        }

        public void Ready(GraphicsDevice graphicsDevice)
        {
            tex = new Texture2D(graphicsDevice, animations.spriteSheetInfo.frameWidth, animations.spriteSheetInfo.frameHeight);
            animations.currentSpriteSheet = animations.spriteSheets.First().Value;
            UpdateTexAndColorData(animations.currentSpriteSheet, graphicsDevice);
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            ready = true;
        }

        public virtual void Update(GameTimeWrapper gameTime, GraphicsDevice graphicsDevice)
        {
            if (isAnimated)
            {
                UpdateAnimation(gameTime, graphicsDevice);
            }
            pos += vel;
            drawRect.X = (int)pos.X;
            drawRect.Y = (int)pos.Y;
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateScale(scale) * Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(pos, 0.0f));
            rect = CalculateBoundingRectangle(new Rectangle(0, 0, tex.Width, tex.Height), spriteTransform);
        }

        void UpdateAnimation(GameTimeWrapper gameTime, GraphicsDevice graphicsDevice)
        {
            if (animations.active)
            {
                animations.elapsedTime += gameTime.ElapsedGameTime.Ticks;
                if (gameTime.GameSpeed > 0)
                {
                    if (animations.elapsedTime > animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed)
                    {
                        long framesMoved = animations.elapsedTime / (long)(animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed);
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
                        animations.elapsedTime = animations.elapsedTime % (long)(animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed);
                    }
                }
                else if (gameTime.GameSpeed < 0)
                {
                    if (animations.elapsedTime < animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed)
                    {
                        long framesMoved = animations.elapsedTime / (long)(animations.currentSpriteSheet.frameTimeTicks / gameTime.GameSpeed);
                        for (int i = 0; i < framesMoved; i++)
                        {
                            animations.currentFrame--;
                            if (animations.currentFrame == 0)
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
            animations.sourceRect = new Rectangle(animations.currentFrame * animations.spriteSheetInfo.frameWidth,
                0,
                animations.spriteSheetInfo.frameWidth,
                animations.spriteSheetInfo.frameHeight);
            UpdateTexAndColorData(animations.currentSpriteSheet, graphicsDevice);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, pos, null, color, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
        }

        public void DrawRect(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, drawRect, null, color, MathHelper.ToRadians(rotation), origin, SpriteEffects.None, 0);
        }

        public enum MovementDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        /// <summary>
        /// Handles basic sprite movement
        /// </summary>
        /// <param name="keyboardState">Current keyboard state</param>
        /// <param name="speed">Speed you want the sprite to move at</param>
        /// <param name="movementDirection">Direction you want the sprite to move in</param>
        /// <param name="key">Key you want to associate with that direction</param>
        public void Move(KeyboardState keyboardState, float speed, MovementDirection movementDirection, Keys key)
        {
            if (movementDirection == MovementDirection.Up)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    pos.Y -= speed;
                }
            }
            if (movementDirection == MovementDirection.Down)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    pos.Y += speed;
                }
            }
            if (movementDirection == MovementDirection.Left)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    pos.X -= speed;
                }
            }
            if (movementDirection == MovementDirection.Right)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    pos.X += speed;
                }
            }
        }

        void UpdateTexAndColorData(SpriteSheet spriteSheet, GraphicsDevice graphicsDevice)
        {
            // Unset the graphics device before setting new graphics data
            graphicsDevice.Textures[0] = null;
            tex.SetData<Color>(spriteSheet.GetFrameColorData(animations.sourceRect).colorData1D);
            colorData = spriteSheet.GetFrameColorData(animations.sourceRect);
        }

        /// <summary>
        /// Used for pixel perfect collision.
        /// </summary>
        /// <param name="rectangle">The current bounding rectangle</param>
        /// <param name="transform">The current sprite transform matrix</param>
        /// <returns>Returns a new bounding rectangle</returns>
        Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
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
