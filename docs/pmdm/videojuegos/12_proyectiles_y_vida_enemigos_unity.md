# Lanzamiento de proyectiles y vida de los enemigos

En este apartado vamos a añadir un ataque a distancia al personaje. El jugador podrá lanzar un objeto horizontalmente hacia la dirección en la que está mirando y ese proyectil podrá golpear a los enemigos.

Partimos del proyecto anterior, donde ya tenemos:

- El personaje con movimiento.
- El personaje puede mirar a izquierda y derecha.
- Los enemigos tienen un script `Enemy`.
- Los enemigos pueden ejecutar una animación de muerte.
- Al finalizar la animación `Death`, un `Animation Event` llama a `Destruir()`.

---

## 1. Crear el proyectil a partir de un sprite

Busca en `Assets` el sprite que quieras utilizar como proyectil y arrástralo directamente a la escena.

Unity creará automáticamente un `GameObject` con un componente `SpriteRenderer`.

Renombra el objeto como:

```text
Projectile
```

Tendremos inicialmente:

```text
Projectile
├── Transform
└── SpriteRenderer
```

Ahora añade desde el Inspector:

```text
Rigidbody2D
CircleCollider2D
```

La configuración básica será:

```text
Projectile
├── Transform
├── SpriteRenderer
├── Rigidbody2D
└── CircleCollider2D
```

En `Rigidbody2D` establece:

```text
Gravity Scale = 0
```

Así el proyectil no caerá y podrá desplazarse horizontalmente.

En `CircleCollider2D` activa:

```text
Is Trigger
```

Esto nos permitirá detectar cuándo el proyectil toca a un enemigo sin producir una colisión física.

---

## 2. Convertir el proyectil en un Prefab

Crea una carpeta para los prefabs si todavía no existe:

```text
Assets
└── Prefabs
```

Arrastra `Projectile` desde la jerarquía hasta la carpeta `Prefabs`.

Tendremos:

```text
Assets
└── Prefabs
    └── Projectile
```

Una vez creado el prefab, podemos eliminar el `Projectile` de la escena.

No necesitamos dejar ningún proyectil colocado de antemano. El jugador irá creando copias del prefab cuando pulse la tecla de lanzamiento.

---

## 3. Crear el punto de lanzamiento

Dentro del objeto `Player` crea un objeto vacío:

```text
Player
├── GroundCheck
├── AttackPoint
└── ThrowPoint
```

`ThrowPoint` no necesita ningún componente adicional. Vamos a utilizar únicamente su posición.

Colócalo delante del personaje, aproximadamente desde donde queremos que aparezca el proyectil.

```text
             ● ThrowPoint
Player ──────►
```

---

## 4. Crear `Projectile.cs`

Crea el script:

```text
Projectile.cs
```

y añádelo al prefab `Projectile`.

El código inicial será:

```csharp
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;

    private Rigidbody2D rb2D;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void Lanzar(float direccion)
    {
        rb2D.linearVelocity = new Vector2(
            direccion * speed,
            0
        );
    }
}
```

El método:

```csharp
Lanzar(float direccion)
```

recibe la dirección en la que debe desplazarse el proyectil.

```text
direccion = 1     → derecha
direccion = -1    → izquierda
```

Por ejemplo:

```csharp
Lanzar(1);
```

lo mueve hacia la derecha.

```csharp
Lanzar(-1);
```

lo mueve hacia la izquierda.

---

## 5. Preparar el `Player`

En `Player.cs` añadimos:

```csharp
public GameObject projectilePrefab;
public Transform throwPoint;
```

Selecciona el `Player` en Unity.

En el Inspector aparecerán los nuevos campos:

```text
Projectile Prefab
Throw Point
```

Asigna:

```text
Projectile Prefab → prefab Projectile
Throw Point       → ThrowPoint
```

---

## 6. Crear el proyectil al pulsar una tecla

En `Update()` utilizamos, por ejemplo, la tecla `C`:

```csharp
if (Input.GetKeyDown(KeyCode.C))
{
    LanzarObjeto();
}
```

Creamos el método:

```csharp
private void LanzarObjeto()
{
    GameObject projectile = Instantiate(
        projectilePrefab,
        throwPoint.position,
        Quaternion.identity
    );

    projectile.GetComponent<Projectile>().Lanzar(1);
}
```

`Instantiate()` crea una copia del prefab en la posición de `ThrowPoint`.

```text
Prefab Projectile
       │
       │ Instantiate()
       ▼
Player ● ─────────► Projectile
       ↑
   ThrowPoint
```

En este momento el proyectil se lanzará siempre hacia la derecha.

---

## 7. Lanzar hacia izquierda y derecha

El movimiento horizontal del personaje ya nos proporciona una dirección:

```text
move = 1     → derecha
move = -1    → izquierda
move = 0     → quieto
```

No podemos utilizar directamente `move` al lanzar porque, si el personaje está quieto, su valor será `0` y el proyectil no se moverá.

Guardamos por tanto la última dirección distinta de cero.

En `Player.cs` añade:

```csharp
private float ultimaDireccion = 1;
```

Después de:

```csharp
move = Input.GetAxisRaw("Horizontal");
```

añade:

```csharp
if (move != 0)
{
    ultimaDireccion = move;
}
```

De esta forma:

```text
Movimiento            ultimaDireccion

derecha     1   →           1
quieto      0   →           1

izquierda  -1   →          -1
quieto      0   →          -1
```

Ahora modifica `LanzarObjeto()`:

```csharp
private void LanzarObjeto()
{
    GameObject projectile = Instantiate(
        projectilePrefab,
        throwPoint.position,
        Quaternion.identity
    );

    projectile.GetComponent<Projectile>().Lanzar(ultimaDireccion);
}
```

El proyectil se moverá hacia la última dirección en la que se haya movido el personaje.

---

## 8. Detectar al enemigo

Añade en `Projectile.cs`:

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    Enemy enemigo = collision.GetComponent<Enemy>();

    if (enemigo != null)
    {
        enemigo.Morir();
        Destroy(gameObject);
    }
}
```

Como el `CircleCollider2D` del proyectil tiene activado:

```text
Is Trigger
```

Unity ejecutará `OnTriggerEnter2D()` cuando el proyectil entre en contacto con otro `Collider2D`.

El funcionamiento es:

```text
Projectile
    │
    ▼
toca un objeto
    │
    ▼
¿Tiene Enemy?
    │
   Sí
    │
    ├── enemigo.Morir()
    └── Destroy(Projectile)
```

---

## 9. Destruir proyectiles que no golpean a nadie

Si el proyectil no toca ningún objeto podría continuar desplazándose indefinidamente.

Añadimos en `Projectile.cs`:

```csharp
void Start()
{
    Destroy(gameObject, 3f);
}
```

La instrucción:

```csharp
Destroy(gameObject, 3f);
```

indica que Unity debe destruir el proyectil pasados tres segundos.

```text
Se crea
   │
   ▼
Se mueve
   │
   ├── toca Enemy → se destruye
   │
   └── no toca nada → se destruye a los 3 segundos
```

---

## 10. Destruir el proyectil al tocar el escenario

También podemos eliminarlo si toca el suelo o una pared que pertenezca a la capa `Ground`.

Modifica `OnTriggerEnter2D()`:

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    Enemy enemigo = collision.GetComponent<Enemy>();

    if (enemigo != null)
    {
        enemigo.Morir();
        Destroy(gameObject);
        return;
    }

    if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
    {
        Destroy(gameObject);
    }
}
```

El `return` hace que, después de golpear a un enemigo, se termine la ejecución del método.

```text
Projectile
    │
    ▼
¿Toca Enemy?
├── Sí
│   ├── Enemy.Morir()
│   └── Destroy(Projectile)
│
└── No
    │
    ▼
¿Toca Layer Ground?
├── Sí → Destroy(Projectile)
└── No  → continúa
```

---

## 11. Hacer que el enemigo aguante dos golpes

Hasta ahora cualquier impacto llama directamente a:

```csharp
enemigo.Morir();
```

Vamos a añadir vida al enemigo.

En `Enemy.cs`:

```csharp
public int vida = 2;
```

Creamos:

```csharp
public void RecibirGolpe()
{
    vida--;

    if (vida <= 0)
    {
        Morir();
    }
}
```

El funcionamiento será:

```text
vida = 2

Primer golpe
2 → 1
sigue vivo

Segundo golpe
1 → 0
Morir()
```

---

## 12. Modificar el proyectil para quitar vida

En `Projectile.cs` sustituimos:

```csharp
enemigo.Morir();
```

por:

```csharp
enemigo.RecibirGolpe();
```

El método queda:

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    Enemy enemigo = collision.GetComponent<Enemy>();

    if (enemigo != null)
    {
        enemigo.RecibirGolpe();
        Destroy(gameObject);
        return;
    }

    if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
    {
        Destroy(gameObject);
    }
}
```

Ahora el proyectil no decide cuándo muere el enemigo.

El proyectil únicamente comunica que ha recibido un golpe:

```text
Projectile
     │
     ▼
Enemy.RecibirGolpe()
     │
     ▼
vida--
     │
     ▼
¿vida <= 0?
├── No → continúa vivo
└── Sí → Morir()
```

---

## 13. Aplicar el mismo sistema al ataque cuerpo a cuerpo

Si en el ataque cuerpo a cuerpo teníamos:

```csharp
enemigo.Morir();
```

lo sustituimos también por:

```csharp
enemigo.RecibirGolpe();
```

Así ambos ataques utilizan el mismo sistema:

```text
Ataque cuerpo a cuerpo ──┐
                         │
                         ▼
                  Enemy.RecibirGolpe()
                         ▲
                         │
Proyectil ───────────────┘
```

---

# Código final de `Projectile.cs`

```csharp
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;

    private Rigidbody2D rb2D;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    public void Lanzar(float direccion)
    {
        rb2D.linearVelocity = new Vector2(
            direccion * speed,
            0
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemigo = collision.GetComponent<Enemy>();

        if (enemigo != null)
        {
            enemigo.RecibirGolpe();
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
```

---

# Parte añadida a `Enemy.cs`

```csharp
public int vida = 2;

public void RecibirGolpe()
{
    vida--;

    if (vida <= 0)
    {
        Morir();
    }
}
```

Los métodos que ya teníamos continúan funcionando:

```csharp
public void Morir()
{
    animator.SetTrigger("Death");
}

public void Destruir()
{
    Destroy(gameObject);
}
```

`Destruir()` sigue siendo llamado mediante el `Animation Event` situado al final de la animación `Death`.

---

# Parte añadida a `Player.cs`

Variables:

```csharp
public GameObject projectilePrefab;
public Transform throwPoint;

private float ultimaDireccion = 1;
```

Después de obtener el movimiento:

```csharp
move = Input.GetAxisRaw("Horizontal");

if (move != 0)
{
    ultimaDireccion = move;
}
```

Para lanzar:

```csharp
if (Input.GetKeyDown(KeyCode.C))
{
    LanzarObjeto();
}
```

Método:

```csharp
private void LanzarObjeto()
{
    GameObject projectile = Instantiate(
        projectilePrefab,
        throwPoint.position,
        Quaternion.identity
    );

    projectile.GetComponent<Projectile>().Lanzar(ultimaDireccion);
}
```

---

## Resultado

```text
                 PLAYER
                    │
                    │ C
                    ▼
            Instantiate()
                    │
                    ▼
              PROJECTILE
                    │
          ┌─────────┴─────────┐
          │                   │
          ▼                   ▼
       Ground               Enemy
          │                   │
          ▼                   ▼
       Destroy()       RecibirGolpe()
                              │
                              ▼
                            vida--
                              │
                       ┌──────┴──────┐
                       │             │
                    vida > 0      vida <= 0
                       │             │
                       ▼             ▼
                  sigue vivo      Morir()
                                      │
                                      ▼
                               animación Death
                                      │
                                      ▼
                              Animation Event
                                      │
                                      ▼
                                  Destruir()
```
