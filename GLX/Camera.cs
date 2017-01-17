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

        private Vector2Tweener pan;
        public Vector2Tweener Pan
        {
            get
            {
                return pan;
            }
            private set
            {
                pan = value;
                isViewTransformDirty = true;
            }
        }
        private FloatTweener zoom;
        public FloatTweener Zoom
        {
            get
            {
                return zoom;
            }
            private set
            {
                zoom = value;
                isViewTransformDirty = true;
            }
        }
        private FloatTweener rotation;
        public FloatTweener Rotation
        {
            get
            {
                return rotation;
            }
            private set
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
                        Pan.startingValue = new Vector2(Pan.Value.X + virtualResolutionRenderer.VirtualResolution.X / 2,
                            Pan.Value.Y + virtualResolutionRenderer.VirtualResolution.Y / 2);
                        Pan._value = Pan.startingValue;
                        Pan.targetValue = Vector2.Zero;
                    }
                }
                else if (focus == CameraFocus.Center)
                {
                    if (value == CameraFocus.TopLeft)
                    {
                        focus = value;
                        Pan.startingValue = new Vector2(Pan.Value.X - virtualResolutionRenderer.VirtualResolution.X / 2,
                            Pan.Value.Y - virtualResolutionRenderer.VirtualResolution.Y / 2);
                        Pan._value = Pan.startingValue;
                        Pan.targetValue = Vector2.Zero;
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
            Pan = new Vector2Tweener();
            Zoom = new FloatTweener();
            Rotation = new FloatTweener();
            Zoom.Value = 1;
            Rotation.Value = 0;
            this.focus = focus;
        }

        private void UpdateTransform()
        {
            cameraTranslationVector.X = -Pan.Value.X;
            cameraTranslationVector.Y = -Pan.Value.Y;

            cameraTranslationMatrix = Matrix.CreateTranslation(cameraTranslationVector);
            cameraRotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Value));

            cameraScaleVector = new Vector3(zoom.Value, zoom.Value, 1);

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
                        1));
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
