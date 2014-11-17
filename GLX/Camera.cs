using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class Camera
    {
        public Matrix transform;
        public Matrix inverseTransform;
        public Vector2 focalPoint;
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
            this.viewport = viewport;
            focalPoint = Vector2.Zero;
            smoothZoom = false;
            zoom = 1;
            targetZoom = 1;
            zoomValue = 0;
            rot = 0;
            this.focus = focus;
        }

        public void Update()
        {
            if (smoothZoom)
            {
                zoom = MathHelper.Lerp(zoom, targetZoom, zoomValue);
            }
            if (focus == Focus.Center)
            {
                transform = Matrix.CreateTranslation(new Vector3(-focalPoint.X, -focalPoint.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rot)) *
                    Matrix.CreateScale(new Vector3(zoom, zoom, 0)) *
                    Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
            }
            else if (focus == Focus.TopLeft)
            {
                transform = Matrix.CreateTranslation(new Vector3(-focalPoint.X, -focalPoint.Y, 0)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rot)) *
                    Matrix.CreateScale(new Vector3(zoom, zoom, 0));
            }
            inverseTransform = Matrix.Invert(transform);
        }

        public void CameraBeginSpriteBatch(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                transform);
        }
    }
}
