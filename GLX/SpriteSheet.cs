using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public struct SpriteSheetInfo
    {
        public readonly int frameWidth;
        public readonly int frameHeight;

        public SpriteSheetInfo(int frameWidth, int frameHeight)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
        }
    }

    public class SpriteSheet
    {
        public Texture2D tex;
        public ColorData colorData;
        private Dictionary<Rectangle, ColorData> frameColorData;
        public SpriteSheetInfo info;
        public int frameCount;
        public int columns;
        public int rows;
        public Direction direction;
        internal long frameTimeTicks;
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
        public bool loop;
        internal List<List<Action>> frameActions;
        internal List<List<Action>> reverseFrameActions;
        public enum Direction
        {
            LeftToRight,
            TopToBottom
        }

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

        public ColorData GetFrameColorData(Rectangle sourceRect)
        {
            return frameColorData[sourceRect];
        }

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
