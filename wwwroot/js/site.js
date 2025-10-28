window.autoguia = (function(){
  function initToggles(){
    // mejora básica para accesibilidad del dropdown (keyboard)
    document.querySelectorAll('.dropdown-toggle').forEach(btn => {
      btn.addEventListener('keydown', (e) => {
        if(e.key === 'Enter' || e.key === ' '){
          e.preventDefault();
          btn.click();
        }
      });
    });
  }

  function lazyLoadImages(){
    const imgs = document.querySelectorAll('img[loading="lazy"]');
    if('loading' in HTMLImageElement.prototype){
      // browser native lazy-loading: no-op
    } else {
      // simple fallback
      const io = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
          if(entry.isIntersecting){
            const img = entry.target;
            img.src = img.dataset.src || img.src;
            io.unobserve(img);
          }
        });
      });
      imgs.forEach(img => io.observe(img));
    }
  }

  function init(){
    initToggles();
    lazyLoadImages();
  }

  return { init };
})();
document.addEventListener('DOMContentLoaded', () => { if(window.autoguia) window.autoguia.init(); });