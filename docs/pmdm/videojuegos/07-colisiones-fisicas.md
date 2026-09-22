# 7. Colisiones y físicas

El escenario y el personaje necesitan colliders para interactuar correctamente.

## Collider del Player

El Player puede utilizar:

```text
Box Collider 2D
Rigidbody 2D
```

Ajusta el collider con **Edit Collider** para que se aproxime al cuerpo del personaje.

## Colisiones del escenario

Selecciona el Tilemap `Ground` y añade:

```text
Tilemap Collider 2D
Composite Collider 2D
Rigidbody 2D
```

En `Rigidbody 2D`:

```text
Body Type: Static
```

En `Tilemap Collider 2D`:

```text
Composite Operation: Merge
```

El `Tilemap Collider 2D` crea las colisiones de cada tile y el `Composite Collider 2D` las combina para formar superficies más continuas.

## Evitar que el personaje se quede pegado a paredes

Crea un material físico:

```text
Assets
→ Botón derecho
→ Create
→ 2D
→ Physics Material 2D
```

Ponle, por ejemplo:

```text
PlayerPhysics
```

Configura:

```text
Friction: 0
```

Después asigna ese material al `Box Collider 2D` del Player.
