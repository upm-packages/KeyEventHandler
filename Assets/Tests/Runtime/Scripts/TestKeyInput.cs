using KeyEventHandler.KeyInput;
using UnityEngine;

namespace KeyEventHandler
{
    public class TestKeyInput : IKeyInput
    {
        public bool KeyDown { get; set; }
        public bool KeyPress { get; set; }
        public bool KeyUp { get; set; }

        bool IKeyInput.GetKeyDown(KeyCode keyCode)
        {
            return KeyDown;
        }

        bool IKeyInput.GetKeyPress(KeyCode keyCode)
        {
            return KeyPress;
        }

        bool IKeyInput.GetKeyUp(KeyCode keyCode)
        {
            return KeyUp;
        }
    }
}