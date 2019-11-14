using System;
using System.Collections.Generic;
using System.Linq;
using KeyEventHandler.KeyInput;
using UniRx;
using UnityEngine;
using Zenject;

namespace KeyEventHandler
{
    public class KeyEventLayer : MonoBehaviour
    {
        [InjectOptional] private IKeyInput KeyInput { get; } = StandardUnityKeyInput.Default;
        private IReactiveProperty<bool> Gate { get; } = new BoolReactiveProperty(false);

        private IDictionary<(KeyEventType keyEventType, KeyCode keyCode), IReactiveCommand<Unit>> Commands { get; } = new Dictionary<(KeyEventType keyEventType, KeyCode keyCode), IReactiveCommand<Unit>>();

        private CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();

        internal IObservable<Unit> OnKeyEventAsObservable(KeyEventType keyEventType, KeyCode keyCode)
        {
            if (!Commands.ContainsKey((keyEventType, keyCode)))
            {
                Commands[(keyEventType, keyCode)] = ResolveGates().
                    CombineLatest()
                    .Select(xs => xs.All(x => x))
                    .TakeUntilDestroy(this)
                    .ToReactiveCommand();
                CompositeDisposable
                    .Add(
                        Observable
                            .EveryUpdate()
                            .Where(
                                _ =>
                                {
                                    switch (keyEventType)
                                    {
                                        case KeyEventType.Down:
                                            return KeyInput.GetKeyDown(keyCode);
                                        case KeyEventType.Press:
                                            return KeyInput.GetKeyPress(keyCode);
                                        case KeyEventType.Up:
                                            return KeyInput.GetKeyUp(keyCode);
                                        default:
                                            throw new ArgumentOutOfRangeException(nameof(keyEventType), keyEventType, null);
                                    }
                                }
                            )
                            .Subscribe(_ => Commands[(keyEventType, keyCode)].Execute(Unit.Default))
                    );
            }

            return Commands[(keyEventType, keyCode)];
        }

        private void Awake()
        {
            Gate.Value = enabled;
        }

        private void OnEnable()
        {
            Gate.Value = true;
        }

        private void OnDisable()
        {
            Gate.Value = false;
        }

        private void OnDestroy()
        {
            CompositeDisposable.Dispose();
        }

        private IEnumerable<IReadOnlyReactiveProperty<bool>> ResolveGates()
        {
            if (gameObject.transform.parent == default)
            {
                return new []{Gate};
            }

            var parentLayer = gameObject.transform.parent.GetComponentInParent<KeyEventLayer>();
            if (parentLayer == default)
            {
                return new []{Gate};
            }

            return parentLayer
                .ResolveGates()
                .Concat(new []{Gate});
        }
    }
}