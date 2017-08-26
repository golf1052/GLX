using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class MenuItem : TextItem
    {
        public bool CanSelect { get; set; }

        public MenuItem(SpriteFont loadedFont, string spriteText = "", bool canSelect = true) : base(loadedFont, spriteText)
        {
            CanSelect = canSelect;
        }
    }
}
