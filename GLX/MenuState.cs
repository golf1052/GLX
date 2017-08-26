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
        public List<MenuItem> menuItems;
        private List<MenuItem> actionableMenuItems;

        private Dictionary<string, Action> menuItemActions;
        public Action BackAction { get; set; }

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
                if (actionableMenuItems.Count > 0)
                {
                    if (value >= actionableMenuItems.Count)
                    {
                        value = 0;
                    }
                    else if (value <= -1)
                    {
                        value = actionableMenuItems.Count - 1;
                    }
                    foreach (var item in actionableMenuItems)
                    {
                        item.color = UnselectedColor;
                    }
                    actionableMenuItems[value].color = SelectedColor;
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

        public World world;

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

        private KeyboardState previousKeyboardState;
        private GamePadState previousGamePadState;

        public bool Loading { get; set; }

        /// <summary>
        /// Creates a new menu state. Unlike <see cref="GameState"/>, menu states come with their own game time because menus typically run at normal speed.
        /// </summary>
        /// <param name="name">The name of the menu state.</param>
        /// <param name="graphics">The graphics device manager.</param>
        /// <param name="world">The world</param>
        public MenuState(string name, GraphicsDeviceManager graphics, World world) : base(name, graphics)
        {
            menuItems = new List<MenuItem>();
            actionableMenuItems = new List<MenuItem>();
            menuItemActions = new Dictionary<string, Action>();
            BackAction = null;
            currentSelection = 0;
            AddDraw(Draw);
            this.world = world;
            UnselectedColor = Color.Black;
            SelectedColor = Color.Yellow;
            spacing = 25;
            menuDirection = Direction.TopToBottom;
            previousKeyboardState = Keyboard.GetState();
            previousGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Adds a new menu item to the list of menu items where the given string is what the menu item will display.
        /// </summary>
        /// <param name="spriteText">The text of the menu item.</param>
        public void AddMenuItem(string spriteText = "", bool canSelect = true)
        {
            MenuItem menuItem = new MenuItem(MenuFont, spriteText, canSelect);
            menuItems.Add(menuItem);
            menuItems.Last().color = UnselectedColor;
            menuItems.Last().scale = 1.5f;
            menuItems.Last().Update();
            if (canSelect)
            {
                if (menuItems.Count == 1)
                {
                    menuItems[0].color = SelectedColor;
                }
                actionableMenuItems.Add(menuItem);
            }
        }

        public void AddMenuItems(params string[] spriteTexts)
        {
            AddMenuItems(new List<string>(spriteTexts));
        }

        public void AddMenuItems(List<string> spriteTexts)
        {
            foreach (var text in spriteTexts)
            {
                AddMenuItem(text);
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
            foreach (TextItem item in actionableMenuItems)
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
        public void DoSelectedAction()
        {
            if (menuItemActions.ContainsKey(actionableMenuItems[CurrentSelection].text))
            {
                // this is bad. if the action modifies the state list then trying to just directly invoke the
                // action will throw an error. instead we have to defer the invocation until after the update
                // loops finish...
                DoAction(menuItemActions[actionableMenuItems[CurrentSelection].text]);
            }
        }

        private void DoAction(Action action)
        {
            World.thingsToDo.Add(action);
        }

        public void UpdateMenuState()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDownAndUp(Keys.Up, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.DPadUp, previousGamePadState))
            {
                CurrentSelection--;
            }

            if (keyboardState.IsKeyDownAndUp(Keys.Down, previousKeyboardState) ||
                gamePadState.IsButtonDownAndUp(Buttons.DPadDown, previousGamePadState))
            {
                CurrentSelection++;
            }

            if (gamePadState.ThumbSticks.Left.Y >= 0.5f &&
                previousGamePadState.ThumbSticks.Left.Y < 0.5f)
            {
                CurrentSelection--;
            }

            if (gamePadState.ThumbSticks.Left.Y <= -0.5f &&
                previousGamePadState.ThumbSticks.Left.Y > -0.5f)
            {
                CurrentSelection++;
            }

            if (!Loading)
            {
                if (keyboardState.IsKeyDownAndUp(Keys.Enter, previousKeyboardState) ||
                    gamePadState.IsButtonDownAndUp(Buttons.A, previousGamePadState))
                {
                    DoSelectedAction();
                }
                else if (keyboardState.IsKeyDownAndUp(Keys.Escape, previousKeyboardState) ||
                    keyboardState.IsKeyDownAndUp(Keys.Back, previousKeyboardState) ||
                    gamePadState.IsButtonDownAndUp(Buttons.B, previousGamePadState))
                {
                    if (BackAction != null)
                    {
                        DoAction(BackAction);
                    }
                }
            }
            Loading = false;

            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
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
