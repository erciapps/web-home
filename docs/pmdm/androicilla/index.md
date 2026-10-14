# Androicilla: videojuego 3D para Android con sensores

Vamos a construir un laberinto en Unity: una bola recorre un circuito y debe caer por un agujero para completar el nivel. Primero lo controlaremos desde el teclado y después con la inclinación del teléfono. El agujero será real, con su correspondiente colisión, no una imagen pintada en el suelo.

**Entorno utilizado:** Unity 6.6 (`6000.6.0f1`), proyecto Universal 3D (URP), Rider, Input System, ProBuilder 6.1.3 y un teléfono Android con depuración USB.

Este tutorial recoge el procedimiento utilizado en clase y las correcciones que hubo que hacer. Las capturas corresponden al desarrollo del proyecto **Androicilla**.

## 1. Proyecto y preparación

En **Unity Hub → Nuevo proyecto**, escoge **Universal 3D** y nombra el proyecto `Androicilla`. En **Instalaciones → Gestionar módulos** comprueba que estén instalados **Android Build Support**, **Android SDK & NDK Tools** y **OpenJDK**.

![Creación de proyecto en Unity Hub](../../assets/images/pmdm/androicilla/20261013-203002.png){ loading=lazy }

![Versión del editor instalada](../../assets/images/pmdm/androicilla/20261013-203028.png){ loading=lazy }

![Módulos de Android](../../assets/images/pmdm/androicilla/20261013-203046.png){ loading=lazy }

![Proyecto abierto en Unity](../../assets/images/pmdm/androicilla/20261013-203448.png){ loading=lazy }

Dentro de `Assets`, crea `Materials`, `Prefabs`, `Scenes` y `Scripts`. Guarda la escena inicial como `Nivel1` dentro de `Scenes`. Unity ya incluye otras carpetas de la plantilla que podemos conservar.

![Carpetas iniciales](../../assets/images/pmdm/androicilla/20261013-203633.png){ loading=lazy }

![Escena Nivel1 guardada](../../assets/images/pmdm/androicilla/20261013-203645.png){ loading=lazy }

## 2. Suelo y paredes exteriores

Crea **3D Object → Plane**, llámalo `Suelo` y configura `Position (0, 0, 0)` y `Scale (2, 1, 2)`. Obtendrás una superficie de 20 × 20 unidades. Comprueba que tiene `Mesh Collider`.

Crea después cuatro **3D Object → Cube**, con estas dimensiones:

| Objeto | Position (X, Y, Z) | Scale (X, Y, Z) |
|---|---|---|
| ParedNorte | (0, 0.5, 10) | (20, 1, 0.5) |
| ParedSur | (0, 0.5, -10) | (20, 1, 0.5) |
| ParedEste | (10, 0.5, 0) | (0.5, 1, 20) |
| ParedOeste | (-10, 0.5, 0) | (0.5, 1, 20) |

Los cubos ya incluyen `Box Collider`. Podemos seleccionar las cuatro paredes y agruparlas bajo un GameObject `Bordes`, manteniendo los colliders; si las fusionamos con ProBuilder, hay que revisar el collider de la malla resultante.

![Crear un Plane](../../assets/images/pmdm/androicilla/20261013-203746.png){ loading=lazy }

![Configurar Suelo y su Mesh Collider](../../assets/images/pmdm/androicilla/20261013-203844.png){ loading=lazy }

![Primera pared del recinto](../../assets/images/pmdm/androicilla/20261013-204111.png){ loading=lazy }

![Cuatro paredes exteriores](../../assets/images/pmdm/androicilla/20261013-204319.png){ loading=lazy }

## 3. La bola y las físicas

Crea **3D Object → Sphere**, llámala `Jugador`, ponle `Position (0, 1, 0)` y `Scale (1, 1, 1)`. Añade un `Rigidbody` **3D** (no `Rigidbody 2D`). Mantén el `Sphere Collider` de la esfera.

| Propiedad Rigidbody | Valor |
|---|---|
| Mass | 1 |
| Use Gravity | Sí |
| Is Kinematic | No |
| Interpolate | Interpolate |
| Collision Detection | Continuous |

Prueba **Play** y comprueba que la bola cae y se apoya en el plano. Cuando abramos el agujero, moveremos la posición inicial a un lugar seguro.

![Creación de Jugador](../../assets/images/pmdm/androicilla/20261013-204452.png){ loading=lazy }

![Rigidbody y Sphere Collider](../../assets/images/pmdm/androicilla/20261013-204631.png){ loading=lazy }

![Prueba de caída](../../assets/images/pmdm/androicilla/bolacae.gif){ loading=lazy }

![Ajustes finales del Rigidbody](../../assets/images/pmdm/androicilla/20261013-205218.png){ loading=lazy }

## 4. Cámara

Para empezar utilizamos una cámara cenital: selecciona `Main Camera`, configura `Position (0, 24, 0)` y `Rotation (90, 0, 0)`. En el componente `Camera` elige **Orthographic**, `Size = 15`.

Más tarde usamos una vista con algo de profundidad: `Position (0, 24, -10)` y `Rotation (65, 0, 0)`. La cámara se inclina, pero **la física de la bola sigue usando X y Z** del mundo. Conviene revisar el resultado en la pestaña `Game`.

## 5. Movimiento con teclado y sensores

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

![MovimientoBola en Rider](../../assets/images/pmdm/androicilla/20261013-205758.png){ loading=lazy }

![Prueba con teclado](../../assets/images/pmdm/androicilla/movimiento.gif){ loading=lazy }

![Lectura GravitySensor añadida](../../assets/images/pmdm/androicilla/20261013-210852.png){ loading=lazy }

## 6. Ejecutar en Android

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

![Menú Build Profiles](../../assets/images/pmdm/androicilla/20261013-210156.png){ loading=lazy }

![Añadir Android](../../assets/images/pmdm/androicilla/20261013-210227.png){ loading=lazy }

![Perfil Android activo](../../assets/images/pmdm/androicilla/20261013-210430.png){ loading=lazy }

![Orientación Landscape Left](../../assets/images/pmdm/androicilla/20261013-210627.png){ loading=lazy }

![Nombre del APK](../../assets/images/pmdm/androicilla/20261013-211038.png){ loading=lazy }

## 7. Crear el agujero de la meta con ProBuilder

Necesitamos un agujero **real** en el tablero, para que la bola lo atraviese. Un cilindro oscuro colocado encima del suelo solo lo simularía visualmente.

Instala **ProBuilder** desde **Package Manager → Unity Registry**. En la versión utilizada, las operaciones booleanas estaban disponibles tras activar **Edit → Preferences → ProBuilder → Experimental Features Enabled**. Unity recompila scripts al cambiarlo; si Rider está interfiriendo con el proceso, ciérralo antes.

Utiliza un **tablero con grosor**, por ejemplo un cubo plano. Crea también un cilindro que lo atraviese de arriba abajo y verifica que su diámetro permita pasar a la bola. Convierte ambos objetos a ProBuilder con **Tools → ProBuilder → Object → Pro Builderize**.

Abre **Tools → ProBuilder → Experimental → Boolean (CSG) Tool**, establece **Subtraction**, con el suelo como primer objeto y el cilindro como segundo, y pulsa **Apply**. Conserva el resultado como `Tablero`; desactiva el suelo y el cilindro originales.

![Instalar ProBuilder](../../assets/images/pmdm/androicilla/20261014-062108.png){ loading=lazy }

![Activar funciones experimentales](../../assets/images/pmdm/androicilla/20261014-062207.png){ loading=lazy }

![Menú ProBuilder](../../assets/images/pmdm/androicilla/20261014-062338.png){ loading=lazy }

![Panel Pro Builderize](../../assets/images/pmdm/androicilla/20261014-062812.png){ loading=lazy }

![Preparar la geometría](../../assets/images/pmdm/androicilla/20261014-062901.png){ loading=lazy }

![Abrir CSG](../../assets/images/pmdm/androicilla/20261014-063211.png){ loading=lazy }

![Sustracción de cilindro](../../assets/images/pmdm/androicilla/20261014-063230.png){ loading=lazy }

![Resultado de la operación](../../assets/images/pmdm/androicilla/20261014-063837.png){ loading=lazy }

![Tablero con agujero](../../assets/images/pmdm/androicilla/20261014-063922.png){ loading=lazy }

![Tablero resultante](../../assets/images/pmdm/androicilla/20261014-064007.png){ loading=lazy }

### El problema que encontramos con el collider

Al principio el agujero se veía, pero la bola **no caía**. El motivo era que `Tablero` conservaba un `Box Collider` rectangular que tapaba el hueco físicamente.

Selecciona `Tablero`, elimina su `Box Collider` y añade **Mesh Collider** con la malla resultante. Mantén `Convex` **desactivado** e `Is Trigger` desactivado. No añadas Rigidbody al tablero.

El aviso de Unity sobre *pre-baked triangle collision* no es el problema que impide la caída, aunque habrá que revisar esa optimización antes de distribuir el juego.

![Mesh Collider no convexo](../../assets/images/pmdm/androicilla/20261014-064235.png){ loading=lazy }

![Caída por el agujero](../../assets/images/pmdm/androicilla/caebola.gif){ loading=lazy }

## 8. Diseñar el circuito

Añade paredes interiores para obligar a rodear obstáculos. Una primera distribución posible:

| Objeto | Position | Scale |
|---|---|---|
| Obstaculo1 | (-3, 0.5, -5) | (14, 1, 0.5) |
| Obstaculo2 | (3, 0.5, 1) | (14, 1, 0.5) |
| Obstaculo3 | (-3, 0.5, 7) | (14, 1, 0.5) |

Durante la práctica ajustamos el recorrido y añadimos otras paredes. Cada cubo debe conservar su `Box Collider`. Puedes agruparlos bajo `Obstaculos`; no es necesario fusionarlos durante el diseño. Comprueba desde arriba que existe un camino transitable desde la salida hasta el agujero.

![Cámara inclinada](../../assets/images/pmdm/androicilla/20261014-065603.png){ loading=lazy }

![Jerarquía simplificada](../../assets/images/pmdm/androicilla/20261014-070147.png){ loading=lazy }

![Agrupación Bordes](../../assets/images/pmdm/androicilla/20261014-070157.png){ loading=lazy }

![Circuito de obstáculos](../../assets/images/pmdm/androicilla/20261014-072224.png){ loading=lazy }

## 9. Materiales

En `Assets/Materials` crea los materiales `M_Suelo`, `M_Paredes` y `M_Bola` con el shader **Universal Render Pipeline/Lit**.

Aplica marrón al tablero, amarillo/madera a las paredes y gris metálico a la esfera. Para el metal puedes aumentar `Metallic` y `Smoothness` y comprobar el resultado según la iluminación. Estos son **materiales visuales**; no sustituyen a los colliders ni a los Physics Materials.

![Crear material](../../assets/images/pmdm/androicilla/20261014-072316.png){ loading=lazy }

![Color base de suelo](../../assets/images/pmdm/androicilla/20261014-072536.png){ loading=lazy }

![Materiales aplicados al circuito](../../assets/images/pmdm/androicilla/20261014-072832.png){ loading=lazy }

## 10. Detectar la caída en la meta

Crea un GameObject vacío llamado `DetectorMeta`, colócalo debajo del agujero y añade un `Box Collider` con **Is Trigger** activado. Debe tener volumen, no escala cero, y estar colocado de forma que detecte la bola cuando empieza a caer, nunca antes.

Selecciona `Jugador` y establece **Tag = Player**.

Crea `Assets/Scripts/Meta.cs` y asígnalo a `DetectorMeta`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    public GameObject panelVictoria;
    private bool completado = false;

    private void Start()
    {
        panelVictoria.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !completado)
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

Primero hicimos una prueba con `Debug.Log` y comprobamos la aparición de `¡Nivel completado!` en la consola.

![DetectorMeta e Is Trigger](../../assets/images/pmdm/androicilla/20261014-073315.png){ loading=lazy }

![Script de detección de meta](../../assets/images/pmdm/androicilla/20261014-073529.png){ loading=lazy }

![Mensaje en Console](../../assets/images/pmdm/androicilla/20261014-074121.png){ loading=lazy }

## 11. Pantalla de victoria

Crea **UI (Canvas) → Canvas** y renómbralo `CanvasFinNivel`. El componente `Canvas Scaler` debe utilizar **Scale With Screen Size**, con referencia **1920 × 1080** y `Match = 0.5`.

Dentro del Canvas crea `PanelVictoria` y configúralo para ocupar toda la pantalla (anclajes Stretch y márgenes a cero). Usa un `Image` negro semitransparente, con alfa aproximado de 180/255.

Dentro del panel añade un **Text - TextMeshPro** (`TextoVictoria`) con «¡NIVEL COMPLETADO!» y un **Button - TextMeshPro** (`BotonReiniciar`) con «REINICIAR». Importa **TMP Essentials** si Unity lo solicita. Utiliza el estilo normal/negrita si no quieres texto en cursiva: el preset *Quote* lo inclina.

La jerarquía debe quedar así:

```text
CanvasFinNivel
└── PanelVictoria
    ├── TextoVictoria
    └── BotonReiniciar
        └── Text (TMP)
```

En el componente `Meta` de `DetectorMeta`, arrastra `PanelVictoria` al campo `Panel Victoria`. En **BotonReiniciar → Button → On Click()**, pulsa **+**, arrastra `DetectorMeta` y escoge **Meta → ReiniciarNivel()**.

Al pulsar Play, el panel debe desaparecer; al caer la bola, aparecerá la victoria; al pulsar Reiniciar, se recargará `Nivel1`. Comprueba que la escena esté incluida en `Build Profiles → Scene List`.

![Crear Canvas](../../assets/images/pmdm/androicilla/20261014-074213.png){ loading=lazy }

![Configurar Canvas Scaler](../../assets/images/pmdm/androicilla/20261014-074525.png){ loading=lazy }

![Crear PanelVictoria](../../assets/images/pmdm/androicilla/20261014-074813.png){ loading=lazy }

![Texto TMP](../../assets/images/pmdm/androicilla/20261014-074958.png){ loading=lazy }

![Crear botón](../../assets/images/pmdm/androicilla/20261014-075048.png){ loading=lazy }

![Interfaz final](../../assets/images/pmdm/androicilla/20261014-075111.png){ loading=lazy }

![Código Meta final](../../assets/images/pmdm/androicilla/20261014-075318.png){ loading=lazy }

![Enlazar PanelVictoria](../../assets/images/pmdm/androicilla/20261014-075428.png){ loading=lazy }

![Configurar On Click](../../assets/images/pmdm/androicilla/20261014-075537.png){ loading=lazy }

![Vista Game](../../assets/images/pmdm/androicilla/20261014-075850.png){ loading=lazy }

![Pantalla victoria en ejecución](../../assets/images/pmdm/androicilla/20261014-075930.png){ loading=lazy }

![Reinicio nivel](../../assets/images/pmdm/androicilla/20261014-080027.png){ loading=lazy }

![Prueba animada de nivel completado](../../assets/images/pmdm/androicilla/nivel_completado.gif){ loading=lazy }

## 12. Comprobaciones finales

- [ ] El proyecto se abre sin errores de compilación.
- [ ] `Nivel1` está guardada e incluida en Android Build Profiles.
- [ ] La bola tiene `Rigidbody` y `Sphere Collider`.
- [ ] El teclado mueve la bola en Windows y Linux.
- [ ] El sensor funciona al menos en el teléfono Android de prueba y se comprueba el sentido de los ejes.
- [ ] El tablero tiene `Mesh Collider` **no convexo** y la bola cae por el agujero.
- [ ] Las paredes interiores conservan sus colliders.
- [ ] El Trigger de meta muestra la victoria al caer.
- [ ] El botón de reinicio vuelve a cargar la escena.

### Próximos cambios

El control por inclinación puede hacerse más cómodo incorporando calibración, inversión de ejes y límite de velocidad. También falta ajustar el encuadre para distintas relaciones de aspecto y mejorar el diseño del panel. No forman parte de la versión que hemos probado en estas capturas.

## Anexo. Otras capturas de la práctica

Estas capturas adicionales se conservaron como referencia del proceso, aunque no eran necesarias para reproducir cada paso.

![20261013-204007](../../assets/images/pmdm/androicilla/20261013-204007.png){ loading=lazy }

![20261013-204554](../../assets/images/pmdm/androicilla/20261013-204554.png){ loading=lazy }

![20261013-204711](../../assets/images/pmdm/androicilla/20261013-204711.png){ loading=lazy }

![20261013-205101](../../assets/images/pmdm/androicilla/20261013-205101.png){ loading=lazy }

![20261013-205351](../../assets/images/pmdm/androicilla/20261013-205351.png){ loading=lazy }

![20261014-064226](../../assets/images/pmdm/androicilla/20261014-064226.png){ loading=lazy }
