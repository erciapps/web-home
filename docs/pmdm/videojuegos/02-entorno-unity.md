# 2. Entorno de Unity

Los paneles que utilizaremos con más frecuencia son:

| Panel | Para qué sirve |
|---|---|
| **Hierarchy** | Objetos que existen en la escena actual. |
| **Scene** | Zona en la que construimos visualmente el nivel. |
| **Game** | Vista que tendrá el jugador. |
| **Inspector** | Propiedades y componentes del objeto seleccionado. |
| **Project** | Archivos y recursos del proyecto. |
| **Console** | Mensajes, avisos y errores. |

## Una idea importante

Un **GameObject** es un objeto de la escena. Sus capacidades aparecen mediante **componentes**.

Por ejemplo, un personaje puede tener:

```text
Player
├── Transform
├── Sprite Renderer
├── Rigidbody2D
├── Collider2D
└── Player.cs
```
