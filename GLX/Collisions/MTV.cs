using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GLX.Collisions
{
    /// <summary>
    /// Minimum Translation Vector
    /// </summary>
    public struct MTV
    {
        public readonly Vector2 vector;
        public readonly float magnitude;

        public MTV(Vector2 vector, float magnitude)
        {
            this.vector = vector;
            this.magnitude = magnitude;
        }
    }
}
