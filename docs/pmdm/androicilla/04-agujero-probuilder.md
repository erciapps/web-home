# 4. Agujero con ProBuilder

Sustracción CSG y colisión real del tablero.

## Crear el agujero de la meta con ProBuilder

Necesitamos un agujero **real** en el tablero, para que la bola lo atraviese. Un cilindro oscuro colocado encima del suelo solo lo simularía visualmente.

Instala **ProBuilder** desde **Package Manager → Unity Registry**. En la versión utilizada, las operaciones booleanas estaban disponibles tras activar **Edit → Preferences → ProBuilder → Experimental Features Enabled**. Unity recompila scripts al cambiarlo; si Rider está interfiriendo con el proceso, ciérralo antes.

Utiliza un **tablero con grosor**, por ejemplo un cubo plano. Crea también un cilindro que lo atraviese de arriba abajo y verifica que su diámetro permita pasar a la bola. Convierte ambos objetos a ProBuilder con **Tools → ProBuilder → Object → Pro Builderize**.

Abre **Tools → ProBuilder → Experimental → Boolean (CSG) Tool**, establece **Subtraction**, con el suelo como primer objeto y el cilindro como segundo, y pulsa **Apply**. Conserva el resultado como `Tablero`; desactiva el suelo y el cilindro originales.

<figure class="unity-shot" markdown="1">

[![Instalar ProBuilder](../../assets/images/pmdm/androicilla/20261014-062108.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-062108.png){ aria-label="Ampliar imagen: Instalar ProBuilder" }

<figcaption>Instalar ProBuilder.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Activar funciones experimentales](../../assets/images/pmdm/androicilla/20261014-062207.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-062207.png){ aria-label="Ampliar imagen: Activar funciones experimentales" }

<figcaption>Activar funciones experimentales.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Menú ProBuilder](../../assets/images/pmdm/androicilla/20261014-062338.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-062338.png){ aria-label="Ampliar imagen: Menú ProBuilder" }

<figcaption>Menú ProBuilder.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Panel Pro Builderize](../../assets/images/pmdm/androicilla/20261014-062812.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-062812.png){ aria-label="Ampliar imagen: Panel Pro Builderize" }

<figcaption>Panel Pro Builderize.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Preparar la geometría](../../assets/images/pmdm/androicilla/20261014-062901.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-062901.png){ aria-label="Ampliar imagen: Preparar la geometría" }

<figcaption>Preparar la geometría.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Abrir CSG](../../assets/images/pmdm/androicilla/20261014-063211.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-063211.png){ aria-label="Ampliar imagen: Abrir CSG" }

<figcaption>Abrir CSG.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Sustracción de cilindro](../../assets/images/pmdm/androicilla/20261014-063230.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-063230.png){ aria-label="Ampliar imagen: Sustracción de cilindro" }

<figcaption>Sustracción de cilindro.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Resultado de la operación](../../assets/images/pmdm/androicilla/20261014-063837.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-063837.png){ aria-label="Ampliar imagen: Resultado de la operación" }

<figcaption>Resultado de la operación.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Tablero con agujero](../../assets/images/pmdm/androicilla/20261014-063922.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-063922.png){ aria-label="Ampliar imagen: Tablero con agujero" }

<figcaption>Tablero con agujero.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Tablero resultante](../../assets/images/pmdm/androicilla/20261014-064007.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-064007.png){ aria-label="Ampliar imagen: Tablero resultante" }

<figcaption>Tablero resultante.</figcaption>
</figure>

### El problema que encontramos con el collider

Al principio el agujero se veía, pero la bola **no caía**. El motivo era que `Tablero` conservaba un `Box Collider` rectangular que tapaba el hueco físicamente.

Selecciona `Tablero`, elimina su `Box Collider` y añade **Mesh Collider** con la malla resultante. Mantén `Convex` **desactivado** e `Is Trigger` desactivado. No añadas Rigidbody al tablero.

El aviso de Unity sobre *pre-baked triangle collision* no es el problema que impide la caída, aunque habrá que revisar esa optimización antes de distribuir el juego.

<figure class="unity-shot" markdown="1">

[![Mesh Collider no convexo](../../assets/images/pmdm/androicilla/20261014-064235.png){ loading=lazy }](../../assets/images/pmdm/androicilla/20261014-064235.png){ aria-label="Ampliar imagen: Mesh Collider no convexo" }

<figcaption>Mesh Collider no convexo.</figcaption>
</figure>

<figure class="unity-shot" markdown="1">

[![Caída por el agujero](../../assets/images/pmdm/androicilla/caebola.gif){ loading=lazy }](../../assets/images/pmdm/androicilla/caebola.gif){ aria-label="Ampliar imagen: Caída por el agujero" }

<figcaption>Caída por el agujero.</figcaption>
</figure>



---

[← 3. Movimiento y Android](03-movimiento-android.md) · [Volver al índice](index.md) · [5. Laberinto y materiales →](05-laberinto-materiales.md)
