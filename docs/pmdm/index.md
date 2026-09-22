# Programación Multimedia y Dispositivos Móviles

<div class="module-intro" markdown>

## PMDM · 2.º DAM

Este módulo **ya forma parte de `web-home`**. No es otra web ni otro GitHub Pages: toda su documentación se genera junto con la página principal mediante MkDocs + Material.

</div>

!!! info "Primera parte del curso"
    Comenzamos trabajando **videojuegos 2D con Unity 6**. Más adelante esta sección puede ampliarse con desarrollo móvil, multimedia u otros bloques del módulo.

## Contenidos

<div class="grid cards" markdown>

-   :material-gamepad-variant:{ .lg .middle } **Videojuegos 2D con Unity**

    ---

    Proyecto, escenas, GameObjects, componentes, sprites, físicas, scripts y movimiento.

    [:octicons-arrow-right-24: Empezar](videojuegos/)

-   :material-folder-multiple-image:{ .lg .middle } **Recursos**

    ---

    Assets, documentación, herramientas y material de apoyo para clase.

    [:octicons-arrow-right-24: Ver recursos](recursos.md)

</div>

## Cómo crecerá esta web

La estructura está pensada para que el resto de módulos se puedan migrar poco a poco al mismo proyecto:

```text
web-home/
└── docs/
    ├── index.md
    ├── pmdm/          ← ya integrado
    │   └── ...
    ├── si/            ← futuro
    │   └── ...
    ├── ddi/           ← futuro
    │   └── ...
    └── proyecto-ii/   ← futuro
        └── ...
```

Mientras un módulo no esté migrado, su tarjeta de la portada seguirá apuntando a su Docusaurus actual.
