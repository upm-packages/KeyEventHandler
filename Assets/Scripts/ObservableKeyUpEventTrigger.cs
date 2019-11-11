using System;
using UniRx;
using UnityEngine;

namespace KeyEventHandler
{
    public class ObservableKeyUpEventTrigger : MonoBehaviour
    {
        internal IObservable<Unit> OnKeyUpEventAsObservable(KeyCode keyCode)
        {
            return gameObject.ResolveLayer().OnKeyEventAsObservable(KeyEventType.Up, keyCode).TakeUntilDestroy(this);
        }
    }
}