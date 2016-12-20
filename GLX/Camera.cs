using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GLX
{
    public class Camera
    {
        private Matrix transform;
        internal Matrix Transform
        {
            get
            {
                UpdateTransform();
                return transform;
            }
        }

        private Matrix inverseTransform;
        public Matrix InverseTransform
        {
            get
            {
                return inverseTransform;
            }
        }
        public Vector2Tweener Pan { get; private set; }
        public FloatTweener Zoom { get; private set; }
        public FloatTweener Rotation { get; private set; }
        public Viewport viewport;

        public enum CameraFocus
        {
            TopLeft,
            Center
        }

        private CameraFocus focus;
        public CameraFocus Focus
        {
            get
            {
                return focus;
            }
            set
            {
                if (focus == CameraFocus.TopLeft)
                {
                    if (value == CameraFocus.Center)
                    {
                        focus = value;
                        Pan.startingValue = new Vector2(Pan.Value.X + viewport.Width / 2,
                            Pan.Value.Y + viewport.Height / 2);
                        Pan._value = Pan.startingValue;
                        Pan.targetValue = Vector2.Zero;
                    }
                }
                else if (focus == CameraFocus.Center)
                {
                    if (value == CameraFocus.TopLeft)
                    {
                        focus = value;
                        Pan.startingValue = new Vector2(Pan.Value.X - viewport.Width / 2,
                            Pan.Value.Y - viewport.Height / 2);
                        Pan._value = Pan.startingValue;
                        Pan.targetValue = Vector2.Zero;
                    }
                }
            }
        }

        public Camera(Viewport viewport, CameraFocus focus)
        {
            Pan = new Vector2Tweener();
            this.viewport = viewport;
            Zoom = new FloatTweener();
            Rotation = new FloatTweener();
            Zoom.Value = 1;
            Rotation.Value = 0;
            this.focus = focus;
        }

        private void UpdateTransform()
        {
            if (Focus == CameraFocus.Center)
            {
                transform = Matrix.CreateTranslation(new Vector3(-Pan.Value.X, -Pan.Value.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Value)) *
                    Matrix.CreateScale(new Vector3(Zoom.Value, Zoom.Value, 1)) *
                    Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
                viewport.X = (int)Pan.Value.X - viewport.Width / 2;
                viewport.Y = (int)Pan.Value.Y - viewport.Height / 2;
            }
            else if (Focus == CameraFocus.TopLeft)
            {
                transform = Matrix.CreateTranslation(new Vector3(-Pan.Value.X, -Pan.Value.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Value)) *
                    Matrix.CreateScale(new Vector3(Zoom.Value, Zoom.Value, 0));
                viewport.X = (int)Pan.Value.X;
                viewport.Y = (int)Pan.Value.Y;
            }
            inverseTransform = Matrix.Invert(transform);
        }

        public void Update()
        {
            Pan.Update();
            Zoom.Update();
            Rotation.Update();
            UpdateTransform();
        }

        public void Update(GameTimeWrapper gameTime)
        {
            Pan.Update(gameTime);
            Zoom.Update(gameTime);
            Rotation.Update(gameTime);
            UpdateTransform();
        }
    }
}
