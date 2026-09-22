# Plantilla para migrar otro módulo

Ejemplo: migrar **Sistemas Informáticos** al mismo `web-home`.

## 1. Estructura

```text
docs/
└── si/
    ├── index.md
    ├── ud1-linux/
    │   ├── index.md
    │   └── comandos.md
    ├── ud2-usuarios/
    └── ud3-redes/
```

## 2. Navegación

En `mkdocs.yml`, sustituir:

```yaml
- Sistemas Informáticos ↗: https://si-erciapps.sytes.net
```

por algo como:

```yaml
- Sistemas Informáticos:
    - Inicio: si/index.md
    - UD1 · Linux:
        - Introducción: si/ud1-linux/index.md
        - Comandos: si/ud1-linux/comandos.md
```

## 3. Portada

En `docs/index.md`, cambiar el `href` de la tarjeta de SI:

```html
href="si/"
```

Con esto SI dejará de depender de otro GitHub Pages y pasará a estar integrado en `web-home` igual que PMDM.
