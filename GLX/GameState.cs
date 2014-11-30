using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GLX
{
    public class GameState
    {
        public string name;
        public List<GameTimeWrapper> gameTimes;
        public List<Action<GameTime>> drawMethods;
        internal List<decimal> previousGameSpeeds;

        public GameState(string name)
        {
            gameTimes = new List<GameTimeWrapper>();
            drawMethods = new List<Action<GameTime>>();
            previousGameSpeeds = new List<decimal>();
        }

        internal void SaveGameSpeeds()
        {
            for (int i = 0; i < gameTimes.Count; i++)
            {
                previousGameSpeeds[i] = gameTimes[i].ActualGameSpeed;
                gameTimes[i].GameSpeed = 0;
            }
        }

        internal void LoadGameSpeeds()
        {
            for (int i = 0; i < gameTimes.Count; i++)
            {
                gameTimes[i].GameSpeed = previousGameSpeeds[i];
            }
        }
    }
}
