# UD3 · Redes TCP/IP y conectividad

En esta unidad aprenderás a conectar equipos en red, configurar sus direcciones IP y comprobar que pueden comunicarse. Empezaremos creando redes locales con **GNS3** y, más adelante, utilizaremos routers OpenWrt para trabajar con puertas de enlace, DHCP, NAT y puertos.

## Objetivos de la unidad

- Identificar los elementos de una red local y comprender la función de un switch y de un router.
- Configurar direcciones IPv4, máscaras de subred y puertas de enlace.
- Comprender el funcionamiento básico de las direcciones MAC y del protocolo ARP.
- Comprobar la conectividad con comandos de diagnóstico.
- Configurar progresivamente servicios y acceso entre redes.

## Ejercicios

<div class="si-units">
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD3.1</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">LAN</div>
    <h3>Creación de una red local con GNS3</h3>
    <p>Conecta tres ordenadores a un switch, configura direcciones IP personalizadas, comprueba la comunicación y descubre ARP y la tabla MAC.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>GNS3 · VPCS · IPv4 · ping · ARP</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud3_1/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD3.2</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">WAN</div>
    <h3>OpenWrt, DHCP, Webterm y acceso a Internet</h3>
    <p>Amplía tu LAN con un router OpenWrt, configura DHCP desde LuCI mediante Webterm y compara tablas ARP.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>GNS3 · OpenWrt · DHCP · Webterm · ARP</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud3_2/">Ver ejercicio <span>→</span></a></div></div>
  </article>
</div>

!!! tip "Entorno de trabajo"
    Accede al servidor GNS3 con tu usuario. Cada alumno debe crear su propio proyecto y utilizar la red indicada por su identificador `damXX`.
