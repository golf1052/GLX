using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public struct ParticleOptions
    {
        public readonly Vector2 position;
        public readonly Tuple<int, int> size;
        public readonly Color startingColor;
        public readonly Color endingColor;
        public readonly Tuple<int, int> aliveTime;
        public readonly Tuple<float, float> velocity;
        public readonly Tuple<float, float> velocityDecayRate;
        public readonly Tuple<float, float> colorShiftRate;
        public readonly Tuple<float, float> fadeRate;
        public readonly float rotation;
        public readonly int spread;
        public readonly bool hasGravity;
        public readonly float gravity;
        public readonly float bounce;

        public ParticleOptions(Vector2 position,
            Color startingColor,
            Color endingColor,
            Tuple<int, int> aliveTime,
            Tuple<float, float> velocity,
            Tuple<float, float> velocityDecayRate,
            Tuple<float, float> colorShiftRate,
            Tuple<float, float> fadeRate,
            float rotation,
            int spread,
            bool hasGravity,
            float bounce,
            float gravity = 0.0f)
        {
            this.position = position;
            this.size = null;
            this.startingColor = startingColor;
            this.endingColor = endingColor;
            this.aliveTime = aliveTime;
            this.velocity = velocity;
            this.velocityDecayRate = velocityDecayRate;
            this.colorShiftRate = colorShiftRate;
            this.fadeRate = fadeRate;
            this.rotation = rotation;
            this.spread = spread;
            this.hasGravity = hasGravity;
            this.gravity = gravity;
            this.bounce = bounce;
        }

        public ParticleOptions(Vector2 position,
            Tuple<int, int> size,
            Color startingColor,
            Color endingColor,
            Tuple<int, int> aliveTime,
            Tuple<float, float> velocity,
            Tuple<float, float> velocityDecayRate,
            Tuple<float, float> colorShiftRate,
            Tuple<float, float> fadeRate,
            float rotation,
            int spread,
            bool hasGravity,
            float bounce,
            float gravity = 0.0f)
        {
            this.position = position;
            this.size = size;
            this.startingColor = startingColor;
            this.endingColor = endingColor;
            this.aliveTime = aliveTime;
            this.velocity = velocity;
            this.velocityDecayRate = velocityDecayRate;
            this.colorShiftRate = colorShiftRate;
            this.fadeRate = fadeRate;
            this.rotation = rotation;
            this.spread = spread;
            this.hasGravity = hasGravity;
            this.gravity = gravity;
            this.bounce = bounce;
        }
    }

    /// <summary>
    /// Particles!
    /// </summary>
    public class Particle : Sprite
    {
        /// <summary>
        /// How long the particle will stay alive before despawning
        /// </summary>
        public TimeSpan aliveTime;

        /// <summary>
        /// The initial color of the particle
        /// </summary>
        public Color startingColor;

        /// <summary>
        /// The color the particle should change to from the starting color
        /// </summary>
        public Color endingColor;

        /// <summary>
        /// The rate at which the particle will change from the starting color to the ending color
        /// </summary>
        public float colorShiftRate;

        /// <summary>
        /// How far along the particle is in shifting colors
        /// </summary>
        private float colorShiftValue;

        /// <summary>
        /// The rate at which the particle should slow down
        /// </summary>
        public float velocityDecayRate;

        /// <summary>
        /// The rate at which the particle should fade and then despawn
        /// </summary>
        public float fadeRate;

        /// <summary>
        /// The rate of gravity on the particle
        /// </summary>
        public float gravity;

        /// <summary>
        /// If the particle has gravity
        /// </summary>
        public bool hasGravity;

        /// <summary>
        /// The amount the particle should bounce
        /// </summary>
        public float bounce;

        /// <summary>
        /// If the particle was loaded from a texture or just a pixel
        /// </summary>
        private bool useDrawRect;

        /// <summary>
        /// Create a particle using a texture
        /// </summary>
        /// <param name="loadedTex">The particle texture</param>
        public Particle(Texture2D loadedTex) : base(loadedTex)
        {
            visible = false;
            useDrawRect = false;
        }

        /// <summary>
        /// Create a particle using a pixel
        /// </summary>
        /// <param name="graphics">The graphics device manager</param>
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

        /// <summary>
        /// Spawn a new particle, use if particle was loaded with a texture
        /// </summary>
        /// <param name="position">The initial position of the particle</param>
        /// <param name="color">The initial color of the particle</param>
        /// <param name="aliveTime">The minimum and maximum alive time of the particle, in milliseconds</param>
        /// <param name="velocity">The minimum and maximum initial velocity of the particle</param>
        /// <param name="velocityDecayRate">The minimum and maximum velocity decay rate. 0 = instant velocity decay. 1 = no velocity decay</param>
        /// <param name="fadeRate">The minimum and maximum fade to despawn rate. 0 = no fade. 1 = instant fade.</param>
        /// <param name="colorShiftRate">The minimum and maximum color shift rate. 0 = no shift. 1 = instant shift</param>
        /// <param name="rotation">The initial direction the particle should fire in. In degrees.</param>
        /// <param name="spread">The deviation in either direction from the rotation. In degrees.</param>
        /// <param name="fadeTo">The ending color of the particle</param>
        /// <param name="hasGravity">If the particle has gravity applied to it</param>
        /// <param name="bounce">How much the particle bounces</param>
        /// <param name="gravity">If the particle has gravity how much force should be applied</param>
        /// <returns>If the particle was spawned returns true</returns>
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

        /// <summary>
        /// Spawn a particle, use if particle was loaded with a pixel
        /// </summary>
        /// <param name="position">The initial position of the particle</param>
        /// <param name="color">The initial color of the particle</param>
        /// <param name="aliveTime">The minimum and maximum alive time of the particle, in milliseconds</param>
        /// <param name="size">The width and height of the particle</param>
        /// <param name="velocity">The minimum and maximum initial velocity of the particle</param>
        /// <param name="velocityDecayRate">The minimum and maximum velocity decay rate. 0 = instant velocity decay. 1 = no velocity decay</param>
        /// <param name="fadeRate">The minimum and maximum fade to despawn rate. 0 = no fade. 1 = instant fade.</param>
        /// <param name="colorShiftRate">The minimum and maximum color shift rate. 0 = no shift. 1 = instant shift</param>
        /// <param name="rotation">The initial direction the particle should fire in. In degrees.</param>
        /// <param name="spread">The deviation in either direction from the rotation. In degrees.</param>
        /// <param name="fadeTo">The ending color of the particle</param>
        /// <param name="hasGravity">If the particle has gravity applied to it</param>
        /// <param name="bounce">How much the particle bounces</param>
        /// <param name="gravity">If the particle has gravity how much force should be applied</param>
        /// <returns>If the particle was spawned returns true</returns>
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

        /// <summary>
        /// Updates the particle
        /// </summary>
        /// <param name="gameTime">The game time the particle exists in</param>
        /// <param name="graphics">The graphics device manager</param>
        public override void Update(GameTimeWrapper gameTime,
            GraphicsDeviceManager graphics)
        {
            if (visible)
            {
                vel *= velocityDecayRate;
                if (hasGravity)
                {
                    vel.Y += gravity * (float)gameTime.GameSpeed;
                }

                aliveTime -= gameTime.ElapsedGameTime;

                //if (pos.Y > graphics.GraphicsDevice.Viewport.Height)
                //{
                //    pos.Y = graphics.GraphicsDevice.Viewport.Height - tex.Height / 2;
                //    vel.Y *= -bounce;
                //}

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

        /// <summary>
        /// Draw the particle
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
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
