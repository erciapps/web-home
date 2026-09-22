# 4. Primer script

Un script permite programar el comportamiento de un GameObject.

```csharp
using UnityEngine;

public class PrimerScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Hola desde Unity");
    }

    void Update()
    {
    }
}
```

`Start()` se ejecuta al comenzar y `Update()` se ejecuta una vez por frame.

!!! warning
    Para que el script haga algo sobre un objeto, debe estar añadido como componente del GameObject correspondiente.
