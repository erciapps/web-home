# UD2 · Usuarios, permisos y procesos

Esta unidad está dedicada a la administración básica de GNU/Linux: **usuarios, grupos, permisos, procesos, servicios y registros del sistema**. Terminaremos creando un pequeño servicio web en Python para preparar el trabajo de redes de la UD3.

## Objetivos de la unidad

Al terminar deberás poder:

- Interpretar los permisos mostrados por `ls -l` y `ls -la`.
- Distinguir propietario, grupo propietario y resto de usuarios.
- Entender qué significa `r`, `w` y `x` en **archivos** y en **directorios**.
- Modificar permisos con `chmod` en modo simbólico y octal.
- Identificar y modificar, cuando tengas autorización, el propietario y el grupo con `chown`.
- Comprobar los cambios realizados sin afectar a otros archivos del sistema.
- Identificar y controlar procesos mediante su PID y comandos de Linux.
- Crear un pequeño proceso Python y observarlo desde la terminal.
- Arrancar, detener y revisar servicios mediante `systemctl` y `journalctl`.
- Ejecutar una aplicación Flask de prueba como servicio local, antes de estudiar puertos y redes.

## 1. Propietario, grupo y otros usuarios

En Linux, cada archivo o directorio tiene un **usuario propietario** y un **grupo propietario**. Sus permisos tradicionales se organizan en tres bloques:

| Bloque | Letra habitual | ¿A quién se aplica? |
| --- | --- | --- |
| Usuario propietario | `u` | Al propietario del archivo o directorio |
| Grupo propietario | `g` | A los usuarios a quienes corresponda ese grupo |
| Otros | `o` | A quienes no entran en los dos casos anteriores |

Puedes consultar tu identidad y tus grupos con:

```bash
whoami
id
groups
```

Y examinar un archivo y un directorio con:

```bash
ls -l archivo.txt
ls -la carpeta
```

!!! tip "¿Qué muestra `ls -la`?"
    `ls -la carpeta` muestra todos los elementos de la carpeta, también los ocultos. La entrada `.` corresponde a **la propia carpeta** y muestra sus permisos; `..` corresponde al directorio superior. En cambio, `ls -l carpeta` no muestra las entradas ocultas.

Ejemplo de salida de `ls -l`:

```text
-rw-r----- 1 ana docentes 120 oct 15  informe.txt
```

| Fragmento | Significado |
| --- | --- |
| `-` | Es un archivo normal (`d` indicaría un directorio) |
| `rw-` | La propietaria puede leer y escribir |
| `r--` | Los integrantes del grupo correspondiente pueden leer |
| `---` | Los demás usuarios no tienen permisos |
| `ana` | Usuario propietario |
| `docentes` | Grupo propietario |

!!! info "El primer carácter no es un permiso"
    En `drwxr-x---`, la `d` identifica un **directorio**. Los nueve caracteres siguientes son los tres bloques de permisos.

## 2. ¿Qué significan `r`, `w` y `x`?

**Los mismos símbolos tienen efectos distintos dependiendo del tipo de elemento.**

| Permiso | En un archivo | En un directorio |
| --- | --- | --- |
| `r` (read) | Leer su contenido | Consultar la lista de nombres que contiene |
| `w` (write) | Modificar o sobrescribir su contenido | Crear, eliminar o cambiar de nombre entradas, normalmente **junto con `x`** |
| `x` (execute) | Ejecutarlo como programa, si es ejecutable válido | **Atravesarlo** y acceder a entradas cuyos nombres se conocen |

### Ejemplo con archivos

- `-rw-r--r--` (**644**): el propietario lee y modifica; el resto puede leer.
- `-rwxr-xr-x` (**755**): el propietario puede leer, escribir y ejecutar; los demás, leer y ejecutar.
- `-rw-------` (**600**): solamente el propietario puede leer y escribir.

### Ejemplo con directorios

- `drwxr-xr-x` (**755**): el propietario puede gestionar las entradas; los demás pueden recorrer la carpeta y listar nombres.
- `drwx------` (**700**): solo el propietario puede entrar, listar y gestionar entradas.
- `drwx--x--x` (**711**): los demás pueden atravesar la carpeta y acceder a entradas cuyo nombre conocen, pero no obtener el listado normal.
- `drwxr--r--` (**744**): los demás pueden conocer los nombres, pero **no atravesar** el directorio.

!!! warning "Importante: eliminar un archivo"
    Para **borrar o renombrar** un archivo se comprueban principalmente los permisos del **directorio que lo contiene** (escritura y ejecución), no el permiso `w` del propio archivo. El *sticky bit*, las ACL y otras restricciones pueden limitarlo.

!!! warning "No confundir"
    Poder entrar en un directorio no concede automáticamente permiso para leer todos sus archivos. También necesitas los permisos adecuados sobre el archivo y poder atravesar los directorios de su ruta.

## 3. `chmod`: modificar permisos

`chmod` significa **change mode**. Cambia los permisos de un archivo o directorio, pero **no cambia su propietario**.

### Modo simbólico

```bash
chmod u+x programa.sh       # Añade ejecución al propietario
chmod g+r informe.txt       # Añade lectura al grupo
chmod o-r informe.txt       # Quita lectura a otros
chmod u=rw,g=r,o= archivo.txt
chmod a+r apuntes.txt       # Añade lectura a todos
```

- `u`: propietario; `g`: grupo; `o`: otros; `a`: todos.
- `+`: añadir; `-`: quitar; `=`: establecer exactamente los permisos indicados.

### Modo numérico u octal

Cada permiso suma un valor:

| Permiso | Valor |
| --- | ---: |
| Lectura `r` | 4 |
| Escritura `w` | 2 |
| Ejecución `x` | 1 |

Para cada bloque se suman los valores. Por ejemplo, `rwx = 4+2+1 = 7`; `r-x = 4+0+1 = 5`; `r-- = 4`.

```bash
chmod 644 informe.txt       # rw- r-- r--
chmod 600 privado.txt       # rw- --- ---
chmod 755 programa.sh       # rwx r-x r-x
chmod 700 carpeta_privada   # rwx --- ---
chmod 750 carpeta_equipo    # rwx r-x ---
```

El **primer dígito** corresponde al propietario, el **segundo** al grupo y el **tercero** a los demás usuarios.

!!! warning "Evita usar `777` como solución universal"
    En un directorio, `777` permite a todos crear, borrar o renombrar entradas (salvo restricciones adicionales). Utiliza únicamente los permisos necesarios. Evita `chmod -R` sin revisar primero a qué directorio afectará.

## 4. `chown`: modificar propietario y grupo

`chown` significa **change owner**. Sirve para modificar **quién es el propietario** y, opcionalmente, **qué grupo es propietario**.

```bash
sudo chown ana informe.txt               # Cambia el usuario propietario
sudo chown ana:docentes informe.txt      # Cambia propietario y grupo
sudo chown :docentes informe.txt         # Cambia solo el grupo
```

También existe `chgrp` para cambiar el grupo:

```bash
chgrp docentes informe.txt
```

Por regla general, cambiar el **usuario propietario** requiere privilegios de administración. Cambiar únicamente el grupo puede estar permitido para el propietario si pertenece al grupo de destino.

!!! danger "Uso de `sudo` y `-R`"
    `sudo` eleva los privilegios y `chown -R` aplica cambios recursivamente. En clase solo los utilizaremos en una **carpeta de pruebas propia** y cuando el entorno lo permita. No cambies la propiedad de `/etc`, `/usr`, `/home` o `/workspace` completos.

### Comparación rápida

| Comando | ¿Qué cambia? | Ejemplo |
| --- | --- | --- |
| `chmod` | Lectura, escritura y ejecución | `chmod 640 informe.txt` |
| `chown` | Usuario propietario y/o grupo | `sudo chown ana:docentes informe.txt` |
| `chgrp` | Solo grupo propietario | `chgrp docentes informe.txt` |

## 5. Comprobar los cambios

```bash
ls -l archivo.txt
ls -la directorio
stat -c '%A %a %U:%G %n' archivo.txt
```

Con `ls -la directorio`, fíjate en la línea `.` para ver los permisos del propio directorio. La salida de `stat` permite comprobar a la vez permisos simbólicos, permisos numéricos, usuario y grupo. Si aparece una `+` al final de los permisos de `ls`, puede haber ACL adicionales que debes revisar aparte.

## 6. Crear usuarios y grupos

Linux también permite administrar **cuentas de usuario** y **grupos** desde la terminal:

- `groupadd`: crea un grupo.
- `useradd -m -g grupo -s /bin/bash usuario`: crea una cuenta, su directorio personal y asigna grupo y shell.
- `chpasswd`: establece contraseñas mediante entrada estándar.
- `id usuario` y `groups usuario`: muestran su identidad y grupos.
- `su - usuario`: intenta iniciar una sesión como ese usuario.

El fichero `/etc/passwd` contiene datos públicos de las cuentas, como nombre y shell; `/etc/group` contiene los grupos. **No contienen las contraseñas en texto claro**: Linux guarda sus hashes normalmente en `/etc/shadow`, un fichero protegido.

A continuación, utilizarás estos comandos en una práctica guiada con un TXT.

## 7. ¿Qué es un proceso?

Un **programa** es un archivo que contiene instrucciones (por ejemplo, `python3`). Un **proceso** es ese programa **mientras se está ejecutando**. Puedes abrir dos veces el mismo programa y tendrás dos procesos diferentes.

Cada proceso dispone de un **PID** (*Process ID*), un número que identifica esa ejecución. Puede consumir CPU, memoria y otros recursos. Un proceso puede ejecutarse en **primer plano** (la terminal queda ocupada) o en **segundo plano** (podemos seguir utilizando la terminal).

### `ps`: consultar procesos

~~~bash
ps                 # Procesos asociados a la terminal
ps -u "$USER"      # Procesos del usuario actual
ps -ef             # Todos los procesos con más información
ps -p 1234         # Consultar un PID concreto (sustituir el número)
~~~

En una salida habitual verás columnas como `PID` (identificador), `TTY` (terminal), `TIME` (CPU acumulada) y `CMD` (comando).

### `top` y `htop`: monitorizar procesos

~~~bash
top
htop
~~~

Ambos permiten observar los procesos y el uso de CPU y memoria. `top` suele estar instalado; `htop` es una alternativa visual, con navegación y funciones interactivas. Para salir de `top` o `htop` pulsa `q` (en `htop` también puedes utilizar F10).

Si `htop` no está instalado, el profesor puede autorizar `sudo apt install htop` **en un Debian de laboratorio**, pero no es imprescindible.

### `kill` y `pkill`: finalizar procesos

~~~bash
sleep 120 &
pid=$!
echo "$pid"
ps -p "$pid"
kill "$pid"
~~~

- `sleep 120` mantiene un proceso esperando.
- `&` lo deja en segundo plano.
- `$!` guarda su PID.
- `kill "$pid"` envía una señal de finalización (SIGTERM) a **ese PID**; el proceso puede cerrarse ordenadamente.

Con `pkill` no hace falta conocer el PID: busca procesos **por nombre**. Puedes comprobar primero qué procesos coincidirían con `pgrep`.

~~~bash
sleep 120 &
pgrep -a -u "$USER" -x sleep
~~~

Si la consulta muestra **solo el proceso de prueba que acabas de lanzar**, puedes detenerlo con:

~~~bash
pkill -u "$USER" -x sleep
~~~

`pkill` podría terminar **varios procesos coincidentes**; por eso debes consultar primero `pgrep` y limitar la búsqueda al usuario actual. Si aparece más de uno, utiliza mejor `kill PID`. No pruebes estas herramientas sobre SSH, XRDP, VS Code, procesos del sistema ni los de otras personas.

| Comando | Función |
| --- | --- |
| `ps` | Consulta puntual de procesos |
| `top` | Monitorización interactiva |
| `htop` | Monitorización visual, si está instalado |
| `kill PID` | Envía una señal a un PID |
| `pgrep nombre` | Busca procesos por nombre |
| `pkill nombre` | Envía una señal a procesos que coinciden por nombre |

En **UD2.3** practicarás estos comandos y comprobarás el PID de un pequeño programa Python.

## 8. Primer servidor web con Flask

**Flask** es una biblioteca (*framework web*) de Python con la que podemos crear páginas y responder a peticiones HTTP. Al ejecutar una aplicación Flask también estamos iniciando **un proceso de Linux**, por lo que sirve de enlace con el apartado anterior.

### Crear y activar un entorno virtual

~~~bash
mkdir -p /workspace/ud2/flask
cd /workspace/ud2/flask

python3 -m venv .venv
source .venv/bin/activate
pip install flask
~~~

Un entorno virtual (`venv`) mantiene las dependencias de este proyecto separadas de las del resto del sistema. Si falta el soporte de entornos virtuales, el profesor podrá instalar `python3-venv` en el Debian de prácticas.

### No subir `.venv` a Git

Crea `/workspace/ud2/flask/.gitignore` con este contenido:

~~~gitignore
.venv/
__pycache__/
*.pyc
~~~

Git ignorará el directorio del entorno virtual y los archivos temporales de Python. **Sí se suben** `app.py`, los HTML y `.gitignore`. Si `.venv` ya estaba añadido a Git, hay que dejar de seguirlo antes de confirmar cambios.

### Nuestro primer `app.py`

~~~python
from flask import Flask

app = Flask(__name__)

@app.route("/saludo")
def saludo():
    return "Esto es un saludo"

@app.route("/")
def inicio():
    return "Esta es mi web"

if __name__ == "__main__":
    app.run(debug=True)
~~~

- `Flask(__name__)` crea la aplicación web.
- `@app.route("/")` asocia una **ruta URL** a una función Python.
- `return` devuelve lo que mostrará el navegador.
- `if __name__ == "__main__":` permite iniciar el servidor al ejecutar directamente el archivo.
- `app.run(debug=True)` inicia el **servidor de desarrollo** de Flask, normalmente en `127.0.0.1:5000`.

~~~bash
python3 app.py
~~~

Puedes visitar `http://127.0.0.1:5000/` y `http://127.0.0.1:5000/saludo`. Para detener el servidor, pulsa **Ctrl+C**. `debug=True` solo debe utilizarse en desarrollo y **nunca** en un servicio publicado.

### Próximo paso: HTML, botones y comandos Linux

En **UD2.4** sustituiremos el texto inicial por una plantilla `templates/index.html` con botones. Cada botón llamará a un **endpoint**, es decir, una ruta de Flask.

Por ejemplo, un botón consultará el usuario que ejecuta Flask (`whoami`), y otro mostrará procesos (`ps`). Python podrá lanzar esos comandos mediante `subprocess.run()` **sin solicitar privilegios adicionales ni recibir comandos arbitrarios del navegador**.

## 9. Servicios y enlace con redes

Un **servicio** es una funcionalidad que puede mantenerse disponible sin una terminal abierta. En Debian, `systemd` puede gestionar servicios y sus registros.

| Comando | Función |
| --- | --- |
| `systemctl status nombre` | Consultar el estado |
| `sudo systemctl start nombre` | Iniciar el servicio |
| `sudo systemctl stop nombre` | Detenerlo |
| `sudo systemctl restart nombre` | Reiniciarlo |
| `sudo systemctl enable nombre` | Habilitar su inicio al arrancar |
| `journalctl -u nombre` | Consultar sus registros |

`start` inicia ahora; `enable` prepara el arranque automático. **No son lo mismo**.

Después de probar Flask manualmente, la **UD2.4** incluye una ampliación con `systemd` para una máquina Debian de laboratorio autorizada. En la **UD3**, estudiaremos las direcciones IP, interfaces, puertos y conectividad que permiten acceder a servicios desde otros equipos.

## Ejercicios

<div class="si-units">
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD2.1</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Permisos y propietarios en Linux</h3>
    <p>Interpreta y modifica los permisos de archivos y directorios con chmod, comprueba sus efectos y practica chown de forma controlada.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>chmod · chown · ls · stat</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud2_1/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD2.2</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Crear usuarios y grupos desde un TXT</h3>
    <p>Lee usuarios, grupos, contraseñas y shells mediante Bash. Comprueba las cuentas y realiza pruebas de acceso por grupo.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>useradd · groupadd · chpasswd</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud2_2/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD2.3</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Procesos con Bash y Python</h3>
    <p>Consulta procesos, trabaja con PID, ejecuta un script Python y aprende a detener tus propios procesos.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>ps · top · htop · kill · pkill</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud2_3/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD2.4</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Flask, HTML y servicios</h3>
    <p>Crea una web Flask con botones que consultan comandos Linux y descubre cómo gestionar un servicio con systemd.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>Flask · HTML · subprocess · systemd</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud2_4/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD2.5</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Gestor de procesos Linux con Flask</h3>
    <p>Inicia, consulta y detén procesos de prueba con scripts Bash desde un formulario web.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>pgrep · kill · Flask · POST</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud2_5/">Ver ejercicio <span>→</span></a></div></div>
  </article>
</div>

!!! tip "Entorno de trabajo"
    Utiliza Debian y el terminal integrado de VS Code. Los scripts estarán en `/workspace/ud2/`. La práctica de creación de cuentas y la instalación de servicios requieren un entorno de pruebas donde tengas autorización para administrar el sistema.
