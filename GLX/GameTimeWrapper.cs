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
        long systemSpeed;
        long gameSpeed;
        decimal _gameSpeed;
        public decimal GameSpeed
        {
            get
            {
                return _gameSpeed;
            }
            set
            {
                gameSpeed = (long)(systemSpeed * value);
                _gameSpeed =  (decimal)gameSpeed / (decimal)systemSpeed;
            }
        }
        internal Action<GameTimeWrapper> time;

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

        public GameTimeWrapper(Action<GameTimeWrapper> time, Game game, decimal gameSpeed)
        {
            this.time = time;
            this.systemSpeed = game.TargetElapsedTime.Ticks;
            this.gameSpeed = game.TargetElapsedTime.Ticks;
            this.GameSpeed = gameSpeed;
            this.TotalGameTime = TimeSpan.Zero;
            this.ElapsedGameTime = TimeSpan.Zero;
            this.IsRunningSlowly = false;
        }

        void Loop(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime)
        {
            TotalGameTime += TimeSpan.FromTicks(gameSpeed);
            ElapsedGameTime = TimeSpan.FromTicks(gameSpeed);
            IsRunningSlowly = gameTime.IsRunningSlowly;
        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
