# 9. Orientación del personaje

Cuando el jugador cambia de dirección, el personaje debe mirar hacia el lado al que se mueve. Continuamos en **Player.cs**, el mismo script de [Movimiento y salto](05-movimiento-salto.md), y utilizamos la variable **`move`** que ya contiene la dirección del teclado.

## Invertir la orientación con la escala X

En esta práctica el sprite original mira a la derecha y el **Player tiene Scale = (1, 1, 1)**. Cambiaremos el signo de la escala X para reflejarlo horizontalmente.

Añade este bloque **dentro del Update existente**, después de leer `move`:

```csharp
// ORIENTACIÓN
if (move != 0)
{
    // Cambiamos el signo de la escala en el eje X.
    transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
}
```

| Valor de move | Escala X | Resultado |
| --- | --- | --- |
| `1` | `1` | Mira a la derecha. |
| `-1` | `-1` | Mira a la izquierda. |
| `0` | Se conserva | Mantiene la última orientación al detenerse. |

**Mathf.Sign(move)** devuelve el signo del número: `-1` si es negativo y `1` si es cero o positivo. La condición **`move != 0`** evita actualizar la orientación al soltar las teclas. Los valores **Y y Z se mantienen en 1**.

## Dónde colocarlo en Player.cs

El principio de **Update** quedará así; conserva debajo las instrucciones de salto y animación que ya tengas:

```csharp
void Update()
{
    move = Input.GetAxisRaw("Horizontal");

    // ORIENTACIÓN
    if (move != 0)
    {
        transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    rb2D.linearVelocity = new Vector2(
        move * speed,
        rb2D.linearVelocity.y
    );

    // Aquí continúa el código de salto y animación existente.
}
```

**Añade el bloque de orientación al método que ya existe.** No crees otro Update ni sustituyas el script completo por este fragmento.

!!! note "La escala también afecta al resto del objeto"
    Este código está preparado para un Player de escala inicial **(1, 1, 1)**. Al invertir su Transform, se reflejan también sus hijos y la geometría de sus colliders. Mantén **GroundCheck centrado en X = 0**, junto a los pies. Si necesitas otro tamaño, revisa los [Pixels Per Unit del sprite](03-assets-sprites.md#que-significa-pixels-per-unit) o adapta el código para conservar esa escala.

## Probar la orientación

Guarda **Player.cs**, vuelve a Unity y espera a que termine de compilar. El script ya está añadido al Player, así que se actualizará su comportamiento.

Pulsa **Play** y haz clic en **Game**. Alterna **A/D o las flechas izquierda y derecha**: el personaje debe mirar hacia donde camina y conservar esa orientación cuando se detiene.

<figure class="unity-shot" markdown="1">

[![El Player cambia su orientación al desplazarse a izquierda y derecha](../../assets/images/pmdm/unity/56-unity-prueba-orientacion.gif){ loading=lazy }](../../assets/images/pmdm/unity/56-unity-prueba-orientacion.gif){ aria-label="Ampliar imagen" }

<figcaption>El Player cambia su orientación al desplazarse a izquierda y derecha</figcaption>
</figure>

Si mira al lado contrario, comprueba hacia dónde mira el sprite original y si existe otro código que también modifica la escala o **SpriteRenderer.flipX**.

!!! tip "Alternativa para reflejar solo la imagen"
    `SpriteRenderer.flipX` permite invertir únicamente el sprite. En estos pasos utilizamos la **escala X del Transform**; elige un único método para controlar la orientación.

Referencias: [Mathf.Sign](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Mathf.Sign.html), [Transform.localScale](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Transform-localScale.html) y [SpriteRenderer.flipX](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SpriteRenderer-flipX.html).
