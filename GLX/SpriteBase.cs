using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public abstract class SpriteBase
    {
        public Vector2 pos;
        public Vector2 vel;
        public bool visible;
        public Rectangle rect;
        public Rectangle drawRect;
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
            drawRect = new Rectangle((int)pos.X, (int)pos.Y, 0, 0);
            color = Color.White;
            origin = new Vector2(0, 0);
            alpha = 1.0f;
            rotation = 0.0f;
            scale = 1.0f;
        }

        public virtual void Update()
        {
            pos += vel;
            drawRect.X = (int)pos.X;
            drawRect.Y = (int)pos.Y;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
