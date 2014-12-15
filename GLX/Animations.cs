using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    /// <summary>
    /// Animation class. Holds all the animation data for a sprite.
    /// </summary>
    public class Animations
    {
        /// <summary>
        /// The sprite sheet info
        /// </summary>
        public SpriteSheetInfo spriteSheetInfo;

        /// <summary>
        /// The dictionary of sprite sheets keyed by the name given to the sheet
        /// </summary>
        internal Dictionary<string, SpriteSheet> spriteSheets;

        /// <summary>
        /// Is the animation active
        /// </summary>
        public bool active;

        /// <summary>
        /// The time elapsed since we last changed frames. In ticks.
        /// </summary>
        public long elapsedTime;

        /// <summary>
        /// The current frame we are on. Zero based.
        /// </summary>
        public int currentFrame;

        /// <summary>
        /// The sprite sheet source rectangle
        /// </summary>
        public Rectangle sourceRect;

        /// <summary>
        /// The game time the sprite exists in
        /// </summary>
        private GameTimeWrapper gameTime;

        /// <summary>
        /// The current sprite sheet
        /// </summary>
        internal SpriteSheet currentSpriteSheet;
        
        /// <summary>
        /// The current animation
        /// </summary>
        private string _currentAnimation;

        internal bool runOneFrame;

        /// <summary>
        /// The current animation.
        /// </summary>
        /// <remarks>When set we first check if the animation exists. If it does set it up.
        /// If it does not throw an exception.</remarks>
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

        /// <summary>
        /// Indexer for adding new sprite sheets
        /// </summary>
        /// <param name="key">The name of the sprite sheet</param>
        /// <returns></returns>
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

        /// <summary>
        /// Create new animation storage
        /// </summary>
        /// <param name="spriteSheetInfo">The sprite sheet info for the animations</param>
        /// <param name="gameTime">The game time the sprite exists in</param>
        public Animations(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime)
        {
            this.gameTime = gameTime;
            this.spriteSheetInfo = spriteSheetInfo;
            spriteSheets = new Dictionary<string, SpriteSheet>();
            ResetAnimation();
        }

        public void RunOneFrame()
        {
            active = true;
            runOneFrame = true;
        }

        /// <summary>
        /// Reset the animation to the beginning
        /// </summary>
        private void ResetAnimation()
        {
            active = true;
            runOneFrame = false;
            if (spriteSheets.Count != 0)
            {
                spriteSheetInfo = spriteSheets[currentAnimation].info;
            }
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
            long frameTime,
            bool loop)
        {
            return new SpriteSheet(spriteSheet,
                spriteSheetInfo,
                frameCount,
                frameCount,
                1,
                SpriteSheet.Direction.LeftToRight,
                frameTime,
                loop);
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

        public SpriteSheet AddSpriteSheet(Texture2D spriteSheet,
            SpriteSheetInfo info,
            int frameCount,
            int columns,
            int rows,
            SpriteSheet.Direction direction,
            long frameTime,
            bool loop)
        {
            return new SpriteSheet(spriteSheet,
                info,
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
