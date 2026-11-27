# Tarea UD2.01 · Panel de mantenimiento

[Descargar proyecto plantilla (Tarea_UD2_01_Monitor.zip)](Tarea_UD2_01_Monitor.zip)

**Puntuación:** 10 puntos.

## Preparación

Desde la carpeta del proyecto:

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```

Completa los `TODO` de los archivos indicados. No es necesario modificar el HTML ni el CSS. Anota en `respuestas.md` los comandos y las comprobaciones solicitadas.

## 1. Gestión de procesos (3 puntos)

Se proporciona `scripts/monitor.sh`, que crea un proceso llamado `ud2monitor`.

Completa los siguientes scripts:

- `check_monitor.sh`: comprueba si el proceso existe. Si no existe, ejecútalo en segundo plano e informa de su PID. Si ya existe, informa sin crear otro proceso.
- `get_state_monitor.sh`: indica si el proceso está en ejecución o detenido.
- `stop_monitor.sh`: detén el proceso si existe e informa del resultado. Si no existe, muestra un mensaje.

Ejecuta los scripts desde la carpeta del proyecto mediante `bash`. Comprueba con comandos de Linux el PID del proceso, que no se duplica al iniciarlo de nuevo y que desaparece al detenerlo. Anota los comandos y sus resultados en `respuestas.md`.

**Pista:** al iniciar el proceso en segundo plano, redirige su salida con `> /dev/null 2>&1 &` para que Flask no se quede esperando.

## 2. Gestión de procesos desde Flask (3 puntos)

Completa el endpoint `/proceso` de `app.py`:

1. Asocia cada acción del formulario con su script correspondiente.
2. Ejecuta el script seleccionado mediante `subprocess.run()` utilizando Bash.
3. Envía la salida del script a la plantilla HTML.

Ejecuta `python app.py` y comprueba desde la interfaz las tres acciones: iniciar, estado y detener. Verifica también que iniciar dos veces no crea otro proceso.

## 3. Tareas programadas con Cron (2 puntos)

Completa `scripts/registro.sh` para añadir la fecha y hora actuales a `ejecuciones.log` sin borrar su contenido anterior. La variable `FICHERO` contiene la ruta del fichero.

Completa el endpoint `/tareacron` de `app.py` utilizando `python-crontab`:

- Programa el script con la frecuencia seleccionada: cada minuto, cada 5 minutos o cada 10 minutos.
- Guarda la tarea programada y muestra el mensaje de confirmación.

Selecciona cada minuto y comprueba que la tarea aparece en crontab y que se escribe en `ejecuciones.log`. Anota en `respuestas.md` los comandos utilizados y dos líneas generadas. Elimina las tareas de prueba con el comentario `TAREA-MONITOR` sin borrar otras tareas.

## 4. Servicios con systemd (2 puntos)

Detén la aplicación Flask iniciada manualmente. Completa `mantenimiento.service` para ejecutar `app.py` utilizando el Python del entorno virtual del proyecto.

Instala la unidad en la ubicación correspondiente y realiza estas operaciones:

1. Registra la unidad en systemd e inicia el servicio.
2. Habilita su arranque automático y comprueba su estado.
3. Comprueba que la aplicación web funciona.
4. Consulta los últimos diez mensajes del servicio y sigue los registros en tiempo real (sal con Ctrl+C).
5. Reinicia el servicio.
6. Detén el proceso Monitor desde la web si está activo. Detén el servicio y deshabilita su arranque automático.

Anota los comandos utilizados en `respuestas.md` y explica la diferencia entre iniciar y habilitar un servicio, y cuándo es necesario recargar las unidades de systemd.

## Entrega

Entrega la carpeta del proyecto con los scripts, `app.py`, `mantenimiento.service`, `ejecuciones.log` y `respuestas.md`. No incluyas `.venv`.
