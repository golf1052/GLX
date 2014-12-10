using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class ParticleHandler
    {
        Particle[] particles;
        public bool collisions;
        public Rectangle? container;

        public ParticleHandler(int numberOfParticles, Texture2D loadedTex)
        {
            particles = new Particle[numberOfParticles];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle(loadedTex);
            }
            collisions = false;
            container = null;
        }

        public ParticleHandler(int numberOfParticles, GraphicsDeviceManager graphics)
        {
            particles = new Particle[numberOfParticles];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle(graphics);
            }
            collisions = false;
            container = null;
        }

        public bool SpawnParticles(int numberOfParticles, ParticleOptions options)
        {
            int particlesSpawned = 0;
            foreach (Particle particle in particles)
            {
                bool spawned;
                if (options.size == null)
                {
                    spawned = particle.SpawnParticle(options.position,
                        options.startingColor, options.aliveTime,
                        options.velocity, options.velocityDecayRate,
                        options.fadeRate, options.colorShiftRate,
                        options.rotation, options.spread,
                        options.endingColor, options.hasGravity,
                        options.bounce, options.gravity);
                }
                else
                {
                    spawned = particle.SpawnParticle(options.position, options.startingColor,
                        options.aliveTime, options.size,
                        options.velocity, options.velocityDecayRate,
                        options.fadeRate, options.colorShiftRate,
                        options.rotation, options.spread,
                        options.endingColor, options.hasGravity,
                        options.bounce, options.gravity);
                }
                if (spawned)
                {
                    particlesSpawned++;
                    if (particlesSpawned >= numberOfParticles)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Update(GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            foreach (Particle particle in particles)
            {
                if (container != null)
                {
                    if (particle.pos.X < container.Value.Left + particle.tex.Width / 2)
                    {
                        particle.pos.X = container.Value.Left + particle.tex.Width / 2;
                        particle.vel.X *= -particle.bounce;
                    }
                    if (particle.pos.X > container.Value.Right - particle.tex.Width / 2)
                    {
                        particle.pos.X = container.Value.Right - particle.tex.Width / 2;
                        particle.vel.X *= -particle.bounce;
                    }
                    if (particle.pos.Y < container.Value.Top + particle.tex.Height / 2)
                    {
                        particle.pos.Y = container.Value.Top + particle.tex.Height / 2;
                        particle.vel.Y *= -particle.bounce;
                    }
                    if (particle.pos.Y > container.Value.Bottom - particle.tex.Height / 2)
                    {
                        particle.pos.Y = container.Value.Bottom - particle.tex.Height / 2;
                        particle.vel.Y *= -particle.bounce;
                    }
                }
                particle.Update(gameTime, graphics);
            }
            if (collisions)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    for (int j = 0; j < particles.Length; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        float distance = Vector2.Distance(particles[i].pos, particles[j].pos);
                        if (distance < particles[i].tex.Width)
                        {
                            float distanceToMove = Math.Abs(particles[i].tex.Width - distance);
                            distanceToMove /= 2;
                            Vector2 intersectVector = new Vector2(particles[i].pos.X - particles[j].pos.X,
                                particles[i].pos.Y - particles[j].pos.Y);
                            intersectVector.Normalize();
                            intersectVector *= distanceToMove;
                            particles[i].pos += intersectVector;
                            particles[j].pos -= intersectVector;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
            {
                particle.Draw(spriteBatch);
            }
        }
    }
}
