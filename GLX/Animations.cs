using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class Animations
    {
        public SpriteSheetInfo spriteSheetInfo;
        internal Dictionary<string, SpriteSheet> spriteSheets;
        public bool active;
        public int elapsedTime;
        public int currentFrame;
        public Rectangle sourceRect;

        internal SpriteSheet currentSpriteSheet;
        private string _currentAnimation;
        public string currentAnimation
        {
            get
            {
                return _currentAnimation;
            }
            set
            {
                if (spriteSheets.ContainsKey(value))
                {
                    _currentAnimation = value;
                    currentSpriteSheet = spriteSheets[_currentAnimation];
                }
                else
                {
                    throw new Exception("That animation does not exist.");
                }
            }
        }
        public SpriteSheet this[string key]
        {
            set
            {
                if (spriteSheets.ContainsKey(key))
                {
                    spriteSheets[key] = value;
                }
                else
                {
                    spriteSheets.Add(key, value);
                }
            }
        }

        public Animations(SpriteSheetInfo spriteSheetInfo)
        {
            this.spriteSheetInfo = spriteSheetInfo;
            spriteSheets = new Dictionary<string, SpriteSheet>();
            this.active = true;
            this.sourceRect = new Rectangle(0, 0, spriteSheetInfo.frameWidth, spriteSheetInfo.frameHeight);
            this.elapsedTime = 0;
            this.currentFrame = 0;
        }

        public SpriteSheet AddSpriteSheet(Texture2D spriteSheet, int frameCount, int frameTime, bool loop)
        {
            return new SpriteSheet(spriteSheet, spriteSheetInfo, frameCount, frameTime, loop);
        }
    }
}
