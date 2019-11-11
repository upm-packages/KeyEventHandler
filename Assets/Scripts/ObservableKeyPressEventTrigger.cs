using System;
using UniRx;
using UnityEngine;

namespace KeyEventHandler
{
    public class ObservableKeyPressEventTrigger : MonoBehaviour
    {
        internal IObservable<Unit> OnKeyPressEventAsObservable(KeyCode keyCode)
        {
            return gameObject.ResolveLayer().OnKeyEventAsObservable(KeyEventType.Press, keyCode).TakeUntilDestroy(this);
        }
    }
}