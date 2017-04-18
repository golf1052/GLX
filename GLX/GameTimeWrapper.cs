using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GLX
{
    /// <summary>
    /// Allows you to speed up or slow down the rate at which things happen in the game.
    /// </summary>
    /// <remarks>
    /// This is probably the most important class in the library. This class helps you control the rate of the game.
    /// In XNA, games are usually set to run at 60 frames per second.
    /// If you want to slow down the game you would need to do everything in the game half as fast, 30 frames per second.
    /// You could just change the rate that frames are drawn but that just makes the game feel strange due to the low draw rate.
    /// This class allows you to manipulate the update rate of a game while keeping the draw rate at 60 FPS (or whatever rate your game runs at).
    /// </remarks>
    public class GameTimeWrapper : GameTime
    {
        /// <summary>
        /// The Game.cs time between frames. Unit = Ticks. Usually set to 166667 AKA 60 FPS.
        /// </summary>
        private long systemSpeed;

        /// <summary>
        /// This time world's time between frames. Unit = Ticks.
        /// </summary>
        private long gameSpeed;

        /// <summary>
        /// The gameSpeed when this GameTime was orignally initalized.
        /// </summary>
        private readonly long originalGameSpeed;

        /// <summary>
        /// This time world's time specified as a ratio. gameSpeed / systemSpeed.
        /// </summary>
        /// <remarks>
        /// If the are the same value (166667 / 166667) that means the game is running at a speed of 1.0.
        /// If gameSpeed is larger than systemSpeed then the game will run faster. (333334 / 166667) means the game is running at a speed of 2.0.
        /// If gameSpeed is smaller than systemSpeed then the game will run slower. (83333 / 166667) means the game is running at a speed of 0.5.
        /// The slowest the game can run is (1 / 166667) or a speed of 0.00000599998.
        /// The fastest the game can run is (9223372036854775807 / 16667) which is 5.534 * 10^13. The gameSpeed value is the max value of a long.
        /// I have not tried running stuff at this speed. I have no idea why you would want a game to run that fast.
        /// </remarks>
        private decimal gameSpeedDecimal;

        /// <summary>
        /// This time world's time specified as a ratio.
        /// </summary>
        /// <remarks>
        /// This time world's time specified as a ratio.
        /// After setting a ratio we recalculate the _gameSpeed value
        /// Multiply desired ratio by systemSpeed then truncate that to a long to get gameSpeed
        /// Then get the new ratio by dividing new gameSpeed by the systemSpeed
        /// </remarks>
        public decimal GameSpeed
        {
            get
            {
                return gameSpeedDecimal;
            }
            set
            {
                gameSpeed = (long)(systemSpeed * value);
                gameSpeedDecimal = (decimal)gameSpeed / (decimal)systemSpeed;
            }
        }

        /// <summary>
        /// The update method that should be run.
        /// </summary>
        internal Action<GameTimeWrapper> UpdateMethod;

        /// <summary>
        /// The speed the game is actually running at. GameSpeed will always be between -1 and 1 due to the way update is handled.
        /// For more information read up on the <see cref="Update(GameTime)"/> method.
        /// </summary>
        public decimal ActualGameSpeed { get; private set; }

        private TimeSpan totalGameTime;

        /// <summary>
        /// From GameTime. How much time has passed since we started the game.
        /// </summary>
        public new TimeSpan TotalGameTime
        {
            get
            {
                return totalGameTime;
            }
            private set
            {
                totalGameTime = value;
            }
        }

        private TimeSpan elapsedGameTime;

        /// <summary>
        /// From GameTime. How much time has passed since we last called update. Should always be the same if running a fixed update rate game.
        /// </summary>
        public new TimeSpan ElapsedGameTime
        {
            get
            {
                return elapsedGameTime;
            }
            private set
            {
                elapsedGameTime = value;
            }
        }

        private bool isRunningSlowly;

        /// <summary>
        /// From GameTime. If we are runnin a fixed update rate game, are we calling update slower than we should be?
        /// </summary>
        public new bool IsRunningSlowly
        {
            get
            {
                return isRunningSlowly;
            }
            private set
            {
                isRunningSlowly = value;
            }
        }

        public bool NormalUpdate { get; set; }

        /// <summary>
        /// Creates a new game time with a given update method, game class, and speed this time should run at.
        /// </summary>
        /// <param name="updateMethod">The update method</param>
        /// <param name="game">The game class</param>
        /// <param name="gameSpeed">The game speed</param>
        public GameTimeWrapper(Action<GameTimeWrapper> updateMethod, Game game, decimal gameSpeed)
        {
            this.UpdateMethod = updateMethod;
            this.systemSpeed = game.TargetElapsedTime.Ticks;
            this.gameSpeed = game.TargetElapsedTime.Ticks;
            this.GameSpeed = gameSpeed;
            this.originalGameSpeed = this.gameSpeed;
            this.TotalGameTime = TimeSpan.Zero;
            this.ElapsedGameTime = TimeSpan.Zero;
            this.IsRunningSlowly = false;
            NormalUpdate = true;
        }

        /// <summary>
        /// Updates the game time
        /// </summary>
        /// <param name="gameTime">The XNA <see cref="GameTime"/></param>
        public void Update(GameTime gameTime)
        {
            long updateLoops = Math.Abs(gameSpeed / systemSpeed);
            long timeLeftOver = gameSpeed % systemSpeed;
            long realGameSpeed = gameSpeed;
            decimal realGameSpeedDecimal = gameSpeedDecimal;
            ActualGameSpeed = realGameSpeedDecimal;
            if (updateLoops > 0)
            {
                if (gameSpeedDecimal >= 0)
                {
                    gameSpeedDecimal = 1.0m;
                    gameSpeed = systemSpeed;
                }
                else
                {
                    gameSpeedDecimal = -1.0m;
                    gameSpeed = systemSpeed * -1;
                }

                for (long i = 0; i < updateLoops; i++)
                {
                    TotalGameTime += TimeSpan.FromTicks(gameSpeed);
                    ElapsedGameTime = TimeSpan.FromTicks(gameSpeed);
                    IsRunningSlowly = gameTime.IsRunningSlowly;
                    UpdateMethod.Invoke(this);
                }
            }
            if (timeLeftOver != 0)
            {
                gameSpeedDecimal = (decimal)timeLeftOver / (decimal)systemSpeed;
                gameSpeed = timeLeftOver;
                TotalGameTime += TimeSpan.FromTicks(gameSpeed);
                ElapsedGameTime = TimeSpan.FromTicks(gameSpeed);
                IsRunningSlowly = gameTime.IsRunningSlowly;
                UpdateMethod.Invoke(this);
            }
            gameSpeed = realGameSpeed;
            gameSpeedDecimal = realGameSpeedDecimal;
        }

        public void UpdateByIncrement(GameTime gameTime)
        {
            long updateLoops = Math.Abs(gameSpeed / systemSpeed);
            if (gameSpeed < systemSpeed)
            {
                if (gameSpeed != 0)
                {
                    updateLoops = Math.Abs(systemSpeed / originalGameSpeed);
                }
            }
            long timeLeftOver = gameSpeed % systemSpeed;
            long realGameSpeed = gameSpeed;
            decimal realGameSpeedDecimal = gameSpeedDecimal;
            ActualGameSpeed = realGameSpeedDecimal;
            if (updateLoops > 0)
            {
                //if (gameSpeedDecimal >= 0)
                //{
                //    gameSpeedDecimal = 1.0m;
                //    gameSpeed = systemSpeed;
                //}
                //else
                //{
                //    gameSpeedDecimal = -1.0m;
                //    gameSpeed = systemSpeed * -1;
                //}

                for (long i = 0; i < updateLoops; i++)
                {
                    TotalGameTime += TimeSpan.FromTicks(gameSpeed);
                    ElapsedGameTime = TimeSpan.FromTicks(gameSpeed);
                    IsRunningSlowly = gameTime.IsRunningSlowly;
                    UpdateMethod.Invoke(this);
                }
            }
            if (timeLeftOver != 0)
            {
                gameSpeedDecimal = (decimal)timeLeftOver / (decimal)systemSpeed;
                gameSpeed = timeLeftOver;
                TotalGameTime += TimeSpan.FromTicks(gameSpeed);
                ElapsedGameTime = TimeSpan.FromTicks(gameSpeed);
                IsRunningSlowly = gameTime.IsRunningSlowly;
                UpdateMethod.Invoke(this);
            }
            gameSpeed = realGameSpeed;
            gameSpeedDecimal = realGameSpeedDecimal;
        }
    }
}
