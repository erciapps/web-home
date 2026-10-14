# 5. Laberinto y materiales

Obstáculos, recorrido y aspecto del juego.

## Diseñar el circuito

Añade paredes interiores para obligar a rodear obstáculos. Una primera distribución posible:

| Objeto | Position | Scale |
|---|---|---|
| Obstaculo1 | (-3, 0.5, -5) | (14, 1, 0.5) |
| Obstaculo2 | (3, 0.5, 1) | (14, 1, 0.5) |
| Obstaculo3 | (-3, 0.5, 7) | (14, 1, 0.5) |

Durante la práctica ajustamos el recorrido y añadimos otras paredes. Cada cubo debe conservar su `Box Collider`. Puedes agruparlos bajo `Obstaculos`; no es necesario fusionarlos durante el diseño. Comprueba desde arriba que existe un camino transitable desde la salida hasta el agujero.

<figure class="unity-shot" markdown="1">

[![Cámara inclinada](../../assets/images/pmdm/androicilla/20261014-065603.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-065603.png){ aria-label="Ampliar imagen: Cámara inclinada" }

<figcaption>Cámara inclinada.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Jerarquía simplificada](../../assets/images/pmdm/androicilla/20261014-070147.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-070147.png){ aria-label="Ampliar imagen: Jerarquía simplificada" }

<figcaption>Jerarquía simplificada.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Agrupación Bordes](../../assets/images/pmdm/androicilla/20261014-070157.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-070157.png){ aria-label="Ampliar imagen: Agrupación Bordes" }

<figcaption>Agrupación Bordes.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Circuito de obstáculos](../../assets/images/pmdm/androicilla/20261014-072224.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-072224.png){ aria-label="Ampliar imagen: Circuito de obstáculos" }

<figcaption>Circuito de obstáculos.</figcaption>
</figure>

## Materiales

En `Assets/Materials` crea los materiales `M_Suelo`, `M_Paredes` y `M_Bola` con el shader **Universal Render Pipeline/Lit**.

Aplica marrón al tablero, amarillo/madera a las paredes y gris metálico a la esfera. Para el metal puedes aumentar `Metallic` y `Smoothness` y comprobar el resultado según la iluminación. Estos son **materiales visuales**; no sustituyen a los colliders ni a los Physics Materials.

<figure class="unity-shot" markdown="1">

[![Crear material](../../assets/images/pmdm/androicilla/20261014-072316.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-072316.png){ aria-label="Ampliar imagen: Crear material" }

<figcaption>Crear material.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Color base de suelo](../../assets/images/pmdm/androicilla/20261014-072536.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-072536.png){ aria-label="Ampliar imagen: Color base de suelo" }

<figcaption>Color base de suelo.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Materiales aplicados al circuito](../../assets/images/pmdm/androicilla/20261014-072832.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-072832.png){ aria-label="Ampliar imagen: Materiales aplicados al circuito" }

<figcaption>Materiales aplicados al circuito.</figcaption>
</figure>



---

[← 4. Agujero con ProBuilder](04-agujero-probuilder.md) · [Volver al índice](index.md) · [6. Meta y pantalla de victoria →](06-meta-victoria.md)
