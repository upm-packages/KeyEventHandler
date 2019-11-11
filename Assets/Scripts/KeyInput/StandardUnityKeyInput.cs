using UnityEngine;

namespace KeyEventHandler.KeyInput
{
    public class StandardUnityKeyInput : IKeyInput
    {
        public static IKeyInput Default { get; } = new StandardUnityKeyInput();

        bool IKeyInput.GetKeyDown(KeyCode keyCode)
        {
            return Input.GetKeyDown(keyCode);
        }

        bool IKeyInput.GetKeyPress(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }

        bool IKeyInput.GetKeyUp(KeyCode keyCode)
        {
            return Input.GetKeyUp(keyCode);
        }
    }
}