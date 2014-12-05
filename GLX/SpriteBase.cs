using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GLX
{
    /// <summary>
    /// The SpriteBase class is the base class for all 2D drawable objects (sprites)
    /// This is an abstract class that contains a bunch of fields common to 2D drawable
    /// objects.
    /// </summary>
    public abstract class SpriteBase
    {
        /// <summary>
        /// Stores the position of the sprite.
        /// </summary>
        public Vector2 pos;

        /// <summary>
        /// Stores the velocity of the sprite.
        /// </summary>
        public Vector2 vel;

        /// <summary>
        /// Stores whether or not the sprite is visible.
        /// </summary>
        public bool visible;

        /// <summary>
        /// Stores the bounding rectangle of the sprite.
        /// </summary>
        public Rectangle rect;

        /// <summary>
        /// Stores the color of the sprite.
        /// </summary>
        public Color color;

        /// <summary>
        /// Stores the origin of the sprite.
        /// </summary>
        public Vector2 origin;

        /// <summary>
        /// Stores the sprite's transparancy value.
        /// </summary>
        public float alpha;

        /// <summary>
        /// Stores the totation of Sprite in degrees.
        /// </summary>
        public float rotation;

        /// <summary>
        /// Stores the scale of the sprite.
        /// </summary>
        public float scale;

        /// <summary>
        /// Creates a new instance of a sprite.
        /// </summary>
        public SpriteBase()
        {
            SpriteInit();
        }

        /// <summary>
        /// Base sprite instantiation. Sets fields to default values.
        /// </summary>
        private void SpriteInit()
        {
            pos = Vector2.Zero;
            vel = Vector2.Zero;
            visible = true;
            rect = new Rectangle(0, 0, 0, 0);
            color = Color.White;
            origin = new Vector2(0, 0);
            alpha = 1.0f;
            rotation = 0.0f;
            scale = 1.0f;
        }

        /// <summary>
        /// Base Update method. Should be overridden in almost all situations.
        /// </summary>
        public virtual void Update()
        {
            pos += vel;
        }

        /// <summary>
        /// Abstract Draw method.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for Game</param>
        public abstract void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// A movement direction, used for basic key inputs
        /// </summary>
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

        /// <summary>
        /// Rotates a sprite so that it is facing a certain position
        /// </summary>
        /// <param name="targetPosition">The position the sprite should point to</param>
        public void Aim(Vector2 targetPosition)
        {
            float XDistance = targetPosition.X - pos.X;
            float YDistance = targetPosition.Y - pos.Y;
            float angle = (float)Math.Atan2(YDistance, XDistance);
            rotation = MathHelper.ToDegrees(angle);
        }

        /// <summary>
        /// Rotates a sprite so that it faces the cursor
        /// </summary>
        /// <param name="mouseState">The mouse state</param>
        /// <param name="world">The current world</param>
        /// <remarks>We use the current world's camera to transform the mouse to the
        /// correct position on the screen.</remarks>
        public void Aim(MouseState mouseState, World world)
        {
            //Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), world.cameras[world.currentCamera].inverseTransform);
            //float XDistance = transformedMouseState.X - pos.X;
            //float YDistance = transformedMouseState.Y - pos.Y;
            //float angle = (float)Math.Atan2(YDistance, XDistance);
            //rotation = MathHelper.ToDegrees(angle);
        }

        public enum ThumbStick
        {
            Left,
            Right
        }

        /// <summary>
        /// Rotates a sprite so that it points in the direction the player is pointing
        /// their thumbstick
        /// </summary>
        /// <param name="gamePadState">The game pad state</param>
        /// <param name="thumbStick">The thumbstick the player is using for aiming</param>
        public void Aim(GamePadState gamePadState, ThumbStick thumbStick)
        {
            Vector2 stick = Vector2.Zero;
            if (thumbStick == ThumbStick.Left)
            {
                stick = gamePadState.ThumbSticks.Left;
            }
            else if (thumbStick == ThumbStick.Right)
            {
                stick = gamePadState.ThumbSticks.Right;
            }

            float x = (stick.X + 1);
            float y = (stick.Y + 1);

            if (gamePadState.ThumbSticks.Right != Vector2.Zero)
            {
                rotation = MathHelper.ToDegrees((float)Math.Atan2(-stick.Y, stick.X));
            }
        }
    }
}
