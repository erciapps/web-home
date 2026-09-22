# 5. Movimiento y salto

Ejemplo sencillo de movimiento horizontal usando un `Rigidbody2D`:

```csharp
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 6;
    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);
    }
}
```

El siguiente paso es añadir detección de suelo y salto.
