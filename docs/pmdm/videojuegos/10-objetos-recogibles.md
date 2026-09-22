# 10. Objetos recogibles

Vamos a crear una moneda que desaparezca cuando el jugador la recoja.

## Preparar la moneda

Arrastra el sprite de la moneda a la escena y renómbralo:

```text
Coin
```

Añade:

```text
Circle Collider 2D
```

Activa:

```text
Is Trigger
```

## Script Coin.cs

Crea `Coin.cs` dentro de la carpeta de scripts:

```csharp
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
```

Añade el script al objeto `Coin`.

## Configurar el Player

Selecciona el Player y establece:

```text
Tag: Player
```

Cuando el Player entre en el trigger de la moneda, esta desaparecerá.

## Siguiente mejora

En lugar de limitarse a destruir la moneda, podemos añadir después:

- contador de monedas;
- sonido al recogerla;
- puntuación;
- animación o partículas;
- condición para abrir una puerta o completar el nivel.
