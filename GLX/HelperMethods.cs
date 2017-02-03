using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GLX
{
    public static class HelperMethods
    {
        /// <summary>
        /// Creates a Vector3 from a Vector2 with 0 as its Z value
        /// </summary>
        /// <param name="vector">The Vector2</param>
        /// <returns>A Vector3</returns>
        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector, 0);
        }

        /// <summary>
        /// Calculate the projection of vector a onto vector b
        /// </summary>
        /// <param name="a">Initial vector</param>
        /// <param name="b">Vector that is being projected onto</param>
        /// <returns>The projection of vector a onto vector b</returns>
        public static Vector2 Projection(this Vector2 a, Vector2 b)
        {
            Vector2 projection = new Vector2();
            projection.X = Vector2.Dot(a, b) / b.LengthSquared() * b.X;
            projection.Y = Vector2.Dot(a, b) / b.LengthSquared() * b.Y;
            return projection;
        }

        public static Vector2 LeftNormal(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Vector2 RightNormal(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static bool IsKeyDownAndUp(this KeyboardState keyboardState, Keys key, KeyboardState previousKeyboardState)
        {
            return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }

        public static Vector2 Intersection(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
        {
            float ua = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
            float ub = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);
            float denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);
            Vector2 intersectionPoint = new Vector2(float.MaxValue, float.MaxValue);
            if (Math.Abs(denominator) <= 0.00001f)
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    intersectionPoint = (point1 + point2) / 2;
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    intersectionPoint.X = point1.X + ua * (point2.X - point1.X);
                    intersectionPoint.Y = point1.Y + ua * (point2.Y - point1.Y);
                }
            }
            if (intersectionPoint.X != float.MaxValue)
            {
                return intersectionPoint;
            }
            else
            {
                return new Vector2(float.NaN, float.NaN);
            }
        }

        public static bool ContainsLine(this Rectangle rectangle, Line line)
        {
            Vector2 along = new Vector2(line.point1.X - line.point2.X, line.point1.Y -line.point2.Y);
            along.Normalize();
            for (int i = 0; i < Vector2.Distance(line.point1, line.point2); i++)
            {
                if (rectangle.Contains(line.point1 + (along * i)))
                {
                    return true;
                }
            }
            return false;
        }

        public static Vector2 PointOnLine(this Rectangle rectangle, Line line)
        {
            Vector2 along = new Vector2(line.point1.X - line.point2.X, line.point1.Y - line.point2.Y);
            along.Normalize();
            for (int i = 0; i < Vector2.Distance(line.point1, line.point2); i++)
            {
                if (rectangle.Contains(line.point1 + (along * i)))
                {
                    return line.point1 + (along * i);
                }
            }
            return new Vector2(float.NaN, float.NaN);
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
    }
}
