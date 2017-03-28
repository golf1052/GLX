using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GLX
{
    public class GameTimeWrapper : GameTime
    {
        // The Game.cs time between frames. Unit = Ticks. Usually set to 166667 AKA 60 FPS.
        private long systemSpeed;

        // This time world's time between frames. Unit = Ticks.
        private long gameSpeed;

        // This time world's time specified as a ratio. gameSpeed / systemSpeed.
        private decimal gameSpeedDecimal;

        // This time world's time specified as a ratio.
        // After setting a ratio we recalculate the _gameSpeed value
        // Multiply desired ratio by systemSpeed then truncate that to a long to get gameSpeed
        // Then get the new ratio by dividing new gameSpeed by the systemSpeed
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
        internal Action<GameTimeWrapper> UpdateMethod;

        public decimal ActualGameSpeed { get; private set; }

        TimeSpan totalGameTime;
        public new TimeSpan TotalGameTime
        {
            get
            {
                return totalGameTime;
            }
            set
            {
                totalGameTime = value;
            }
        }

        TimeSpan elapsedGameTime;
        public new TimeSpan ElapsedGameTime
        {
            get
            {
                return elapsedGameTime;
            }
            set
            {
                elapsedGameTime = value;
            }
        }

        bool isRunningSlowly;
        public new bool IsRunningSlowly
        {
            get
            {
                return isRunningSlowly;
            }
            set
            {
                isRunningSlowly = value;
            }
        }

        public bool NormalUpdate { get; set; }

        public GameTimeWrapper(Action<GameTimeWrapper> time, Game game, decimal gameSpeed)
        {
            this.UpdateMethod = time;
            this.systemSpeed = game.TargetElapsedTime.Ticks;
            this.gameSpeed = game.TargetElapsedTime.Ticks;
            this.GameSpeed = gameSpeed;
            this.TotalGameTime = TimeSpan.Zero;
            this.ElapsedGameTime = TimeSpan.Zero;
            this.IsRunningSlowly = false;
            NormalUpdate = true;
        }

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
                    updateLoops = Math.Abs(systemSpeed / gameSpeed);
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
