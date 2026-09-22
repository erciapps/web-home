# 9. Orientación del personaje

Cuando el jugador cambia de dirección, el sprite debe mirar hacia el lado correcto.

Una forma sencilla es utilizar `SpriteRenderer.flipX`.

Declara:

```csharp
private SpriteRenderer spriteRenderer;
```

En `Start()`:

```csharp
spriteRenderer = GetComponent<SpriteRenderer>();
```

Y en `Update()`:

```csharp
if (move != 0)
{
    spriteRenderer.flipX = move < 0;
}
```

Con este sistema no modificamos la escala del Player ni alteramos sus colliders.
