# 10. Peligros y reinicio del nivel

Ya podemos recoger [monedas](10-objetos-recogibles.md). Ahora añadiremos una **zona de caída debajo del mapa** y **obstáculos que reinician el nivel al tocar al Player**. Utilizaremos triggers, como en la moneda, pero esta vez recargaremos la escena.

## 1. Crear la zona de caída

1. Detén Play. En **Hierarchy**, pulsa el botón derecho y elige **2D Object → Sprites → Square**.

<figure class="unity-shot" markdown="1">

[![Crea el cuadrado desde Hierarchy → 2D Object → Sprites → Square](../../assets/images/pmdm/unity/89-unity-crear-killzone.png){ loading=lazy }](../../assets/images/pmdm/unity/89-unity-crear-killzone.png){ aria-label="Ampliar imagen" }

<figcaption>Crea el cuadrado desde Hierarchy → 2D Object → Sprites → Square</figcaption>
</figure>

2. Renombra el objeto como **Killzone**.
3. Comprueba que es un objeto independiente: **debe quedar fuera del Player**, para que no se mueva con él. Si se ha creado como hijo, arrástralo al nivel raíz de Hierarchy.

<figure class="unity-shot" markdown="1">

[![La zona de caída es un objeto independiente del Player y del Grid](../../assets/images/pmdm/unity/90-unity-killzone-hierarchy.png){ loading=lazy }](../../assets/images/pmdm/unity/90-unity-killzone-hierarchy.png){ aria-label="Ampliar imagen" }

<figcaption>La zona de caída es un objeto independiente del Player y del Grid</figcaption>
</figure>

4. Muévelo **por debajo de las plataformas más bajas** y amplía su escala X para cubrir todo el ancho por el que puede caer el jugador. Dale altura suficiente para detectar la caída.

<figure class="unity-shot" markdown="1">

[![Mueve y amplía el cuadrado para cubrir la zona situada debajo del mapa](../../assets/images/pmdm/unity/91-unity-ajustar-killzone.gif){ loading=lazy }](../../assets/images/pmdm/unity/91-unity-ajustar-killzone.gif){ aria-label="Ampliar imagen" }

<figcaption>Mueve y amplía el cuadrado para cubrir la zona situada debajo del mapa</figcaption>
</figure>

5. Añade **Box Collider 2D** si todavía no lo tiene.
6. Usa **Edit Collider** para revisar la zona y marca **Is Trigger**.

<figure class="unity-shot" markdown="1">

[![Killzone extendida bajo el nivel, con Box Collider 2D e Is Trigger activado](../../assets/images/pmdm/unity/92-unity-killzone-trigger.png){ loading=lazy }](../../assets/images/pmdm/unity/92-unity-killzone-trigger.png){ aria-label="Ampliar imagen" }

<figcaption>Killzone extendida bajo el nivel, con Box Collider 2D e Is Trigger activado</figcaption>
</figure>

La zona debe detectar al jugador después de caer por un hueco. Ajusta su posición según tu mapa; los números de la captura son un ejemplo. Mantén **Layer Default** para que GroundCheck no la considere suelo.

Puedes dejar el cuadrado blanco visible durante la configuración. Si quieres ocultarlo en Game, **desactiva solo Sprite Renderer**: el objeto y su Box Collider 2D deben seguir activos.

## 2. Preparar la escena para recargarla

Guarda la escena con **Ctrl+S**. Si todavía no tiene un archivo guardado, utiliza **Save As** para darle un nombre y guardarla en la carpeta Scenes del proyecto.

En **Unity 6**, abre **File → Build Profiles → Platforms → Scene List**. Pulsa **Add Open Scenes** o arrastra la escena guardada desde Project, y comprueba que su casilla de inclusión está marcada. Si un perfil de compilación utiliza una lista propia mediante **Override Global Scene List**, incluye también la escena en la lista que usa ese perfil.

**SceneManager.LoadScene necesita que la escena que cargamos esté incluida en la lista de escenas de la compilación.** Esta configuración sirve tanto para la zona de caída como para los pinchos que añadiremos después.

## 3. Crear Killzone.cs

En la misma carpeta **Scripts** donde están Player.cs y Coin.cs, crea un **MonoBehaviour Script** llamado **Killzone**.

<figure class="unity-shot" markdown="1">

[![Crea Killzone.cs en la misma carpeta scripts que Coin.cs y Player.cs](../../assets/images/pmdm/unity/93-unity-killzone-script.png){ loading=lazy }](../../assets/images/pmdm/unity/93-unity-killzone-script.png){ aria-label="Ampliar imagen" }

<figcaption>Crea Killzone.cs en la misma carpeta scripts que Coin.cs y Player.cs</figcaption>
</figure>

Ábrelo y sustituye su contenido por:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            // Reinicia el nivel cargando de nuevo la escena activa.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
```

<figure class="unity-shot" markdown="1">

[![Killzone.cs recarga la escena activa al detectar un objeto con Tag Player](../../assets/images/pmdm/unity/94-unity-killzone-codigo.png){ loading=lazy }](../../assets/images/pmdm/unity/94-unity-killzone-codigo.png){ aria-label="Ampliar imagen" }

<figcaption>Killzone.cs recarga la escena activa al detectar un objeto con Tag Player</figcaption>
</figure>

El archivo se llama **Killzone.cs** y su clase **Killzone**, con la misma escritura. En Hierarchy, el objeto de la captura aparece como KillZone; el nombre del objeto puede variar, pero el del archivo y el de la clase deben coincidir.

| Instrucción | Función |
| --- | --- |
| **using UnityEngine.SceneManagement** | Permite utilizar las clases de gestión de escenas. |
| **OnTriggerEnter2D** | Detecta que otro collider entra en la zona. |
| **CompareTag("Player")** | Limita el reinicio al contacto con el jugador. |
| **SceneManager.GetActiveScene().name** | Obtiene el nombre de la escena activa, la del nivel que estamos jugando. |
| **SceneManager.LoadScene(...)** | Carga esa escena de nuevo. |

Al recargar la escena, **el Player vuelve a su posición guardada y las monedas recogidas vuelven a aparecer**, porque recuperamos el estado inicial de los objetos de este nivel.

## 4. Asignar el script y probar la caída

1. Guarda Killzone.cs y espera a que Unity compile.
2. Arrastra **Killzone.cs desde Project hasta Killzone en Hierarchy**.

<figure class="unity-shot" markdown="1">

[![Arrastra Killzone.cs al objeto de la zona de caída](../../assets/images/pmdm/unity/95-unity-asignar-killzone-script.gif){ loading=lazy }](../../assets/images/pmdm/unity/95-unity-asignar-killzone-script.gif){ aria-label="Ampliar imagen" }

<figcaption>Arrastra Killzone.cs al objeto de la zona de caída</figcaption>
</figure>

3. Selecciona la zona y comprueba **Box Collider 2D → Is Trigger** y el componente **Killzone (Script)**.
4. El Player debe conservar **Tag Player**, Rigidbody 2D Dynamic y su collider, como configuramos al recoger monedas.
5. Guarda la escena, pulsa Play y haz clic en Game.
6. Deja caer al jugador por un hueco: al entrar en la zona inferior, el nivel debe reiniciarse.

En esta fase el contacto produce **un reinicio directo**. En el apartado de combate trabajaremos la animación de muerte antes de eliminar al personaje.

## 5. Añadir pinchos u otros obstáculos

1. Detén Play y abre **sprites** en Project.
2. Elige un sprite de pinchos u otro elemento que vayas a utilizar como peligro y arrástralo a Scene.
3. Renombra el objeto como **Spikes** y colócalo sobre una plataforma.

<figure class="unity-shot" markdown="1">

[![Arrastra un elemento de sprites a Scene; en la captura se utiliza una seta como obstáculo](../../assets/images/pmdm/unity/96-unity-obstaculo-escena.gif){ loading=lazy }](../../assets/images/pmdm/unity/96-unity-obstaculo-escena.gif){ aria-label="Ampliar imagen" }

<figcaption>Arrastra un elemento de sprites a Scene; en la captura se utiliza una seta como obstáculo</figcaption>
</figure>

En las capturas utilizamos **una seta como obstáculo**. El funcionamiento es el mismo si eliges pinchos: la imagen establece su aspecto y el trigger establece la zona de contacto.

4. Selecciona Spikes y añade **Box Collider 2D**.
5. Pulsa **Edit Collider** para ajustar el área de contacto y marca **Is Trigger**. Deja el objeto en **Layer Default**; el suelo sólido sigue perteneciendo a Floor.

<figure class="unity-shot" markdown="1">

[![Añade Box Collider 2D al obstáculo, ajusta su forma y activa Is Trigger](../../assets/images/pmdm/unity/97-unity-obstaculo-collider.png){ loading=lazy }](../../assets/images/pmdm/unity/97-unity-obstaculo-collider.png){ aria-label="Ampliar imagen" }

<figcaption>Añade Box Collider 2D al obstáculo, ajusta su forma y activa Is Trigger</figcaption>
</figure>

### Crear Spikes.cs

En Scripts, crea un **MonoBehaviour Script** llamado **Spikes**. Utilizaremos el mismo método que en Killzone, dentro de una clase con el nombre del nuevo archivo:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            // Reinicia el nivel al tocar el obstáculo.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
```

Guarda y vuelve a Unity. **Arrastra Spikes.cs al objeto Spikes** para añadirlo como componente. Comprueba que **Spikes (Script)** aparece junto al collider, guarda la escena y prueba el contacto con el personaje.

Si prefieres reutilizar el comportamiento ya creado, puedes asignar **Killzone.cs al obstáculo** en lugar de crear Spikes.cs. Añade **uno de los dos scripts** al objeto: ambos realizan el mismo reinicio.

## 6. Comprobar el conjunto

| Objeto | Componentes y ajustes que debemos comprobar |
| --- | --- |
| **Player** | Tag Player, Rigidbody 2D Dynamic, Box Collider 2D con Is Trigger desactivado y Player (Script). |
| **Coin** | Animator de la moneda, Circle Collider 2D con Is Trigger activado y Coin (Script). |
| **Killzone** | Zona bajo el mapa, Box Collider 2D con Is Trigger activado y Killzone (Script). |
| **Spikes** | Obstáculo sobre una plataforma, Box Collider 2D con Is Trigger activado y Spikes (Script), o Killzone (Script) si lo reutilizas. |

Prueba una recogida, una caída y el contacto con el obstáculo. Después de cada reinicio, comprueba que el jugador vuelve al comienzo y las monedas se restauran.

### Si el nivel no se reinicia

Revisa **Console**, la inclusión de la escena en **Scene List**, el **Tag Player**, la asignación del script al peligro y **Is Trigger** en su collider. Para recibir eventos de trigger 2D, al menos uno de los objetos necesita Rigidbody 2D: en este proyecto lo lleva el Player. Comprueba también que las layers pueden interactuar en **Project Settings → Physics 2D**.

Si el nivel se reinicia nada más empezar, revisa la posición inicial del Player y el tamaño de los triggers: el personaje debe comenzar fuera de la zona de caída y de los obstáculos.

Continúa con [Ataque y enemigos](11-ataque-enemigos.md).

Referencias: [SceneManager.LoadScene](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.SceneManager.LoadScene.html), [Scene List en Unity 6](https://docs.unity3d.com/6000.0/Documentation/Manual/build-profile-scene-list.html) y [eventos de trigger 2D](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MonoBehaviour.OnTriggerEnter2D.html).
