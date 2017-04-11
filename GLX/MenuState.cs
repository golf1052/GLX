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
    /// <summary>
    /// Holds information about a menu state. Inherits from <see cref="GameState"/>
    /// </summary>
    /// <remarks>
    /// There is some information about MenuState in the <see cref="GameState"/> documentation file.
    /// Typically menus have text that takes the user to a new place in the game whether that is another menu or actual game play.
    /// MenuState provides extra functionality to handle selecting menu items.
    /// </remarks>
    public class MenuState : GameState
    {
        /// <summary>
        /// The list of menu items.
        /// </summary>
        public List<TextItem> menuItems;

        private Dictionary<string, Action> menuItemActions;

        /// <summary>
        /// The color a menu item should be when it is not selected.
        /// </summary>
        public Color UnselectedColor { get; set; }

        /// <summary>
        /// The color a menu item should be when it is selected.
        /// </summary>
        public Color SelectedColor { get; set; }

        private int currentSelection;

        /// <summary>
        /// Get or set the currently selected menu item index.
        /// </summary>
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
                        item.color = UnselectedColor;
                    }
                    menuItems[value].color = SelectedColor;
                    currentSelection = value;
                }
            }
        }

        /// <summary>
        /// The font that should be used when drawing text.
        /// </summary>
        public SpriteFont MenuFont { get; set; }

        /// <summary>
        /// The position where text should start to be drawn.
        /// </summary>
        public Vector2 initialPosition;

        /// <summary>
        /// The spacing between menu items.
        /// </summary>
        public float spacing;

        private World world;

        /// <summary>
        /// The directions menu items can be arranged.
        /// </summary>
        public enum Direction
        {
            LeftToRight,
            TopToBottom
        }

        /// <summary>
        /// The direction the menu should go, either left to right or top to bottom.
        /// </summary>
        public Direction menuDirection;

        /// <summary>
        /// Creates a new menu state. Unlike <see cref="GameState"/>, menu states come with their own game time because menus typically run at normal speed.
        /// </summary>
        /// <param name="name">The name of the menu state.</param>
        /// <param name="graphics">The graphics device manager.</param>
        /// <param name="world">The world</param>
        public MenuState(string name, GraphicsDeviceManager graphics, World world) : base(name, graphics)
        {
            menuItems = new List<TextItem>();
            menuItemActions = new Dictionary<string, Action>();
            currentSelection = 0;
            AddDraw(Draw);
            this.world = world;
            UnselectedColor = Color.Black;
            SelectedColor = Color.Yellow;
            spacing = 10;
            menuDirection = Direction.TopToBottom;
        }

        /// <summary>
        /// Adds a new menu item to the list of menu items where the given string is what the menu item will display.
        /// </summary>
        /// <param name="spriteText">The text of the menu item.</param>
        public void AddMenuItem(string spriteText = "")
        {
            menuItems.Add(new TextItem(MenuFont, spriteText));
            menuItems.Last().color = UnselectedColor;
            menuItems.Last().Update();
            if (menuItems.Count == 1)
            {
                menuItems[0].color = SelectedColor;
            }
        }

        /// <summary>
        /// Sets an action on a menu item.
        /// This is used so that the menu items can cause actions to happen like switching to a new menu or launching into gameplay.
        /// </summary>
        /// <param name="text">The menu item to set the action on.</param>
        /// <param name="action">The action.</param>
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

        /// <summary>
        /// Invokes the action on the currently selected menu item (if there is an action).
        /// </summary>
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

        private void Draw()
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
