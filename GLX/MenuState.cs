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
        private int currentSelection;
        public int CurrentSelection
        {
            get
            {
                return currentSelection;
            }
            set
            {
                if (menuItems.Count > 0)
                {
                    if (value >= menuItems.Count)
                    {
                        value = 0;
                    }
                    else if (value <= -1)
                    {
                        value = menuItems.Count - 1;
                    }
                    foreach (var item in menuItems)
                    {
                        item.color = unselectedColor;
                    }
                    menuItems[value].color = selectedColor;
                    currentSelection = value;
                }
            }
        }

        public SpriteFont MenuFont { get; set; }
        public Vector2 initialPosition;
        public float spacing;
        World world;

        public enum Direction
        {
            LeftToRight,
            TopToBottom
        }
        public Direction menuDirection;

        public MenuState(string name, GraphicsDeviceManager graphics, Game game, World world) : base(name, graphics)
        {
            menuItems = new List<TextItem>();
            menuItemActions = new Dictionary<string, Action>();
            currentSelection = 0;
            AddDraw(Draw);
            this.world = world;
            unselectedColor = Color.Gray;
            selectedColor = Color.White;
            spacing = 10;
            menuDirection = Direction.TopToBottom;
        }

        public void AddMenuItem(string spriteText = "")
        {
            menuItems.Add(new TextItem(MenuFont, spriteText));
            menuItems.Last().color = unselectedColor;
            menuItems.Last().Update();
            if (menuItems.Count == 1)
            {
                menuItems[0].color = selectedColor;
            }
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

        public void DoAction()
        {
            if (menuItemActions.ContainsKey(menuItems[CurrentSelection].text))
            {
                // this is bad. if the action modifies the state list then trying to just directly invoke the
                // action will throw an error. instead we have to defer the invocation until after the update
                // loops finish...
                World.thingsToDo.Add(menuItemActions[menuItems[CurrentSelection].text]);
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
