# UD2 · Usuarios, permisos y procesos

Esta unidad está dedicada a la administración básica de GNU/Linux. Comenzamos por **usuarios, grupos, permisos y propietarios**; más adelante incorporaremos procesos, servicios y registros del sistema.

## Objetivos de esta primera parte

Al terminar deberás poder:

- Interpretar los permisos mostrados por `ls -l` y `ls -ld`.
- Distinguir propietario, grupo propietario y resto de usuarios.
- Entender qué significa `r`, `w` y `x` en **archivos** y en **directorios**.
- Modificar permisos con `chmod` en modo simbólico y octal.
- Identificar y modificar, cuando tengas autorización, el propietario y el grupo con `chown`.
- Comprobar los cambios realizados sin afectar a otros archivos del sistema.

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
ls -ld carpeta
```

!!! tip "¿Por qué `ls -ld`?"
    Con `ls -l carpeta` se listan normalmente los elementos **dentro** de la carpeta. Con `ls -ld carpeta` se muestran los permisos de **la propia carpeta**.

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
ls -ld directorio
stat -c '%A %a %U:%G %n' archivo.txt
```

La salida de `stat` permite comprobar a la vez permisos simbólicos, permisos numéricos, usuario y grupo. Si aparece una `+` al final de los permisos de `ls`, puede haber ACL adicionales que debes revisar aparte.

## Ejercicios

<div class="si-units">
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD2.1</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Permisos y propietarios en Linux</h3>
    <p>Interpreta y modifica los permisos de archivos y directorios con chmod, comprueba sus efectos y practica chown de forma controlada.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>chmod · chown · ls · stat</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud2_1/">Ver ejercicio <span>→</span></a></div></div>
  </article>
</div>

!!! tip "Entorno de trabajo"
    Realiza las pruebas desde el terminal integrado de VS Code, en Debian, y exclusivamente dentro de `/workspace/ud2/`.
