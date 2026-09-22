# web-home · MkDocs + Material

Este repositorio es la web principal de ERCI Apps y ahora utiliza **MkDocs + Material**.

## Estado de los módulos

- **PMDM**: integrado dentro de este mismo proyecto.
- **SI**: sigue enlazando temporalmente a su Docusaurus actual.
- **DDI**: sigue enlazando temporalmente a su Docusaurus actual.
- **Proyecto II**: sigue enlazando temporalmente a su Docusaurus actual.

La idea es migrar los demás módulos poco a poco a carpetas dentro de `docs/`.

## Ejecutar en local

### Windows

Doble clic en `iniciar.bat` o:

```powershell
python -m venv .venv
.venv\Scripts\Activate.ps1
pip install -r requirements.txt
mkdocs serve
```

### Linux/macOS

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
mkdocs serve
```

Abre: http://127.0.0.1:8000

## Publicar

Copia la carpeta `.git` de tu repositorio `web-home` dentro de este proyecto y ejecuta:

```bash
git add .
git commit -m "Migrar web-home a MkDocs Material e integrar PMDM"
git push
```

El workflow de GitHub Actions construye la web y actualiza la rama `gh-pages`.

En GitHub, deja **Settings → Pages → Deploy from a branch → gh-pages / root**.

El dominio personalizado se conserva mediante `docs/CNAME`.

## Migrar otro módulo más adelante

Ejemplo para Sistemas Informáticos:

1. Crear `docs/si/index.md` y sus unidades.
2. Cambiar en `mkdocs.yml` la entrada externa de SI por una navegación local.
3. Cambiar en `docs/index.md` el enlace de la tarjeta de SI a `si/`.

Consulta también `PLANTILLA_MODULO.md`.
