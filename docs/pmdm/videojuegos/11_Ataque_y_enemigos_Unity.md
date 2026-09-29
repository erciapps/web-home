# 11. Ataque y enemigos

En este apartado vamos a incorporar un enemigo al escenario y una mecánica básica de combate.

Al terminar tendremos:

- una animación de ataque para el Player;
- una zona de ataque delante del personaje;
- detección de enemigos mediante una `Layer`;
- una animación de muerte para el enemigo;
- eliminación del enemigo cuando termina la animación;
- muerte del Player al tocar un enemigo;
- un enemigo que patrulla entre dos puntos.

---

## 1. Crear la animación de ataque

Selecciona `Player` y abre:

```text
Window
→ Animation
→ Animation
```

Crea la animación:

```text
Attack.anim
```

Añade los frames correspondientes al ataque.

Como el ataque debe reproducirse una sola vez, selecciona el clip `Attack` en **Project** y comprueba que:

```text
Loop Time: desactivado
```

### Añadir el estado al Animator

Abre:

```text
Window
→ Animation
→ Animator
```

Añade el estado `Attack`.

En **Parameters** crea:

```text
Trigger → attack
```

La transición de entrada queda:

```text
Idle → Attack
Condition: attack
Has Exit Time: desactivado
```

La transición de vuelta:

```text
Attack → Idle
Has Exit Time: activado
Conditions: ninguna
```

De esta forma el código inicia el ataque y, cuando termina la animación, el Animator vuelve a `Idle`.

En `Player.cs`:

```csharp
if (Input.GetKeyDown(KeyCode.X))
{
    anim.SetTrigger("attack");
}
```

!!! warning "Mayúsculas y minúsculas"
    El parámetro se llama `attack`. El texto utilizado en `SetTrigger()` debe coincidir exactamente con el nombre creado en el Animator.

---

## 2. Crear el punto de ataque

Para decidir desde qué posición comprobamos si hay un enemigo, crea un objeto vacío como hijo del Player:

```text
Player
├── GroundCheck
└── AttackPoint
```

Selecciona `AttackPoint` y colócalo delante del personaje, aproximadamente en la zona que alcanza el golpe.

No necesita ningún componente adicional. Utilizaremos únicamente su posición.

En `Player.cs` añade:

```csharp
public Transform attackPoint;
public float attackRadius = 0.5f;
public LayerMask enemyLayer;
```

Después selecciona `Player` y, en el Inspector:

```text
Attack Point  → AttackPoint
Attack Radius → ajustarlo al alcance del golpe
Enemy Layer   → Enemy
```

---

## 3. Crear la Layer Enemy

Necesitamos distinguir a los enemigos del resto de objetos del escenario.

Selecciona el objeto que hará de enemigo:

```text
Layer
→ Add Layer...
```

Crea:

```text
Enemy
```

Vuelve a seleccionar el enemigo y asígnale:

```text
Layer: Enemy
```

El enemigo también necesita un `Collider2D`. Por ejemplo:

```text
Box Collider 2D
```

La `Layer` se utilizará como filtro para que el ataque no detecte el suelo, monedas u otros objetos.

---

## 4. Detectar un enemigo dentro del alcance

Amplía el código que ejecutamos al pulsar ++x++:

```csharp
if (Input.GetKeyDown(KeyCode.X))
{
    anim.SetTrigger("attack");

    Collider2D enemyCollider = Physics2D.OverlapCircle(
        attackPoint.position,
        attackRadius,
        enemyLayer
    );

    if (enemyCollider != null)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Morir();
        }
    }
}
```

`Physics2D.OverlapCircle()` comprueba si existe un `Collider2D` dentro de un círculo.

En este caso recibe:

```text
attackPoint.position → centro del círculo
attackRadius         → radio del círculo
enemyLayer           → objetos que queremos detectar
```

Por tanto, solo nos interesa un collider que pertenezca a la `Layer Enemy`.

---

## 5. Ver la zona de ataque

Para poder ajustar el alcance desde la escena, añade al final de `Player.cs`:

```csharp
private void OnDrawGizmosSelected()
{
    if (attackPoint != null)
    {
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}
```

No hay que llamar manualmente a `OnDrawGizmosSelected()`. Unity lo ejecuta automáticamente cuando el objeto está seleccionado.

Para verlo:

1. Selecciona `Player`.
2. Comprueba que **Gizmos** está activado en la ventana **Scene**.
3. Modifica `Attack Radius` desde el Inspector hasta obtener el alcance que necesites.
4. Si el círculo aparece desplazado, mueve el objeto `AttackPoint`.

---

## 6. Crear el script Enemy

Dentro de la carpeta de scripts crea:

```text
Enemy.cs
```

De momento el enemigo tendrá su propio método para morir. El Player no lo destruye directamente: le indica al enemigo que debe morir y es `Enemy` quien controla el proceso.

```csharp
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Morir()
    {
        animator.SetTrigger("Death");
    }

    public void Destruir()
    {
        Destroy(gameObject);
    }
}
```

Añade `Enemy.cs` al mismo objeto que contiene el `Animator` y el `Collider2D` del enemigo.

---

## 7. Animación de muerte del enemigo

Crea una animación:

```text
Death.anim
```

En el clip:

```text
Loop Time: desactivado
```

En el Animator del enemigo crea:

```text
Trigger → Death
```

y la transición:

```text
Idle → Death
Condition: Death
Has Exit Time: desactivado
```

No necesitamos una transición de vuelta a `Idle`, ya que el enemigo se eliminará al terminar la animación.

!!! warning
    En este proyecto el parámetro del enemigo se llama `Death`, con `D` mayúscula. Por eso el código utiliza `animator.SetTrigger("Death")`.

---

## 8. Destruir el enemigo al terminar la animación

No llamamos a `Destroy()` al recibir el golpe porque desaparecería inmediatamente y no llegaríamos a ver la animación.

Utilizaremos un **Animation Event**.

1. Selecciona el enemigo.
2. Abre `Window → Animation → Animation`.
3. Selecciona el clip `Death`.
4. Sitúate en el último fotograma de la animación.
5. Pulsa **Add Animation Event**.
6. Selecciona el marcador creado.
7. En el Inspector, en `Function`, selecciona:

```text
Enemy/Methods/Destruir()
```

El flujo queda:

```text
Player pulsa X
        ↓
se detecta Enemy
        ↓
Enemy.Morir()
        ↓
Trigger Death
        ↓
animación Death
        ↓
Animation Event
        ↓
Destruir()
        ↓
Destroy(gameObject)
```

El método que recibe el evento debe ser público:

```csharp
public void Destruir()
{
    Destroy(gameObject);
}
```

---

## 9. Evitar que el enemigo arrastre al Player

Si el collider del enemigo produce una colisión física, puede empujar al Player mientras este está muriendo.

Para este comportamiento sencillo, selecciona el `Collider2D` del enemigo y activa:

```text
Is Trigger
```

Así seguirá detectándose el contacto, pero el enemigo no empujará físicamente al Player.

El Player ya dispone de `Rigidbody2D`, por lo que podemos detectar la entrada en el trigger.

---

## 10. Hacer que el Player muera al tocar al enemigo

En `Player.cs` añade:

```csharp
private bool muerto = false;
```

Después crea:

```csharp
public void Morir()
{
    if (muerto)
        return;

    muerto = true;

    rb2D.linearVelocity = Vector2.zero;
    anim.SetTrigger("death");
}
```

Y detecta el contacto con el enemigo:

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    Enemy enemy = collision.GetComponent<Enemy>();

    if (enemy != null)
    {
        Morir();
    }
}
```

`muerto` es una variable que hemos creado nosotros. No pertenece a Unity.

Su función es indicar el estado actual del Player:

```text
muerto = false → puede moverse, saltar y atacar
muerto = true  → está ejecutando su muerte
```

Al principio de `Update()` añadimos:

```csharp
if (muerto)
    return;
```

De esta forma, cuando comienza la muerte, dejamos de ejecutar el movimiento, el salto y el ataque.

---

## 11. Animación de muerte del Player

En el Animator del Player crea:

```text
Trigger → death
```

Añade el estado `Death`.

Una forma sencilla de permitir que el Player muera independientemente de lo que estuviera haciendo es:

```text
Any State → Death
Condition: death
Has Exit Time: desactivado
```

El clip `Death` no debe estar en bucle:

```text
Loop Time: desactivado
```

Si quieres eliminar el Player cuando termine la animación, añade un **Animation Event** al último fotograma que llame a:

```csharp
public void Destruir()
{
    Destroy(gameObject);
}
```

---

## 12. Hacer que el enemigo patrulle

Vamos a hacer que el enemigo se desplace entre dos posiciones fijas.

Crea dos objetos vacíos en la escena:

```text
PuntoA
PuntoB
```

No los hagas hijos del enemigo. Deben permanecer fijos mientras el enemigo se mueve.

La jerarquía puede quedar así:

```text
SampleScene
├── Player
├── Enemy
├── PuntoA
└── PuntoB
```

Coloca ambos puntos en los extremos de la zona de patrulla.

Amplía `Enemy.cs`:

```csharp
public float speed = 2f;

public Transform puntoA;
public Transform puntoB;

private Transform objetivo;
```

En `Start()`:

```csharp
objetivo = puntoB;
```

Y en `Update()`:

```csharp
transform.position = Vector2.MoveTowards(
    transform.position,
    objetivo.position,
    speed * Time.deltaTime
);

if (Vector2.Distance(transform.position, objetivo.position) < 0.1f)
{
    if (objetivo == puntoA)
    {
        objetivo = puntoB;
    }
    else
    {
        objetivo = puntoA;
    }
}
```

En el Inspector del enemigo asigna:

```text
Punto A → PuntoA
Punto B → PuntoB
Speed   → 2
```

---

## 13. Detener la patrulla cuando el enemigo muere

Ahora que el enemigo se mueve, conviene detener su `Update()` mientras reproduce `Death`.

Añade a `Enemy.cs`:

```csharp
private bool muerto = false;
```

Al principio de `Update()`:

```csharp
if (muerto)
    return;
```

Y modifica `Morir()`:

```csharp
public void Morir()
{
    if (muerto)
        return;

    muerto = true;

    GetComponent<Collider2D>().enabled = false;
    animator.SetTrigger("Death");
}
```

Además de detener el movimiento, desactivamos el collider para que el enemigo que ya está muriendo no vuelva a golpear al Player ni pueda recibir otro ataque.

---

## 14. Ajustar AttackPoint al cambio de dirección

En el apartado anterior orientamos el personaje con:

```csharp
spriteRenderer.flipX = move < 0;
```

`flipX` gira únicamente el sprite. Los objetos hijos no cambian de posición, por lo que `AttackPoint` se quedaría en el mismo lado.

Para que el punto de ataque acompañe a la dirección del Player, declara:

```csharp
private float attackPointX;
```

En `Start()` guarda su distancia horizontal:

```csharp
attackPointX = Mathf.Abs(attackPoint.localPosition.x);
```

Y sustituye el bloque de orientación por:

```csharp
if (move != 0)
{
    bool miraIzquierda = move < 0;

    spriteRenderer.flipX = miraIzquierda;

    Vector3 attackPosition = attackPoint.localPosition;

    attackPosition.x = miraIzquierda
        ? -attackPointX
        : attackPointX;

    attackPoint.localPosition = attackPosition;
}
```

Ahora:

```text
mira a la derecha → AttackPoint a la derecha
mira a la izquierda → AttackPoint a la izquierda
```

---

## 15. Player.cs completo

El script final reúne el movimiento, salto, animaciones, orientación, ataque y muerte:

```csharp
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 6f;

    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public LayerMask enemyLayer;

    private Rigidbody2D rb2D;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private float move;
    private bool isGrounded;
    private bool muerto = false;

    private float attackPointX;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        attackPointX = Mathf.Abs(attackPoint.localPosition.x);
    }

    void Update()
    {
        if (muerto)
            return;

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

        anim.SetFloat("Speed", Mathf.Abs(move));
        anim.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        anim.SetBool("IsGrounded", isGrounded);

        if (move != 0)
        {
            bool miraIzquierda = move < 0;

            spriteRenderer.flipX = miraIzquierda;

            Vector3 attackPosition = attackPoint.localPosition;

            attackPosition.x = miraIzquierda
                ? -attackPointX
                : attackPointX;

            attackPoint.localPosition = attackPosition;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("attack");

            Collider2D enemyCollider = Physics2D.OverlapCircle(
                attackPoint.position,
                attackRadius,
                enemyLayer
            );

            if (enemyCollider != null)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.Morir();
                }
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            Morir();
        }
    }

    public void Morir()
    {
        if (muerto)
            return;

        muerto = true;

        rb2D.linearVelocity = Vector2.zero;
        anim.SetTrigger("death");
    }

    public void Destruir()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }
    }
}
```

---

## 16. Enemy.cs completo

El enemigo patrulla, puede recibir el ataque y se elimina al terminar su animación de muerte:

```csharp
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;

    public Transform puntoA;
    public Transform puntoB;

    private Transform objetivo;
    private Animator animator;
    private Collider2D enemyCollider;

    private bool muerto = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        objetivo = puntoB;
    }

    void Update()
    {
        if (muerto)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            objetivo.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, objetivo.position) < 0.1f)
        {
            if (objetivo == puntoA)
            {
                objetivo = puntoB;
            }
            else
            {
                objetivo = puntoA;
            }
        }
    }

    public void Morir()
    {
        if (muerto)
            return;

        muerto = true;

        enemyCollider.enabled = false;
        animator.SetTrigger("Death");
    }

    public void Destruir()
    {
        Destroy(gameObject);
    }
}
```

---

## 17. Configuración final

### Player

```text
Player
├── Sprite Renderer
├── Rigidbody 2D
├── Box Collider 2D
├── Animator
├── Player (Script)
├── GroundCheck
└── AttackPoint
```

En `Player (Script)`:

```text
Ground Check  → GroundCheck
Ground Layer  → Floor
Attack Point  → AttackPoint
Enemy Layer   → Enemy
```

### Enemy

```text
Enemy
├── Sprite Renderer
├── Box Collider 2D
│   └── Is Trigger: activado
├── Animator
└── Enemy (Script)
```

En `Enemy (Script)`:

```text
Punto A → PuntoA
Punto B → PuntoB
```

Y el objeto debe pertenecer a:

```text
Layer: Enemy
```

---

## Comprobación

Antes de continuar, comprueba:

- [ ] El Player reproduce `Attack` al pulsar ++x++.
- [ ] `Attack` vuelve a `Idle` al terminar.
- [ ] `AttackPoint` está situado delante del personaje.
- [ ] El círculo de ataque se ve al seleccionar el Player.
- [ ] El enemigo pertenece a la `Layer Enemy`.
- [ ] El enemigo tiene un `Collider2D`.
- [ ] El golpe solo afecta al enemigo cuando está dentro del radio.
- [ ] El enemigo reproduce `Death` antes de desaparecer.
- [ ] El `Animation Event` llama a `Destruir()`.
- [ ] El enemigo no empuja al Player.
- [ ] El Player muere al entrar en contacto con el enemigo.
- [ ] El Player deja de aceptar movimiento cuando comienza su muerte.
- [ ] El enemigo patrulla entre `PuntoA` y `PuntoB`.
- [ ] El enemigo deja de moverse cuando comienza `Death`.
- [ ] `AttackPoint` cambia de lado cuando cambia la orientación del Player.
