# SpriteSheet Class
Holds information for a sprite sheet

## SpriteSheetInfo Struct
Holds frame width and height information for a sprite sheet

## Fields
- Sprite sheet texture
- Sprite sheet color data
- Dictionary of frame color data keyed by source rectangles
- The SpriteSheetInfo for this sprite sheet
- The frame count
- The number of columns in the sprite sheet
- The number of rows in the sprite sheet
- The direction the sprite sheet goes in (either left to right or top to bottom)
- How long we should stay on each frame. Stored as ticks but given back as milliseconds. When setting set as milliseconds.
- Should this sprite sheet loop?
- The actions we should run on each frame
- The actions we should run on each frame if time is going backwards

## Constructor
Creates a new sprite sheet

## Methods
- Returns the color data for the given source rectangle
- Populates the frame color data dictionary after we load in a sprite sheet
