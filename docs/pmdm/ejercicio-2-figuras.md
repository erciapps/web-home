# Ejercicio 2 · Clasificador de figuras 3D

Vamos a crear un tablero con agujeros de distintas formas en **Unity 6**. La idea es coger una figura con el ratón o con el dedo, arrastrarla hasta su agujero y soltarla. Si cabe y el agujero está abierto, caerá gracias a la gravedad.

!!! info "Qué vamos a hacer ahora"
    Prepararemos el tablero, las colisiones y el arrastre. De momento **no vamos a comprobar si la figura es correcta ni a contar puntos**.

## 1. Preparar el tablero

1. Crea un proyecto **Universal 3D**. Instala **ProBuilder** desde el Package Manager y utiliza el paquete **Input System**.
2. Crea un tablero con grosor y haz los agujeros con ProBuilder. Los agujeros tienen que atravesarlo realmente.
3. En `Tablero`, añade un **Mesh Collider**, deja `Convex` **desactivado** y no añadas Rigidbody.
4. Elimina cualquier **Box Collider** que esté tapando los agujeros. Si existe, la figura chocará con él aunque visualmente veamos el hueco.
5. Coloca una cámara que muestre el tablero. Debe conservar la etiqueta `MainCamera`.

## 2. Preparar las figuras

Crea un cubo, un cilindro y las figuras que quieras. Cada una necesita:

- **Rigidbody**: `Use Gravity` activado y `Is Kinematic` desactivado al empezar.
- **Collider**: por ejemplo `Box Collider`, `Capsule Collider` o un `Mesh Collider` con `Convex` activado si hay un Rigidbody dinámico.
- **ArrastrarFigura3D**: un único componente con este script.

!!! warning "Antes de mirar el código, revisa el Collider"
    El **Collider** es lo que Unity detecta cuando pulsamos. Si no existe o no cubre bien la figura, no podremos cogerla. Para comprobar su tamaño, selecciona el objeto y pulsa **Edit Collider**. En el caso de los animales del siguiente ejercicio, esto también es fundamental.

## 3. Script · ArrastrarFigura3D.cs

Crea `Assets/Scripts/ArrastrarFigura3D.cs`. Este script nos valdrá para **cualquier objeto 3D que queramos arrastrar**, no solo para las figuras de este ejercicio.

**[Abrir o descargar ArrastrarFigura3D.cs desde GitHub](https://github.com/erciapps/web-home/blob/main/docs/pmdm/scripts/ArrastrarFigura3D.cs)**

### Paso 1. Localizar los componentes

En `Start()` buscamos la cámara, el Rigidbody y, si existe, el Animator:

```csharp
camara = Camera.main;
rb = GetComponent<Rigidbody>();
animator = GetComponentInChildren<Animator>();
```

- `Camera.main`: la cámara desde la que estamos viendo el juego.
- `GetComponent<Rigidbody>()`: el componente encargado de la gravedad y las físicas.
- `GetComponentInChildren<Animator>()`: busca las animaciones del objeto, incluso si están en un hijo.

El Animator es **opcional**. Un cubo no necesita tener animaciones.

### Paso 2. Saber cuándo pulsamos

En `Update()` comprobamos si el usuario ha pulsado la pantalla o el botón izquierdo del ratón.

```csharp
Touchscreen.current.primaryTouch.press.wasPressedThisFrame
Mouse.current.leftButton.wasPressedThisFrame
```

`wasPressedThisFrame` significa **«se acaba de pulsar»**. Solo intentamos coger una figura en ese momento.

Después, mientras mantenemos pulsado, usamos `Mover()`. Cuando levantamos el dedo o soltamos el botón, llamamos a `Soltar()`.

### Paso 3. Saber qué figura hemos tocado

```csharp
Ray rayo = camara.ScreenPointToRay(posicion);

if (!Physics.Raycast(rayo, out RaycastHit hit) ||
    !hit.collider.transform.IsChildOf(transform))
    return;
```

El **Ray** es como un puntero láser invisible que sale de la cámara y pasa por donde hemos pulsado.

- `Physics.Raycast` comprueba qué **Collider** toca primero ese rayo.
- `hit` guarda el resultado.
- `IsChildOf(transform)` comprueba que el objeto tocado sea **esta figura o alguno de sus hijos**.
- `return` hace que no sigamos si hemos pulsado en otro sitio.

Esto explica por qué es tan importante tener un Collider bien ajustado.

### Paso 4. Arrastrar sin cambiar la altura

```csharp
plano = new Plane(Vector3.up, transform.position);
plano.Raycast(rayo, out float distancia);
desplazamiento = transform.position - rayo.GetPoint(distancia);
```

Creamos un **plano horizontal invisible** a la altura de la figura. Así, aunque movamos el dedo por la pantalla, la figura se desplaza por la superficie sin subir ni bajar.

Guardamos también `desplazamiento` para que la figura **no salte de repente al centro del ratón** cuando la cogemos.

Mientras arrastramos, usamos:

```csharp
rb.isKinematic = true;
```

Con `isKinematic = true` paramos temporalmente la respuesta del Rigidbody a la gravedad y las fuerzas físicas. Además, dejamos su velocidad a cero para que no siga con la velocidad que llevaba antes de cogerlo.

### Paso 5. Soltar la figura

Al levantar el dedo o el botón:

```csharp
arrastrando = false;
rb.isKinematic = false;
```

Volvemos a activar las físicas. Si la figura está encima de su hueco y su Collider cabe por él, **caerá**.

El script tiene dos opciones para reutilizarlo más adelante:

- `Movimiento Automatico`: se deja **vacío** en este ejercicio. En el clasificador de animales servirá para pausar su patrulla mientras lo arrastramos.
- `Usar Animaciones`: se deja **desactivado** para los objetos sin Animator. Si activamos esta opción, el Animator debe tener los triggers `Run` e `Idle`.

### Código completo

Este es el archivo que hay que pegar en Unity. Los comentarios resumen lo que acabamos de ver.

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ArrastrarFigura3D : MonoBehaviour
{
    // Opcionales: si el objeto se mueve solo o tiene animaciones.
    public MonoBehaviour movimientoAutomatico;
    public bool usarAnimaciones = false;

    private Camera camara;
    private Rigidbody rb;
    private Animator animator;

    private Plane plano;
    private Vector3 desplazamiento;
    private bool arrastrando;
    private bool tactil;

    void Start()
    {
        // Buscamos la cámara y los componentes del objeto.
        camara = Camera.main;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Si no tenemos cogido el objeto, esperamos un clic o un toque.
        if (!arrastrando)
        {
            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                Coger(Touchscreen.current.primaryTouch.position.ReadValue(), true);
            else if (Mouse.current != null &&
                     Mouse.current.leftButton.wasPressedThisFrame)
                Coger(Mouse.current.position.ReadValue(), false);

            return;
        }

        // Si lo tenemos cogido, seguimos el dedo o el ratón.
        if (tactil)
        {
            if (Touchscreen.current == null ||
                !Touchscreen.current.primaryTouch.press.isPressed)
                Soltar();
            else
                Mover(Touchscreen.current.primaryTouch.position.ReadValue());
        }
        else
        {
            if (Mouse.current == null || !Mouse.current.leftButton.isPressed)
                Soltar();
            else
                Mover(Mouse.current.position.ReadValue());
        }
    }

    void Coger(Vector2 posicion, bool esTactil)
    {
        // Un rayo invisible indica qué Collider hemos pulsado.
        Ray rayo = camara.ScreenPointToRay(posicion);

        // Solo cogemos ESTE objeto o uno de sus hijos.
        if (!Physics.Raycast(rayo, out RaycastHit hit) ||
            !hit.collider.transform.IsChildOf(transform))
            return;

        arrastrando = true;
        tactil = esTactil;

        // Detener cualquier script de movimiento asignado en el Inspector.
        if (movimientoAutomatico != null)
            movimientoAutomatico.enabled = false;

        // Plano horizontal para arrastrar sin cambiar la altura.
        plano = new Plane(Vector3.up, transform.position);
        plano.Raycast(rayo, out float distancia);

        // Guardamos la separación entre el clic y el centro del objeto.
        desplazamiento = transform.position - rayo.GetPoint(distancia);

        // Paramos las físicas mientras sujetamos el objeto.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (usarAnimaciones && animator != null)
        {
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Run");
        }
    }

    void Mover(Vector2 posicion)
    {
        // Calculamos dónde toca el rayo el plano horizontal.
        Ray rayo = camara.ScreenPointToRay(posicion);

        if (plano.Raycast(rayo, out float distancia))
            transform.position = rayo.GetPoint(distancia) + desplazamiento;
    }

    void Soltar()
    {
        arrastrando = false;

        // El objeto vuelve a responder a la gravedad y las colisiones.
        rb.isKinematic = false;

        // Si patrullaba antes, vuelve a moverse automáticamente.
        if (movimientoAutomatico != null)
            movimientoAutomatico.enabled = true;

        if (usarAnimaciones && animator != null)
        {
            animator.ResetTrigger("Run");
            animator.ResetTrigger("Idle");

            // Un objeto con patrulla sigue corriendo; el resto queda en reposo.
            animator.SetTrigger(movimientoAutomatico != null ? "Run" : "Idle");
        }
    }
}
```

## 4. Comprobar que funciona

Pulsa **Play**, arrastra el cilindro hasta el agujero circular y suéltalo. Repite con el cubo y comprueba que también se puede utilizar el dedo en Android.

Si al pulsar no puedes coger una figura, revisa primero **su Collider**. Si la puedes coger pero no cae por el agujero, revisa el **Mesh Collider del tablero** y el tamaño del hueco.

[← Volver a PMDM](index.md) · [Ejercicio 3 · Clasificador de animales →](ejercicio-3-animales.md)
