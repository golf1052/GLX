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
                        Pan = new Vector2(Pan.X + virtualResolutionRenderer.VirtualResolution.X / 2,
                            Pan.Y + virtualResolutionRenderer.VirtualResolution.Y / 2);
                    }
                }
                else if (focus == CameraFocus.Center)
                {
                    if (value == CameraFocus.TopLeft)
                    {
                        focus = value;
                        Pan = new Vector2(Pan.X - virtualResolutionRenderer.VirtualResolution.X / 2,
                            Pan.Y - virtualResolutionRenderer.VirtualResolution.Y / 2);
                    }
                }
            }
        }

        private VirtualResolutionRenderer virtualResolutionRenderer;
        private Vector3 cameraTranslationVector;
        private Vector3 cameraScaleVector;
        private Vector3 resolutionTranslationVector;
        private Matrix cameraTranslationMatrix;
        private Matrix cameraRotationMatrix;
        private Matrix cameraScaleMatrix;
        private Matrix resolutionTranslationMatrix;

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
            cameraTranslationVector.X = -Pan.X;
            cameraTranslationVector.Y = -Pan.Y;

            cameraTranslationMatrix = Matrix.CreateTranslation(cameraTranslationVector);
            cameraRotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));

            cameraScaleVector = new Vector3(zoom, zoom, 1);

            cameraScaleMatrix = Matrix.CreateScale(cameraScaleVector);

            resolutionTranslationVector = new Vector3(virtualResolutionRenderer.VirtualResolution.X * 0.5f,
                virtualResolutionRenderer.VirtualResolution.Y * 0.5f,
                0);

            resolutionTranslationMatrix = Matrix.CreateTranslation(resolutionTranslationVector);
            if (Focus == CameraFocus.Center)
            {
                transform = cameraTranslationMatrix *
                    cameraRotationMatrix *
                    cameraScaleMatrix *
                    resolutionTranslationMatrix *
                    virtualResolutionRenderer.GetTransformationMatrix() *
                    Matrix.CreateTranslation(
                        new Vector3(virtualResolutionRenderer.VirtualResolution.X * 0.5f,
                        virtualResolutionRenderer.VirtualResolution.Y * 0.5f,
                        0));
            }
            else if (Focus == CameraFocus.TopLeft)
            {
                transform = cameraTranslationMatrix *
                    cameraRotationMatrix *
                    cameraScaleMatrix *
                    resolutionTranslationMatrix *
                    virtualResolutionRenderer.GetTransformationMatrix();
            }
            inverseTransform = Matrix.Invert(transform);
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
