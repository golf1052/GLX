using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class VirtualResolutionRenderer
    {
        private GraphicsDeviceManager graphics;
        private Vector2 ratio;
        private bool dirtyMatrix;
        private static Matrix scaleMatrix;
        private float scale;

        public Viewport Viewport { get; private set; }
        public Color BackgroundColor { get; set; }
        private Vector2 virtualResolution;
        public Vector2 VirtualResolution
        {
            get
            {
                return virtualResolution;
            }
            set
            {
                virtualResolution = value;
                SetupVirtualScreenViewport();
                ratio = new Vector2(Viewport.Width / VirtualResolution.X, Viewport.Height / VirtualResolution.Y);
                dirtyMatrix = true;
            }
        }
        public Vector2 WindowResolution { get; private set; }

        public VirtualResolutionRenderer(GraphicsDeviceManager graphics) : this(graphics, new Vector2(1920, 1080))
        {
        }

        public VirtualResolutionRenderer(GraphicsDeviceManager graphics, Vector2 virtualResolution)
        {
            this.graphics = graphics;
            this.virtualResolution = virtualResolution;
            BackgroundColor = Color.CornflowerBlue;
            WindowResolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            SetupVirtualScreenViewport();
            ratio = new Vector2(Viewport.Width / VirtualResolution.X, Viewport.Height / VirtualResolution.Y);
            dirtyMatrix = true;
        }

        public void BeginDraw()
        {
            // Start by resetting viewport to (0, 0, 1, 1)
            SetupFullViewport();
            // Then clear the screen
            graphics.GraphicsDevice.Clear(BackgroundColor);
            // Then calculate proper viewport according to aspect ratio
            SetupVirtualScreenViewport();
        }

        public void SetupFullViewport()
        {
            Viewport viewport = new Viewport(0, 0, (int)WindowResolution.X, (int)WindowResolution.Y);
            graphics.GraphicsDevice.Viewport = viewport;
            dirtyMatrix = true;
        }

        public void SetupVirtualScreenViewport()
        {
            Vector2 scale = new Vector2(WindowResolution.X / VirtualResolution.X,
                WindowResolution.Y / VirtualResolution.Y);
            this.scale = Math.Min(scale.X, scale.Y);
            float targetAspectRatio = VirtualResolution.X / VirtualResolution.Y;
            float width = (int)(VirtualResolution.X * this.scale);
            float height = (int)(VirtualResolution.Y * this.scale);

            Viewport = new Viewport((int)((WindowResolution.X / 2) - (width / 2)),
                (int)((WindowResolution.Y / 2) - (height / 2)),
                (int)width,
                (int)height);
            graphics.GraphicsDevice.Viewport = Viewport;
        }

        public Matrix GetTransformationMatrix()
        {
            if (dirtyMatrix)
            {
                RecreateScaleMatrix();
            }

            return scaleMatrix;
        }

        private void RecreateScaleMatrix()
        {
            scaleMatrix = Matrix.CreateScale(scale, scale, 1);
            dirtyMatrix = false;
        }
    }
}
