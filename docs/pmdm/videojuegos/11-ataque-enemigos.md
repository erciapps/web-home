# 12. Ataque y enemigos

Después de [crear el enemigo y su patrulla](11-enemigo-patrulla.md), añadiremos el combate. Conservamos Player.Morir(), su animación y el reinicio del nivel preparados en [Peligros y reinicio](10-peligros-reinicio.md). Ahora programaremos el ataque y la muerte del enemigo.

Al terminar tendremos:

- una animación de ataque para el Player;
- una zona de ataque delante del personaje;
- detección de enemigos mediante una `Layer`;
- una animación de muerte para el enemigo;
- eliminación del enemigo cuando termina la animación;
- muerte del Player al tocar un enemigo;
- la patrulla que ya tenemos se detendrá cuando muera el enemigo.

---

## 1. Crear la animación de ataque

Si ya preparaste **attack** en [Animaciones](08-animaciones.md), reutiliza ese clip. Si todavía no existe, selecciona `Player` y abre:

```text
Window
→ Animation
→ Animation
```

Crea la animación:

```text
attack.anim
```

Añade los frames correspondientes al ataque.

Como el ataque debe reproducirse una sola vez, selecciona el clip `attack` en **Project** y comprueba que:

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

Añade el estado `attack`.

En **Parameters** crea:

```text
Trigger → attack
```

La transición de entrada queda:

```text
idle → attack
Condition: attack
Has Exit Time: desactivado
Transition Duration: 0
```

La transición de vuelta:

```text
attack → idle
Has Exit Time: activado
Transition Duration: 0
Conditions: ninguna
```

De esta forma el código inicia el ataque y, cuando termina la animación, el Animator vuelve a `idle`.

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

## 6. Ampliar el script Enemy

Abre **Enemy.cs**, que ya está asignado al enemigo. Conserva speed, los puntos, objetivo, animator, Start y la patrulla de Update.

Añade dentro de la clase estos dos métodos; más abajo incorporaremos la comprobación que impide repetir la muerte y detiene la patrulla:

```csharp
public void Morir()
{
    // Activamos el trigger de muerte del Animator del enemigo.
    animator.SetTrigger("Death");
}

public void Destruir()
{
    // Se llamará cuando termine la animación de muerte.
    Destroy(gameObject);
}
```

El Player no destruye al enemigo directamente: llama a Enemy.Morir(). Enemy controla su animación y se elimina al terminarla.

---

## 7. Animación de muerte del enemigo

Selecciona los fotogramas de muerte del enemigo y crea un clip distinto del death del Player:

```text
EnemyDeath.anim
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
enemy → Death
Condition: Death
Has Exit Time: desactivado
Transition Duration: 0
```

El estado inicial **enemy** reproduce la patrulla animada que creamos antes. Arrastra EnemyDeath.anim al controlador **del enemigo** y renombra el nuevo estado como **Death**, sin modificar el del Player. No necesitamos una transición de vuelta a `enemy`, ya que el enemigo se eliminará al terminar la animación.

!!! warning
    En este proyecto el parámetro del enemigo se llama `Death`, con `D` mayúscula. Por eso el código utiliza `animator.SetTrigger("Death")`.

---

## 8. Destruir el enemigo al terminar la animación

No llamamos a `Destroy()` al recibir el golpe porque desaparecería inmediatamente y no llegaríamos a ver la animación.

Utilizaremos un **Animation Event**.

1. Selecciona el enemigo.
2. Abre `Window → Animation → Animation`.
3. Selecciona el clip `EnemyDeath`.
4. Sitúate en el último fotograma de la animación.
5. Pulsa **Add Animation Event**.
6. Selecciona el marcador creado.
7. En el Inspector, en `Function`, selecciona:

```text
Enemy/Methods/Destruir()
```

Al pulsar X, el Player detecta al Enemy y llama a Morir(). El trigger Death inicia el clip; su evento final llama a Destruir(), que elimina el objeto.

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

El Player ya tiene **muerto**, **Morir()**, la comprobación al principio de Update y **ReiniciarNivel()**, preparados en [Peligros y reinicio](10-peligros-reinicio.md#muerte-player). **No los declares otra vez**. Añade únicamente este método dentro de Player:

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    // Comprobamos si el objeto que entra es un enemigo.
    Enemy enemy = collision.GetComponent<Enemy>();

    if (enemy != null)
    {
        // Reutilizamos la misma muerte que provocan Killzone y Spikes.
        Morir();
    }
}
```

El trigger del enemigo permite detectar el contacto sin empujar al jugador. Este conserva su Rigidbody 2D. Al morir, deja de aceptar controles y su Animation Event reinicia la escena al terminar death, igual que al tocar los otros peligros.

---

## 11. Detener la patrulla cuando el enemigo muere

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

## 12. Ajustar AttackPoint al cambio de dirección

Continuamos con la orientación mediante **la escala X del Transform**, que añadimos en [Movimiento, orientación y salto](05-movimiento-salto.md#orientacion-del-personaje):

```csharp
if (move != 0)
{
    transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
}
```

Como **AttackPoint es hijo del Player**, también se refleja al cambiar el signo de X. Colócalo delante del personaje, con posición local X positiva cuando el sprite original mira a la derecha. GroundCheck debe seguir centrado en X = 0.

No inviertas además la posición local de AttackPoint ni apliques SpriteRenderer.flipX: reflejarlo dos veces dejaría el punto de ataque en el lado equivocado. Conserva un único bloque de orientación en Update.

Prueba a caminar a izquierda y derecha y comprueba el círculo de ataque con el Player seleccionado.

---

## 13. Player.cs completo

El script final reúne el movimiento, salto, animaciones, orientación, ataque, muerte y reinicio. En Morir también limpiamos el trigger attack: un ataque pendiente no debe interrumpir la muerte. Mantén el evento ReiniciarNivel() en el clip death del Player:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Animator anim;

    private float move;
    private bool isGrounded;
    private bool muerto = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
            isGrounded = false;
        }

        anim.SetFloat("Speed", Mathf.Abs(move));
        anim.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        anim.SetBool("IsGrounded", isGrounded);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
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
        ) != null && rb2D.linearVelocity.y <= 0.1f;
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
        anim.SetFloat("Speed", 0f);
        anim.SetFloat("VerticalVelocity", 0f);
        anim.ResetTrigger("attack");
        anim.SetTrigger("Death");
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

## 14. Enemy.cs completo

El enemigo patrulla, puede recibir el ataque y se elimina al terminar su animación de muerte:

```csharp
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;

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

## 15. Configuración final

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

- [ ] El Player reproduce `attack` al pulsar ++x++.
- [ ] `attack` vuelve a `idle` al terminar.
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
- [ ] El evento ReiniciarNivel() del Player recarga la escena al terminar death.
- [ ] El enemigo patrulla entre `PuntoA` y `PuntoB`.
- [ ] El enemigo deja de moverse cuando comienza `Death`.
- [ ] `AttackPoint` cambia de lado cuando cambia la orientación del Player.

Continúa con [Proyectiles y vida de los enemigos](12_proyectiles_y_vida_enemigos_unity.md).
