using System;
using System.Collections.Generic;
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
        private string currentAnimationName;

        internal bool runOneFrame;

        /// <summary>
        /// The current animation.
        /// </summary>
        /// <remarks>When set we first check if the animation exists. If it does set it up.
        /// If it does not throw an exception.</remarks>
        public string CurrentAnimationName
        {
            get
            {
                return currentAnimationName;
            }
            set
            {
                if (value == null)
                {
                    currentAnimationName = null;
                    currentSpriteSheet = null;
                    active = false;
                }
                else
                {
                    if (spriteSheets.ContainsKey(value))
                    {
                        currentAnimationName = value;
                        currentSpriteSheet = spriteSheets[currentAnimationName];
                        ResetAnimation();
                    }
                    else
                    {
                        throw new Exception("That animation does not exist.");
                    }
                }
            }
        }

        public SpriteSheet CurrentAnimation
        {
            get
            {
                return currentSpriteSheet;
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

        public Animations(SpriteSheetInfo spriteSheetInfo)
        {
            this.spriteSheetInfo = spriteSheetInfo;
            spriteSheets = new Dictionary<string, SpriteSheet>();
            ResetAnimation();
        }

        /// <summary>
        /// Create new animation storage
        /// </summary>
        /// <param name="spriteSheetInfo">The sprite sheet info for the animations</param>
        /// <param name="gameTime">The game time the sprite exists in</param>
        public Animations(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime) : this(spriteSheetInfo)
        {
            this.gameTime = gameTime;
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
                spriteSheetInfo = CurrentAnimation.info;
            }
            sourceRect = new Rectangle(0, 0, spriteSheetInfo.frameWidth, spriteSheetInfo.frameHeight);
            elapsedTime = 0;

            if (gameTime == null)
            {
                elapsedTime = 0;
                currentFrame = 0;
            }
            else
            {
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
