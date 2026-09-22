# 2. Entorno de Unity

Antes de construir el juego conviene identificar las zonas principales del editor.

| Panel | Para qué sirve |
|---|---|
| **Hierarchy** | Objetos que existen en la escena actual. |
| **Scene** | Zona en la que construimos visualmente el nivel. |
| **Game** | Vista que tendrá el jugador. |
| **Inspector** | Propiedades y componentes del objeto seleccionado. |
| **Project** | Archivos y recursos del proyecto. |
| **Console** | Mensajes, avisos y errores. |

## La escena

En la **Hierarchy** aparecen todos los objetos que forman la escena. Por ejemplo:

```text
SampleScene
├── Main Camera
├── Global Light 2D
├── Grid
├── Player
└── Coins
```

### SampleScene

Es la escena actual. Una escena puede representar un nivel, un menú, una pantalla de victoria o cualquier otra parte del juego.

### Main Camera

Determina qué parte del escenario verá el jugador.

### Global Light 2D

Controla la iluminación general de la escena 2D.

## GameObject y componentes

Un **GameObject** es un objeto de la escena. Sus capacidades se añaden mediante **componentes**.

Por ejemplo:

```text
Player
├── Transform
├── Sprite Renderer
├── Rigidbody2D
├── Box Collider 2D
└── Player.cs
```

El objeto es el mismo, pero cada componente añade una responsabilidad distinta: posición, imagen, físicas, colisiones o comportamiento programado.
