# Androicilla · Laberinto 3D para Android

Vamos a crear en **Unity 6.6** un laberinto con una bola que se mueve mediante teclado o inclinando un teléfono Android. Partiremos de una escena vacía y llegaremos hasta la detección de la meta, la pantalla de victoria y el reinicio.

**Herramientas:** Unity 6000.6.0f1 · Universal 3D (URP) · Rider · Input System · ProBuilder 6.1.3 · Android.

Sigue los capítulos en orden. Las capturas corresponden al proyecto que hemos desarrollado y **puedes pulsar sobre cualquiera para ampliarla**.

## Tutorial paso a paso

<div class="grid cards" markdown>

-   :material-cube-outline:{ .lg .middle } **1. Preparación y tablero**

    ---

    Crear el proyecto, instalar Android y levantar el recinto.

    [:octicons-arrow-right-24: Abrir capítulo](01-preparacion-tablero.md)

-   :material-camera-outline:{ .lg .middle } **2. Bola y cámara**

    ---

    Rigidbody, colliders y cámara cenital.

    [:octicons-arrow-right-24: Abrir capítulo](02-jugador-camara.md)

-   :material-cellphone:{ .lg .middle } **3. Movimiento y Android**

    ---

    WASD, GravitySensor y compilación APK.

    [:octicons-arrow-right-24: Abrir capítulo](03-movimiento-android.md)

-   :material-shape-outline:{ .lg .middle } **4. Agujero con ProBuilder**

    ---

    Sustracción CSG y colisión real del tablero.

    [:octicons-arrow-right-24: Abrir capítulo](04-agujero-probuilder.md)

-   :material-wall:{ .lg .middle } **5. Laberinto y materiales**

    ---

    Obstáculos, recorrido y aspecto del juego.

    [:octicons-arrow-right-24: Abrir capítulo](05-laberinto-materiales.md)

-   :material-flag-checkered:{ .lg .middle } **6. Meta y pantalla de victoria**

    ---

    Trigger, TextMeshPro y reinicio de nivel.

    [:octicons-arrow-right-24: Abrir capítulo](06-meta-victoria.md)

</div>

## Resultado del juego

<figure class="unity-shot" markdown="1">

[![Animación del nivel completado](../../assets/images/pmdm/androicilla/nivel_completado.gif){ loading=lazy }](../../assets/images/pmdm/androicilla/nivel_completado.gif){ aria-label="Ampliar animación de nivel completado" }

<figcaption>La bola entra en el agujero y aparece la pantalla de victoria.</figcaption>
</figure>
