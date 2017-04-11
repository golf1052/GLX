using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    /// <summary>
    /// Makes it easy to display debug text on the screen
    /// </summary>
    public static class DebugText
    {
        /// <summary>
        /// Corners of the screen where debug text can be displayed
        /// </summary>
        public enum Corner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        /// <summary>
        /// The text items that should be displayed
        /// </summary>
        public static List<TextItem> debugTexts;

        private static List<TextItem> otherDebugTexts;

        /// <summary>
        /// The corner text items should be displayed in
        /// </summary>
        public static Corner corner;

        /// <summary>
        /// The position of text items relative to the corner
        /// </summary>
        public static Vector2 position;

        /// <summary>
        /// The spacing between each item
        /// </summary>
        public static float spacing;

        /// <summary>
        /// The sprite font to use when drawing items
        /// </summary>
        public static SpriteFont spriteFont;

        private static bool initialized;

        static DebugText()
        {
            debugTexts = new List<TextItem>();
            otherDebugTexts = new List<TextItem>();
            initialized = false;
        }

        /// <summary>
        /// Sets up DebugText. Should be called before using DebugText.
        /// </summary>
        /// <param name="spriteFont">The sprite font that should be used.</param>
        public static void Initialize(SpriteFont spriteFont)
        {
            Initialize(spriteFont, Vector2.Zero, Corner.TopLeft, 0);
        }

        /// <summary>
        /// Sets up DebugText. Should be called before using DebugText.
        /// </summary>
        /// <param name="spriteFont">The sprite font that should be used.</param>
        /// <param name="pos">The position of the text items relative to the corner.</param>
        /// <param name="corner">The corner text items should be displayed in.</param>
        /// <param name="spacing">The spacing between each item.</param>
        public static void Initialize(SpriteFont spriteFont, Vector2 pos, Corner corner, float spacing)
        {
            DebugText.spriteFont = spriteFont;
            DebugText.position = pos;
            DebugText.corner = corner;
            DebugText.spacing = spacing;
            initialized = true;
        }

        /// <summary>
        /// Adds the given TextItems to the list of drawn debug texts
        /// </summary>
        /// <param name="items"></param>
        public static void Add(params TextItem[] items)
        {
            debugTexts.AddRange(items);
        }

        public static void AddOther(string key, string text)
        {
            TextItem item = new TextItem(spriteFont, text);
            otherDebugTexts.Add(item);
            debugTexts.AddRange(otherDebugTexts);
        }

        /// <summary>
        /// Draws the debug texts
        /// </summary>
        /// <param name="spriteBatch">A sprite batch</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            Vector2 startPosition = position;

            if (corner == Corner.TopLeft)
            {
                foreach (TextItem textItem in debugTexts)
                {
                    textItem.position = startPosition;
                    startPosition.Y += textItem.textSize.Y + spacing;
                    textItem.origin = Vector2.Zero;
                    textItem.Draw(spriteBatch);
                }
            }
            else if (corner == Corner.TopRight)
            {
                foreach (TextItem textItem in debugTexts)
                {
                    textItem.position.X = startPosition.X - textItem.textSize.X;
                    textItem.position.Y = startPosition.Y;
                    startPosition.Y += textItem.textSize.Y + spacing;
                    textItem.origin = Vector2.Zero;
                    textItem.Draw(spriteBatch);
                }
            }
            else if (corner == Corner.BottomLeft)
            {
                foreach (TextItem textItem in debugTexts)
                {
                    textItem.position.X = startPosition.X;
                    textItem.position.Y = startPosition.Y - textItem.textSize.Y;
                    startPosition.Y = textItem.position.Y - spacing;
                    textItem.origin = Vector2.Zero;
                    textItem.Draw(spriteBatch);
                }
            }
            else if (corner == Corner.BottomRight)
            {
                foreach (TextItem textItem in debugTexts)
                {
                    textItem.position.X = startPosition.X - textItem.textSize.X;
                    textItem.position.Y = startPosition.Y - textItem.textSize.Y;
                    startPosition.Y = textItem.position.Y - spacing;
                    textItem.origin = Vector2.Zero;
                    textItem.Draw(spriteBatch);
                }
            }
            debugTexts.Clear();
            otherDebugTexts.Clear();
        }
    }
}
