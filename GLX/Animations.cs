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
                        throw new GLXException("That animation does not exist.");
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

        /// <summary>
        /// Sets the animation to run one frame
        /// </summary>
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

        /// <summary>
        /// Creates a new sprite sheet
        /// </summary>
        /// <param name="spriteSheet">The texture the sprite sheet is referencing</param>
        /// <param name="frameCount">The number of frames</param>
        /// <param name="frameTime">The time between frames. In milliseconds.</param>
        /// <param name="loop">If the animation should loop</param>
        /// <returns>A sprite sheet</returns>
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

        /// <summary>
        /// Creates a new sprite sheet
        /// </summary>
        /// <param name="spriteSheet">The texture the sprite sheet is referencing</param>
        /// <param name="frameCount">The number of frames</param>
        /// <param name="columns">The number of columns in the sprite sheet</param>
        /// <param name="rows">The number of rows in the sprite sheet</param>
        /// <param name="direction">The direction the sprite sheet goes in (either left to right or top to bottom)</param>
        /// <param name="frameTime">The time between frames. In milliseconds.</param>
        /// <param name="loop">If the animation should loop</param>
        /// <returns>A sprite sheet</returns>
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

        /// <summary>
        /// Creates a new sprite sheet
        /// </summary>
        /// <param name="spriteSheet">The texture the sprite sheet is referencing</param>
        /// <param name="info">The sprite sheet info</param>
        /// <param name="frameCount">The number of frames</param>
        /// <param name="columns">The number of columns in the sprite sheet</param>
        /// <param name="rows">The number of rows in the sprite sheet</param>
        /// <param name="direction">The direction the sprite sheet goes in (either left to right or top to bottom)</param>
        /// <param name="frameTime">The time between frames. In milliseconds.</param>
        /// <param name="loop">If the animation should loop</param>
        /// <returns>A sprite sheet</returns>
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

        /// <summary>
        /// Sets an action on a frame
        /// </summary>
        /// <param name="animation">The animation name to set the action on</param>
        /// <param name="frameNumber">The frame of the animation to set the action on</param>
        /// <param name="action">The action to set</param>
        public void SetFrameAction(string animation, int frameNumber, Action action)
        {
            if (spriteSheets.ContainsKey(animation))
            {
                spriteSheets[animation].frameActions[frameNumber].Add(action);
            }
            else
            {
                throw new GLXException("That animation does not exist.");
            }
        }

        /// <summary>
        /// Sets an action on a reverse frame (for when the animation plays in reverse)
        /// </summary>
        /// <param name="animation">The animation name to set the action on</param>
        /// <param name="frameNumber">The frame of the animation to set the action on</param>
        /// <param name="action">The action to set</param>
        public void SetReverseFrameAction(string animation, int frameNumber, Action action)
        {
            if (spriteSheets.ContainsKey(animation))
            {
                spriteSheets[animation].reverseFrameActions[frameNumber].Add(action);
            }
            else
            {
                throw new GLXException("That animation does not exist.");
            }
        }
    }
}
