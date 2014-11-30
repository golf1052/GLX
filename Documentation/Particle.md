# Particle Class
Holds values that describe a particle

## Fields
- How long the particle has been alive
- The particle starting color
- The color the particle should end up as
- How fast the particle should change color
- How fast the particle should slow down
- How fast the particle should fade away
- How fast the particle should move towards the bottom of the screen
- Does the particle have gravity
- How much the particle should bounce after hitting the ground (between 0 and 1, 0 meaning lose all vertical velocity and 1 meaning bounce upwards at the same speed the particle hit the ground)

## Constructors
### Use Texture
### Use Square

## Methods
- Spawn a particle using a starting position, starting color, the how long it should be alive, starting velocity, velocity decay rate, fade rate, color shift rate, direction the particle should start moving in, the deviation from that direction, the color the paritcle should fade to, if the particle has gravity, the bounce value, and the gravity value
- Same as above but also including the size of the particle
- Update
- Draw
