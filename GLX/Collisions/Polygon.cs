using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GLX.Collisions
{
    public class Polygon : Shape
    {
        public List<Vector2> vertices;

        public Polygon() : this(new List<Vector2>())
        {
        }

        public Polygon(List<Vector2> vertices)
        {
            this.vertices = vertices;
        }

        public List<Vector2> GetNormals()
        {
            List<Vector2> normals = new List<Vector2>();
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 p1 = vertices[i];
                Vector2 p2 = Vector2.Zero;
                if (i + 1 == vertices.Count)
                {
                    p2 = vertices[0];
                }
                else
                {
                    p2 = vertices[i + 1];
                }
                Vector2 edge = p1 - p2;

                normals.Add(Vector2.Normalize(edge.LeftNormal()));
            }
            return normals;
        }

        public Projection Project(Vector2 axis)
        {
            float min = Vector2.Dot(axis, vertices[0]);
            float max = min;

            for (int i = 1; i < vertices.Count; i++)
            {
                float p = Vector2.Dot(axis, vertices[i]);
                if (p < min)
                {
                    min = p;
                }
                else if (p > max)
                {
                    max = p;
                }
            }
            return new Projection(min, max);
        }
    }
}
