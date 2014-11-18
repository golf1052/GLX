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

        public enum SmoothingType
        {
            Linear,
            Smoothstep,
            RecursiveLinear,
            RecursiveSmoothstep,
        }

        public bool smoothPan;
        public SmoothingType panSmoothingType;
        public float panSmoothingRate;
        private float panSmoothingValue;

        private Vector2 _focalPoint;
        public Vector2 focalPoint
        {
            get
            {
                return _focalPoint;
            }
            set
            {
                panSmoothingValue = 0;
                if (smoothPan)
                {
                    startingFocalPoint = _focalPoint;
                    targetFocalPoint = value;
                }
                else
                {
                    startingFocalPoint = value;
                    _focalPoint = value;
                    targetFocalPoint = value;
                }
            }
        }
        private Vector2 startingFocalPoint;
        private Vector2 targetFocalPoint;
        public float zoom;
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

        public Focus focus;

        public Camera(Viewport viewport, Focus focus)
        {
            smoothPan = false;
            panSmoothingType = SmoothingType.Linear;
            panSmoothingRate = 0.1f;
            panSmoothingValue = 0;
            this.viewport = viewport;
            focalPoint = Vector2.Zero;
            startingFocalPoint = Vector2.Zero;
            targetFocalPoint = Vector2.Zero;
            smoothZoom = false;
            zoom = 1;
            targetZoom = 1;
            zoomValue = 0;
            rot = 0;
            this.focus = focus;
        }

        void UpdateTransform()
        {
            if (focus == Focus.Center)
            {
                _transform = Matrix.CreateTranslation(new Vector3(-focalPoint.X, -focalPoint.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rot)) *
                    Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
            }
            else if (focus == Focus.TopLeft)
            {
                _transform = Matrix.CreateTranslation(new Vector3(-focalPoint.X, -focalPoint.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rot)) *
                    Matrix.CreateScale(new Vector3(zoom, zoom, 0));
            }
            _inverseTransform = Matrix.Invert(_transform);
        }

        public void Update()
        {
            if (smoothPan)
            {
                if (_focalPoint != targetFocalPoint)
                {
                    if (panSmoothingType != SmoothingType.RecursiveLinear &&
                        panSmoothingType != SmoothingType.RecursiveSmoothstep)
                    {
                        panSmoothingValue += panSmoothingRate;
                        if (panSmoothingType == SmoothingType.Linear)
                        {
                            _focalPoint = Vector2.Lerp(startingFocalPoint, targetFocalPoint, panSmoothingValue);
                        }
                        else if (panSmoothingType == SmoothingType.Smoothstep)
                        {
                            _focalPoint = Vector2.SmoothStep(startingFocalPoint, targetFocalPoint, panSmoothingValue);
                        }

                        if (panSmoothingValue >= 1)
                        {
                            _focalPoint = targetFocalPoint;
                            panSmoothingValue = 0;
                        }
                    }
                    else
                    {
                        if (panSmoothingType == SmoothingType.RecursiveLinear)
                        {
                            _focalPoint = Vector2.Lerp(_focalPoint, targetFocalPoint, panSmoothingRate);
                        }
                        else if (panSmoothingType == SmoothingType.RecursiveSmoothstep)
                        {
                            _focalPoint = Vector2.SmoothStep(_focalPoint, targetFocalPoint, panSmoothingRate);
                        }
                    }
                }
                else
                {
                    panSmoothingValue = 0;
                }
            }
            if (smoothZoom)
            {
                zoom = MathHelper.Lerp(zoom, targetZoom, zoomValue);
            }
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
