using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GLX
{
    /// <summary>
    /// Defines a 2D camera
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Camera focuses. Where the 0, 0 of the camera is.
        /// </summary>
        public enum CameraFocus
        {
            TopLeft,
            Center
        }

        private Matrix virtualTransform;
        private Matrix projectionTransform;
        private BoundingFrustum boundingFrustum;

        private Matrix transform;

        /// <summary>
        /// The camera transform matrix
        /// </summary>
        public Matrix Transform
        {
            get
            {
                UpdateVirtualTransform();
                UpdateTransform();
                return transform;
            }
        }
        private bool isViewTransformDirty;

        private Matrix inverseTransform;

        /// <summary>
        /// The camera inverse transform matrix
        /// </summary>
        public Matrix InverseTransform
        {
            get
            {
                return inverseTransform;
            }
        }

        private Vector2 pan;

        /// <summary>
        /// The camera pan (position).
        /// </summary>
        public Vector2 Pan
        {
            get
            {
                return pan;
            }
            set
            {
                if (Focus == CameraFocus.Center)
                {
                    pan = value;
                }
                else
                {
                    pan = value;
                }
                isViewTransformDirty = true;
            }
        }

        private Vector2 origin;

        /// <summary>
        /// The camera origin
        /// </summary>
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

        /// <summary>
        /// The camera zoom
        /// </summary>
        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                zoom = value;
                if (Focus == CameraFocus.Center)
                {
                    Origin = -(new Vector2(virtualResolutionRenderer.VirtualResolution.Width / 2,
                            virtualResolutionRenderer.VirtualResolution.Height / 2)) * (1 / zoom);
                }
                isViewTransformDirty = true;
            }
        }

        private float rotation;

        /// <summary>
        /// The camera rotation
        /// </summary>
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

        /// <summary>
        /// The camera focus
        /// </summary>
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
                        Origin = -(new Vector2(virtualResolutionRenderer.VirtualResolution.Width / 2,
                            virtualResolutionRenderer.VirtualResolution.Height / 2));
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

        /// <summary>
        /// Creates a new camera
        /// </summary>
        /// <param name="virtualResolutionRenderer">A virtual resolution renderer</param>
        /// <param name="focus">The camera focus</param>
        public Camera(VirtualResolutionRenderer virtualResolutionRenderer, CameraFocus focus)
        {
            this.virtualResolutionRenderer = virtualResolutionRenderer;
            isViewTransformDirty = true;
            Pan = Vector2.Zero;
            Zoom = 1;
            Rotation = 0;
            this.focus = focus;
        }

        private void UpdateVirtualTransform()
        {
            Vector3 cameraTranslationVector = new Vector3(-Pan, 0);
            Matrix cameraTranslationMatrix = Matrix.CreateTranslation(cameraTranslationVector);

            Vector3 originTranslationVector = new Vector3(-Origin, 0);
            Matrix originTranslationMatrix = Matrix.CreateTranslation(originTranslationVector);

            Matrix cameraRotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));

            Vector3 cameraScaleVector = new Vector3(zoom, zoom, 1);
            Matrix cameraScaleMatrix = Matrix.CreateScale(cameraScaleVector);

            if (Focus == CameraFocus.Center)
            {
                virtualTransform = cameraTranslationMatrix *
                    originTranslationMatrix *
                    cameraRotationMatrix *
                    cameraScaleMatrix;
            }
            else if (Focus == CameraFocus.TopLeft)
            {
                virtualTransform = cameraTranslationMatrix *
                    originTranslationMatrix *
                    cameraRotationMatrix *
                    cameraScaleMatrix;
            }
        }

        private void UpdateTransform()
        {
            transform = virtualTransform * virtualResolutionRenderer.GetTransformationMatrix();
            inverseTransform = Matrix.Invert(transform);
        }

        private void UpdateProjectionTransform()
        {
            projectionTransform = Matrix.CreateOrthographicOffCenter(0,
                virtualResolutionRenderer.VirtualResolution.Width,
                virtualResolutionRenderer.VirtualResolution.Height,
                0, -1, 0);
            Matrix.Multiply(ref virtualTransform, ref projectionTransform, out projectionTransform);
        }

        private void UpdateBoundingFrustum()
        {
            boundingFrustum = new BoundingFrustum(projectionTransform);
        }

        /// <summary>
        /// Converts a mouse point position into real screen coordinates
        /// </summary>
        /// <param name="mouseScreenPosition">The mouse point position</param>
        /// <returns>A Vector2 containing the mouse screen coordinates</returns>
        public Vector2 MouseToScreenCoords(Point mouseScreenPosition)
        {
            Vector2 screenPosition = new Vector2(mouseScreenPosition.X - virtualResolutionRenderer.Viewport.X, mouseScreenPosition.Y - virtualResolutionRenderer.Viewport.Y);
            return Vector2.Transform(screenPosition, inverseTransform);
        }

        /// <summary>
        /// Checks if the current camera view contanins the given point
        /// </summary>
        /// <param name="vector">The point to check</param>
        /// <returns>If the point is contained in the current camera view</returns>
        public bool Contains(Vector2 vector)
        {
            ContainmentType containmentType = boundingFrustum.Contains(vector.ToVector3());
            return !(containmentType == ContainmentType.Disjoint);
        }

        /// <summary>
        /// DOESN'T WORK: Checks if the current camera view contains or intersects with the given rectangle
        /// </summary>
        /// <param name="rectangle">The rectangle to check</param>
        /// <returns>If the rectangle is contained or intersects with the current camera view</returns>
        public bool Contains(Rectangle rectangle)
        {
            // doesn't look like it's working atm
            Vector3 min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            Vector3 max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            BoundingBox box = new BoundingBox(min, max);
            ContainmentType containmentType = boundingFrustum.Contains(boundingFrustum);
            return !(containmentType == ContainmentType.Disjoint);
        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        public void Update()
        {
            UpdateVirtualTransform();
            UpdateTransform();
            UpdateProjectionTransform();
            UpdateBoundingFrustum();
        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        /// <param name="gameTime">The game time the camera is in</param>
        public void Update(GameTimeWrapper gameTime)
        {
            UpdateVirtualTransform();
            UpdateTransform();
            UpdateProjectionTransform();
            UpdateBoundingFrustum();
        }
    }
}
