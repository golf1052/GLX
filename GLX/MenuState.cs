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
