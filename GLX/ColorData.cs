using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class ColorData
    {
        int width;
        int height;
        Color[] _colorData1D;
        public Color[] colorData1D
        {
            get
            {
                return _colorData1D;
            }
            set
            {
                _colorData1D = value;
                OneDToTwoD();
            }
        }
        Color[,] _colorData2D;
        public Color[,] colorData2D
        {
            get
            {
                return _colorData2D;
            }
            set
            {
                _colorData2D = value;
                TwoDToOneD();
            }
        }

        public ColorData(Texture2D tex)
        {
            width = tex.Width;
            height = tex.Height;
            _colorData1D = new Color[tex.Width * tex.Height];
            _colorData2D = new Color[tex.Width, tex.Height];
            tex.GetData(colorData1D);
            OneDToTwoD();
        }

        public ColorData(int width, int height)
        {
            this.width = width;
            this.height = height;
            _colorData1D = new Color[width * height];
            _colorData2D = new Color[width, height];
        }

        internal void OneDToTwoD()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorData2D[x, y] = colorData1D[x + (y * width)];
                }
            }
        }

        internal void TwoDToOneD()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorData1D[x + (y * width)] = colorData2D[x, y];
                }
            }
        }
    }
}
