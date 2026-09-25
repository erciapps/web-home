# Git · Guía práctica

Git es una herramienta de **control de versiones**. Nos permite guardar el historial de un proyecto, saber qué hemos cambiado, volver a versiones anteriores y trabajar con repositorios remotos como **GitErcilla**, GitHub o GitLab.

!!! tip "Idea principal"
    No trabajamos directamente «en GitHub». **Git** controla las versiones en nuestro equipo y un **repositorio remoto** nos permite guardar y compartir esos commits en un servidor.

## 1. Conceptos básicos

| Concepto | Qué significa |
| --- | --- |
| **Repositorio** | Proyecto cuyo historial controla Git. |
| **Repositorio local** | El repositorio que tenemos en nuestro equipo. |
| **Repositorio remoto** | El repositorio alojado en un servidor. |
| **Commit** | Punto guardado del historial del proyecto. |
| **Staging** | Zona donde preparamos los cambios que irán al siguiente commit. |
| **Rama** | Línea de trabajo independiente. La principal suele llamarse `main`. |
| **Clone** | Copiar un repositorio remoto a nuestro equipo. |
| **Push** | Enviar nuestros commits al remoto. |
| **Pull** | Traer e integrar cambios del remoto. |
| **.gitignore** | Archivo que indica qué archivos o carpetas no debe controlar Git. |

## 2. Instalación

=== "Debian / Ubuntu"

    ```bash
    sudo apt update
    sudo apt install git
    ```

=== "Windows"

    Instala **Git for Windows** desde su página oficial. También puedes utilizar Git desde la terminal integrada de VS Code.

=== "macOS"

    ```bash
    brew install git
    ```

Comprueba la instalación:

```bash
git --version
```

## 3. Configuración inicial

La primera vez indicamos el nombre y correo que aparecerán como autor de nuestros commits:

```bash
git config --global user.name "Nombre Apellidos"
git config --global user.email "correo@ejemplo.com"
```

Comprobar la configuración:

```bash
git config --global --list
```

!!! note
    `--global` aplica la configuración a todos los repositorios de ese usuario. La autenticación contra GitErcilla, GitHub u otro servidor se configurará según el método que utilicemos en clase; no es necesario guardar la contraseña dentro de la configuración de Git.

## 4. Dos formas de empezar

### Opción A · Crear un repositorio desde un proyecto existente

Sitúate en la carpeta raíz del proyecto:

```bash
cd mi-proyecto
git init
git branch -M main
```

Comprueba su estado:

```bash
git status
```

### Opción B · Clonar un repositorio que ya existe

```bash
git clone URL_DEL_REPOSITORIO
cd nombre-del-repositorio
```

`git clone` descarga los archivos, el historial y la configuración del remoto.

## 5. El ciclo de trabajo diario

Este es el flujo que más utilizaremos:

```text
MODIFICAR → STATUS → ADD → COMMIT → PUSH
```

### 1. Ver qué ha cambiado

```bash
git status
```

### 2. Preparar los cambios

Un archivo concreto:

```bash
git add Program.cs
```

Todos los cambios:

```bash
git add .
```

### 3. Crear un commit

```bash
git commit -m "Añade formulario de usuarios"
```

El mensaje debe explicar brevemente **qué cambio contiene** el commit.

### 4. Enviar los commits al servidor

```bash
git push
```

La primera vez, si la rama todavía no está asociada al remoto, puede ser necesario:

```bash
git push -u origin main
```

## 6. Conectar un proyecto local con un remoto

Si hemos utilizado `git init` y después hemos creado un repositorio vacío en GitErcilla/GitHub:

```bash
git remote add origin URL_DEL_REPOSITORIO
git push -u origin main
```

Ver los remotos configurados:

```bash
git remote -v
```

## 7. Traer cambios

Antes de continuar un proyecto que también se ha modificado desde otro equipo:

```bash
git pull
```

Para descargar información del remoto **sin integrarla todavía**:

```bash
git fetch
```

!!! warning "Trabajo en varios equipos"
    Si has trabajado desde casa y después continúas en clase, acostúmbrate a hacer `git pull` **antes de empezar a modificar archivos**.

## 8. `.gitignore`

`.gitignore` evita subir archivos generados, temporales, configuraciones locales o carpetas que no deben formar parte del repositorio.

Ejemplo:

```gitignore
bin/
obj/
.idea/
.vscode/
*.log
.env
```

!!! warning
    Un archivo con contraseñas, tokens o claves **no debe subirse al repositorio**. Añadirlo después a `.gitignore` no borra automáticamente las copias que ya existan en el historial.

## 9. Ver historial y diferencias

Historial resumido:

```bash
git log --oneline
```

Historial con ramas:

```bash
git log --oneline --graph --all
```

Cambios que todavía no hemos añadido a staging:

```bash
git diff
```

Cambios que ya están preparados para el próximo commit:

```bash
git diff --staged
```

## 10. Ramas

Crear una rama y cambiar directamente a ella:

```bash
git switch -c feature-login
```

Ver las ramas:

```bash
git branch
```

Volver a la principal:

```bash
git switch main
```

Integrar los cambios de otra rama en la actual:

```bash
git merge feature-login
```

Cuando la rama ya no sea necesaria:

```bash
git branch -d feature-login
```

## 11. Conflictos

Un conflicto aparece cuando Git no puede decidir automáticamente cómo combinar dos cambios.

El flujo habitual es:

```text
1. Git indica los archivos con conflicto
2. Abrimos y corregimos esos archivos
3. git add ...
4. git commit
5. git push
```

Comprueba siempre qué archivos están afectados con:

```bash
git status
```

## 12. Deshacer cambios sin complicarnos

Descartar cambios locales de un archivo que **todavía no hemos añadido**:

```bash
git restore archivo.txt
```

Sacar un archivo del staging sin borrar sus cambios:

```bash
git restore --staged archivo.txt
```

!!! danger "Cuidado con los comandos destructivos"
    Comandos como `git reset --hard` pueden eliminar cambios locales. No los utilices como primera opción para «arreglar Git».

## 13. Comandos de consulta útiles

```bash
git status
git log --oneline
git branch
git remote -v
git diff
```

Estos comandos son seguros para consultar el estado del repositorio y son un buen punto de partida cuando no sabes qué está ocurriendo.

## 14. Flujo recomendado en clase

Al comenzar:

```bash
git pull
```

Trabaja normalmente y, cuando completes un cambio con sentido:

```bash
git status
git add .
git commit -m "Describe el cambio realizado"
git push
```

No esperes al final del proyecto para hacer un único commit. Es mejor crear **commits pequeños y con sentido**.

## 15. Chuleta rápida

| Quiero... | Comando |
| --- | --- |
| Ver el estado | `git status` |
| Clonar un repositorio | `git clone URL` |
| Traer cambios | `git pull` |
| Preparar todos los cambios | `git add .` |
| Crear un commit | `git commit -m "mensaje"` |
| Enviar commits | `git push` |
| Ver el historial | `git log --oneline` |
| Crear una rama | `git switch -c nombre` |
| Cambiar de rama | `git switch nombre` |
| Ver diferencias | `git diff` |
