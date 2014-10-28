using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class TextItem : SpriteBase
    {
        public SpriteFont font;
        string _text;
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                textSize = font.MeasureString(_text);
            }
        }
        public Vector2 textSize { get; private set; }

        public TextItem(SpriteFont loadedFont, string spriteText)
        {
            font = loadedFont;
            text = spriteText;
            pos = Vector2.Zero;
            vel = Vector2.Zero;
            rect = new Rectangle((int)pos.X, (int)pos.Y, (int)textSize.X, (int)textSize.Y);
            visible = true;
            color = Color.White;
            alpha = 1.0f;
            rotation = 0.0f;
            scale = 1.0f;
            origin = new Vector2(textSize.X / 2, textSize.Y / 2);
        }

        public void Update(GameTimeWrapper gameTime, GraphicsDevice graphicsDevice)
        {
            pos += vel;
            rect = new Rectangle((int)pos.X, (int)pos.Y, (int)textSize.X, (int)textSize.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, pos, color, rotation, origin, scale, SpriteEffects.None, 0);
        }
    }
}
