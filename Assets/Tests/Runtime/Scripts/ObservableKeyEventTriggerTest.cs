using System.Collections;
using System.Collections.Generic;
using KeyEventHandler.KeyInput;
using Moq;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace KeyEventHandler
{
    public class ObservableKeyEventTriggerTest : ZenjectIntegrationTestFixture
    {
        private KeyEventLayer RootLayer { get; set; }
        private KeyEventLayer ParentLayer { get; set; }
        private GameObject KeyDownEventTrigger { get; set; }
        private GameObject KeyPressEventTrigger { get; set; }
        private GameObject KeyUpEventTrigger { get; set; }

        private IDictionary<(KeyEventType keyEventType, KeyCode keyCode), bool> KeyStatuses { get; } = new Dictionary<(KeyEventType keyEventType, KeyCode keyCode), bool>
        {
            {(KeyEventType.Down, KeyCode.Return), false},
            {(KeyEventType.Press, KeyCode.Space), false},
            {(KeyEventType.Up, KeyCode.Escape), false},
        };

        [SetUp]
        public void Prepare()
        {
            PreInstall();

            var input = new Mock<IKeyInput>();
            input.Setup(x => x.GetKeyDown(It.IsAny<KeyCode>())).Returns<KeyCode>(keyCode => KeyStatuses[(KeyEventType.Down, keyCode)]);
            input.Setup(x => x.GetKeyPress(It.IsAny<KeyCode>())).Returns<KeyCode>(keyCode => KeyStatuses[(KeyEventType.Press, keyCode)]);
            input.Setup(x => x.GetKeyUp(It.IsAny<KeyCode>())).Returns<KeyCode>(keyCode => KeyStatuses[(KeyEventType.Up, keyCode)]);
            Container.BindInstance(input.Object).AsCached();

            Container
                .Bind<KeyEventLayer>()
                .WithId("RootLayer")
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("RootLayer")
                .AsCached()
                .NonLazy();

            Container
                .Bind<KeyEventLayer>()
                .WithId("ParentLayer")
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("ParentLayer")
                .UnderTransform(injectContext => injectContext.Container.ResolveId<KeyEventLayer>("RootLayer").transform)
                .AsCached()
                .NonLazy();

            RootLayer = Container.ResolveId<KeyEventLayer>("RootLayer");
            ParentLayer = Container.ResolveId<KeyEventLayer>("ParentLayer");
            KeyDownEventTrigger = new GameObject("KeyDownEventTrigger");
            KeyDownEventTrigger.transform.SetParent(ParentLayer.transform);
            KeyPressEventTrigger = new GameObject("KeyPressEventTrigger");
            KeyPressEventTrigger.transform.SetParent(ParentLayer.transform);
            KeyUpEventTrigger = new GameObject("KeyUpEventTrigger");
            KeyUpEventTrigger.transform.SetParent(ParentLayer.transform);

            PostInstall();
        }

        [UnityTest]
        public IEnumerator Standard()
        {
            yield return new WaitForEndOfFrame();
            var hasKeyDown = false;
            var hasKeyPress = false;
            var hasKeyUp = false;
            KeyDownEventTrigger.OnKeyDownAsObservable(KeyCode.Return).Subscribe(_ => hasKeyDown = true);
            KeyPressEventTrigger.OnKeyPressAsObservable(KeyCode.Space).Subscribe(_ => hasKeyPress = true);
            KeyUpEventTrigger.OnKeyUpAsObservable(KeyCode.Escape).Subscribe(_ => hasKeyUp = true);
            yield return new WaitForEndOfFrame();
            Assert.False(hasKeyDown);
            Assert.False(hasKeyPress);
            Assert.False(hasKeyUp);
            yield return new WaitForEndOfFrame();
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = true;
            yield return new WaitForEndOfFrame();
            Assert.True(hasKeyDown);
            Assert.False(hasKeyPress);
            Assert.False(hasKeyUp);
            yield return new WaitForEndOfFrame();
            KeyStatuses[(KeyEventType.Press, KeyCode.Space)] = true;
            yield return new WaitForEndOfFrame();
            Assert.True(hasKeyDown);
            Assert.True(hasKeyPress);
            Assert.False(hasKeyUp);
            yield return new WaitForEndOfFrame();
            KeyStatuses[(KeyEventType.Up, KeyCode.Escape)] = true;
            yield return new WaitForEndOfFrame();
            Assert.True(hasKeyDown);
            Assert.True(hasKeyPress);
            Assert.True(hasKeyUp);
        }

        [UnityTest]
        public IEnumerator LayerManagement()
        {
            var counter = 0;
            yield return new WaitForEndOfFrame();
            // Register event
            KeyDownEventTrigger.OnKeyDownAsObservable(KeyCode.Return).Subscribe(_ => counter++);
            yield return new WaitForEndOfFrame();

            // Check before fire key event
            Assert.AreEqual(0, counter);
            yield return new WaitForEndOfFrame();

            // Fire key event
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, counter);

            // Revert key status
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = false;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, counter);

            // De-activate KeyEventLayer attached in parent transform
            ParentLayer.enabled = false;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, counter);

            // Fire key event, but ignored by KeyEventLayer
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, counter);

            // Revert key status
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = false;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, counter);

            // Revert KeyEventLayer status
            ParentLayer.enabled = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, counter);

            // Fire key event
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(2, counter);

            // Revert key status
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = false;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(2, counter);

            // De-activate KeyEventLayer attached in parent transform
            RootLayer.enabled = false;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(2, counter);

            // Fire key event, but ignored by KeyEventLayer
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(2, counter);

            // Revert key status
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = false;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(2, counter);

            // Revert KeyEventLayer status
            RootLayer.enabled = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(2, counter);

            // Fire key event
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(3, counter);

            // Revert key status
            KeyStatuses[(KeyEventType.Down, KeyCode.Return)] = false;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(3, counter);
        }
    }
}