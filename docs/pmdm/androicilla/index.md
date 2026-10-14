# Androicilla: laberinto 3D controlado con el móvil

En esta práctica vamos a crear un laberinto sencillo con **Unity 6.6**. La bola se mueve con el teclado cuando trabajamos en el ordenador y con la inclinación del teléfono cuando ejecutamos el juego en Android. Después añadiremos un agujero real al suelo, obstáculos y una pantalla de victoria.

**Software utilizado:** Unity 6000.6.0f1 · Universal 3D (URP) · Rider · Input System · ProBuilder 6.1.3 · Android.

> Las capturas se tomaron durante el desarrollo del juego Androicilla. Aquí hemos ordenado el procedimiento definitivo e incluido las correcciones que surgieron en clase. No es necesario repetir las pruebas fallidas.

## 1. Preparar Unity y crear el proyecto

Desde **Unity Hub → Nuevo proyecto**, selecciona **Universal 3D**, escribe `Androicilla` y elige dónde guardar el proyecto.

En **Instalaciones → Gestionar módulos**, comprueba que Unity tiene instalados:

- Android Build Support
- Android SDK & NDK Tools
- OpenJDK

Dentro del editor, organiza `Assets` en las carpetas `Materials`, `Prefabs`, `Scenes` y `Scripts`. Guarda la primera escena con el nombre `Nivel1` dentro de `Scenes`.

## 2. Crear el tablero

En **Hierarchy → clic derecho → 3D Object → Plane**, crea `Suelo`. Configura el `Transform`:

| Propiedad | X | Y | Z |
| --- | ---: | ---: | ---: |
| Position | 0 | 0 | 0 |
| Rotation | 0 | 0 | 0 |
| Scale | 2 | 1 | 2 |

El plano ocupa 20 × 20 unidades. Comprueba que tiene `Mesh Collider` y que no es un *Trigger*.

Crea cuatro objetos `Cube` para cerrar el tablero:

| Objeto | Position | Scale |
| --- | --- | --- |
| ParedNorte | (0, 0.5, 10) | (20, 1, 0.5) |
| ParedSur | (0, 0.5, -10) | (20, 1, 0.5) |
| ParedEste | (10, 0.5, 0) | (0.5, 1, 20) |
| ParedOeste | (-10, 0.5, 0) | (0.5, 1, 20) |

Estas paredes incluyen `Box Collider`. Más adelante pueden agruparse en `Bordes` mediante ProBuilder, siempre comprobando que la malla agrupada conserva la colisión.

## 3. La bola y sus físicas

Crea **3D Object → Sphere** y renómbrala `Jugador`. Colócala inicialmente en `(0, 1, 0)`, con escala `(1, 1, 1)`.

Con `Jugador` seleccionado, añade un **Rigidbody de 3D** y deja:

| Propiedad | Valor |
| --- | --- |
| Mass | 1 |
| Use Gravity | Activado |
| Is Kinematic | Desactivado |
| Interpolate | Interpolate |
| Collision Detection | Continuous |

La esfera también debe tener `Sphere Collider`. Pulsa **Play** para comprobar que cae hasta el suelo. Cuando creemos un agujero, no podremos dejar su posición inicial encima de él.

## 4. Cámara

Selecciona `Main Camera`. Para una vista totalmente cenital utilizamos `Position (0, 24, 0)`, `Rotation (90, 0, 0)`, **Projection → Orthographic** y **Size → 15**.

Cuando el circuito esté listo podemos darle profundidad con `Position (0, 24, -10)` y `Rotation (65, 0, 0)`, manteniendo la proyección ortográfica. La inclinación de la cámara **no cambia los ejes físicos** del tablero.

## 5. Movimiento en Windows y Linux

Crea `Assets/Scripts/MovimientoBola.cs` y añádelo a `Jugador`. Utilizaremos el nuevo Input System de Unity, no `Input.GetAxis`.

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

Prueba primero con **W, A, S y D** en el editor. La lectura de `GravitySensor` es válida como punto de partida, pero la correspondencia de ejes debe probarse y calibrarse en el dispositivo real. Si el movimiento parece lento, ajusta `fuerza` en el Inspector; `ForceMode.Acceleration` no introduce por sí solo una mejora de respuesta.

> Estamos utilizando el **sensor de gravedad**, no la velocidad angular del giroscopio. Son lecturas diferentes.

## 6. Preparar el teléfono Android

En el teléfono, activa **Opciones de desarrollador** pulsando varias veces el número de compilación y habilita **Depuración USB**. Conéctalo con un cable de datos y acepta la autorización.

=== "Windows"

    En PowerShell puedes ejecutar el ADB incluido con Unity:

    ```powershell
    & "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe" devices
    ```

=== "Debian / Ubuntu"

    ```bash
    sudo apt update
    sudo apt install adb android-sdk-platform-tools-common
    adb devices
    ```

    Si aparece `no permissions`, revisa las reglas `udev` y el acceso al dispositivo, en lugar de abrir Unity como root.

El teléfono debe figurar con el estado **`device`**, no `unauthorized`.

## 7. Configurar Android en Unity

Abre **File → Build Profiles**, añade un perfil **Android** y selecciona **Switch Profile**. Verifica que aparece como **Active**.

En **Player Settings → Android → Resolution and Presentation**, selecciona **Default Orientation → Landscape Left**. Asegúrate de incluir `Nivel1` en **Scene List**.

Con el móvil conectado, selecciónalo en `Run Device` y pulsa **Build And Run** para crear e instalar el APK. Para una instalación directa no hace falta activar `Build App Bundle (Google Play)`.

## 8. Abrir un agujero real con ProBuilder

La meta no debe ser un círculo pintado encima del suelo: **la bola tiene que atravesarlo**.

Instala **ProBuilder** desde **Window → Package Management → Package Manager → Unity Registry**.

En el proyecto utilizado habilitamos **Edit → Preferences → ProBuilder → Experimental Features Enabled**; Unity recompila scripts al cambiar esta opción. Si Rider mantiene archivos bloqueados, ciérralo antes de activarla.

1. Prepara un tablero con **volumen**, por ejemplo un cubo plano, en vez de operar sobre un Plane sin grosor.
2. Crea un cilindro que atraviese por completo el suelo: será la pieza que restaremos.
3. Convierte ambos objetos a geometría ProBuilder desde **Tools → ProBuilder → Object → Pro Builderize**.
4. Abre **Tools → ProBuilder → Experimental → Boolean (CSG) Tool**.
5. Selecciona el tablero como primer objeto y el cilindro como segundo; elige **Subtraction** y pulsa **Apply**.
6. Conserva la malla resultante como `Tablero` y desactiva los objetos originales usados para la operación.

**Comprobación imprescindible:** el agujero debe poder verse y atravesarse.

### Corregir el collider del tablero

Después de la resta vimos que el tablero tenía un `Box Collider` que continuaba cubriendo todo el suelo. Aunque el agujero se veía, la bola no caía.

La solución fue eliminar ese `Box Collider` y añadir un **Mesh Collider** que utilice la malla resultante de ProBuilder:

- **Mesh**: la nueva malla del tablero.
- **Convex**: desactivado.
- **Is Trigger**: desactivado.
- **Rigidbody en el tablero**: no hace falta.

No actives `Convex`, ya que la aproximación convexa podría cerrar físicamente el agujero. El aviso sobre los datos de colisión precalculados no es el causante de que la bola no caiga, aunque conviene revisarlo para futuras compilaciones.

## 9. Diseñar el circuito

Coloca paredes interiores en forma de zigzag. Una distribución inicial para el tablero de 20 × 20 es:

| Obstáculo | Position | Scale |
| --- | --- | --- |
| Obstaculo1 | (-3, 0.5, -5) | (14, 1, 0.5) |
| Obstaculo2 | (3, 0.5, 1) | (14, 1, 0.5) |
| Obstaculo3 | (-3, 0.5, 7) | (14, 1, 0.5) |

Son medidas de referencia: en las capturas del proyecto acabamos modificando la distribución. Crea las piezas con `Cube` y comprueba que sus colliders permiten pasar por los corredores.

Para ordenar el proyecto puedes agrupar los obstáculos bajo un GameObject llamado `Obstaculos`. No es necesario fusionarlos en una única malla durante el diseño.

## 10. Materiales URP

Dentro de `Assets/Materials`, crea `M_Suelo`, `M_Paredes` y `M_Bola` mediante **Create → Material**. Utiliza **Universal Render Pipeline/Lit**.

Asigna un marrón al tablero, amarillo o madera clara a las paredes y un gris metálico a la bola. Para el metal, aumenta los valores de **Metallic** y **Smoothness** y comprueba cómo se comporta con la iluminación de la escena.

Estos son **materiales visuales**: no sustituyen a un `Physics Material` ni cambian la geometría del collider.

## 11. Detectar la meta

Crea un objeto vacío llamado `DetectorMeta` debajo del agujero. Añade `Box Collider` y marca **Is Trigger**. Dale tamaño suficiente para abarcar la zona de caída; evita escala cero y comprueba en la Scene que no detecte la bola antes de atravesar el suelo.

Selecciona `Jugador` y establece su **Tag → Player**.

Crea `Assets/Scripts/Meta.cs` y asígnalo a `DetectorMeta`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    public GameObject panelVictoria;
    private bool completado;

    private void Start()
    {
        panelVictoria.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!completado && other.CompareTag("Player"))
        {
            completado = true;
            panelVictoria.SetActive(true);
            Debug.Log("¡Nivel completado!");
        }
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
```

Antes de conectar la interfaz, comprueba en la **Console** que al caer la bola aparece `¡Nivel completado!`.

## 12. Pantalla de victoria

Crea **UI (Canvas) → Canvas** y llámalo `CanvasFinNivel`. En `Canvas Scaler` selecciona **Scale With Screen Size**, resolución **1920 × 1080** y **Match Width Or Height = 0.5**.

Dentro crea un `PanelVictoria` a pantalla completa (anclajes Stretch en ambos ejes y cuatro márgenes a cero). En el componente `Image`, utiliza negro con un alfa aproximado de 180/255.

Añade dos elementos:

- `TextoVictoria`, mediante **Text - TextMeshPro**, con «¡NIVEL COMPLETADO!» en blanco y centrado.
- `BotonReiniciar`, mediante **Button - TextMeshPro**, con texto «REINICIAR».

Si Unity solicita **TMP Essentials**, impórtalos. No confundas `Text Style → Quote` con negrita: ese estilo pone el título en cursiva.

La jerarquía quedará así:

```text
CanvasFinNivel
└── PanelVictoria
    ├── TextoVictoria
    └── BotonReiniciar
        └── Text (TMP)
```

Para enlazar el comportamiento:

1. En `DetectorMeta`, arrastra `PanelVictoria` a la propiedad **Panel Victoria** del script `Meta`.
2. En `BotonReiniciar → Button → On Click()`, pulsa `+`, arrastra `DetectorMeta` y selecciona **Meta → ReiniciarNivel()**.
3. Ejecuta el juego. El panel se ocultará al iniciar y aparecerá cuando la bola entre en el detector; al pulsar el botón se recargará `Nivel1`.

Si el botón funciona en el editor pero no en Android, revisa que el `EventSystem` tenga el módulo de entrada de UI compatible con el Input System.

## 13. Qué comprobar antes de entregar

- La escena `Nivel1` está guardada y añadida a `Build Profiles → Scene List`.
- La bola responde a WASD en ordenador y a la inclinación en Android.
- El tablero conserva un agujero real con `Mesh Collider` no convexo.
- Las paredes interiores no dejan salidas involuntarias.
- `DetectorMeta` detecta la caída y muestra el panel.
- El botón reinicia la escena.
- El juego se instala en un teléfono real mediante **Build And Run**.

**Siguiente mejora:** ajustar el control por inclinación según la orientación física del móvil y añadir calibración, además de pulir el tamaño del tablero y la tipografía para las distintas pantallas.
