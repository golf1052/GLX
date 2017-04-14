using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GLX
{
    /// <summary>
    /// Holds information about and controls the world for a game.
    /// </summary>
    public sealed class World
    {
        public const string Camera1Name = "camera1";

        public static ContentManager<Texture2D> TextureManager;
        public static ContentManager<SpriteFont> FontManager;
        public static ContentManager<SoundEffect> SoundManager;
        public static ContentManager<Song> SongManager;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public VirtualResolutionRenderer virtualResolutionRenderer;

        /// <summary>
        /// A random number generator.
        /// </summary>
        public static Random random = new Random();

        public static List<Action> thingsToDo;

        /// <summary>
        /// Dictionary of game states. Keyed by the name of the game state.
        /// </summary>
        public Dictionary<string, GameState> gameStates;

        /// <summary>
        /// List of active game states.
        /// </summary>
        public List<KeyValuePair<string, GameState>> activeGameStates;

        /// <summary>
        /// Dictionary of menu states. Keyed by the name of the menu state.
        /// </summary>
        public Dictionary<string, MenuState> menuStates;

        /// <summary>
        /// List of active menu states.
        /// </summary>
        public List<KeyValuePair<string, MenuState>> activeMenuStates;

        private GameState currentGameState;

        public Dictionary<string, Camera> Cameras { get; private set; }
        private string currentCameraName;
        public string CurrentCameraName
        {
            get
            {
                return currentCameraName;
            }
            set
            {
                if (Cameras.ContainsKey(value))
                {
                    currentCameraName = value;
                    CurrentCamera = Cameras[CurrentCameraName];
                }
            }
        }
        public Camera CurrentCamera { get; private set; }

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="graphics">The graphics device manager.</param>
        /// <param name="Content">The main content manager</param>
        public World(GraphicsDeviceManager graphics, Microsoft.Xna.Framework.Content.ContentManager Content) : this(graphics, new VirtualResolutionRenderer(graphics), Content)
        {
        }

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="graphics">The graphics device manager</param>
        /// <param name="virtualResolutionRenderer">The custom virtual resolution renderer</param>
        /// <param name="Content">The main content manager</param>
        public World(GraphicsDeviceManager graphics, VirtualResolutionRenderer virtualResolutionRenderer, Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            this.virtualResolutionRenderer = virtualResolutionRenderer;
            gameStates = new Dictionary<string, GameState>();
            activeGameStates = new List<KeyValuePair<string, GameState>>();
            menuStates = new Dictionary<string, MenuState>();
            activeMenuStates = new List<KeyValuePair<string, MenuState>>();
            thingsToDo = new List<Action>();
            Cameras = new Dictionary<string, Camera>();

            TextureManager = new ContentManager<Texture2D>(Content);
            FontManager = new ContentManager<SpriteFont>(Content);
            SoundManager = new ContentManager<SoundEffect>(Content);
            SongManager = new ContentManager<Song>(Content);

            Cameras.Add(Camera1Name, new Camera(virtualResolutionRenderer, Camera.CameraFocus.TopLeft));
            currentCameraName = Camera1Name;
            CurrentCamera = Cameras[Camera1Name];
        }

        public void AddCamera(string name, Camera camera)
        {
            if (!Cameras.ContainsKey(name) && name != Camera1Name)
            {
                Cameras.Add(name, camera);
            }
        }

        public void RemoveCamera(string name)
        {
            if (name == Camera1Name)
            {
                throw new GLXException($"You cannot remove {Camera1Name}");
            }
            else
            {
                if (Cameras.ContainsKey(name))
                {
                    if (CurrentCameraName == name)
                    {
                        CurrentCameraName = Camera1Name;
                    }
                    Cameras.Remove(name);
                }
            }
        }

        public void UpdateCamera(string name, GameTimeWrapper gameTime)
        {
            if (Cameras.ContainsKey(name))
            {
                Cameras[name].Update(gameTime);
            }
        }

        public void UpdateCurrentCamera(GameTimeWrapper gameTime)
        {
            CurrentCamera.Update(gameTime);
        }

        /// <summary>
        /// Adds a game state to the list of game states.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        public void AddGameState(GameState gameState)
        {
            gameStates.Add(gameState.name, gameState);
        }

        /// <summary>
        /// Adds a game state to the list of game states.
        /// </summary>
        /// <param name="name">The name of the game state.</param>
        public void AddGameState(string name)
        {
            AddGameState(name, graphics);
        }

        /// <summary>
        /// Adds a game state to the list of game states.
        /// </summary>
        /// <param name="name">The name of the game state.</param>
        /// <param name="time">A game time that should be added to the game state.</param>
        public void AddGameState(string name, GameTimeWrapper time)
        {
            AddGameState(name, graphics, time);
        }

        /// <summary>
        /// Adds a game state to the list of game states.
        /// </summary>
        /// <param name="name">The name of the game state.</param>
        /// <param name="time">A game time that should be added to the game state.</param>
        /// <param name="drawMethod">A draw method that should be added to the game state.</param>
        public void AddGameState(string name, GameTimeWrapper time, Action drawMethod)
        {
            AddGameState(name, graphics, time, drawMethod);
        }

        /// <summary>
        /// Adds a game state to the list of game states.
        /// </summary>
        /// <param name="name">The name of the game state.</param>
        /// <param name="graphics">The graphics device manager.</param>
        public void AddGameState(string name, GraphicsDeviceManager graphics)
        {
            gameStates.Add(name, new GameState(name, graphics));
        }

        /// <summary>
        /// Adds a game state to the list of game states.
        /// </summary>
        /// <param name="name">The name of the game state.</param>
        /// <param name="graphics">The graphics device manager.</param>
        /// <param name="time">A game time that should be added to the game state.</param>
        public void AddGameState(string name, GraphicsDeviceManager graphics, GameTimeWrapper time)
        {
            GameState gameState = new GameState(name, graphics);
            gameState.AddTime(time);
            gameStates.Add(name, gameState);
        }

        /// <summary>
        /// Adds a game state to the list of game states.
        /// </summary>
        /// <param name="name">The name of the game state.</param>
        /// <param name="graphics">The graphics device manager.</param>
        /// <param name="time">A game time that should be added to the game state.</param>
        /// <param name="drawMethod">A draw method that should be added to the game state.</param>
        public void AddGameState(string name, GraphicsDeviceManager graphics, GameTimeWrapper time, Action drawMethod)
        {
            GameState gameState = new GameState(name, graphics);
            gameState.AddTime(time);
            gameState.AddDraw(drawMethod);
            gameStates.Add(name, gameState);
        }

        /// <summary>
        /// Adds a menu state to the list of menu states.
        /// </summary>
        /// <param name="menuState">The menu state.</param>
        public void AddMenuState(MenuState menuState)
        {
            menuStates.Add(menuState.name, menuState);
        }

        /// <summary>
        /// Adds a menu state to the list of menu states.
        /// </summary>
        /// <param name="name">The name of the menu state.</param>
        /// <returns>The newly created menu state.</returns>
        public MenuState AddMenuState(string name)
        {
            return AddMenuState(name, graphics);
        }

        /// <summary>
        /// Adds a menu state to the list of menu states.
        /// </summary>
        /// <param name="name">The name of the menu state.</param>
        /// <param name="graphics">The graphics device manager.</param>
        /// <returns>The newly created menu state.</returns>
        public MenuState AddMenuState(string name, GraphicsDeviceManager graphics)
        {
            MenuState menuState = new MenuState(name, graphics, this);
            menuStates.Add(name, menuState);
            return menuState;
        }

        /// <summary>
        /// Activates a game state so that it gets updated and drawn when the world is updated and drawn.
        /// </summary>
        /// <param name="name">The name of the game state to activate.</param>
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

        /// <summary>
        /// Deactivates a game state.
        /// </summary>
        /// <param name="name">The name of the game state to deactivate.</param>
        public void DeactivateGameState(string name)
        {
            for (int i = 0; i < activeGameStates.Count; i++)
            {
                if (activeGameStates[i].Key == name)
                {
                    activeGameStates.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Activates a menu state.
        /// </summary>
        /// <param name="name">The name of the menu state to activate.</param>
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

        /// <summary>
        /// Deactivates a menu state.
        /// </summary>
        /// <param name="name">The name of the menu state to deactivate.</param>
        public void DeactivateMenuState(string name)
        {
            for (int i = 0; i < activeMenuStates.Count; i++)
            {
                if (activeMenuStates[i].Key == name)
                {
                    activeMenuStates.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Clears both the activated game states and the activated menu states.
        /// </summary>
        public void ClearStates()
        {
            activeGameStates.Clear();
            activeMenuStates.Clear();
        }

        /// <summary>
        /// Updates the world.
        /// </summary>
        /// <param name="gameTime">The XNA <see cref="GameTime"/>.</param>
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

        /// <summary>
        /// Begins the sprite batch.
        /// </summary>
        public void BeginDraw()
        {
            BeginDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                CurrentCamera.Transform);
        }

        public void BeginDraw(SpriteSortMode spriteSortMode, BlendState blendState)
        {
            BeginDraw(spriteSortMode, blendState,
                null, null, null, null,
                CurrentCamera.Transform);
        }

        public void BeginDraw(SpriteSortMode sortMode, BlendState blendState,
            SamplerState samplerState, DepthStencilState depthStencilState,
            RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            CurrentCamera.virtualResolutionRenderer.BeginDraw();
            spriteBatch.Begin(sortMode, blendState, samplerState,
                depthStencilState, rasterizerState, effect, transformMatrix);
        }

        /// <summary>
        /// Draws the given draw method.
        /// </summary>
        /// <param name="drawMethod">The draw method.</param>
        public void Draw(Action<SpriteBatch> drawMethod)
        {
            drawMethod.Invoke(spriteBatch);
        }

        /// <summary>
        /// Ends the sprite batch.
        /// </summary>
        public void EndDraw()
        {
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the world.
        /// </summary>
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
