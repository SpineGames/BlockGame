using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockGame
{
    public class KeyWatcher
    {
        Keys _keyToWatch;
        KeyboardState _prevKeyState;
        EventHandler _onPressed;
        EventHandler _onDown;

        public KeyWatcher(Keys key)
        {
            _keyToWatch = key;
        }

        public KeyWatcher(Keys key, EventHandler onPressed)
        {
            _keyToWatch = key;
            _onPressed = onPressed;
        }

        internal void update()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(_keyToWatch))
            {
                if (_prevKeyState.IsKeyUp(_keyToWatch))
                {
                    if (_onPressed != null)
                        _onPressed.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    if (_onDown != null)
                        _onDown.Invoke(this, EventArgs.Empty);
                }
            }

            _prevKeyState = keyState;
        }
    }
}
