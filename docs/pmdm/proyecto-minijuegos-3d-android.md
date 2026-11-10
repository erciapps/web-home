# Proyecto común DDI + PMDM · Colección de minijuegos 3D para Android

**Curso:** 2026-2027  
**Módulos:** Desarrollo de Interfaces (DDI) y Programación Multimedia y Dispositivos Móviles (PMDM)  
**Modalidad:** Por parejas  
**Tecnología:** **Unity 6 y C#**, con aplicación final para **Android**

!!! info "Entrega y exposición"
    **Fecha de entrega: martes, 8 de diciembre de 2026.**  
    **Fecha indicada para la exposición en directo: martes, 8 de diciembre de 2026.**

    El **8 de diciembre es festivo** en Castilla-La Mancha. Por ello, la **fecha de la exposición presencial está pendiente de confirmación**. Se mantiene provisionalmente la fecha indicada hasta que el profesorado comunique el ajuste.

## ¿En qué consiste?

Vais a crear **un único videojuego 3D para Android que reúna cuatro minijuegos diferentes**. Cada pareja elegirá una temática común y diseñará cuatro desafíos originales relacionados con ella.

Por ejemplo, podéis ambientar la colección en un templo antiguo, una isla llena de animales, una estación espacial, una fábrica, una academia de magia o cualquier otra temática que os interese.

**No son cuatro aplicaciones distintas ni cuatro niveles que haya que superar en orden.** Todos los minijuegos forman parte de la **misma aplicación** y se pueden elegir libremente desde el **menú principal**.

Al terminar cualquiera de ellos, aparecerá su resultado y el jugador **volverá al menú principal**, desde donde podrá iniciar otro minijuego o repetir el anterior.

## Los cuatro minijuegos

Cada minijuego utilizará una de las formas de interacción que hemos trabajado en clase. El orden y la temática son libres.

| Minijuego | Tecnología | ¿Qué podéis hacer? |
| --- | --- | --- |
| **Interacción táctil** | Selección, arrastre y controles táctiles en Unity | Clasificar objetos 3D, colocar figuras, construir mecanismos o resolver puzles moviendo elementos con el dedo. |
| **Sensores del móvil** | Inclinación del dispositivo (por ejemplo, `GravitySensor`) | Dirigir una bola por un laberinto, equilibrar una plataforma, controlar un vehículo o esquivar obstáculos. |
| **Reconocimiento de voz** | Speech to Text | Dar órdenes como «avanza 3», «retrocede 2» o «gira derecha», responder preguntas o activar mecanismos mediante instrucciones habladas. |
| **Reconocimiento de gestos** | Cámara y reconocimiento de manos con HandForge | Abrir puertas, seleccionar opciones o activar elementos al mostrar mano abierta, puño cerrado o pulgar arriba/abajo. |

!!! tip "Elegid mecánicas distintas"
    No basta con cambiar los modelos o los colores: **cada minijuego debe proponer un reto y una forma de jugar diferentes**. Podéis utilizar los ejercicios realizados en clase como punto de partida, pero tendréis que adaptarlos y ampliarlos a vuestra propia temática.

## Funcionamiento de la aplicación

Al abrir el juego aparecerá un **menú principal** con acceso a los cuatro minijuegos, las instrucciones y los resultados.

En cada minijuego, el jugador debe saber qué tiene que hacer antes de empezar. Deberá existir:

- Un **escenario 3D** con un objetivo claro.
- Una mecánica que utilice realmente la tecnología asignada.
- Una forma de detectar si el reto se ha completado, y de informar de errores o intentos fallidos cuando corresponda.
- Un indicador de **tiempo, puntuación, aciertos o errores**, según el tipo de juego.
- Una **pantalla de resultado** al finalizar.
- Una opción para **regresar al menú principal** sin necesidad de terminar el reto.

Los minijuegos serán **independientes**: superar uno no desbloquea ni obliga a jugar otro. Desde el menú se podrán consultar los **mejores resultados**, que deberán conservarse al cerrar y volver a abrir la aplicación.

## Diseño y desarrollo

La aplicación se desarrollará **únicamente con Unity 6 y C#**. No se utilizará Avalonia ni será necesario crear una segunda aplicación.

Aprovecharemos las herramientas y tecnologías vistas en clase: **Unity 3D, Input System, ProBuilder, escenas, cámaras, colliders, Rigidbody, materiales, animaciones, TextMeshPro, controles táctiles, sensores, reconocimiento de voz y reconocimiento de gestos**, según las necesidades de cada minijuego.

Prestad especial atención a:

- **Interfaz y navegación:** menús, botones y mensajes claros, con una apariencia común y tamaños adecuados para el móvil.
- **Experiencia de usuario:** instrucciones comprensibles, información durante la partida y resultados fáciles de interpretar.
- **Escenarios y mecánicas:** objetos y entornos 3D, colisiones, físicas, cámaras y animaciones cuando correspondan.
- **Sonido y respuesta visual:** efectos que acompañen las acciones y permitan entender qué está ocurriendo.
- **Organización del código:** scripts bien estructurados y componentes reutilizables para los menús, resultados u otras funciones compartidas.
- **Pruebas:** comprobar la navegación, la detección de aciertos y errores y las cuatro tecnologías en un **dispositivo Android real**. Gestionad los permisos de cámara y micrófono.
- **Trabajo en pareja:** organización de las tareas y uso de **Git** para registrar los avances.

!!! note "Proyecto común de dos módulos"
    El proyecto sirve para trabajar y evaluar aprendizajes de **DDI y PMDM**, pero **solo se desarrolla un juego en Unity**. Cada módulo valorará los aspectos que le correspondan; no es un proyecto intermodular.

## Entrega

La entrega deberá incluir:

1. **Proyecto completo de Unity**, con `Assets`, `Packages`, `ProjectSettings` y los recursos necesarios para abrirlo.
2. **APK para Android** que permita probar la colección de minijuegos.
3. **README.md** con la temática, la explicación de los cuatro minijuegos, instrucciones y controles, herramientas utilizadas y procedencia de los recursos.

**Fecha de entrega: 8 de diciembre de 2026.**

## Exposición en directo

**No hay que entregar un vídeo.** Cada pareja realizará una **presentación en tiempo real** de su aplicación, ejecutando el juego y enseñando cómo funciona.

Durante la exposición deberéis:

1. Presentar la temática y enseñar el **menú principal**.
2. **Jugar en directo a los cuatro minijuegos**, mostrando sus controles y cómo se obtienen los resultados.
3. Demostrar el funcionamiento de la **interacción táctil, los sensores, la voz y los gestos** en Android.
4. Enseñar las partes principales del **código C#** y explicar cómo habéis desarrollado las mecánicas e interfaces.
5. Explicar brevemente cómo os habéis repartido el trabajo y qué problemas habéis resuelto.

**Ambos integrantes de la pareja deberán intervenir y conocer el funcionamiento del proyecto.**

**Fecha indicada para la exposición: 8 de diciembre de 2026**, pendiente de confirmación presencial al coincidir con festivo.

---

**Objetivo:** conseguir una colección de cuatro minijuegos 3D que sea coherente, original, fácil de utilizar y completamente funcional en Android.
