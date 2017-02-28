using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public static class DebugText
    {
        public enum Corner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public static List<TextItem> debugTexts;
        public static Corner corner;
        public static Vector2 position;
        public static float spacing;
        public static SpriteFont spriteFont;

        static DebugText()
        {
            debugTexts = new List<TextItem>();
        }

        public static void Initialize(SpriteFont spriteFont)
        {
            Initialize(spriteFont, Vector2.Zero, Corner.TopLeft, 0);
        }

        public static void Initialize(SpriteFont spriteFont, Vector2 pos, Corner corner, float spacing)
        {
            DebugText.spriteFont = spriteFont;
            DebugText.position = pos;
            DebugText.corner = corner;
            DebugText.spacing = spacing;
        }

        public static void Add(params TextItem[] items)
        {
            debugTexts.AddRange(items);
        }

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
        }
    }
}
