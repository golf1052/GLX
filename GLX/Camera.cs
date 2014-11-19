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
        private Matrix _transform;
        internal Matrix transform
        {
            get
            {
                UpdateTransform();
                return _transform;
            }
        }

        private Matrix _inverseTransform;
        public Matrix inverseTransform
        {
            get
            {
                return _inverseTransform;
            }
        }
        public Vector2Tweener pan { get; private set; }
        public FloatTweener zoom { get; private set; }
        public float rot;
        private Viewport viewport;
        public bool smoothZoom;
        public float targetZoom;
        public float zoomValue;

        public enum Focus
        {
            TopLeft,
            Center
        }

        private Focus _focus;
        public Focus focus
        {
            get
            {
                return _focus;
            }
            set
            {
                if (_focus == Focus.TopLeft)
                {
                    if (value == Focus.Center)
                    {
                        _focus = value;
                        pan.startingValue = new Vector2(pan.Value.X + viewport.Width / 2,
                            pan.Value.Y + viewport.Height / 2);
                        pan._value = pan.startingValue;
                        pan.targetValue = Vector2.Zero;
                    }
                }
                else if (_focus == Focus.Center)
                {
                    if (value == Focus.TopLeft)
                    {
                        _focus = value;
                        pan.startingValue = new Vector2(pan.Value.X - viewport.Width / 2,
                            pan.Value.Y - viewport.Height / 2);
                        pan._value = pan.startingValue;
                        pan.targetValue = Vector2.Zero;
                    }
                }
            }
        }

        public Camera(Viewport viewport, Focus focus)
        {
            pan = new Vector2Tweener();
            this.viewport = viewport;
            smoothZoom = false;
            zoom = new FloatTweener();
            zoom.Value = 1;
            targetZoom = 1;
            zoomValue = 0;
            rot = 0;
            this._focus = focus;
        }

        void UpdateTransform()
        {
            if (focus == Focus.Center)
            {
                _transform = Matrix.CreateTranslation(new Vector3(-pan.Value.X, -pan.Value.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rot)) *
                    Matrix.CreateScale(new Vector3(zoom.Value, zoom.Value, 1)) *
                    Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
            }
            else if (focus == Focus.TopLeft)
            {
                _transform = Matrix.CreateTranslation(new Vector3(-pan.Value.X, -pan.Value.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rot)) *
                    Matrix.CreateScale(new Vector3(zoom.Value, zoom.Value, 0));
            }
            _inverseTransform = Matrix.Invert(_transform);
        }

        public void Update(GameTimeWrapper gameTime)
        {
            pan.Update(gameTime);
            zoom.Update(gameTime);
            UpdateTransform();
        }

        public void CameraBeginSpriteBatch(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                transform);
        }
    }
}
