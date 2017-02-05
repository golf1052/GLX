using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLX.Collisions
{
    public struct Projection
    {
        public float min;
        public float max;

        public Projection(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Overlaps(Projection p)
        {
            return !(min > p.max || p.min > max);
        }

        public float Overlap(Projection p)
        {
            if (Overlaps(p))
            {
                return Math.Min(max, p.max) - Math.Max(min, p.min);
            }
            return 0;
        }
    }
}
