# 3. Movimiento y Android

WASD, GravitySensor y compilación APK.

## Movimiento con teclado y sensores

Crea `Assets/Scripts/MovimientoBola.cs`, pega este código y asígnalo a `Jugador`.

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoBola : MonoBehaviour
{
    public float fuerza = 25f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (GravitySensor.current != null)
            InputSystem.EnableDevice(GravitySensor.current);
    }

    void FixedUpdate()
    {
        float x = 0f;
        float z = 0f;

        if (GravitySensor.current != null)
        {
            Vector3 gravedad = GravitySensor.current.gravity.ReadValue();
            x = gravedad.x;
            z = gravedad.y;
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) x = -1f;
            if (Keyboard.current.dKey.isPressed) x = 1f;
            if (Keyboard.current.wKey.isPressed) z = 1f;
            if (Keyboard.current.sKey.isPressed) z = -1f;
        }

        Vector3 direccion = Vector3.ClampMagnitude(new Vector3(x, 0, z), 1f);
        rb.AddForce(direccion * fuerza, ForceMode.Acceleration);
    }
}
```

Probamos las teclas **WASD** en el editor. Después comprobamos el control en Android inclinando el teléfono. El vector del `GravitySensor` detecta la dirección de gravedad; **no estamos leyendo directamente la velocidad angular del giroscopio**. Los ejes son una primera aproximación: en un móvil real hay que comprobar si responden en el sentido correcto, especialmente con la pantalla horizontal.

En las primeras pruebas el movimiento resultó algo lento. Aumentamos `fuerza` de 10 a 25; cambiar de `ForceMode.Force` a `ForceMode.Acceleration` no aumenta automáticamente la respuesta con una masa de 1.

<figure class="unity-shot" markdown="1">

[![MovimientoBola en Rider](../../assets/images/pmdm/androicilla/20261013-205758.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261013-205758.png){ aria-label="Ampliar imagen: MovimientoBola en Rider" }

<figcaption>MovimientoBola en Rider.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Prueba con teclado](../../assets/images/pmdm/androicilla/movimiento.gif){ loading=lazy }](../../assets/images/pmdm/androicilla/movimiento.gif){ aria-label="Ampliar imagen: Prueba con teclado" }

<figcaption>Prueba con teclado.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Lectura GravitySensor añadida](../../assets/images/pmdm/androicilla/20261013-210852.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261013-210852.png){ aria-label="Ampliar imagen: Lectura GravitySensor añadida" }

<figcaption>Lectura GravitySensor añadida.</figcaption>
</figure>

## Ejecutar en Android

En el móvil, activa **Opciones de desarrollador** (pulsando repetidamente sobre «Número de compilación») y después **Depuración USB**. Conecta el teléfono con un cable de datos y acepta la autorización.

=== "Windows"

    En PowerShell, utilizando el SDK de Unity 6.6 instalado en su ruta predeterminada:

    ```powershell
    & "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe" devices
    ```

=== "Linux (Debian/Ubuntu)"

    ```bash
    sudo apt update
    sudo apt install adb android-sdk-platform-tools-common
    adb devices
    ```

    Si aparece `no permissions`, revisa las reglas `udev` y el acceso USB. No abras Unity como root.

`adb devices` debe indicar `device`, no `unauthorized`.

En Unity abre **File → Build Profiles**, añade el perfil **Android** y actívalo (**Active**). En **Player Settings → Resolution and Presentation**, configura `Default Orientation = Landscape Left`. Comprueba en **Scene List** que esté incluida `Nivel1`.

Selecciona el teléfono en `Run Device` y pulsa **Build And Run**. Para instalar un APK directamente no es necesario activar `Build App Bundle (Google Play)`.

<figure class="unity-shot" markdown="1">

[![Menú Build Profiles](../../assets/images/pmdm/androicilla/20261013-210156.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261013-210156.png){ aria-label="Ampliar imagen: Menú Build Profiles" }

<figcaption>Menú Build Profiles.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Añadir Android](../../assets/images/pmdm/androicilla/20261013-210227.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261013-210227.png){ aria-label="Ampliar imagen: Añadir Android" }

<figcaption>Añadir Android.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Perfil Android activo](../../assets/images/pmdm/androicilla/20261013-210430.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261013-210430.png){ aria-label="Ampliar imagen: Perfil Android activo" }

<figcaption>Perfil Android activo.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Orientación Landscape Left](../../assets/images/pmdm/androicilla/20261013-210627.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261013-210627.png){ aria-label="Ampliar imagen: Orientación Landscape Left" }

<figcaption>Orientación Landscape Left.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Nombre del APK](../../assets/images/pmdm/androicilla/20261013-211038.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261013-211038.png){ aria-label="Ampliar imagen: Nombre del APK" }

<figcaption>Nombre del APK.</figcaption>
</figure>



---

[← 2. Bola y cámara](02-jugador-camara.md) · [Volver al índice](index.md) · [4. Agujero con ProBuilder →](04-agujero-probuilder.md)
