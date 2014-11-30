# TweenerBase Class
The base class for interpolation between two values.

## Notes
There are three classes that currently implement this base class. FloatTweener, Vector2Tweener, and ColorTweener. These classes contain an update function that calls a interpolation function on their respective float fields.

## Fields
- Is smoothing active
- The smoothing type we should use
- The rate we should smooth at

## Constructor
Create a new value that can be interpolated

## Methods
- Update
- Interpolation wrappers for
  - float
  - Vector2
  - Color
- Interpolation functions
  - Linear
  - Smoothstep
