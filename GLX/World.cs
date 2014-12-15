using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GLX
{
    public sealed class World
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Random random = new Random();

        public static KeyboardState previousKeyboardState;
        public static GamePadState[] previousGamePadStates = new GamePadState[4];
        public static MouseState previousMouseState;

        public static KeyboardState keyboardState;
        public static GamePadState[] gamePadStates = new GamePadState[4];
        public static MouseState mouseState;

        public static List<Action> thingsToDo;

        public Dictionary<string, GameState> gameStates;
        public List<KeyValuePair<string, GameState>> activeGameStates;

        public Dictionary<string, MenuState> menuStates;
        public List<KeyValuePair<string, MenuState>> activeMenuStates;

        private GameState currentGameState;

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            gameStates = new Dictionary<string, GameState>();
            activeGameStates = new List<KeyValuePair<string, GameState>>();
            menuStates = new Dictionary<string, MenuState>();
            activeMenuStates = new List<KeyValuePair<string, MenuState>>();
            thingsToDo = new List<Action>();
            for (int i = 0; i < gamePadStates.Length; i++)
            {
                gamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }

        public void AddGameState(string name, GraphicsDeviceManager graphics)
        {
            gameStates.Add(name, new GameState(name, graphics));
        }

        public void AddMenuState(string name, GraphicsDeviceManager graphics, Game game)
        {
            menuStates.Add(name, new MenuState(name, graphics, game, this));
        }

        public void ActivateGameState(string name)
        {
            foreach (KeyValuePair<string, GameState> state in activeGameStates)
            {
                if (state.Key == name)
                {
                    return;
                }
            }
            activeGameStates.Add(new KeyValuePair<string, GameState>(name, gameStates[name]));
        }

        public void ActivateMenuState(string name)
        {
            foreach (KeyValuePair<string, MenuState> state in activeMenuStates)
            {
                if (state.Key == name)
                {
                    return;
                }
            }
            activeMenuStates.Add(new KeyValuePair<string, MenuState>(name, menuStates[name]));
        }

        public void ClearStates()
        {
            activeGameStates.Clear();
            activeMenuStates.Clear();
        }

        public void LoadSpriteBatch()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            gamePadStates[0] = GamePad.GetState(PlayerIndex.One);
            gamePadStates[1] = GamePad.GetState(PlayerIndex.Two);
            gamePadStates[2] = GamePad.GetState(PlayerIndex.Three);
            gamePadStates[3] = GamePad.GetState(PlayerIndex.Four);
            mouseState = Mouse.GetState();
            foreach (KeyValuePair<string, GameState> gameState in activeGameStates)
            {
                gameState.Value.Update(gameTime);
            }
            foreach (KeyValuePair<string, MenuState> menuState in activeMenuStates)
            {
                menuState.Value.Update(gameTime);
            }
            foreach (Action action in thingsToDo)
            {
                action.Invoke();
            }
            thingsToDo.Clear();
            previousKeyboardState = keyboardState;
            previousGamePadStates = gamePadStates;
            previousMouseState = mouseState;
        }

        public void BeginDraw()
        {
            BeginDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                currentGameState.cameras[currentGameState.currentCamera].transform);
        }

        public void BeginDraw(SpriteSortMode spriteSortMode, BlendState blendState)
        {
            BeginDraw(spriteSortMode, blendState,
                null, null, null, null,
                currentGameState.cameras[currentGameState.currentCamera].transform);
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

        public void DrawWorld()
        {
            foreach (KeyValuePair<string, GameState> gameState in activeGameStates)
            {
                currentGameState = gameState.Value;
                foreach (Action drawMethod in gameState.Value.drawMethods)
                {
                    drawMethod.Invoke();
                }
            }
            foreach (KeyValuePair<string, MenuState> menuState in activeMenuStates)
            {
                currentGameState = menuState.Value;
                foreach (Action drawMethod in menuState.Value.drawMethods)
                {
                    drawMethod.Invoke();
                }
            }
        }
    }
}
