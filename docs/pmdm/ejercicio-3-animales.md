# Ejercicio 3 · Clasificador de animales 3D

Prepararemos un juego para arrastrar animales hasta la **cesta de su grupo**: **mamíferos, reptiles o aves**.

!!! info "De momento, solo la preparación"
    En esta primera fase descargaremos los animales, colocaremos las cestas y podremos arrastrar los modelos en Unity. **La detección de la cesta correcta, las puntuaciones y los mensajes de acierto/error se programarán más adelante.**

## 1. Descargar los animales

Usaremos estos modelos 3D:

**[Descargar assets · Cube Pets Vol. 3 (itch.io)](https://sneznyj.itch.io/cube-pets-vol-3?download)**

Descarga el paquete desde esa página e importa los modelos que necesites en el proyecto Unity.

## 2. Preparar las cestas y los animales

1. Crea tres zonas o cestas llamadas `CestaMamiferos`, `CestaReptiles` y `CestaAves`.
2. Importa algunos animales, colócalos en la escena y ajusta su tamaño.
3. A cada animal añádele un **Rigidbody** y un Collider adecuado. Si tiene **Mesh Collider** y Rigidbody dinámico, activa **Convex**; normalmente un **Box Collider** ajustado resulta más sencillo.
4. Añade a los animales el script **ArrastrarFigura3D.cs** del ejercicio 2. **Es el mismo sistema de arrastre**: funciona con ratón y pantalla táctil, así que no necesitamos duplicar el código.

**[Abrir el script de arrastre y su explicación](ejercicio-2-figuras.md#script-arrastrarfigura3dcs)** · [Ver el archivo .cs en GitHub](https://github.com/erciapps/web-home/blob/main/docs/pmdm/scripts/ArrastrarFigura3D.cs)

Si un animal utiliza Animator y tiene transiciones por triggers `Run` e `Idle`, puedes activar `Usar Animaciones` en el script de arrastre. En caso contrario, déjalo desactivado.

## 3. Script opcional · MovimientoCorgi.cs

Para practicar con un animal animado, también tenemos el movimiento automático del Corgi entre dos puntos. **No es necesario para clasificar**; es una prueba adicional de movimiento y Animator.

Crea dos objetos vacíos (`PuntoA` y `PuntoB`), asígnalos en el Inspector y añade este script al Corgi:

[Ver o guardar MovimientoCorgi.cs en GitHub](https://github.com/erciapps/web-home/blob/main/docs/pmdm/scripts/MovimientoCorgi.cs)

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoCorgi : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;

    private Rigidbody rb;
    private Animator animator;
    private Transform destino;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (puntoA == null || puntoB == null) return;

        // Comenzar en A y dirigirse hacia B.
        rb.position = puntoA.position;
        destino = puntoB;

        if (animator != null)
            animator.SetTrigger("Run");
    }

    void FixedUpdate()
    {
        if (destino == null) return;

        // Dirección horizontal: no modificamos la velocidad vertical de la gravedad.
        Vector3 direccion = destino.position - rb.position;
        direccion.y = 0;

        // Al llegar a un extremo, cambiar de destino: A -> B -> A -> B...
        if (direccion.magnitude < 0.1f)
        {
            destino = destino == puntoB ? puntoA : puntoB;
            return;
        }

        direccion.Normalize();

        // Avanzar manteniendo la velocidad vertical del Rigidbody.
        rb.linearVelocity = new Vector3(
            direccion.x * velocidad,
            rb.linearVelocity.y,
            direccion.z * velocidad
        );

        // Mirar hacia la dirección del desplazamiento.
        rb.MoveRotation(Quaternion.LookRotation(direccion));
    }
}
```

El perro recorre **A → B → A → B…** continuamente. En el Animator, configura `Idle → Run` mediante el trigger `Run` (sin `Has Exit Time`) y el clip `Run` en bucle. Desactiva **Apply Root Motion**.

!!! warning "No mezclar controles durante la prueba"
    Si utilizas `MovimientoCorgi` para patrullar, **desactiva `ArrastrarFigura3D` en ese mismo animal** mientras haces la prueba. Los dos scripts intentan controlar su posición de formas diferentes.

## Próxima ampliación

Cuando tengamos los modelos y las tres cestas, añadiremos la lógica para identificar si un animal pertenece a **mamíferos**, **reptiles** o **aves**, detectar en qué cesta se suelta y dar una respuesta al jugador.

[← Ejercicio 2 · Clasificador de figuras](ejercicio-2-figuras.md) · [Volver a PMDM](index.md)
