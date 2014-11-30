using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class Particle : Sprite
    {
        public TimeSpan aliveTime;
        public Color startingColor;
        public Color endingColor;
        public float colorShiftRate;
        public float colorShiftValue;
        public float velocityDecayRate;
        public float fadeRate;
        public float gravity;
        public bool hasGravity;
        public float bounce;
        private bool useDrawRect;

        public Particle(Texture2D loadedTex) : base(loadedTex)
        {
            visible = false;
            useDrawRect = false;
        }

        public Particle(GraphicsDeviceManager graphics) : base(graphics)
        {
            visible = false;
            useDrawRect = true;
        }

        private bool SpawnParticleBase(Vector2 position,
            Color color,
            Tuple<int, int> aliveTime,
            Tuple<float, float> velocity,
            Tuple<float, float> velocityDecayRate,
            Tuple<float, float> fadeRate,
            Tuple<float, float> colorShiftRate,
            float rotation,
            int spread,
            Color fadeTo,
            bool hasGravity,
            float bounce,
            float gravity = 0.0f)
        {
            if (!visible)
            {
                visible = true;
                alpha = 1.0f;
                vel = new Vector2((float)Math.Cos((MathHelper.ToRadians(rotation) +
                    MathHelper.ToRadians(World.random.Next(-spread, spread)))),
                    (float)Math.Sin((MathHelper.ToRadians(rotation) +
                    MathHelper.ToRadians(World.random.Next(-spread, spread))))) *
                    RandomBetween(velocity.Item1, velocity.Item2);
                this.velocityDecayRate = RandomBetween(velocityDecayRate.Item1, velocityDecayRate.Item2);
                this.fadeRate = RandomBetween(fadeRate.Item1, fadeRate.Item2);
                this.colorShiftRate = RandomBetween(colorShiftRate.Item1, colorShiftRate.Item2);
                startingColor = color;
                this.color = color;
                pos = position;
                this.aliveTime = TimeSpan.FromMilliseconds(World.random.Next(aliveTime.Item1, aliveTime.Item2));
                colorShiftValue = 1.0f;
                endingColor = fadeTo;
                this.bounce = bounce;
                this.hasGravity = hasGravity;
                if (hasGravity)
                {
                    this.gravity = gravity;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SpawnParticle(Vector2 position,
            Color color,
            Tuple<int, int> aliveTime,
            Tuple<float, float> velocity,
            Tuple<float, float> velocityDecayRate,
            Tuple<float, float> fadeRate,
            Tuple<float, float> colorShiftRate,
            float rotation,
            int spread,
            Color fadeTo,
            bool hasGravity,
            float bounce,
            float gravity = 0.0f)
        {
            return SpawnParticleBase(position,
                color,
                aliveTime,
                velocity,
                velocityDecayRate,
                fadeRate,
                colorShiftRate,
                rotation,
                spread,
                fadeTo,
                hasGravity,
                bounce,
                gravity);
        }

        public bool SpawnParticle(Vector2 position,
            Color color,
            Tuple<int, int> aliveTime,
            Tuple<int, int> size,
            Tuple<float, float> velocity,
            Tuple<float, float> velocityDecayRate,
            Tuple<float, float> fadeRate,
            Tuple<float, float> colorShiftRate,
            float rotation,
            int spread,
            Color fadeTo,
            bool hasGravity,
            float bounce,
            float gravity = 0.0f)
        {
            drawRect = new Rectangle((int)pos.X, (int)pos.Y, size.Item1, size.Item2);
            return SpawnParticleBase(position,
                color,
                aliveTime,
                velocity,
                velocityDecayRate,
                fadeRate,
                colorShiftRate,
                rotation,
                spread,
                fadeTo,
                hasGravity,
                bounce,
                gravity);
        }

        public override void Update(GameTimeWrapper gameTime,
            GraphicsDeviceManager graphics)
        {
            if (visible)
            {
                vel *= velocityDecayRate;
                if (hasGravity)
                {
                    vel.Y += gravity;
                }

                aliveTime -= gameTime.ElapsedGameTime;

                if (pos.Y > graphics.GraphicsDevice.Viewport.Height)
                {
                    pos.Y = graphics.GraphicsDevice.Viewport.Height - tex.Height / 2;
                    vel.Y *= -bounce;
                }

                if (aliveTime <= TimeSpan.Zero)
                {
                    if (colorShiftRate != 0)
                    {
                        if (colorShiftValue > 0)
                        {
                            colorShiftValue -= colorShiftRate * (float)gameTime.GameSpeed;
                            color = Color.Lerp(endingColor, startingColor, colorShiftValue);
                        }
                    }
                    else
                    {
                        colorShiftValue = 0;
                    }
                }

                if (colorShiftValue <= 0)
                {
                    if (fadeRate != 0)
                    {
                        alpha -= fadeRate * (float)gameTime.GameSpeed;
                        color *= alpha;
                    }
                    else
                    {
                        alpha = 0;
                    }
                }

                if (alpha <= 0)
                {
                    visible = false;
                }

                base.Update(gameTime, graphics);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                if (!useDrawRect)
                {
                    base.Draw(spriteBatch);
                }
                else
                {
                    base.DrawRect(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Gets a random value between two floats. The minimum number can also be negative!
        /// </summary>
        /// <param name="min">The smallest number you could get</param>
        /// <param name="max">The largest number you could get, not inclusive</param>
        /// <returns>Returns a random number of type float.</returns>
        private float RandomBetween(float min, float max)
        {
            return min + (float)World.random.NextDouble() * (max - min);
        }
    }
}
