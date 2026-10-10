document.addEventListener('DOMContentLoaded', () => {
  const cards = document.querySelectorAll('.erci-card');

  cards.forEach(card => {
    card.addEventListener('mousemove', event => {
      if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

      const rect = card.getBoundingClientRect();
      const x = event.clientX - rect.left;
      const y = event.clientY - rect.top;

      card.style.setProperty('--rx', `${((y / rect.height) - .5) * -4}deg`);
      card.style.setProperty('--ry', `${((x / rect.width) - .5) * 4}deg`);
      card.style.setProperty('--tx', `${((x / rect.width) - .5) * 3}px`);
      card.style.setProperty('--ty', `${((y / rect.height) - .5) * 3}px`);
      card.style.setProperty('--scale', '1.01');
    });

    card.addEventListener('mouseleave', () => {
      card.style.setProperty('--rx', '0deg');
      card.style.setProperty('--ry', '0deg');
      card.style.setProperty('--tx', '0px');
      card.style.setProperty('--ty', '0px');
      card.style.setProperty('--scale', '1');
    });
  });
});
