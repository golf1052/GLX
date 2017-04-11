using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    /// <summary>
    /// Holds the color data info for sprites and sprite sheets in 1D and 2D arrays
    /// </summary>
    /// <remarks>
    /// <see href="http://gamedev.stackexchange.com/a/46777">Found here.</see> This class allows for easy conversion between the 1D array given by XNA and a 2D array that is easier to manipulate.
    /// </remarks>
    public class ColorData
    {
        private int width;
        private int height;
        private Color[] _colorData1D;

        /// <summary>
        /// 1D array of color data
        /// </summary>
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

        /// <summary>
        /// 2D array of color data
        /// </summary>
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

        /// <summary>
        /// Creates a new ColorData using texture data
        /// </summary>
        /// <param name="tex"></param>
        public ColorData(Texture2D tex)
        {
            width = tex.Width;
            height = tex.Height;
            _colorData1D = new Color[tex.Width * tex.Height];
            _colorData2D = new Color[tex.Width, tex.Height];
            tex.GetData(colorData1D);
            OneDToTwoD();
        }

        /// <summary>
        /// Creates a new empty ColorData using the given width and height
        /// </summary>
        /// <param name="width">The color data width</param>
        /// <param name="height">The color data height</param>
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
