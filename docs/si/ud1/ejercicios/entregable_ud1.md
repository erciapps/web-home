# Entregable UD1 · Gestor de archivos con Bash

**Módulo:** Sistemas Informáticos  
**Ciclo:** 1.º DAM  
**Duración:** 45 minutos  
**Modalidad:** Individual  
**Entorno:** Debian GNU/Linux  
**RA principal:** RA4  
**Criterio de evaluación principal:** CE 4.e

---

## 1. Objetivo

Una empresa necesita automatizar la organización de sus documentos mediante un script Bash.

Tu tarea consiste en desarrollar un programa que permita:

- Crear una estructura de directorios y archivos.
- Utilizar rutas absolutas y relativas.
- Leer instrucciones almacenadas en un fichero de texto.
- Copiar y mover archivos según las instrucciones recibidas.
- Mostrar la estructura de directorios y archivos.
- Utilizar un menú interactivo con funciones y condiciones.

Para desarrollar esta actividad deberás aplicar los conocimientos adquiridos durante la UD1.

## 2. Preparación del proyecto

Crea un script denominado:

```text
entregable.sh
```

Este script será el encargado de ejecutar todas las funcionalidades solicitadas.

El programa debe disponer de un menú que permita seleccionar las distintas operaciones.

**Condiciones generales:**

1. Debes utilizar Bash.
2. El programa debe estar organizado mediante funciones.
3. Las condiciones deben implementarse utilizando `if [[ ... ]]`, `elif` y `else`.
4. Debes utilizar bucles para controlar el menú y recorrer el fichero de instrucciones.
5. No se permite crear manualmente los archivos o directorios que debe generar el script.
6. El fichero de instrucciones debe procesarse utilizando `sed`.
7. No debes modificar el formato del fichero de instrucciones proporcionado.
8. El script debe poder ejecutarse varias veces sin producir errores por intentar crear directorios que ya existen.

---

## 3. Apartado 1 — Creación de la estructura inicial (2 puntos)

Desarrolla una función llamada `crearEstructura`.

Esta función debe crear la siguiente estructura de directorios y archivos dentro de `/workspace/ud1/entregable`:

```text
/workspace/ud1/entregable/
├── documentos/
│   ├── informes/
│   └── contratos/
├── imagenes/
├── copias/
├── temporales/
│   └── prueba.txt
└── README.txt
```

### Requisitos

1. Utiliza al menos **dos rutas absolutas y dos rutas relativas** en las operaciones de creación.
2. Crea los directorios utilizando `mkdir`.
3. Crea los archivos utilizando `touch` cuando corresponda.
4. El archivo `README.txt` debe contener exactamente la siguiente línea:

   ```text
   Proyecto de Sistemas Informáticos
   ```

5. El archivo `prueba.txt` debe estar inicialmente vacío.
6. Muestra un mensaje cuando finalice la creación de la estructura.

**Importante:** los directorios y archivos deben generarse al seleccionar la opción correspondiente del menú, no manualmente.

---

## 4. Apartado 2 — Procesamiento de instrucciones (4 puntos)

Desarrolla una función llamada `procesarInstrucciones`.

La empresa dispone de un fichero llamado `operaciones.txt` con las operaciones que debe realizar el programa.

### Contenido de `operaciones.txt`

```text
COPY;README.txt;copias/README.txt
MOVE;temporales/prueba.txt;documentos/informes/prueba.txt
COPY;copias/README.txt;documentos/contratos/contrato.txt
MOVE;documentos/contratos/contrato.txt;documentos/informes/contrato.txt
```

El fichero debe estar ubicado en el mismo directorio que `entregable.sh`.

### Formato de las instrucciones

Cada línea contiene tres campos separados por punto y coma (`;`):

```text
OPERACION;ORIGEN;DESTINO
```

| Campo | Descripción |
|---|---|
| OPERACION | Acción que debe ejecutar el programa. |
| ORIGEN | Archivo sobre el que se realizará la operación. |
| DESTINO | Ruta donde se copiará o moverá el archivo. |

Las rutas de origen y destino del fichero son relativas al directorio `/workspace/ud1/entregable`.

### Operaciones admitidas

**COPY**

Copia el archivo indicado en el campo ORIGEN a la ubicación especificada en DESTINO.

El archivo original debe permanecer en su ubicación.

**MOVE**

Mueve el archivo indicado en ORIGEN a DESTINO.

Después de la operación, el archivo dejará de existir en su ubicación original.

### Requisitos de programación

La función debe:

1. Obtener el número de líneas del fichero de instrucciones.
2. Recorrer todas las líneas utilizando un bucle `for`.
3. Obtener cada línea mediante `sed`.
4. Separar los tres campos utilizando `IFS` y `read`.
5. Almacenar los campos en variables.
6. Utilizar condiciones `if [[ ... ]]` y `elif` para determinar la operación.
7. Ejecutar la operación correspondiente mediante `cp` o `mv`.
8. Mostrar un mensaje después de cada operación.

### Ejemplo de salida

```text
================================
    PROCESANDO INSTRUCCIONES
================================

COPY: README.txt -> copias/README.txt

MOVE: temporales/prueba.txt -> documentos/informes/prueba.txt

COPY: copias/README.txt -> documentos/contratos/contrato.txt

MOVE: documentos/contratos/contrato.txt -> documentos/informes/contrato.txt

Procesamiento finalizado.
```

**Importante:** las operaciones deben ejecutarse en el mismo orden en el que aparecen en el fichero, ya que algunas dependen de archivos creados por instrucciones anteriores.

El programa no debe contener las operaciones escritas directamente en el código: debe interpretarlas desde `operaciones.txt`.

---

## 5. Apartado 3 — Menú y visualización (3 puntos)

El script debe mostrar el siguiente menú:

```text
================================
   ENTREGABLE UD1 - SISTEMAS
================================

1. Crear estructura inicial
2. Ejecutar operaciones
3. Mostrar estructura actual
4. Salir

Selecciona una opción:
```

### Funcionamiento del menú

**Opción 1 — Crear estructura inicial**

Debe ejecutar la función `crearEstructura`.

**Opción 2 — Ejecutar operaciones**

Debe ejecutar la función `procesarInstrucciones`.

**Opción 3 — Mostrar estructura actual**

Debe mostrar de forma recursiva todos los directorios y archivos existentes en `/workspace/ud1/entregable`.

Puedes utilizar el comando `ls -R`.

**Opción 4 — Salir**

Debe finalizar la ejecución del programa.

### Requisitos adicionales

- El menú debe mostrarse repetidamente hasta que el usuario seleccione la opción de salir.
- Cada opción debe ejecutar la función correspondiente.
- Si el usuario introduce una opción no válida, debe aparecer un mensaje de error.
- Después de ejecutar una opción, el programa debe regresar al menú.
- El programa debe poder ejecutar las opciones 1, 2 y 3 en el orden adecuado sin errores.

---

## 6. Apartado 4 — Búsqueda de archivos (1 punto)

Añade una nueva opción al menú:

```text
5. Buscar archivos
```

Desarrolla una función llamada `buscarArchivos`.

Esta función debe:

1. Solicitar al usuario el nombre de un archivo.
2. Buscarlo dentro de `/workspace/ud1/entregable` y todos sus subdirectorios.
3. Mostrar las rutas de los archivos encontrados.
4. Utilizar el comando `find`.

### Ejemplo

```text
Introduce el nombre del archivo: README.txt

Resultados:

/workspace/ud1/entregable/README.txt
/workspace/ud1/entregable/copias/README.txt
```

La opción debe permitir buscar archivos por su nombre sin tener que conocer previamente el directorio donde se encuentran.

---

## 7. Resultado esperado

Después de ejecutar correctamente las opciones 1 y 2, la estructura deberá ser:

```text
/workspace/ud1/entregable/
├── documentos/
│   ├── informes/
│   │   ├── prueba.txt
│   │   └── contrato.txt
│   └── contratos/
├── imagenes/
├── copias/
│   └── README.txt
├── temporales/
└── README.txt
```

Los archivos `README.txt` y `contrato.txt` deberán conservar el texto:

```text
Proyecto de Sistemas Informáticos
```

El archivo `prueba.txt` deberá permanecer vacío.
