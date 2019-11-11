using UnityEngine;

namespace KeyEventHandler.KeyInput
{
    public interface IKeyInput
    {
        bool GetKeyDown(KeyCode keyCode);
        bool GetKeyPress(KeyCode keyCode);
        bool GetKeyUp(KeyCode keyCode);
    }
}