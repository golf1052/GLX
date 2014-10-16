using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class Sprite
    {
        public Texture2D tex;
        public Vector2 pos;
        public Vector2 vel;
        public bool visible;
        public Rectangle rect;
        public Rectangle drawRect;
        public Color color;
        public float alpha;
        public Color[] colorData;
        public Vector2 origin;
        /// <summary>
        /// Rotation of Sprite in degrees
        /// </summary>
        public float rotation;
        public float scale;
        public Matrix spriteTransform;
        public bool isAnimated;
        public Animations animations;

        private bool ready;

        public Sprite(Texture2D loadedTex)
        {
            tex = loadedTex;
            SpriteBase();
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            colorData = new Color[tex.Width * tex.Height];
            tex.GetData(colorData);
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
        public Sprite(SpriteSheetInfo spriteSheetInfo)
        {
            SpriteBase();
            isAnimated = true;
            animations = new Animations(spriteSheetInfo);
            ready = false;
        }

        void SpriteBase()
        {
            pos = Vector2.Zero;
            vel = Vector2.Zero;
            visible = true;
            drawRect = new Rectangle((int)pos.X, (int)pos.Y, 0, 0);
            color = Color.White;
            alpha = 1.0f;
            rotation = 0.0f;
            scale = 1.0f;
        }

        public void Ready(GraphicsDevice graphicsDevice)
        {
            tex = new Texture2D(graphicsDevice, animations.spriteSheetInfo.frameWidth, animations.spriteSheetInfo.frameHeight);
            animations.currentSpriteSheet = animations.spriteSheets.First().Value;
            UpdateTexAndColorData(animations.currentSpriteSheet);
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
            ready = true;
        }

        public void Update(GameTime gameTime)
        {
            if (isAnimated)
            {
                UpdateAnimation(gameTime);
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

        void UpdateAnimation(GameTime gameTime)
        {
            if (animations.active)
            {
                animations.elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (animations.elapsedTime > animations.currentSpriteSheet.frameTime)
                {
                    animations.currentFrame++;
                    if (animations.currentFrame == animations.currentSpriteSheet.frameCount)
                    {
                        animations.currentFrame = 0;
                        if (!animations.currentSpriteSheet.loop)
                        {
                            animations.active = false;
                        }
                    }
                    animations.elapsedTime = 0;
                }
            }
            animations.sourceRect = new Rectangle(animations.currentFrame * animations.spriteSheetInfo.frameWidth,
                0,
                animations.spriteSheetInfo.frameWidth,
                animations.spriteSheetInfo.frameHeight);
            UpdateTexAndColorData(animations.currentSpriteSheet);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, pos, null, color, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
        }

        void UpdateTexAndColorData(SpriteSheet spriteSheet)
        {
            tex.SetData(spriteSheet.GetFrameColorData(animations.sourceRect));
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
