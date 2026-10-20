# Ejercicio 2 · Clasificador de figuras 3D

Vamos a construir en **Unity 6** un tablero con agujeros de distintas formas. El jugador arrastrará figuras (cilindros, cubos, etc.) con el **ratón o la pantalla táctil** para soltarlas sobre su hueco correspondiente.

!!! info "Objetivo de esta primera versión"
    Preparar el escenario, permitir el arrastre y aprovechar las colisiones y la gravedad para que las figuras caigan al soltar el objeto sobre un agujero donde quepa. **Todavía no programamos puntuación ni comprobación de aciertos.**

## Preparar la escena

1. Crea un proyecto **Universal 3D** y, desde **Package Manager**, instala **ProBuilder**. Activa el paquete **Input System**.
2. Construye un tablero con grosor y practica los agujeros con ProBuilder. Deben atravesar realmente la malla; no basta con un dibujo oscuro.
3. En **Tablero**, utiliza **Mesh Collider** con `Convex` **desactivado**, `Is Trigger` desactivado y **sin Rigidbody**. Elimina cualquier `Box Collider` que tape los huecos.
4. Crea las figuras 3D. Cada figura tendrá **Rigidbody** con `Use Gravity` activado, `Is Kinematic` desactivado y un **Box/Sphere/Capsule Collider** adecuado, o un **Mesh Collider** con `Convex` activado.
5. Configura una cámara que permita ver todos los agujeros y tenga la etiqueta `MainCamera`.

## Script · ArrastrarFigura3D.cs

Crea `Assets/Scripts/ArrastrarFigura3D.cs`, copia el código y añade el componente a **cada figura**, no al tablero.

[Ver o guardar el archivo .cs en GitHub](https://github.com/erciapps/web-home/blob/main/docs/pmdm/scripts/ArrastrarFigura3D.cs)

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ArrastrarFigura3D : MonoBehaviour
{
    private Camera camara;
    private Rigidbody rb;
    private Animator animator;

    private Plane plano;
    private Vector3 desplazamiento;
    private bool arrastrando;
    private bool tactil;

    // Actívalo en el Inspector solo si el Animator tiene los triggers Run e Idle.
    public bool usarAnimaciones = false;

    void Start()
    {
        camara = Camera.main;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Comenzar a arrastrar con el dedo o el botón izquierdo del ratón.
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

        // Mientras mantenemos pulsado, mover el objeto; al soltar, recuperar la física.
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
        // El Raycast identifica el objeto pulsado (también si su Collider está en un hijo).
        Ray rayo = camara.ScreenPointToRay(posicion);
        if (!Physics.Raycast(rayo, out RaycastHit hit) ||
            !hit.collider.transform.IsChildOf(transform))
            return;

        arrastrando = true;
        tactil = esTactil;

        // Plano horizontal invisible: desplazamos la figura sin cambiar su altura.
        plano = new Plane(Vector3.up, transform.position);
        plano.Raycast(rayo, out float distancia);
        desplazamiento = transform.position - rayo.GetPoint(distancia);

        // Suspender temporalmente la gravedad y las fuerzas durante el arrastre.
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
        Ray rayo = camara.ScreenPointToRay(posicion);

        if (plano.Raycast(rayo, out float distancia))
            transform.position = rayo.GetPoint(distancia) + desplazamiento;
    }

    void Soltar()
    {
        arrastrando = false;

        // Volver a activar las físicas: si está sobre un agujero real, caerá.
        rb.isKinematic = false;

        if (usarAnimaciones && animator != null)
        {
            animator.ResetTrigger("Run");
            animator.SetTrigger("Idle");
        }
    }
}
```

### Cómo funciona

- **Raycast:** averigua qué figura hemos pulsado, aunque el Collider pertenezca a un objeto hijo.
- **Plane:** permite moverla horizontalmente a la misma altura.
- **isKinematic = true:** suspende temporalmente las físicas mientras la arrastramos.
- **isKinematic = false:** al soltarla, la gravedad vuelve a actuar. Si el hueco está realmente abierto y el Collider cabe, la figura puede caer.
- **Animator (opcional):** marca `Usar Animaciones` solo para modelos con triggers `Run` e `Idle`. Desactiva `Apply Root Motion` en estos modelos.

!!! warning "Si la figura no entra en el agujero"
    Comprueba que el agujero atraviesa la geometría, que no hay otro Collider tapándolo y que la figura (incluido su Collider) es suficientemente pequeña. El script mantiene la altura **mientras está pulsada**: la caída empieza **al soltarla**.

## Prueba

Ejecuta la escena, arrastra un cilindro hasta el agujero circular y suéltalo. Repite con el cubo. Prueba también la entrada táctil en Android.

[← Volver a PMDM](index.md) · [Ejercicio 3 · Clasificador de animales →](ejercicio-3-animales.md)
