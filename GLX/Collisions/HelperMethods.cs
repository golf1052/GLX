using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GLX.Collisions
{
    public static class HelperMethods
    {
        public static MTV? Colliding(Polygon p1, Polygon p2)
        {
            Vector2? vector = null;
            float magnitude = float.MaxValue;
            List<Vector2> axes1 = p1.GetNormals();
            List<Vector2> axes2 = p2.GetNormals();

            foreach (var axis in axes1)
            {
                Projection proj1 = p1.Project(axis);
                Projection proj2 = p2.Project(axis);
                if (!proj1.Overlaps(proj2))
                {
                    return null;
                }
                else
                {
                    float mag = proj1.Overlap(proj2);
                    if (proj1.Contains(proj2) || proj2.Contains(proj1))
                    {
                        float mins = Math.Abs(proj1.min - proj2.min);
                        float maxs = Math.Abs(proj1.max - proj2.max);
                        if (mins < maxs)
                        {
                            mag += mins;
                        }
                        else
                        {
                            mag += maxs;
                        }
                    }
                    if (mag < magnitude)
                    {
                        magnitude = mag;
                        vector = axis;
                    }
                }
            }

            foreach (var axis in axes2)
            {
                Projection proj1 = p1.Project(axis);
                Projection proj2 = p2.Project(axis);
                if (!proj1.Overlaps(proj2) || Double.IsNaN(axis.X))
                {
                    return null;
                }
                else
                {
                    float mag = proj1.Overlap(proj2);
                    if (proj1.Contains(proj2) || proj2.Contains(proj1))
                    {
                        float mins = Math.Abs(proj1.min - proj2.min);
                        float maxs = Math.Abs(proj1.max - proj2.max);
                        if (mins < maxs)
                        {
                            mag += mins;
                        }
                        else
                        {
                            mag += maxs;
                        }
                    }
                    if (mag < magnitude)
                    {
                        magnitude = mag;
                        vector = axis;
                    }
                }
            }
            return new MTV(vector.Value, magnitude);
        }
    }
}
