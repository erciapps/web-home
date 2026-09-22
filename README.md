# web-home · MkDocs + Material

Portal único de ERCI Apps para los módulos de DAM.

## Módulos integrados

- **Sistemas Informáticos** → `docs/si/`
- **PMDM** → `docs/pmdm/`

## Módulos todavía externos

- Desarrollo de Interfaces
- Proyecto II

## Ejecutar en local

### Windows

Doble clic en `iniciar.bat`.

### Linux/macOS

```bash
chmod +x iniciar.sh
./iniciar.sh
```

## Publicación

Cada `push` a `main` o `master` ejecuta GitHub Actions y publica el sitio generado en la rama `gh-pages`.

GitHub Pages debe configurarse como:

- **Source:** Deploy from a branch
- **Branch:** `gh-pages`
- **Folder:** `/ (root)`
- **Custom domain:** `erciapps.sytes.net`
