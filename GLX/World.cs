using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class World
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Random random = new Random();

        List<GameTimeWrapper> gameTimes;

        Dictionary<string, Camera> cameras;
        private string _currentCamera;
        public string currentCamera
        {
            get
            {
                return _currentCamera;
            }
            set
            {
                if (cameras.ContainsKey(value))
                {
                    _currentCamera = value;
                }
                else
                {
                    throw new Exception("That camera doesn't exist");
                }
            }
        }

        private Camera _camera1;
        public Camera camera1
        {
            get
            {
                return _camera1;
            }
        }
        public Vector2 cameraFocalPoint;

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            gameTimes = new List<GameTimeWrapper>();

            cameras = new Dictionary<string, Camera>();
            _camera1 = new Camera(graphics.GraphicsDevice.Viewport, Camera.Focus.TopLeft);
            AddCamera("camera1", camera1);
            _currentCamera = "camera1";
        }

        public void LoadSpriteBatch()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        public void AddTime(GameTimeWrapper time)
        {
            gameTimes.Add(time);
        }

        public bool AddCamera(string name, Camera camera)
        {
            if (cameras.ContainsKey(name))
            {
                return false;
            }
            else
            {
                cameras.Add(name, camera);
                return true;
            }
        }

        public bool RemoveCamera(string name)
        {
            if (name == "camera1")
            {
                throw new Exception("You cannot remove camera1");
            }
            else
            {
                if (cameras.ContainsKey(name))
                {
                    if (_currentCamera == name)
                    {
                        _currentCamera = "camera1";
                    }
                    cameras.Remove(name);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            cameras[currentCamera].Update();
            foreach (GameTimeWrapper time in gameTimes)
            {
                time.Update(gameTime);
            }
        }

        public void BeginDraw()
        {
            BeginDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                cameras[currentCamera].transform);
        }

        public void BeginDraw(SpriteSortMode spriteSortMode, BlendState blendState)
        {
            BeginDraw(spriteSortMode, blendState,
                null, null, null, null,
                cameras[currentCamera].transform);
        }

        void BeginDraw(SpriteSortMode sortMode, BlendState blendState,
            SamplerState samplerState, DepthStencilState depthStencilState,
            RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState,
                depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public void Draw(Action<SpriteBatch> drawMethod)
        {
            drawMethod.Invoke(spriteBatch);
        }

        public void EndDraw()
        {
            spriteBatch.End();
        }
    }
}
