# 1. Crear el proyecto

## Objetivo

Crear un proyecto 2D en Unity 6, dejar preparado el editor y organizar desde el principio las carpetas que vamos a utilizar.

## Crear el proyecto

En **Unity Hub**:

1. Pulsa **New project**.
2. Selecciona la plantilla **Universal 2D**.
3. Indica el nombre del proyecto.
4. Selecciona la ubicación donde se guardará.
5. Pulsa **Create project**.

!!! note
    Para las prácticas de clase utilizaremos Unity 6 y una plantilla 2D.

## Configurar Rider

En Unity:

```text
Edit
→ Preferences
→ External Tools
→ External Script Editor
→ Rider
```

A partir de ese momento, al abrir un script C# desde Unity se utilizará Rider.

## Organización inicial

Dentro de `Assets` crea una carpeta para el juego y separa los recursos por tipo:

```text
Assets
└── MiJuego
    ├── Animations
    ├── Scripts
    ├── Sprites
    └── Tiles
```

No es obligatorio utilizar exactamente estos nombres, pero sí mantener una estructura clara desde el principio.
