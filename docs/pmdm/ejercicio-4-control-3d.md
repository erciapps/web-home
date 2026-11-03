# Ejercicio 4 · Rescate 3D: personaje, cámaras y controles táctiles

Vamos a desarrollar un pequeño juego en **Unity 6.6 (URP)**: controlaremos a un Goblin que recorre un escenario, supera una escalera curva y **rescata tres Cube Pets**. También podrá encontrar un **bláster** que gira como objeto especial.

El ejercicio es **una única escena** que iremos ampliando. Primero entenderemos el movimiento con una esfera provisional; después la sustituiremos por un personaje animado. No es necesario crear un cubo para señalar el frente: utilizaremos el eje **Z positivo** del personaje.

!!! info "Qué vamos a aprender"
    Movimiento 3D con **Character Controller**, ProBuilder, seguimiento y cambio de cámara, **Input System**, controles táctiles que funcionan mientras se pulsan, importación de modelos y materiales URP, **Animator**, prefabs, **triggers**, objetos recolectables y una interfaz con contador.

## Recursos y preparación

Trabajaremos en un proyecto **Universal 3D (URP)** y utilizaremos estos recursos:

| Recurso | Uso en nuestra práctica |
| --- | --- |
| [Goblin Skirmisher (G4G Assets)](https://g4g-assets.itch.io/goblin-skirmisher-100-animations) | Personaje principal, con animaciones de reposo, caminar y girar |
| [Cube Pets Free (Sneznyj)](https://sneznyj.itch.io/cube-pets-free) | Tres animales para rescatar: pollito, conejo y mariquita |
| [Blaster Kit (Kenney)](https://kenney-assets.itch.io/blaster-kit) | Un bláster coleccionable y modelos de decoración |

El Goblin tiene **110 animaciones** y un paquete específico para Unity. Cube Pets Free incluye **tres modelos animados** en FBX, GLB y Blender. Blaster Kit contiene modelos de armas, objetivos y proyectiles. **Los archivos se descargan de las páginas originales**, no se redistribuyen con este enunciado.

!!! note "Alcance"
    El objetivo obligatorio es **mover al Goblin, cambiar de cámara y rescatar las tres mascotas**. El bláster será un objeto coleccionable sencillo. **Disparar proyectiles y crear enemigos** son posibles ampliaciones, no requisitos de esta primera versión.

## 1. Crear el escenario y un jugador provisional

Crea y guarda la escena **Control3D** en la carpeta **Assets/Scenes**.

### 1.1. Suelo

Crea **GameObject → 3D Object → Plane** y llámalo **Suelo**:

| Propiedad | Valor |
| --- | --- |
| Position | (0, 0, 0) |
| Scale | (2, 1, 2) |

Comprueba que conserva su **Mesh Collider**.

### 1.2. Jugador

Crea una **Sphere** llamada **Jugador**. También puedes utilizar una esfera de ProBuilder, como en clase.

| Propiedad | Valor inicial |
| --- | --- |
| Position | (0, 0.5, 0) |
| Rotation | (0, 0, 0) |
| Scale | (1, 1, 1) |

Añade el componente **Character Controller** y configura inicialmente:

| Propiedad | Valor |
| --- | ---: |
| Height | 1 |
| Radius | 0.5 |
| Center | (0, 0, 0) |
| Step Offset | 0.3 |
| Slope Limit | 45 |

Elimina el **Sphere Collider / Mesh Collider** adicional de la figura, si existe. **No añadas un Rigidbody al jugador**: utilizaremos CharacterController.Move.

!!! tip "Si utilizas ProBuilder"
    El pivote de la malla puede estar descentrado. Si la esfera parece flotar aunque Position sea (0, 0.5, 0), utiliza **Tools → ProBuilder → Object → Center Pivot** y comprueba de nuevo la posición. El centro visual de la malla y el **Center** del Character Controller no son lo mismo.

La jerarquía inicial será sencilla:

~~~text
Control3D
├── Main Camera
├── Directional Light
├── Global Volume
├── Suelo
└── Jugador
~~~

**No creamos el antiguo cubo Frente**. La dirección en la que avanzará el personaje es **transform.forward**, correspondiente a su eje Z positivo.

## 2. Movimiento con WASD

Crea **Assets/Scripts/MovimientoJugador.cs** y asigna el script a **Jugador**.

~~~csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MovimientoJugador : MonoBehaviour
{
    public float velocidad = 4f;
    public float velocidadGiro = 120f;
    public float gravedad = -9.81f;

    private CharacterController controlador;
    private float velocidadVertical;

    void Start()
    {
        controlador = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        float mover = 0f;
        float girar = 0f;

        if (Keyboard.current.wKey.isPressed) mover += 1f;
        if (Keyboard.current.sKey.isPressed) mover -= 1f;
        if (Keyboard.current.aKey.isPressed) girar -= 1f;
        if (Keyboard.current.dKey.isPressed) girar += 1f;

        // A/D giran el personaje sobre su eje Y.
        transform.Rotate(0, girar * velocidadGiro * Time.deltaTime, 0);

        // Character Controller no aplica gravedad automáticamente.
        if (controlador.isGrounded && velocidadVertical < 0f)
            velocidadVertical = -2f;

        velocidadVertical += gravedad * Time.deltaTime;

        // W/S avanzan o retroceden según la orientación del jugador.
        Vector3 movimiento = transform.forward * (mover * velocidad);
        movimiento.y = velocidadVertical;

        controlador.Move(movimiento * Time.deltaTime);
    }
}
~~~

| Tecla | Acción |
| --- | --- |
| W | Avanzar |
| S | Retroceder |
| A | Girar a la izquierda |
| D | Girar a la derecha |

**Comprueba en Play** que la figura gira y se desplaza, sin atravesar el suelo.

!!! note "Más adelante sustituiremos este script"
    Esta primera versión sirve para explicar WASD y la gravedad. En el **apartado 5** pondremos la versión completa para teclado y botones táctiles. No conserves las dos versiones del código a la vez.

## 3. Crear una escalera curva con ProBuilder

Con **ProBuilder** instalado, pulsa con el botón derecho en la jerarquía y selecciona **ProBuilder → Curved Stair**. Llámala **Escalera**.

En el generador de la forma, utiliza estos valores como punto de partida:

| Opción | Valor |
| --- | --- |
| Shape Size | Aproximadamente (8, 1.2, 4) |
| Steps Generation | Count |
| Steps Count | 6 |
| Circumference | 90° |
| Inner Radius | 2 |
| Sides | Activado |

La altura total es **1,2 unidades**, por lo que seis peldaños uniformes tienen una subida aproximada de **0,2 unidades** cada uno.

**Coloca la escalera visualmente** con las herramientas Move y Rotate. La entrada debe quedar al alcance del jugador. Comprueba en vista lateral que la base toca el suelo; según el pivote elegido, puede ser necesario elevar la forma aproximadamente **0,6 unidades**.

Verifica que conserva un **Mesh Collider** (no Trigger) para que podamos subir sus escalones.

Pulsa **Play** y sube utilizando **W** y los giros **A/D**. Al tratarse de una escalera curva, necesitaremos corregir la orientación mientras avanzamos.

!!! tip "Qué revisar si no sube"
    Comprueba el Mesh Collider de la escalera, la altura del primer peldaño, el **Step Offset** del Character Controller y que el jugador no esté enterrado en el suelo. No es necesario modificar Step Offset durante el ejercicio si ya funciona correctamente.

## 4. Cámara en tercera y primera persona

Selecciona **Main Camera** y añade el script **CamaraSeguimiento.cs**. La cámara seguirá la posición y orientación de Jugador.

~~~csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraSeguimiento : MonoBehaviour
{
    public Transform objetivo;
    public float distancia = 5f;
    public float altura = 3f;
    public float suavidad = 5f;

    private bool primeraPersona = false;

    // Permite cambiar de cámara desde un botón de la interfaz.
    public void CambiarVista()
    {
        primeraPersona = !primeraPersona;
    }

    void Update()
    {
        // Tecla C para cambiar la vista en el ordenador.
        if (Keyboard.current != null &&
            Keyboard.current.cKey.wasPressedThisFrame)
            CambiarVista();
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicion;
        Quaternion rotacion;

        if (primeraPersona)
        {
            // Ajustaremos esta altura cuando coloquemos el Goblin.
            posicion = objetivo.position
                     + objetivo.forward * 0.7f
                     + Vector3.up * 0.6f;

            rotacion = Quaternion.LookRotation(objetivo.forward);
        }
        else
        {
            posicion = objetivo.position
                     - objetivo.forward * distancia
                     + Vector3.up * altura;

            Vector3 mirada = objetivo.position + Vector3.up * 0.4f;
            rotacion = Quaternion.LookRotation(mirada - posicion);
        }

        float t = Mathf.Clamp01(suavidad * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, posicion, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, t);
    }
}
~~~

En el Inspector de **Main Camera**, arrastra **Jugador** al campo **Objetivo**.

Prueba **WASD** y pulsa **C** para alternar las vistas.

- **LateUpdate** permite colocar la cámara después de mover al personaje.
- **Lerp** suaviza el desplazamiento y **Slerp** suaviza la rotación.
- **CambiarVista()** será también el método que llamará un botón táctil.

!!! warning "Primera persona y modelos 3D"
    Cuando sustituyamos la esfera por el Goblin, revisaremos la altura de la vista en primera persona. Si la cámara atraviesa la cabeza del modelo, habrá que ajustar el desplazamiento frontal y vertical o esconder la malla propia para esa vista. **No necesitamos crear cubos que simulen manos o armas.**

## 5. Controles táctiles para Android

### 5.1. Crear la interfaz

Crea un **Canvas** llamado **CanvasControles**. En **Canvas Scaler** selecciona:

- **UI Scale Mode:** Scale With Screen Size.
- **Reference Resolution:** 1920 × 1080.
- **Screen Match Mode:** Match Width Or Height, con **Match = 0.5**.

Dentro crea cinco **Button - TextMeshPro**:

| Botón | Texto | Acción |
| --- | --- | --- |
| BtnAdelante | ↑ | Avanzar mientras se pulsa |
| BtnAtras | ↓ | Retroceder mientras se pulsa |
| BtnIzquierda | ↶ | Girar a la izquierda mientras se pulsa |
| BtnDerecha | ↷ | Girar a la derecha mientras se pulsa |
| BtnCamara | Cámara o C | Cambiar de vista una vez |

Coloca los cuatro botones de movimiento agrupados en la **esquina inferior izquierda** usando anclajes. Deja el botón de cámara en otra zona cómoda, por ejemplo la **esquina inferior derecha**.

!!! tip "Si no aparece el texto"
    Importa **Window → TextMeshPro → Import TMP Essential Resources** y asigna una fuente TMP al hijo **Text (TMP)**. Si la fuente no muestra bien las flechas, utiliza W, S, A y D.

Comprueba que la escena tiene un **EventSystem** con **Input System UI Input Module**.

### 5.2. Crear BotonTactil.cs

No utilizaremos **On Click()** para desplazarnos: solo se ejecuta una pulsación, mientras que necesitamos **mantener el movimiento** hasta levantar el dedo.

Crea este script:

~~~csharp
using UnityEngine;
using UnityEngine.EventSystems;

public class BotonTactil : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    public MovimientoJugador jugador;
    public MovimientoJugador.Control control;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (jugador != null)
            jugador.CambiarControl(control, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (jugador != null)
            jugador.CambiarControl(control, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (jugador != null)
            jugador.CambiarControl(control, false);
    }

    void OnDisable()
    {
        if (jugador != null)
            jugador.CambiarControl(control, false);
    }
}
~~~

Añade **BotonTactil** a los cuatro botones de movimiento, **no** a sus hijos Text (TMP). En cada botón arrastra **Jugador** al campo **Jugador** y elige el valor adecuado en **Control**.

| Objeto | Control |
| --- | --- |
| BtnAdelante | Adelante |
| BtnAtras | Atras |
| BtnIzquierda | Izquierda |
| BtnDerecha | Derecha |

El botón **BtnCamara** es distinto: en **Button → On Click()**, arrastra **Main Camera** y selecciona **CamaraSeguimiento → CambiarVista()**.

### 5.3. Sustituir MovimientoJugador.cs por su versión completa

La siguiente versión permite utilizar **WASD o los botones táctiles**. También deja preparado el parámetro **Movimiento** que usaremos después con las animaciones del Goblin.

**Sustituye completamente** el contenido del script anterior:

~~~csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MovimientoJugador : MonoBehaviour
{
    public float velocidad = 4f;
    public float velocidadGiro = 120f;
    public float gravedad = -9.81f;

    private CharacterController controlador;
    private Animator animador;
    private float velocidadVertical;

    // Botones táctiles mantenidos.
    private bool adelante;
    private bool atras;
    private bool izquierda;
    private bool derecha;

    public enum Control
    {
        Adelante,
        Atras,
        Izquierda,
        Derecha
    }

    void Start()
    {
        controlador = GetComponent<CharacterController>();
        animador = GetComponentInChildren<Animator>();
    }

    // Lo llaman los botones al pulsar o soltar.
    public void CambiarControl(Control control, bool pulsado)
    {
        switch (control)
        {
            case Control.Adelante:
                adelante = pulsado;
                break;
            case Control.Atras:
                atras = pulsado;
                break;
            case Control.Izquierda:
                izquierda = pulsado;
                break;
            case Control.Derecha:
                derecha = pulsado;
                break;
        }
    }

    void Update()
    {
        // El teclado es opcional: en Android puede no existir.
        bool w = Keyboard.current != null &&
                 Keyboard.current.wKey.isPressed;
        bool s = Keyboard.current != null &&
                 Keyboard.current.sKey.isPressed;
        bool a = Keyboard.current != null &&
                 Keyboard.current.aKey.isPressed;
        bool d = Keyboard.current != null &&
                 Keyboard.current.dKey.isPressed;

        float mover = 0f;
        float girar = 0f;

        if (w || adelante) mover += 1f;
        if (s || atras) mover -= 1f;
        if (a || izquierda) girar -= 1f;
        if (d || derecha) girar += 1f;

        transform.Rotate(0, girar * velocidadGiro * Time.deltaTime, 0);

        if (controlador.isGrounded && velocidadVertical < 0f)
            velocidadVertical = -2f;

        velocidadVertical += gravedad * Time.deltaTime;

        Vector3 movimiento = transform.forward * (mover * velocidad);
        movimiento.y = velocidadVertical;
        controlador.Move(movimiento * Time.deltaTime);

        // Estados del Animator, cuando añadamos el Goblin.
        if (animador != null)
        {
            int estado = 0; // Idle

            if (mover != 0f)
            {
                if (girar < 0f) estado = 4;       // WalkTurnLeft
                else if (girar > 0f) estado = 5;  // WalkTurnRight
                else estado = 1;                  // Walking
            }
            else
            {
                if (girar < 0f) estado = 2;       // TurnLeft
                else if (girar > 0f) estado = 3;  // TurnRight
            }

            animador.SetInteger("Movimiento", estado);
        }
    }
}
~~~

**Prueba en Game** que mantener pulsado cada botón con el ratón produce movimiento continuo y que al soltarlo el personaje se detiene. Comprueba que WASD sigue funcionando.

## 6. Sustituir la esfera por el Goblin

### 6.1. Importar el personaje

Descarga el archivo **GoblinSkirmisher-Unity-1.0.0.zip** de la página enlazada al principio.

1. Descomprime el ZIP.
2. En Unity, selecciona **Assets → Import Package → Custom Package**.
3. Importa **GoblinSkirmisher.unitypackage**.
4. En **Assets/GoblinSkirmisher/Prefabs**, arrastra el prefab **GoblinSkirmisher** **dentro de Jugador**.

**Conservamos Jugador** como objeto principal, porque tiene el Character Controller, MovimientoJugador y las referencias de los botones y la cámara. Desactiva solamente su **Mesh Renderer** para ocultar la esfera.

~~~text
Jugador                         ← MovimientoJugador + Character Controller
└── GoblinSkirmisher            ← Modelo visual (prefab)
    └── GoblinSkirmisher        ← Modelo animado, con Animator
        └── GoblinRig           ← Esqueleto y mallas
~~~

La estructura del prefab puede tener otros hijos, pero lo importante es **no añadir un segundo Animator al objeto Jugador**: utiliza el que ya trae el modelo animado dentro del prefab.

### 6.2. Corregir los materiales rosa o blancos

El proyecto utiliza **URP**. Si el Goblin aparece rosa o sin textura:

1. Abre **Assets/GoblinSkirmisher/Materials**.
2. Selecciona el material **GoblinSkirmisher**.
3. Establece el shader **Universal Render Pipeline/Lit**.
4. En **Base Map**, arrastra la textura correspondiente desde la carpeta **Textures**, sobre el **cuadrado pequeño** del mapa.
5. Repite el proceso con el material **Dagger**, utilizando su textura.

No hay que modificar los FBX ni rehacer las animaciones para corregir los materiales.

### 6.3. Ajustar posición y colisiones

El Goblin mide aproximadamente **1,35 unidades**. Como punto de partida:

- En el hijo visual **GoblinSkirmisher**: Position local aproximada **(0, -0.5, 0)**, Rotation (0, 0, 0) y Scale (1, 1, 1).
- En **Jugador / Character Controller**: Height **1.35**, Radius **0.3**, Center Y aproximado **0.175**, manteniendo Jugador en Y **0.5**.

Son valores **orientativos**: ajusta el modelo para que **sus pies toquen el suelo** y la cápsula del controlador se adapte al cuerpo. Comprueba también que el Goblin mire hacia la dirección **Z positiva de Jugador**. Si hace falta girar el modelo, gira **solo el hijo visual**.

En el Animator del Goblin desactiva **Apply Root Motion**. El movimiento real seguirá controlándolo **MovimientoJugador.cs**.

## 7. Configurar las animaciones de caminar y girar

El paquete incluye **clips y un controlador de previsualización**, pero no un sistema de control completo conectado a nuestros botones. Crearemos un Animator Controller sencillo para nuestra práctica.

Crea **Assets/Animators/JugadorAnimator** desde **Create → Animation → Animator Controller**.

Arrastra estos clips desde **Assets/GoblinSkirmisher/Animations**:

| Estado en nuestro Animator | Clip del paquete | Movimiento |
| --- | --- | --- |
| Idle | A000_Idle | Quieto |
| Walking | A-02_Walking | Avanzar o retroceder |
| TurnLeft | A576_Idle_Turn_Left | Girar quieto a la izquierda |
| TurnRight | A586_Idle_Turn_Right | Girar quieto a la derecha |
| WalkTurnLeft | A572_Walk_Turn_Left | Avanzar y girar a la izquierda |
| WalkTurnRight | A583_Walk_Turn_Right | Avanzar y girar a la derecha |

Haz **Idle** el estado predeterminado (naranja).

### 7.1. Un parámetro para seis estados

En **Animator → Parameters**, crea un parámetro de tipo **Int** llamado **Movimiento**.

| Movimiento | Animación | Entrada |
| ---: | --- | --- |
| 0 | Idle | Sin pulsar |
| 1 | Walking | W o S |
| 2 | TurnLeft | A |
| 3 | TurnRight | D |
| 4 | WalkTurnLeft | W + A |
| 5 | WalkTurnRight | W + D |

### 7.2. Transiciones

Crea **seis transiciones desde Any State**, una hacia cada estado. En cada transición:

- **Has Exit Time:** desactivado.
- **Fixed Duration:** activado; **Transition Duration:** 0.1 segundos como punto de partida.
- **Can Transition To Self:** desactivado.
- **Condition:** **Movimiento Equals** el número de la tabla.

No mantengas transiciones antiguas que dependan de otros parámetros, como **Caminando**, si has pasado a utilizar **Movimiento**.

!!! tip "Has Exit Time"
    Si está activado, Unity espera al momento de salida configurado del clip antes de permitir la transición. Para responder rápidamente a nuestros botones, lo desactivamos. **Transition Duration** regula, en cambio, cuánto dura la mezcla entre animaciones.

### 7.3. Asignar el controlador correcto

Selecciona el **objeto interior del prefab GoblinSkirmisher que contiene Animator**. En el campo **Controller**, asigna **JugadorAnimator**.

No añadas el Animator al objeto padre Jugador. El código ya utiliza **GetComponentInChildren<Animator>()** para encontrar el componente del modelo.

Pulsa **Play** y comprueba las seis acciones con el teclado o los botones táctiles.

!!! warning "Animaciones con desplazamiento y bucles"
    El paquete conserva movimientos de cadera en algunos clips. Como el desplazamiento del juego lo controla Character Controller, verifica que **Apply Root Motion** esté desactivado y revisa si algún clip produce saltos o deslizamientos. Comprueba que **Idle y Walking** se repiten correctamente; en las animaciones cortas de giro, prueba el bucle antes de activarlo, porque no todas están preparadas para repetirse sin salto.

## 8. Rescatar tres Cube Pets

Ahora añadiremos un objetivo al juego: **encontrar y rescatar el pollito, el conejo y la mariquita**.

### 8.1. Importar los animales

Descarga **Cube Pets Free** desde el enlace de recursos y descomprime el ZIP. Importa en **Assets/Models/CubePets** los **FBX y sus texturas** (el paquete también ofrece GLB y Blender).

Coloca un animal cerca del inicio, otro en el recorrido de la escalera y el tercero en una zona que requiera explorar el escenario. **No dependas de coordenadas idénticas para todas las escenas:** ajusta posición y escala según el modelo.

Si sus materiales no muestran el aspecto adecuado, crea materiales **Universal Render Pipeline/Lit** y asigna sus mapas de color. Puedes utilizar su animación **Idle** si la has configurado; para la mecánica de rescate basta con que los animales sean visibles.

### 8.2. Preparar cada mascota para detectar el contacto

En cada animal crea un objeto raíz con un nombre identificable, por ejemplo **MascotaPollito**, que contenga el modelo visual.

Añade al objeto raíz:

1. **Box Collider** o **Sphere Collider**, ajustado al tamaño del animal.
2. **Is Trigger:** activado.
3. **Rigidbody** con **Use Gravity** desactivado e **Is Kinematic** activado.
4. El script **Recolectable**, que crearemos en el apartado siguiente.

En el objeto **Jugador**, cambia la etiqueta (**Tag**) a **Player**. Conserva su Character Controller.

Después de configurar la primera mascota, arrástrala a **Assets/Prefabs** para crear un **Prefab**. Repite con los otros dos animales.

### 8.3. Interfaz de rescate

En el Canvas añade dos textos **Text - TextMeshPro**:

- **TextoContador**: mostrará **Mascotas: 0/3** y si tenemos el bláster.
- **TextoVictoria**: mostrará **¡Has rescatado a todas las mascotas!**. Déjalo visible en el editor para configurarlo; el script lo ocultará al iniciar.

Crea un objeto vacío **GestorJuego** y añade el script **GestorRescate.cs**:

~~~csharp
using UnityEngine;
using TMPro;

public class GestorRescate : MonoBehaviour
{
    public TMP_Text textoContador;
    public GameObject textoVictoria;
    public int totalMascotas = 3;

    private int mascotasRescatadas = 0;
    private bool blasterRecogido = false;

    void Start()
    {
        if (textoVictoria != null)
            textoVictoria.SetActive(false);

        ActualizarInterfaz();
    }

    public void Recoger(TipoRecolectable tipo)
    {
        if (tipo == TipoRecolectable.Mascota)
            mascotasRescatadas = Mathf.Min(
                mascotasRescatadas + 1, totalMascotas);
        else
            blasterRecogido = true;

        ActualizarInterfaz();

        if (mascotasRescatadas >= totalMascotas &&
            textoVictoria != null)
            textoVictoria.SetActive(true);
    }

    void ActualizarInterfaz()
    {
        if (textoContador == null) return;

        string blaster = blasterRecogido ? "Sí" : "No";
        textoContador.text =
            $"Mascotas: {mascotasRescatadas}/{totalMascotas} | " +
            $"Bláster: {blaster}";
    }
}
~~~

En el Inspector de **GestorJuego** asigna los dos textos a sus campos.

### 8.4. Script de recolección

Crea **Recolectable.cs**:

~~~csharp
using UnityEngine;

public enum TipoRecolectable
{
    Mascota,
    Blaster
}

public class Recolectable : MonoBehaviour
{
    public TipoRecolectable tipo = TipoRecolectable.Mascota;
    public float velocidadGiro = 0f;

    private bool recogido = false;

    void Update()
    {
        // Si velocidadGiro es 0, el objeto no gira.
        transform.Rotate(0, velocidadGiro * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (recogido || !other.CompareTag("Player")) return;

        GestorRescate gestor =
            FindFirstObjectByType<GestorRescate>();

        if (gestor == null) return;

        recogido = true;
        gestor.Recoger(tipo);

        Destroy(gameObject);
    }
}
~~~

En las tres mascotas configura:

- **Tipo:** Mascota.
- **Velocidad Giro:** 0 (de momento no necesitan girar).

**Prueba:** al tocar cada mascota con el Goblin debe desaparecer y el contador debe avanzar de **0/3** a **3/3**. Al rescatar la tercera aparecerá el mensaje de victoria.

!!! note "Por qué usamos un Trigger"
    Un **Collider con Is Trigger** detecta el contacto sin bloquear el desplazamiento del personaje. El Rigidbody cinemático del coleccionable permite configurar la detección de forma sencilla, mientras que el jugador sigue utilizando **Character Controller**. **Destroy(gameObject)** elimina el objeto raíz de la mascota, junto con su modelo.

## 9. El bláster como objeto coleccionable que gira

Descarga el **Blaster Kit** de Kenney y elige **uno de sus modelos FBX**. Puedes utilizar también algún objetivo o caja del paquete para decorar el nivel, pero **no programaremos disparos todavía**.

Coloca el bláster en una zona visible del escenario, por ejemplo cerca de la escalera. Llámalo **Blaster**.

Como ya hemos comprobado en clase, **no hace falta crear un objeto vacío solo para hacerlo girar**: podemos asignar un script directamente al modelo cuando su pivote está bien colocado.

### 9.1. GirarObjeto.cs

~~~csharp
using UnityEngine;

public class GirarObjeto : MonoBehaviour
{
    public float velocidad = 90f;

    void Update()
    {
        transform.Rotate(0, velocidad * Time.deltaTime, 0);
    }
}
~~~

Asigna **GirarObjeto** al bláster y comprueba que gira sobre el eje Y. Si gira descentrado, revisa el pivote del modelo antes de complicar la jerarquía.

### 9.2. Hacerlo coleccionable

Igual que con las mascotas, añade **un Collider con Is Trigger** y **un Rigidbody cinemático sin gravedad** al objeto que detectará la recogida.

Añade **Recolectable**, pero configura:

- **Tipo:** Blaster.
- **Velocidad Giro:** 0, porque ya gira con **GirarObjeto**.

Al tocar el objeto, desaparecerá y el contador indicará **Bláster: Sí**.

!!! tip "Evitar duplicar el giro"
    Si prefieres utilizar únicamente **Recolectable**, elimina **GirarObjeto** del bláster y establece **Velocidad Giro = 90** en Recolectable. Las dos soluciones son válidas; **no actives ambas rotaciones a la vez**.

Recoger el bláster **no sustituye el objetivo principal**: el nivel se completa cuando rescatas las **tres mascotas**.

## 10. Preparar la prueba en Android

Cuando todo funcione en el editor:

1. Comprueba que Unity tiene instalado **Android Build Support** y sus herramientas necesarias.
2. Abre **File → Build Profiles** y selecciona **Android**; cambia al perfil Android si fuera necesario.
3. Añade la escena **Control3D** a la lista de escenas de la compilación.
4. Configura el juego en **orientación horizontal** y revisa la colocación de los controles en **Game**, con una relación de aspecto parecida a la del móvil.
5. Genera la aplicación y pruébala en un dispositivo Android.

En el móvil debemos comprobar especialmente las **pulsaciones mantenidas**, la posibilidad de **avanzar y girar a la vez**, el botón de cámara y la recogida de objetos.

## Resultado mínimo y comprobaciones

La práctica estará terminada cuando se cumpla lo siguiente:

- [ ] El Goblin reemplaza visualmente a la esfera, pero **Jugador** mantiene Character Controller y MovimientoJugador.
- [ ] Se mueve con **WASD** y con los **cuatro botones táctiles**.
- [ ] Puede recorrer la **escalera curva**.
- [ ] La cámara lo sigue y cambia entre **primera y tercera persona** con **C** o el botón.
- [ ] El Goblin utiliza **Idle**, **Walking** y animaciones de giro mediante **Animator**.
- [ ] Los **tres Cube Pets** se recogen por contacto y actualizan el contador.
- [ ] El **bláster gira** y puede recogerse como objeto especial.
- [ ] Al rescatar las tres mascotas aparece un **mensaje de victoria**.
- [ ] La escena puede probarse en **Android** con controles táctiles.

!!! info "Ampliaciones voluntarias"
    Podemos añadir una animación de recogida (**A284_Collect_Object**), una de victoria (**A412_Victory**), efectos de sonido, objetivos del Blaster Kit y disparos. **No son necesarios para comprender ni superar el núcleo de este ejercicio.**

!!! note "Uso de recursos externos"
    Descarga los modelos desde los enlaces originales. **Goblin Skirmisher** y **Cube Pets Free** permiten utilizarlos en proyectos, pero prohíben redistribuir los archivos originales como paquete de recursos. **Blaster Kit** se distribuye bajo licencia **CC0**. Cita a los autores en los créditos del juego por buena práctica y revisa las condiciones que acompañan cada descarga.

---

[← Ejercicio 3 · Clasificador de animales](ejercicio-3-animales.md) · [Volver a PMDM](index.md)
