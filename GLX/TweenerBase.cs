using System;
using Microsoft.Xna.Framework;

namespace GLX
{
    public abstract class TweenerBase
    {
        public bool smoothingActive;
        public enum SmoothingType
        {
            Linear,
            Smoothstep,
            RecursiveLinear,
            RecursiveSmoothStep
        }
        public SmoothingType smoothingType;
        public float smoothingRate;
        protected float smoothingValue;

        public TweenerBase()
        {
            smoothingActive = false;
            smoothingType = SmoothingType.Linear;
            smoothingRate = 0.1f;
            smoothingValue = 0;
        }

        public abstract void Update();

        public abstract void Update(GameTimeWrapper gameTime);

        public float TweenerWrapper(float startingValue, float targetValue, float amount, Func<float, float, float, float> tweenerFunction)
        {
            return tweenerFunction.Invoke(startingValue, targetValue, amount);
        }

        public Vector2 TweenerWrapper(Vector2 startingValue, Vector2 targetValue, float amount, Func<float, float, float, float> tweenerFunction)
        {
            return new Vector2(tweenerFunction.Invoke(startingValue.X, targetValue.X, amount),
                tweenerFunction.Invoke(startingValue.Y, targetValue.Y, amount));
        }

        public Color TweenerWrapper(Color startingValue, Color targetValue, float amount, Func<float, float, float, float> tweenerFunction)
        {
            return new Color(tweenerFunction.Invoke(startingValue.ToVector4().X, targetValue.ToVector4().X, amount),
                tweenerFunction.Invoke(startingValue.ToVector4().Y, targetValue.ToVector4().Y, amount),
                tweenerFunction.Invoke(startingValue.ToVector4().Z, targetValue.ToVector4().Z, amount),
                tweenerFunction.Invoke(startingValue.ToVector4().W, targetValue.ToVector4().W, amount));
        }

        public float Linear(float startingValue, float targetValue, float amount)
        {
            return MathHelper.Lerp(startingValue, targetValue, amount);
        }

        public float Smoothstep(float startingValue, float targetValue, float amount)
        {
            return MathHelper.SmoothStep(startingValue, targetValue, amount);
        }
    }
}
