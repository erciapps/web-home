# Ejercicio 4 · Control de personaje y cámaras 3D

En este ejercicio vamos a controlar una figura en un escenario 3D. El movimiento será **tipo vehículo**: avanzar, retroceder y girar sobre sí misma. Primero utilizaremos las teclas **W, A, S y D**; después añadiremos **botones táctiles** y una cámara que pueda cambiar entre **primera y tercera persona**.

Seguimos trabajando con **Unity 6.6**, **ProBuilder** y el **Input System**. No necesitamos modelos descargados ni animaciones para entender el movimiento.

!!! info "Objetivo de la práctica"
    Crear una escena `Control3D`, controlar un personaje con `Character Controller` y cambiar el punto de vista de la cámara. El código y las siguientes fases se incorporarán a medida que los probemos en clase.

## 1. Preparar la escena Control3D

Crea una escena llamada **`Control3D`** y guarda el archivo en `Assets/Scenes`.

En la jerarquía deben aparecer:

```text
Control3D
├── Main Camera
├── Directional Light
├── Global Volume
├── Suelo
└── Jugador
    └── Frente
```

### El suelo

Crea un `Plane` como `Suelo`, con `Position (0, 0, 0)` y `Scale (2, 1, 2)`. Asegúrate de que tiene un `Mesh Collider` que actúe como suelo.

### La figura Jugador

Podemos utilizar una **Sphere de ProBuilder**, como la que estamos preparando en clase. No tiene que ser una cápsula: lo importante es que seamos capaces de distinguir su orientación y mover su controlador.

En `Jugador`:

1. Añade un **Character Controller** desde `Add Component`.
2. No añadas un `Rigidbody`: en esta actividad moveremos el objeto con `CharacterController.Move()`.
3. Si el objeto conserva un `Mesh Collider` o `Sphere Collider` adicional, elimínalo para no duplicar la colisión del propio personaje.
4. Con **Edit Collider**, comprueba que el contorno del `Character Controller` envuelve la figura y que su base toca el suelo. **ProBuilder puede situar el pivote de la malla en un lugar distinto de su centro visual**: si ocurre, ajusta `Center` y `Height` del controlador o corrige la geometría antes de continuar.

> En nuestra primera prueba usamos una esfera ProBuilder, un controlador de altura 1 y radio 0,5. Esos valores son de partida, no una garantía de que el collider esté alineado con cualquier malla.

### Indicar el frente del personaje

Añade un cubo pequeño llamado **`Frente`**, hijo de `Jugador` (no otro objeto al mismo nivel). Sus coordenadas son **locales al jugador**:

| Transform de Frente | X | Y | Z |
| --- | ---: | ---: | ---: |
| Position | 0 | 0 | 0.6 |
| Rotation | 0 | 0 | 0 |
| Scale | 0.3 | 0.3 | 0.3 |

`Frente` señala el **eje Z positivo** de `Jugador`, que será la dirección de avance. Como es una referencia visual, quita su `Mesh Collider` si ProBuilder lo ha creado.

!!! tip "Por qué es hijo de Jugador"
    Al girar o mover al personaje, `Frente` se desplazará y girará con él. Así veremos claramente hacia dónde apunta, aunque el cuerpo sea una esfera simétrica.

### Comprobación antes de programar

- `Frente` aparece indentado dentro de `Jugador` en la jerarquía.
- `Jugador` tiene `Character Controller` y no tiene `Rigidbody`.
- El controlador envuelve la malla visible y queda apoyado sobre `Suelo`.
- No hay colliders sobrantes en `Frente`.
- `Main Camera` conserva la etiqueta `MainCamera`.

## 2. Próximo paso: movimiento tipo vehículo

Prepararemos un script `MovimientoJugador.cs` con este comportamiento:

| Tecla | Acción |
| --- | --- |
| W | Avanzar hacia `transform.forward` |
| S | Retroceder |
| A | Girar alrededor del eje Y hacia la izquierda |
| D | Girar alrededor del eje Y hacia la derecha |

Después crearemos botones táctiles que respondan **mientras estén pulsados**, no solo a `onClick`, y alternaremos entre dos enfoques de cámara.

---

[← Ejercicio 3 · Clasificador de animales](ejercicio-3-animales.md) · [Volver a PMDM](index.md)
