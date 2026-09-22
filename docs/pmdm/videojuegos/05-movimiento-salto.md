# 5. Movimiento y salto

Vamos a convertir el personaje en un objeto controlable mediante físicas 2D.

## Preparar el Player

Selecciona `Player` y añade:

```text
Rigidbody 2D
Box Collider 2D
```

En `Rigidbody 2D` activa:

```text
Constraints
→ Freeze Rotation Z
```

Así evitamos que el personaje gire al chocar.

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

y asígnala al suelo, no al Player.

## Añadir el salto

Amplía el script:

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
