using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    /// <summary>
    /// Holds information about a game state
    /// </summary>
    /// <remarks>
    /// GameState and <see cref="MenuState"/> provide an easy way to switch between states in game.
    /// Typically a game can have different parts and also have different menus.
    /// GameState and <see cref="MenuState"/> help make managing those states easy.
    /// Most of the things that used to be in the <see cref="World"/> class are now in the GameState class.
    /// </remarks>
    public class GameState
    {
        /// <summary>
        /// The name of the game state
        /// </summary>
        public string name;

        /// <summary>
        /// The list of game times in this state
        /// </summary>
        public List<GameTimeWrapper> gameTimes;

        /// <summary>
        /// The list of draw methods in this state
        /// </summary>
        public List<Action> drawMethods;

        /// <summary>
        /// Creates a new game state
        /// </summary>
        /// <param name="name">The name of the game state</param>
        /// <param name="graphics">The graphics device manager</param>
        public GameState(string name, GraphicsDeviceManager graphics)
        {
            this.name = name;
            gameTimes = new List<GameTimeWrapper>();
            drawMethods = new List<Action>();
            gameTimes = new List<GameTimeWrapper>();
        }

        /// <summary>
        /// Adds a game time to the list of game times
        /// </summary>
        /// <param name="time"></param>
        public void AddTime(GameTimeWrapper time)
        {
            gameTimes.Add(time);
        }

        /// <summary>
        /// Updates the game state
        /// </summary>
        /// <param name="gameTime">The XNA <see cref="GameTime"/></param>
        public void Update(GameTime gameTime)
        {
            foreach (GameTimeWrapper time in gameTimes)
            {
                if (time.NormalUpdate)
                {
                    time.Update(gameTime);
                }
                else
                {
                    time.UpdateByIncrement(gameTime);
                }
            }
        }

        /// <summary>
        /// Adds a draw method to the list of draw methods. The draw methods get called from first to last (index 0 to index N).
        /// </summary>
        /// <param name="drawMethod">The draw method</param>
        public void AddDraw(Action drawMethod)
        {
            drawMethods.Add(drawMethod);
        }
    }
}
