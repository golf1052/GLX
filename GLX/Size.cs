﻿using System;
using Microsoft.Xna.Framework;

namespace GLX
{
    /// <summary>
    /// Holds size data.
    /// </summary>
    public struct Size : IEquatable<Size>
    {
        private static readonly Size zeroSize = new Size();

        /// <summary>
        /// The width.
        /// </summary>
        public float Width;

        /// <summary>
        /// The height.
        /// </summary>
        public float Height;

        /// <summary>
        /// A size of zero.
        /// </summary>
        public static Size Zero
        {
            get
            {
                return zeroSize;
            }
        }

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public Size(float value)
        {
            Width = value;
            Height = value;
        }

        public static implicit operator Vector2(Size s)
        {
            return s.ToVector2();
        }

        public static implicit operator Size(Vector2 v)
        {
            return new Size(v.X, v.Y);
        }

        public static Size operator +(Size a, Size b)
        {
            return new Size(a.Width + b.Width, a.Height + b.Height);
        }

        public static Size operator -(Size a, Size b)
        {
            return new Size(a.Width - b.Width, a.Height - b.Height);
        }

        public static Size operator *(Size a, Size b)
        {
            return new Size(a.Width * b.Width, a.Height * b.Height);
        }

        public static Size operator *(Size size, float scaleFactor)
        {
            return new Size(size.Width * scaleFactor, size.Height * scaleFactor);
        }

        public static Size operator /(Size a, Size b)
        {
            return new Size(a.Width / b.Width, a.Height / b.Height);
        }

        public static Size operator /(Size size, float divider)
        {
            return new Size(size.Width / divider, size.Height / divider);
        }

        public static bool operator ==(Size a, Size b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Size a, Size b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            return (obj is Size) && Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return ((Width == other.Width) && (Height == other.Height));
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() + Height.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Width: {Width} Height: {Height}}}";
        }

        public Vector2 ToVector2()
        {
            return new Vector2(Width, Height);
        }

        public Point ToPoint()
        {
            return new Point((int)Width, (int)Height);
        }
    }
}
