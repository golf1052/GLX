using System;
using Microsoft.Xna.Framework;

namespace GLX
{
    /// <summary>
    /// The base class for interpolation between two values.
    /// </summary>
    /// <remarks>
    /// There are three classes that currently implement this base class.
    /// <see cref="FloatTweener"/>, <see cref="Vector2Tweener"/>, and <see cref="ColorTweener"/>.
    /// These classes contain an update function that calls a interpolation function on their respective float fields.
    /// </remarks>
    public abstract class TweenerBase
    {
        /// <summary>
        /// If smoothing is active.
        /// </summary>
        public bool smoothingActive;

        /// <summary>
        /// Smoothing types.
        /// </summary>
        public enum SmoothingType
        {
            Linear,
            Smoothstep,
            RecursiveLinear,
            RecursiveSmoothStep
        }

        /// <summary>
        /// The smoothing type we should use.
        /// </summary>
        public SmoothingType smoothingType;

        /// <summary>
        /// The rate we should smooth at.
        /// </summary>
        public float smoothingRate;

        protected float smoothingValue;

        /// <summary>
        /// Sets up the tweener.
        /// </summary>
        public TweenerBase()
        {
            smoothingActive = false;
            smoothingType = SmoothingType.Linear;
            smoothingRate = 0.1f;
            smoothingValue = 0;
        }

        /// <summary>
        /// Updates the tweener.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Updates the tweener.
        /// </summary>
        /// <param name="gameTime">The game time this tweener is in.</param>
        public abstract void Update(GameTimeWrapper gameTime);

        /// <summary>
        /// Interpolation wrapper.
        /// </summary>
        /// <param name="startingValue">The starting value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="amount">The amount we've tweened. Between 0 and 1.</param>
        /// <param name="tweenerFunction">The tweener function.</param>
        /// <returns>The output value.</returns>
        public float TweenerWrapper(float startingValue, float targetValue, float amount, Func<float, float, float, float> tweenerFunction)
        {
            return tweenerFunction.Invoke(startingValue, targetValue, amount);
        }

        /// <summary>
        /// Interpolation wrapper.
        /// </summary>
        /// <param name="startingValue">The starting value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="amount">The amount we've tweened. Between 0 and 1.</param>
        /// <param name="tweenerFunction">The tweener function.</param>
        /// <returns>The output value.</returns>
        public Vector2 TweenerWrapper(Vector2 startingValue, Vector2 targetValue, float amount, Func<float, float, float, float> tweenerFunction)
        {
            return new Vector2(tweenerFunction.Invoke(startingValue.X, targetValue.X, amount),
                tweenerFunction.Invoke(startingValue.Y, targetValue.Y, amount));
        }

        /// <summary>
        /// Interpolation wrapper.
        /// </summary>
        /// <param name="startingValue">The starting value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="amount">The amount we've tweened. Between 0 and 1.</param>
        /// <param name="tweenerFunction">The tweener function.</param>
        /// <returns>The output value.</returns>
        public Color TweenerWrapper(Color startingValue, Color targetValue, float amount, Func<float, float, float, float> tweenerFunction)
        {
            return new Color(tweenerFunction.Invoke(startingValue.ToVector4().X, targetValue.ToVector4().X, amount),
                tweenerFunction.Invoke(startingValue.ToVector4().Y, targetValue.ToVector4().Y, amount),
                tweenerFunction.Invoke(startingValue.ToVector4().Z, targetValue.ToVector4().Z, amount),
                tweenerFunction.Invoke(startingValue.ToVector4().W, targetValue.ToVector4().W, amount));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="startingValue">The starting value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="amount">The amount we've tweened. Between 0 and 1.</param>
        /// <returns>The output value.</returns>
        public float Linear(float startingValue, float targetValue, float amount)
        {
            return MathHelper.Lerp(startingValue, targetValue, amount);
        }

        /// <summary>
        /// Smoothstep interpolation.
        /// </summary>
        /// <param name="startingValue">The starting value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="amount">The amount we've tweened. Between 0 and 1.</param>
        /// <returns>The output value.</returns>
        public float Smoothstep(float startingValue, float targetValue, float amount)
        {
            return MathHelper.SmoothStep(startingValue, targetValue, amount);
        }
    }
}
