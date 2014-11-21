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
        public long elapsedTime;
        public int currentFrame;
        public Rectangle sourceRect;
        GameTimeWrapper gameTime;

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
                    ResetAnimation();
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

        public Animations(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime)
        {
            this.gameTime = gameTime;
            this.spriteSheetInfo = spriteSheetInfo;
            spriteSheets = new Dictionary<string, SpriteSheet>();
            ResetAnimation();
        }

        void ResetAnimation()
        {
            active = true;
            sourceRect = new Rectangle(0, 0, spriteSheetInfo.frameWidth, spriteSheetInfo.frameHeight);
            elapsedTime = 0;
            if (gameTime.GameSpeed > 0)
            {
                currentFrame = 0;
            }
            else
            {
                if (currentSpriteSheet != null)
                {
                    currentFrame = currentSpriteSheet.frameCount - 1;
                }
                else
                {
                    currentFrame = 0;
                }
            }
        }

        public SpriteSheet AddSpriteSheet(Texture2D spriteSheet,
            int frameCount,
            int columns,
            int rows,
            SpriteSheet.Direction direction,
            long frameTime,
            bool loop)
        {
            return new SpriteSheet(spriteSheet,
                spriteSheetInfo,
                frameCount,
                columns,
                rows,
                direction,
                frameTime,
                loop);
        }

        public void SetFrameAction(string animation, int frameNumber, Action action)
        {
            if (spriteSheets.ContainsKey(animation))
            {
                spriteSheets[animation].frameActions[frameNumber].Add(action);
            }
            else
            {
                throw new Exception("That animation does not exist.");
            }
        }

        public void SetReverseFrameAction(string animation, int frameNumber, Action action)
        {
            if (spriteSheets.ContainsKey(animation))
            {
                spriteSheets[animation].reverseFrameActions[frameNumber].Add(action);
            }
            else
            {
                throw new Exception("That animation does not exist.");
            }
        }
    }
}
