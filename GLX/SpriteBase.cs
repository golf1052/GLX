using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GLX
{
    public abstract class SpriteBase
    {
        public Vector2 pos;
        public Vector2 vel;
        public bool visible;
        public Rectangle rect;
        public Color color;
        public Vector2 origin;
        public float alpha;
        /// <summary>
        /// Rotation of Sprite in degrees
        /// </summary>
        public float rotation;
        public float scale;

        public SpriteBase()
        {
            SpriteInit();
        }

        void SpriteInit()
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

        public virtual void Update()
        {
            pos += vel;
        }

        public abstract void Draw(SpriteBatch spriteBatch);

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

        public void Aim(Vector2 targetPosition)
        {
            float XDistance = targetPosition.X - pos.X;
            float YDistance = targetPosition.Y - pos.Y;
            float angle = (float)Math.Atan2(YDistance, XDistance);
            rotation = MathHelper.ToDegrees(angle);
        }

        public void Aim(MouseState mouseState)
        {
            float XDistance = mouseState.X - pos.X;
            float YDistance = mouseState.Y - pos.Y;
            float angle = (float)Math.Atan2(YDistance, XDistance);
            rotation = MathHelper.ToDegrees(angle);
        }

        public enum ThumbStick
        {
            Left,
            Right
        }

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
