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
        public enum CameraFocus
        {
            TopLeft,
            Center
        }

        private Matrix transform;
        internal Matrix Transform
        {
            get
            {
                UpdateTransform();
                return transform;
            }
        }
        private bool isViewTransformDirty;

        private Matrix inverseTransform;
        public Matrix InverseTransform
        {
            get
            {
                return inverseTransform;
            }
        }

        private Vector2 pan;
        public Vector2 Pan
        {
            get
            {
                return pan;
            }
            set
            {
                pan = value;
                isViewTransformDirty = true;
            }
        }
        private Vector2 origin;
        public Vector2 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
                isViewTransformDirty = true;
            }
        }
        private float zoom;
        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;
                isViewTransformDirty = true;
            }
        }
        private float rotation;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                isViewTransformDirty = true;
            }
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
                        Origin = new Vector2(virtualResolutionRenderer.VirtualResolution.Width / 2,
                            virtualResolutionRenderer.VirtualResolution.Height / 2);
                    }
                }
                else if (focus == CameraFocus.Center)
                {
                    if (value == CameraFocus.TopLeft)
                    {
                        focus = value;
                        Origin = Vector2.Zero;
                    }
                }
            }
        }

        internal VirtualResolutionRenderer virtualResolutionRenderer;

        public Camera(VirtualResolutionRenderer virtualResolutionRenderer, CameraFocus focus)
        {
            this.virtualResolutionRenderer = virtualResolutionRenderer;
            isViewTransformDirty = true;
            Pan = Vector2.Zero;
            Zoom = 1;
            Rotation = 0;
            this.focus = focus;
        }

        private void UpdateTransform()
        {
            Vector3 cameraTranslationVector = new Vector3(-Pan, 0);
            Matrix cameraTranslationMatrix = Matrix.CreateTranslation(cameraTranslationVector);

            Vector3 originTranslationVector = new Vector3(-Origin, 0);
            Matrix originTranslationMatrix = Matrix.CreateTranslation(originTranslationVector);

            Matrix cameraRotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));

            Vector3 cameraScaleVector = new Vector3(zoom, zoom, 1);
            Matrix cameraScaleMatrix = Matrix.CreateScale(cameraScaleVector);

            Vector3 resolutionTranslationVector = new Vector3(virtualResolutionRenderer.VirtualResolution.Width * 0.5f,
                virtualResolutionRenderer.VirtualResolution.Height * 0.5f,
                0);
            Matrix resolutionTranslationMatrix = Matrix.CreateTranslation(resolutionTranslationVector);

            if (Focus == CameraFocus.Center)
            {
                Vector3 reverseOriginTranslationVector = new Vector3(Origin, 0);
                Matrix reverseOriginTranslationMatrix = Matrix.CreateTranslation(reverseOriginTranslationVector);
                transform = cameraTranslationMatrix *
                    originTranslationMatrix *
                    cameraRotationMatrix *
                    cameraScaleMatrix *
                    virtualResolutionRenderer.GetTransformationMatrix() *
                    reverseOriginTranslationMatrix;
            }
            else if (Focus == CameraFocus.TopLeft)
            {
                transform = cameraTranslationMatrix *
                    originTranslationMatrix *
                    cameraRotationMatrix *
                    cameraScaleMatrix *
                    virtualResolutionRenderer.GetTransformationMatrix();
            }
            inverseTransform = Matrix.Invert(transform);
        }

        public Vector2 MouseToScreenCoords(Point mouseScreenPosition)
        {
            Vector2 screenPosition = new Vector2(mouseScreenPosition.X - virtualResolutionRenderer.Viewport.X, mouseScreenPosition.Y - virtualResolutionRenderer.Viewport.Y);
            return Vector2.Transform(screenPosition, inverseTransform);
        }

        public void Update()
        {
            UpdateTransform();
        }

        public void Update(GameTimeWrapper gameTime)
        {
            UpdateTransform();
        }
    }
}
