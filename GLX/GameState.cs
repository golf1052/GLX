using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class GameState
    {
        public string name;
        public List<GameTimeWrapper> gameTimes;
        public List<Action> drawMethods;

        internal Dictionary<string, Camera> cameras;
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

        public GameState(string name, GraphicsDeviceManager graphics)
        {
            gameTimes = new List<GameTimeWrapper>();
            drawMethods = new List<Action>();
            gameTimes = new List<GameTimeWrapper>();

            cameras = new Dictionary<string, Camera>();
            _camera1 = new Camera(graphics.GraphicsDevice.Viewport, Camera.Focus.TopLeft);
            cameras.Add("camera1", camera1);
            _currentCamera = "camera1";
        }

        public void AddTime(GameTimeWrapper time)
        {
            gameTimes.Add(time);
        }

        public bool AddCamera(string name, Camera camera)
        {
            if (cameras.ContainsKey(name) || name == "camera1")
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

        public void UpdateCurrentCamera(GameTimeWrapper gameTime)
        {
            cameras[currentCamera].Update(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameTimeWrapper time in gameTimes)
            {
                time.Update(gameTime);
            }
        }

        public void AddDraw(Action drawMethod)
        {
            drawMethods.Add(drawMethod);
        }
    }
}
