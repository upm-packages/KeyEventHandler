using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace KeyEventHandler
{
    [PublicAPI]
    public static class ObservableTriggerExtensions
    {
        public static IObservable<Unit> OnKeyDownAsObservable(this Component component, KeyCode keyCode)
        {
            return component.GetOrAddComponent<ObservableKeyDownEventTrigger>()?.OnKeyDownEventAsObservable(keyCode);
        }

        public static IObservable<Unit> OnKeyPressAsObservable(this Component component, KeyCode keyCode)
        {
            return component.GetOrAddComponent<ObservableKeyPressEventTrigger>()?.OnKeyPressEventAsObservable(keyCode);
        }

        public static IObservable<Unit> OnKeyUpAsObservable(this Component component, KeyCode keyCode)
        {
            return component.GetOrAddComponent<ObservableKeyUpEventTrigger>()?.OnKeyUpEventAsObservable(keyCode);
        }

        public static IObservable<Unit> OnKeyDownAsObservable(this GameObject gameObject, KeyCode keyCode)
        {
            return gameObject.GetOrAddComponent<ObservableKeyDownEventTrigger>()?.OnKeyDownEventAsObservable(keyCode);
        }

        public static IObservable<Unit> OnKeyPressAsObservable(this GameObject gameObject, KeyCode keyCode)
        {
            return gameObject.GetOrAddComponent<ObservableKeyPressEventTrigger>()?.OnKeyPressEventAsObservable(keyCode);
        }

        public static IObservable<Unit> OnKeyUpAsObservable(this GameObject gameObject, KeyCode keyCode)
        {
            return gameObject.GetOrAddComponent<ObservableKeyUpEventTrigger>()?.OnKeyUpEventAsObservable(keyCode);
        }

        internal static KeyEventLayer ResolveLayer(this GameObject gameObject)
        {
            var layer = gameObject.GetComponent<KeyEventLayer>();
            if (layer == default)
            {
                layer = gameObject.GetComponentInParent<KeyEventLayer>();
            }

            if (layer == default)
            {
                layer = gameObject.AddComponent<KeyEventLayer>();
            }

            return layer;
        }

        private static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component == default || component.gameObject == default)
            {
                return null;
            }

            return component.gameObject.GetOrAddComponent<T>();
        }

        private static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}