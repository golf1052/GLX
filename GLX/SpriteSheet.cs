using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    /// <summary>
    /// Holds frame width and height info for a sprite sheet.
    /// </summary>
    public struct SpriteSheetInfo
    {
        /// <summary>
        /// The frame width.
        /// </summary>
        public readonly int frameWidth;

        /// <summary>
        /// The frame height.
        /// </summary>
        public readonly int frameHeight;

        /// <summary>
        /// Creates new sprite sheet info.
        /// </summary>
        /// <param name="frameWidth">The frame width.</param>
        /// <param name="frameHeight">The frame height.</param>
        public SpriteSheetInfo(int frameWidth, int frameHeight)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
        }
    }

    /// <summary>
    /// Holds information for a sprite sheet.
    /// </summary>
    public class SpriteSheet
    {
        /// <summary>
        /// The sprite sheet texture.
        /// </summary>
        public Texture2D tex;

        /// <summary>
        /// The sprite sheet color data.
        /// </summary>
        public ColorData colorData;

        /// <summary>
        /// A dictionary of frame color datas keyed by source rectangles.
        /// </summary>
        private Dictionary<Rectangle, ColorData> frameColorData;

        /// <summary>
        /// The sprite sheet info for this sprite sheet
        /// </summary>
        public SpriteSheetInfo info;

        /// <summary>
        /// The number of frames in this sprite sheet.
        /// </summary>
        public int frameCount;

        /// <summary>
        /// The number of columns in this sprite sheet.
        /// </summary>
        public int columns;

        /// <summary>
        /// The number of rows in this sprite sheet.
        /// </summary>
        public int rows;

        /// <summary>
        /// The direction the sprite sheet goes in (either left to right or top to bottom).
        /// </summary>
        public Direction direction;

        /// <summary>
        /// How long we should staty on each frame. Stored as ticks.
        /// </summary>
        internal long frameTimeTicks;

        /// <summary>
        /// How long we should stay on each frame. In milliseconds.
        /// </summary>
        public long frameTime
        {
            get
            {
                return frameTimeTicks / 10000;
            }
            set
            {
                frameTimeTicks = value * 10000;
            }
        }

        /// <summary>
        /// If this sprite sheet should loop.
        /// </summary>
        public bool loop;

        /// <summary>
        /// The actions we should run on each frame.
        /// </summary>
        internal List<List<Action>> frameActions;

        /// <summary>
        /// The actions we should run on each frame if time is going backwards.
        /// </summary>
        internal List<List<Action>> reverseFrameActions;

        /// <summary>
        /// Directions sprite sheets can be laid out in.
        /// </summary>
        public enum Direction
        {
            LeftToRight,
            TopToBottom
        }

        /// <summary>
        /// Creates a new sprite sheet
        /// </summary>
        /// <param name="loadedTex">The texture</param>
        /// <param name="info">The sprite sheet info</param>
        /// <param name="frameCount">The number of frames in this sprite sheet.</param>
        /// <param name="columns">The number of columns in this sprite sheet.</param>
        /// <param name="rows">The number of rows in this sprite sheet.</param>
        /// <param name="direction">The direction the sprite sheet goes in.</param>
        /// <param name="frameTime">How long we should stay on each frame.</param>
        /// <param name="loop">If this sprite sheet should loop.</param>
        public SpriteSheet(Texture2D loadedTex,
            SpriteSheetInfo info,
            int frameCount,
            int columns,
            int rows,
            Direction direction,
            long frameTime,
            bool loop)
        {
            tex = loadedTex;
            colorData = new ColorData(tex);
            this.info = info;
            this.frameCount = frameCount;
            this.columns = columns;
            this.rows = rows;
            this.direction = direction;
            this.frameTime = frameTime;
            this.loop = loop;

            frameActions = new List<List<Action>>();
            reverseFrameActions = new List<List<Action>>();
            for (int i = 0; i < frameCount; i++)
            {
                frameActions.Add(new List<Action>());
                reverseFrameActions.Add(new List<Action>());
            }
            frameColorData = new Dictionary<Rectangle, ColorData>();
            GenerateFrameColorData(columns, rows, direction);
        }

        /// <summary>
        /// Returns the color data for the given source rectangle.
        /// </summary>
        /// <param name="sourceRect">The source rectangle</param>
        /// <returns>The color data.</returns>
        public ColorData GetFrameColorData(Rectangle sourceRect)
        {
            return frameColorData[sourceRect];
        }

        /// <summary>
        /// Populates the frame color dictionary after we load in a sprite sheet.
        /// </summary>
        /// <param name="columns">The number of columns in this sprite sheet.</param>
        /// <param name="rows">The number of rows in this sprite sheet.</param>
        /// <param name="direction">The direction the sprite sheet goes in.</param>
        private void GenerateFrameColorData(int columns, int rows, Direction direction)
        {
            int framesCollected = 0;
            while (framesCollected < frameCount)
            {
                if (direction == Direction.LeftToRight)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            Rectangle rect = new Rectangle(j * info.frameWidth, i * info.frameHeight, info.frameWidth, info.frameHeight);
                            ColorData tmpColorData = new ColorData(info.frameWidth, info.frameHeight);
                            tex.GetData(0, rect, tmpColorData.colorData1D, 0, tmpColorData.colorData1D.Length);
                            frameColorData.Add(rect, tmpColorData);
                            framesCollected++;
                            if (framesCollected == frameCount)
                            {
                                break;
                            }
                        }
                    }
                }
                else if (direction == Direction.TopToBottom)
                {
                    for (int i = 0; i < columns; i++)
                    {
                        for (int j = 0; j < rows; j++)
                        {
                            Rectangle rect = new Rectangle(i * info.frameWidth, j * info.frameHeight, info.frameWidth, info.frameHeight);
                            ColorData tmpColorData = new ColorData(info.frameWidth, info.frameHeight);
                            tex.GetData(0, rect, tmpColorData.colorData1D, 0, tmpColorData.colorData1D.Length);
                            frameColorData.Add(rect, tmpColorData);
                            framesCollected++;
                            if (framesCollected == frameCount)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
