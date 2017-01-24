using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GLX;

namespace GLX
{
    public class MenuState : GameState
    {
        public List<TextItem> menuItems;
        Dictionary<string, Action> menuItemActions;
        public Color unselectedColor;
        public Color selectedColor;
        int currentSelection;
        public SpriteFont menuFont;
        public Vector2 initialPosition;
        public float spacing;
        World world;

        GameTimeWrapper menuGameTime;

        public enum Direction
        {
            LeftToRight,
            TopToBottom
        }
        public Direction menuDirection;

        public MenuState(string name, GraphicsDeviceManager graphics, Game game, World world,
            VirtualResolutionRenderer virtualResolutionRenderer) : base(name, graphics, virtualResolutionRenderer)
        {
            menuItems = new List<TextItem>();
            menuItemActions = new Dictionary<string, Action>();
            currentSelection = 0;
            menuGameTime = new GameTimeWrapper(Update, game, 1.0m);
            AddTime(menuGameTime);
            AddDraw(Draw);
            this.world = world;
            unselectedColor = Color.Black;
            selectedColor = Color.Yellow;
        }

        public void AddMenuItem(string spriteText = "")
        {
            menuItems.Add(new TextItem(menuFont, spriteText));
            menuItems.Last().color = unselectedColor;
            menuItems.Last().Update();
        }

        public void SetMenuAction(string text, Action action)
        {
            foreach (TextItem item in menuItems)
            {
                if (item.text == text)
                {
                    menuItemActions.Add(text, action);
                    break;
                }
            }
        }

        public void Update(GameTimeWrapper gameTime)
        {
            if (menuItems.Count > 0)
            {
                menuItems[currentSelection].color = selectedColor;
                if (menuDirection == Direction.LeftToRight)
                {
                    if (World.keyboardState.IsKeyDown(Keys.Left) && World.previousKeyboardState.IsKeyUp(Keys.Left) ||
                        World.gamePadStates[0].ThumbSticks.Left.X < -0.5f && World.previousGamePadStates[0].ThumbSticks.Left.X > -0.5f ||
                        World.gamePadStates[0].DPad.Left == ButtonState.Pressed && World.previousGamePadStates[0].DPad.Left == ButtonState.Released)
                    {
                        menuItems[currentSelection].color = unselectedColor;
                        currentSelection--;
                        if (currentSelection == -1)
                        {
                            currentSelection = menuItems.Count - 1;
                        }
                    }
                    else if (World.keyboardState.IsKeyDown(Keys.Right) && World.previousKeyboardState.IsKeyUp(Keys.Right) ||
                        World.gamePadStates[0].ThumbSticks.Left.X > 0.5f && World.previousGamePadStates[0].ThumbSticks.Left.X < 0.5f ||
                        World.gamePadStates[0].DPad.Right == ButtonState.Pressed && World.previousGamePadStates[0].DPad.Right == ButtonState.Released)
                    {
                        menuItems[currentSelection].color = unselectedColor;
                        currentSelection++;
                        if (currentSelection == menuItems.Count)
                        {
                            currentSelection = 0;
                        }
                    }
                }
                else if (menuDirection == Direction.TopToBottom)
                {
                    if (World.keyboardState.IsKeyDown(Keys.Up) && World.previousKeyboardState.IsKeyUp(Keys.Up) ||
                        World.gamePadStates[0].ThumbSticks.Left.Y < -0.5f && World.previousGamePadStates[0].ThumbSticks.Left.Y > -0.5f ||
                        World.gamePadStates[0].DPad.Up == ButtonState.Pressed && World.previousGamePadStates[0].DPad.Up == ButtonState.Released)
                    {
                        menuItems[currentSelection].color = unselectedColor;
                        currentSelection--;
                        if (currentSelection == -1)
                        {
                            currentSelection = menuItems.Count - 1;
                        }
                    }
                    else if (World.keyboardState.IsKeyDown(Keys.Down) && World.previousKeyboardState.IsKeyUp(Keys.Down) ||
                        World.gamePadStates[0].ThumbSticks.Left.Y > 0.5f && World.previousGamePadStates[0].ThumbSticks.Left.Y < 0.5f ||
                        World.gamePadStates[0].DPad.Down == ButtonState.Pressed && World.previousGamePadStates[0].DPad.Down == ButtonState.Released)
                    {
                        menuItems[currentSelection].color = unselectedColor;
                        currentSelection++;
                        if (currentSelection == menuItems.Count)
                        {
                            currentSelection = 0;
                        }
                    }
                }

                if (World.keyboardState.IsKeyDown(Keys.Enter) && World.previousKeyboardState.IsKeyUp(Keys.Enter) ||
                    World.gamePadStates[0].Buttons.A == ButtonState.Pressed && World.previousGamePadStates[0].Buttons.A == ButtonState.Released)
                {
                    string text = menuItems[currentSelection].text;
                    World.thingsToDo.Add(menuItemActions[text]);
                }
            }
        }

        void Draw()
        {
            world.BeginDraw();
            Vector2 currentPosition = initialPosition;
            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].position = currentPosition;
                if (menuDirection == Direction.LeftToRight)
                {
                    currentPosition.X += menuItems[i].textSize.X + spacing;
                }
                else if (menuDirection == Direction.TopToBottom)
                {
                    currentPosition.Y += menuItems[i].textSize.Y + spacing;
                }
            }
            foreach (TextItem menuItem in menuItems)
            {
                world.Draw(menuItem.Draw);
            }
            world.EndDraw();
        }
    }
}
