# 8. Animaciones

Vamos a utilizar el sistema **Animator** para cambiar la animación del personaje según lo que esté haciendo.

## Crear animaciones

Crea una carpeta:

```text
Assets/MiJuego/Animations
```

Selecciona el Player y abre:

```text
Window
→ Animation
→ Animation
```

Crea, como mínimo:

```text
Idle.anim
Run.anim
Jump.anim
```

Arrastra a cada animación los frames correspondientes.

## Animator Controller

El Player necesita un componente `Animator` y un `Animator Controller` asignado.

Abre:

```text
Window
→ Animation
→ Animator
```

Arrastra las animaciones al Animator.

## Parámetro Speed

En **Parameters** crea:

```text
Float → Speed
```

Transiciones:

```text
Idle → Run
Speed > 0.1
```

```text
Run → Idle
Speed < 0.1
```

En ambas:

```text
Has Exit Time: desactivado
Transition Duration: 0
```

## Actualizar el Animator desde C#

Declara:

```csharp
private Animator anim;
```

En `Start()`:

```csharp
anim = GetComponent<Animator>();
```

Y en `Update()`:

```csharp
anim.SetFloat("Speed", Mathf.Abs(move));
```

## Animación de salto

Puedes añadir:

```text
Float → VerticalVelocity
Bool  → IsGrounded
```

Y actualizar ambos parámetros desde el script:

```csharp
anim.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
anim.SetBool("IsGrounded", isGrounded);
```
