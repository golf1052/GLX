﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public sealed class World
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public VirtualResolutionRenderer virtualResolutionRenderer;

        public static Random random = new Random();

        public static List<Action> thingsToDo;

        public Dictionary<string, GameState> gameStates;
        public List<KeyValuePair<string, GameState>> activeGameStates;

        public Dictionary<string, MenuState> menuStates;
        public List<KeyValuePair<string, MenuState>> activeMenuStates;

        public GameState currentGameState;

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            virtualResolutionRenderer = new VirtualResolutionRenderer(graphics);
            gameStates = new Dictionary<string, GameState>();
            activeGameStates = new List<KeyValuePair<string, GameState>>();
            menuStates = new Dictionary<string, MenuState>();
            activeMenuStates = new List<KeyValuePair<string, MenuState>>();
            thingsToDo = new List<Action>();
        }

        public void AddGameState(string name)
        {
            AddGameState(name, graphics);
        }

        public void AddGameState(string name, GraphicsDeviceManager graphics)
        {
            gameStates.Add(name, new GameState(name, graphics, virtualResolutionRenderer));
        }

        public void AddMenuState(string name, Game game)
        {
            AddMenuState(name, graphics, game);
        }

        public void AddMenuState(string name, GraphicsDeviceManager graphics, Game game)
        {
            menuStates.Add(name, new MenuState(name, graphics, game, this, virtualResolutionRenderer));
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

        public void Update(GameTime gameTime)
        {
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
        }

        public void BeginDraw()
        {
            BeginDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                currentGameState.cameras[currentGameState.currentCamera].Transform);
        }

        public void BeginDraw(SpriteSortMode spriteSortMode, BlendState blendState)
        {
            BeginDraw(spriteSortMode, blendState,
                null, null, null, null,
                currentGameState.cameras[currentGameState.currentCamera].Transform);
        }

        public void BeginDraw(SpriteSortMode sortMode, BlendState blendState,
            SamplerState samplerState, DepthStencilState depthStencilState,
            RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            virtualResolutionRenderer.BeginDraw();
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
