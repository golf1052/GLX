using System;
using System.Collections.Generic;
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
        public SpriteSheetInfo info;
        public int frameCount;
        public int frameTime;
        public bool loop;

        public SpriteSheet(Texture2D loadedTex,
            SpriteSheetInfo info,
            int frameCount,
            int frameTime,
            bool loop)
        {
            tex = loadedTex;
            this.info = info;
            this.frameCount = frameCount;
            this.frameTime = frameTime;
            this.loop = loop;
        }

        public Color[] GetFrameColorData(Rectangle sourceRect)
        {
            Color[] frameColorData = new Color[info.frameWidth * info.frameHeight];
            tex.GetData(0, sourceRect, frameColorData, 0, frameColorData.Length);
            return frameColorData;
        }
    }
}
