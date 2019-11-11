# Key Event Handler

## Installation

### Unity Package Manager

Add below setting for *Unofficial Unity Package Manager Registry* into `scopedRegistries`.

```json
  "scopedRegistries": [
    {
      "name": "Unofficial Unity Package Manager Registry",
      "url": "https://upm-packages.dev",
      "scopes": [
        "com.unity.simpleanimation",
        "com.stevevermeulen",
        "jp.cysharp",
        "dev.monry",
        "dev.upm-packages"
      ]
    }
  ]
```



### Command Line

```bash
upm add package dev.upm-packages.keyeventhandler
```

## Usages

### Event Subscription

You can subscribe **down**, **press** and **up** key event.

```csharp
using UnityEngine;
using UniRx;
using KeyEventHandler;

class Sample : MonoBehaviour
{
    private void Start()
    {
        this.OnKeyDownAsObservable(KeyCode.Return)
            .Subscribe(_ => Debug.Log("On key down Return"))
            .AddTo(this);
        this.OnKeyPressAsObservable(KeyCode.Space)
            .Subscribe(_ => Debug.Log("On key press Space"))
            .AddTo(this);
        this.OnKeyUpAsObservable(KeyCode.Escape)
            .Subscribe(_ => Debug.Log("On key up Esc"))
            .AddTo(this);
    }
}
```

These methods are implementing as extension method for `Component` or `GameObject`.

### Event layering

You need to set up a component called KeyEventLayer to accept key events.
(If not set, it will be added automatically)

KeyEventLayer is a function to switch the validity of KeyEvents subscribed by the underlying GameObject in a batch.

KeyEventLayers can be nested, and KeyEvents will not be executed unless all KeyEventLayers in the parent-child relationship are valid.
