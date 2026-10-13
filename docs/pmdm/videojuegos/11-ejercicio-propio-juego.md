# 15. Ejercicio · Tu propio videojuego

## Propuesta

Desarrolla **tu propio juego de plataformas 2D en Unity 6**, aplicando lo trabajado en clase. Utiliza assets y un tileset diferentes a los del ejemplo, elige una temática y diseña tus propios escenarios.

El juego debe tener **al menos tres niveles** y poder jugarse desde el menú principal hasta completar el último. Cada nivel debe tener un recorrido propio, enemigos u obstáculos y una condición clara para superarlo y avanzar al siguiente.

**Fecha de entrega: 27 de octubre de 2026.** La entrega se realizará en la tarea que se habilitará en **Educamos**.

## Requisitos mínimos

### Escenarios, personaje y cámara

- Crea el proyecto con la plantilla **Universal 2D**. Construye los escenarios con **Tilemap**, separando el suelo y la decoración.
- Configura las colisiones del suelo, las plataformas y los peligros. Incluye una zona de caída y obstáculos que puedan matar al personaje.
- El jugador debe poder moverse, saltar cuando está apoyado y cambiar de orientación. Utiliza **Rigidbody 2D**, **GroundCheck** y una layer para detectar el suelo.
- La cámara debe **desplazarse para seguir al personaje** y permitir recorrer escenarios que no quepan completos en una sola pantalla.
- Al superar un nivel se debe acceder al siguiente. Al terminar el tercero, o el último si añades más, el juego debe indicar que la partida se ha completado.

### Información en pantalla y objetos recogibles

La interfaz debe mostrar y actualizar estos datos durante la partida:

| Dato | Funcionamiento requerido |
| --- | --- |
| **Tiempo** | Un contador del tiempo transcurrido o del tiempo restante, según el diseño del juego. |
| **Monedas** | Cuántas monedas se han recogido. Debe haber monedas repartidas por los niveles. |
| **Disparos u objetos disponibles** | Cada disparo o lanzamiento consume una unidad. Si quedan cero, el jugador no puede seguir disparando o lanzando. |
| **Salud del personaje** | La cantidad de daño que puede recibir antes de morir. Debe disminuir al recibir daño. |
| **Vidas disponibles** | Los intentos que quedan para continuar la partida. Cada muerte consume una vida. |

Debe existir alguna forma de **recuperar munición u objetos para lanzar**: recogerlos del escenario, recuperar los que se han lanzado u otra mecánica que diseñes. El contador tiene que reflejar tanto el gasto como la recuperación.

Incluye también **objetos que recuperen salud**, como corazones, botiquines o alimentos. La salud recuperada no debe superar el máximo del personaje. Puedes añadir, además, objetos que concedan vidas extra.

**Salud y vidas son datos distintos:** perder toda la salud provoca la muerte; perder una vida reduce los intentos disponibles. Explica en el vídeo cómo funcionan y qué ocurre con los contadores al reiniciar o cambiar de nivel.

### Enemigos y combate

- Debe haber **enemigos animados que puedan dañar y matar al jugador**.
- El jugador debe poder atacarlos mediante disparos o lanzamiento de objetos.
- Los enemigos deben tener resistencia: **necesitan al menos dos impactos para morir**. Su daño recibido debe conservarse entre un golpe y el siguiente.
- Al morir, el enemigo debe reproducir su animación de muerte antes de desaparecer.

### Menú principal y muerte del jugador

El juego debe comenzar en un **menú principal**, desde el que se pueda iniciar una partida y consultar los controles.

Cuando el personaje muera, se deben detener sus controles, descontar una vida y mostrar las opciones de **reiniciar el nivel** o **volver al menú principal**.

**Solo se podrá reiniciar si, después de descontar la vida, quedan vidas disponibles: vidas > 0.** Si se agotan, muestra el fin de la partida y permite volver al menú; la opción de reiniciar debe quedar desactivada u oculta.

El reinicio debe restaurar el nivel y la salud del personaje, **conservando las vidas restantes**. No vale recargar la escena de forma que el contador de vidas vuelva a su valor inicial y permita reintentos ilimitados. Las opciones de la pantalla de muerte deben esperar a que el jugador elija; no reinicies automáticamente.

### Sonidos y animaciones

El personaje debe tener animaciones de **reposo, movimiento, salto y muerte**, y cambiar correctamente entre ellas. Los enemigos también deben estar animados durante su comportamiento y al morir.

Añade **efectos de sonido** para acciones del juego: recoger monedas, saltar, disparar o lanzar objetos y recibir daño o morir. Si añades música de fondo, ajusta el volumen para que los efectos se escuchen con claridad.

## Entrega

Sube a la tarea de **Educamos**, como máximo el **27 de octubre de 2026**:

1. **El proyecto completo de Unity en un archivo ZIP**, con el código del juego, las escenas, los recursos y la configuración necesaria para abrirlo y probarlo. Incluye las carpetas **Assets, Packages y ProjectSettings**; deja fuera Library, Temp, Logs y las carpetas temporales generadas por el editor.
2. **Un vídeo explicativo a modo de tutorial**, en el que enseñes el juego y expliques cómo has desarrollado sus partes principales. Debes mostrar los tres niveles, los contadores, la recuperación de munición y salud, los enemigos, la cámara, los sonidos y las animaciones. Enseña también qué ocurre al morir con vidas disponibles y cuando se agotan, y señala en el código cómo has resuelto esas funciones.
3. **Un README.md** dentro del proyecto con el nombre del juego, su objetivo, los controles, las instrucciones para abrirlo, los assets utilizados y su procedencia.

En el vídeo se debe ver la pantalla y escucharse tu explicación. Organízalo para que otra persona pueda seguir el funcionamiento del proyecto y entender las decisiones que has tomado. Comprueba antes de entregar que el ZIP se abre correctamente y que el vídeo tiene imagen y sonido.

