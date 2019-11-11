using System;
using UniRx;
using UnityEngine;

namespace KeyEventHandler
{
    public class ObservableKeyDownEventTrigger : MonoBehaviour
    {
        internal IObservable<Unit> OnKeyDownEventAsObservable(KeyCode keyCode)
        {
            return gameObject.ResolveLayer().OnKeyEventAsObservable(KeyEventType.Down, keyCode).TakeUntilDestroy(this);
        }
    }
}