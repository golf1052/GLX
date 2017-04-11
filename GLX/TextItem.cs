using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    /// <summary>
    /// Allows for easier manipulation of text displayed on the scren. Basically treats text like a sprite.
    /// </summary>
    public class TextItem : SpriteBase
    {
        /// <summary>
        /// The sprite font this item is using.
        /// </summary>
        public SpriteFont font;

        private string _text;

        /// <summary>
        /// The text that should be displayed.
        /// </summary>
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

        /// <summary>
        /// The text size.
        /// </summary>
        public Vector2 textSize { get; private set; }

        /// <summary>
        /// Creates a new text item using a loaded sprite and the text that should be displayed.
        /// </summary>
        /// <param name="loadedFont">The sprite font.</param>
        /// <param name="spriteText">The text that should be displayed.</param>
        public TextItem(SpriteFont loadedFont, string spriteText = "")
        {
            font = loadedFont;
            text = spriteText;
            position = Vector2.Zero;
            velocity = Vector2.Zero;
            rectangle = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y),
                (int)Math.Round(textSize.X), (int)Math.Round(textSize.Y));
            visible = true;
            color = Color.White;
            alpha = 1.0f;
            rotation = 0.0f;
            scale = 1.0f;
            origin = new Vector2(textSize.X / 2, textSize.Y / 2);
        }

        /// <summary>
        /// Updates the text item.
        /// </summary>
        public override void Update()
        {
            position += velocity;
            rectangle = new Rectangle((int)position.X, (int)position.Y,
                (int)textSize.X, (int)textSize.Y);
        }

        /// <summary>
        /// Updates the text item.
        /// </summary>
        /// <param name="gameTime">The game time this text item is in.</param>
        public void Update(GameTimeWrapper gameTime)
        {
            position += velocity * (float)gameTime.GameSpeed;
            rectangle = new Rectangle((int)position.X, (int)position.Y,
                (int)textSize.X, (int)textSize.Y);
        }

        /// <summary>
        /// Draws the text item.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, position, color * alpha, rotation, origin, scale, SpriteEffects.None, 0);
        }
    }
}
