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
        public int frameWidth;
        public int frameHeight;

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
        Dictionary<Rectangle, ColorData> frameColorData;
        public SpriteSheetInfo info;
        public int frameCount;
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

        public SpriteSheet(Texture2D loadedTex,
            SpriteSheetInfo info,
            int frameCount,
            long frameTime,
            bool loop)
        {
            tex = loadedTex;
            colorData = new ColorData(tex);
            this.info = info;
            this.frameCount = frameCount;
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
            GenerateFrameColorData();
        }

        public ColorData GetFrameColorData(Rectangle sourceRect)
        {
            return frameColorData[sourceRect];
        }

        void GenerateFrameColorData()
        {
            for (int i = 0; i < frameCount; i++)
            {
                Rectangle rect = new Rectangle(i * info.frameWidth, 0, info.frameWidth, info.frameHeight);
                ColorData tmpColorData = new ColorData(info.frameWidth, info.frameHeight);
                tex.GetData(0, rect, tmpColorData.colorData1D, 0, tmpColorData.colorData1D.Length);
                frameColorData.Add(rect, tmpColorData);
            }
        }
    }
}
