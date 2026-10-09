# UD1 · Introducción al entorno Linux

Esta unidad introduce el trabajo básico con GNU/Linux desde terminal y prepara las herramientas que utilizaremos durante el módulo.

## Contenidos

- Estructura básica del sistema y rutas.
- Navegación por directorios.
- Creación, copia, movimiento y eliminación de archivos y carpetas.
- Permisos y uso básico de `sudo`.
- Instalación y actualización de paquetes.
- Introducción a scripts de shell.

## Primeros comandos

```bash
pwd
ls
cd
mkdir
cp
mv
rm
```

!!! tip "Objetivo"
    Al finalizar esta parte debes poder moverte con soltura por el sistema de archivos y realizar las operaciones habituales sin depender del entorno gráfico.

## Recursos de la unidad

### Presentación · Introducción a Linux

<div class="resource-embed">
  <iframe src="https://view.genial.ly/61ed48646b85b90013222284/presentation-presentacion-basica" width="100%" height="600px" frameborder="0" allowfullscreen="true" loading="lazy"></iframe>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">ENTREGABLE UD1</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Gestor de archivos con Bash · 45 minutos</h3>
    <p>Actividad individual: crea una estructura y procesa instrucciones COPY y MOVE desde un fichero.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>RA4 · CE 4.e</span></div><div class="si-actions"><a class="si-button" href="ejercicios/entregable_ud1/">Ver entregable <span>→</span></a></div></div>
  </article>
</div>

## Ejercicios

Los ejercicios están ordenados de menor a mayor complejidad. Se trabaja principalmente el **RA4, CE 4.e**, y se introducen actividades del **RA3 (CE 3.b, 3.c y parcialmente 3.f)**. La relación curricular no implica que los criterios estén automáticamente superados.

<div class="si-units">
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.2</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Creación de directorios con Bash</h3>
    <p>Crea una estructura mediante scripts y comprueba el resultado.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>mkdir · tree</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_2/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.3</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Rutas absolutas y relativas</h3>
    <p>Practica el acceso a archivos y carpetas utilizando ambos tipos de rutas.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>Rutas · tree</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_3/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.4</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Scripts y gestión de estructuras</h3>
    <p>Menús, funciones, copias, movimientos y borrado.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>Bash · funciones</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_4/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.5</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Edición de ficheros con sed</h3>
    <p>Consulta, modifica y elimina líneas de un fichero.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>sed · ficheros</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_5/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.6</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Cuestionario con Bash y sed</h3>
    <p>Lee preguntas y respuestas desde ficheros y crea un cuestionario.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>Bash · sed</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_6/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.7</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Generación de estructuras desde un fichero de instrucciones</h3>
    <p>Interpreta operaciones y rutas separadas por punto y coma mediante arrays.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>IFS · arrays</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_7/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.8</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Gestión automatizada de archivos y directorios</h3>
    <p>Interpreta tres campos para copiar, mover y buscar con find.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>cp · mv · find</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_8/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD1.9</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">$</div>
    <h3>Validador automático de entregas</h3>
    <p>Comprueba requisitos de archivos y directorios y genera un informe.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>grep · wc · find</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud1_9/">Ver ejercicio <span>→</span></a></div></div>
  </article>
</div>
