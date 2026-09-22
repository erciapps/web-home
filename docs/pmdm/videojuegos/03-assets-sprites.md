# 3. Assets y sprites

Los **assets** son los recursos que utiliza el juego: imágenes, sonidos, animaciones, fuentes, scripts, tiles, etc.

Para un juego 2D normalmente necesitaremos como mínimo:

- fondo;
- tiles o elementos del escenario;
- personaje;
- objetos recogibles;
- obstáculos;
- elementos de interfaz.

## Importar recursos

Arrastra los archivos a la ventana **Project** de Unity y colócalos dentro de la carpeta correspondiente del proyecto.

## Configurar sprites de pixel art

Una configuración habitual es:

```text
Texture Type: Sprite (2D and UI)
Filter Mode: Point (no filter)
Compression: None
Pixels Per Unit: 16
```

`Point` evita que Unity suavice los píxeles y mantiene el aspecto del pixel art.

!!! note
    `Pixels Per Unit` dependerá del asset que utilicemos. Si el recurso está diseñado en 16×16, 32×32, etc., conviene respetar esa referencia.

## Spritesheets

Si una imagen contiene varios sprites:

```text
Sprite Mode: Multiple
```

Después:

```text
Open Sprite Editor
→ Slice
→ Type: Grid by Cell Size
→ Pixel Size: 16 x 16
→ Slice
→ Apply
```

El tamaño de la celda debe coincidir con el spritesheet utilizado.
