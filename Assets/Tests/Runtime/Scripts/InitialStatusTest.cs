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
    public class InitialStatusTest : ZenjectIntegrationTestFixture
    {
        private IDictionary<(KeyEventType keyEventType, KeyCode keyCode), bool> KeyStatuses { get; } = new Dictionary<(KeyEventType keyEventType, KeyCode keyCode), bool>
        {
            {(KeyEventType.Down, KeyCode.Return), false},
            {(KeyEventType.Press, KeyCode.Space), false},
            {(KeyEventType.Up, KeyCode.Escape), false},
        };

        private GameObject KeyPressEventTrigger { get; set; }
        private KeyEventLayer KeyEventLayer { get; set; }

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
                .WithId("KeyEventLayer")
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("KeyEventLayer")
                .AsCached()
                .NonLazy();
            KeyEventLayer = Container.ResolveId<KeyEventLayer>("KeyEventLayer");
            KeyPressEventTrigger = new GameObject("KeyPressEventTrigger");
            KeyPressEventTrigger.transform.SetParent(KeyEventLayer.transform);
            PostInstall();
        }

        [UnityTest]
        public IEnumerator DisabledAtInitialization()
        {
            var counter = 0;
            KeyPressEventTrigger.OnKeyPressAsObservable(KeyCode.Space).Subscribe(_ => counter++);
            KeyEventLayer.enabled = false;

            KeyStatuses[(KeyEventType.Press, KeyCode.Space)] = true;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(0, counter);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(0, counter);

            KeyEventLayer.enabled = true;

            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, counter);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(4, counter);
        }

        [UnityTest]
        public IEnumerator ToggleEnabled()
        {
            var counter = 0;
            Debug.Log("EnabledAtInitialization: 0");
            KeyPressEventTrigger.OnKeyPressAsObservable(KeyCode.Space).Subscribe(_ => counter++);
            KeyEventLayer.enabled = true;

            KeyStatuses[(KeyEventType.Press, KeyCode.Space)] = true;
            do
            {
                yield return new WaitForEndOfFrame();
            } while (counter == 0);
            Assert.AreEqual(1, counter);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(4, counter);

            KeyEventLayer.enabled = false;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(4, counter);

            KeyEventLayer.enabled = true;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(7, counter);
        }
    }
}