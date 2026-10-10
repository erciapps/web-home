# 5. Movimiento y salto

Vamos a convertir el personaje en un objeto controlable mediante físicas 2D.

## Antes de programar

Completa primero [el escenario con Tilemap](06-tilemap-escenario.md) y [la preparación del Player y sus físicas](07-colisiones-fisicas.md). El personaje debe caer y apoyarse en el suelo al pulsar Play. Aquí utilizaremos su `Rigidbody 2D` y su `Box Collider 2D`, que ya están añadidos.

Crea **`Player.cs`** en **Assets → Gamercilla → Scripts**, añade el código de movimiento horizontal y arrastra el script al objeto **`Player`** de la Hierarchy.

## Movimiento horizontal

```csharp
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 6f;

    private Rigidbody2D rb2D;
    private float move;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

        rb2D.linearVelocity = new Vector2(
            move * speed,
            rb2D.linearVelocity.y
        );
    }
}
```

`Input.GetAxisRaw("Horizontal")` devuelve normalmente:

```text
-1 → izquierda
 0 → sin movimiento
 1 → derecha
```

## Activar el sistema de entrada

El ejemplo utiliza el sistema de entrada clásico. En Unity:

```text
Edit
→ Project Settings
→ Player
→ Active Input Handling
→ Both
```

Unity puede pedir reiniciar el editor.

## Detectar el suelo

Crea un objeto vacío como hijo del Player:

```text
Player
└── GroundCheck
```

Colócalo justo debajo de los pies.

Después crea una `Layer` llamada:

```text
Floor
```

y asígnala al Tilemap **`Escenario`**, que contiene el suelo sólido. `Player` y `Decoracion` deben permanecer fuera de esa layer. Esta es una **Layer de físicas**, distinta de la **Sorting Layer** utilizada para ordenar los sprites.

## Añadir el salto

Sustituye el contenido de **`Player.cs`** por esta versión, que incorpora el salto; no crees un segundo script con la misma clase:

```csharp
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 6f;

    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb2D;
    private float move;
    private bool isGrounded;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

        rb2D.linearVelocity = new Vector2(
            move * speed,
            rb2D.linearVelocity.y
        );

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(
                rb2D.linearVelocity.x,
                jumpForce
            );
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }
}
```

## Configurar el Inspector

En `Player (Script)` asigna:

```text
Ground Check  → GroundCheck
Ground Radius → 0.1
Ground Layer  → Floor
```

!!! tip
    Si el personaje no salta, comprueba primero que `GroundCheck` está asignado, que el suelo pertenece a la layer `Floor` y que `Ground Layer` apunta a esa layer.
