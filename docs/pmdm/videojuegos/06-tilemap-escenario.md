# 6. Escenario con Tilemap

Las **Tilemaps** permiten construir escenarios 2D a partir de pequeñas piezas reutilizables.

## Crear una Tilemap

En **Hierarchy**:

```text
Botón derecho
→ 2D Object
→ Tilemap
→ Rectangular
```

Unity creará una estructura similar a:

```text
Grid
└── Tilemap
```

Renombra el Tilemap como `Ground`.

## Crear una Tile Palette

Abre:

```text
Window
→ 2D
→ Tile Palette
```

Crea una paleta y arrastra dentro los sprites que utilizarás como tiles.

Guarda los tiles generados, por ejemplo, en:

```text
Assets/MiJuego/Tiles
```

## Separar suelo y decoración

Es recomendable utilizar Tilemaps diferentes:

```text
Grid
├── Ground
└── Decoration
```

- `Ground`: elementos sobre los que puede caminar o chocar el jugador.
- `Decoration`: elementos puramente visuales.

## Fondo

Si utilizas una imagen de fondo, puedes dejarla detrás del escenario configurando en su `Sprite Renderer`:

```text
Order in Layer: -1
```
