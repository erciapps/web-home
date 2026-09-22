# 4. Primer script

Un script permite programar el comportamiento de un `GameObject`.

Crea una carpeta:

```text
Assets/MiJuego/Scripts
```

Dentro crea un script llamado, por ejemplo, `PrimerScript.cs`:

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

## Start y Update

`Start()` se ejecuta una vez cuando comienza el objeto.

`Update()` se ejecuta continuamente, una vez por frame.

Para probarlo:

1. Arrastra el script a un `GameObject` de la escena.
2. Pulsa **Play**.
3. Abre la ventana **Console**.
4. Comprueba que aparece el mensaje.

!!! warning
    Crear el archivo no es suficiente. El script debe estar añadido como componente a un objeto de la escena para que se ejecute.
