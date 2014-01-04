using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XyBorg.GUIElements
{
    class BaseElement
    {
        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
        public virtual void HandleInput(MouseState mstate, KeyboardState kstate)
        {
        }

        internal virtual void Dispose()
        {
        }
    }
}
