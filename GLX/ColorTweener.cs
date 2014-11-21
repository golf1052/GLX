using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GLX
{
    public class ColorTweener : TweenerBase
    {
        internal Color startingValue;
        internal Color _value;
        internal Color targetValue;
        public Color Value
        {
            get
            {
                return _value;
            }
            set
            {
                smoothingValue = 0;
                if (smoothingActive)
                {
                    startingValue = _value;
                    targetValue = value;
                }
                else
                {
                    startingValue = value;
                    _value = value;
                    targetValue = value;
                }
            }
        }

        public ColorTweener() : base()
        {
        }

        public override void Update(GameTimeWrapper gameTime)
        {
            if (smoothingActive)
            {
                if (_value != targetValue)
                {
                    if (smoothingType != SmoothingType.RecursiveLinear &&
                        smoothingType != SmoothingType.RecursiveSmoothStep)
                    {
                        smoothingValue += smoothingRate * (float)gameTime.GameSpeed;
                        if (smoothingType == SmoothingType.Linear)
                        {
                            _value = TweenerWrapper(startingValue, targetValue, smoothingValue, Linear);
                        }
                        else if (smoothingType == SmoothingType.Smoothstep)
                        {
                            _value = TweenerWrapper(startingValue, targetValue, smoothingValue, Smoothstep);
                        }

                        if (smoothingValue >= 1)
                        {
                            _value = targetValue;
                            smoothingValue = 0;
                        }
                    }
                    else
                    {
                        if (smoothingType == SmoothingType.RecursiveLinear)
                        {
                            _value = TweenerWrapper(_value, targetValue,
                                smoothingRate * (float)gameTime.GameSpeed, Linear);
                        }
                        else if (smoothingType == SmoothingType.RecursiveSmoothStep)
                        {
                            _value = TweenerWrapper(_value, targetValue,
                                smoothingRate * (float)gameTime.GameSpeed, Smoothstep);
                        }
                    }
                }
                else
                {
                    smoothingValue = 0;
                }
            }
        }
    }
}
