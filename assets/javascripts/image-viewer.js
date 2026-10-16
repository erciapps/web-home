/* Delegation also handles Material's instant page navigation. */
(() => {
  let viewer;
  let opener;

  function getViewer() {
    if (viewer) return viewer;
    viewer = document.createElement('dialog');
    viewer.className = 'unity-image-viewer';
    viewer.setAttribute('aria-labelledby', 'unity-image-caption');
    viewer.innerHTML = `
      <button type="button" class="unity-image-close" aria-label="Cerrar imagen">Cerrar ×</button>
      <img alt="">
      <p id="unity-image-caption"></p>`;
    document.body.appendChild(viewer);
    viewer.querySelector('button').addEventListener('click', () => viewer.close());
    viewer.addEventListener('click', event => {
      const rect = viewer.getBoundingClientRect();
      if (event.target === viewer &&
          (event.clientX < rect.left || event.clientX > rect.right ||
           event.clientY < rect.top || event.clientY > rect.bottom)) {
        viewer.close();
      }
    });
    viewer.addEventListener('close', () => {
      document.documentElement.classList.remove('unity-image-open');
      if (opener?.isConnected) opener.focus({ preventScroll: true });
      viewer.querySelector('img').removeAttribute('src');
    });
    return viewer;
  }

  document.addEventListener('click', event => {
    const link = event.target.closest?.('.unity-shot a');
    if (!link || event.button !== 0 || event.ctrlKey || event.metaKey ||
        event.shiftKey || event.altKey || !('HTMLDialogElement' in window)) return;
    const original = link.querySelector('img');
    if (!original) return;
    event.preventDefault();
    const dialog = getViewer();
    opener = link;
    const image = dialog.querySelector('img');
    image.src = link.href;
    image.alt = original.alt;
    dialog.querySelector('p').textContent =
      link.closest('figure').querySelector('figcaption')?.textContent || original.alt;
    dialog.showModal();
    document.documentElement.classList.add('unity-image-open');
  });
})();
