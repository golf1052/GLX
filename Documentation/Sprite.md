# Sprite Class
The Sprite class is the more implemented version of the SpriteBase class. This class holds actual texture data.

## Fields
- Texture
- Draw rectangle
- Texture color data
- Sprite transform data. Used when the sprite is rotated and/or scaled.
- Sprite animation data

## Constructors
### Texture
Used for creating a sprite with a single non animated texture.

### Pixel
Used for creating a sprite with a 1 by 1 white pixel as its texture. This pixel can be resized using the sprite's draw rectangle.

### Animated Sprite
Used for creating a sprite that has animation. Must call Ready after adding sprite sheets.

## Methods
- Ready. Must be called after adding sprite sheets. Sets texture to first frame of the first animation or to the first frame of the set animation.
- Update
- Draw
- Draw using draw rectangle
