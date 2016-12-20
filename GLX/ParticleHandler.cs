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
                    if (particle.position.X < container.Value.Left + particle.tex.Width / 2)
                    {
                        particle.position.X = container.Value.Left + particle.tex.Width / 2;
                        particle.velocity.X *= -particle.bounce;
                    }
                    if (particle.position.X > container.Value.Right - particle.tex.Width / 2)
                    {
                        particle.position.X = container.Value.Right - particle.tex.Width / 2;
                        particle.velocity.X *= -particle.bounce;
                    }
                    if (particle.position.Y < container.Value.Top + particle.tex.Height / 2)
                    {
                        particle.position.Y = container.Value.Top + particle.tex.Height / 2;
                        particle.velocity.Y *= -particle.bounce;
                    }
                    if (particle.position.Y > container.Value.Bottom - particle.tex.Height / 2)
                    {
                        particle.position.Y = container.Value.Bottom - particle.tex.Height / 2;
                        particle.velocity.Y *= -particle.bounce;
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

                        float distance = Vector2.Distance(particles[i].position, particles[j].position);
                        if (distance < particles[i].tex.Width)
                        {
                            float distanceToMove = Math.Abs(particles[i].tex.Width - distance);
                            distanceToMove /= 2;
                            Vector2 intersectVector = new Vector2(particles[i].position.X - particles[j].position.X,
                                particles[i].position.Y - particles[j].position.Y);
                            intersectVector.Normalize();
                            Vector2 particle1Force = particles[i].mass * (particles[i].velocity * 0.9f);
                            Vector2 particle2Force = particles[j].mass * (particles[j].velocity * 0.9f);
                            intersectVector *= distanceToMove;
                            particles[i].position += intersectVector + particle2Force;
                            particles[j].position -= intersectVector + particle1Force;
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
