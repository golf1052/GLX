using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public sealed class World
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Random random = new Random();

        List<GameTimeWrapper> gameTimes;

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

        private List<GameState> gameStates;
        private string _gameState;
        public string gameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                bool foundGameState = false;
                foreach (GameState gameState in gameStates)
                {
                    if (gameState.name == value)
                    {
                        _gameState = value;
                        gameState.LoadGameSpeeds();
                        foundGameState = true;
                    }
                }

                if (foundGameState)
                {
                    foreach (GameState gameState in gameStates)
                    {
                        if (gameState.name != value)
                        {
                            gameState.SaveGameSpeeds();
                        }
                    }
                }
                else
                {
                    throw new Exception("That game state does not exist");
                }
            }
        }

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            gameTimes = new List<GameTimeWrapper>();

            cameras = new Dictionary<string, Camera>();
            _camera1 = new Camera(graphics.GraphicsDevice.Viewport, Camera.Focus.TopLeft);
            cameras.Add("camera1", camera1);
            _currentCamera = "camera1";

            gameStates = new List<GameState>();
        }

        public void AddGameStateUpdate(string name, GameTimeWrapper gameTime)
        {
            foreach (GameState gameState in gameStates)
            {
                if (gameState.name == name)
                {
                    gameState.gameTimes.Add(gameTime);
                    return;
                }
            }

            gameStates.Add(new GameState(name));
            gameStates.Last().gameTimes.Add(gameTime);
        }

        public void AddGameStateDraw(string name, Action<GameTime> drawMethod)
        {
            foreach (GameState gameState in gameStates)
            {
                if (gameState.name == name)
                {
                    gameState.drawMethods.Add(drawMethod);
                    return;
                }
            }

            gameStates.Add(new GameState(name));
            gameStates.Last().drawMethods.Add(drawMethod);
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

        public void BeginDraw(SpriteSortMode sortMode, BlendState blendState,
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
