window.Designer = (function(){
  let dotnetRef = null;
  function onMouseDown(e){
    const item = e.currentTarget;
    const id = item.dataset.id;
    let startX = e.clientX;
    let startY = e.clientY;
    // Use offset within positioned parent (the sheet)
    let left = item.offsetLeft;
    let top = item.offsetTop;
    function onMove(ev){
      const dx = ev.clientX - startX;
      const dy = ev.clientY - startY;
      item.style.left = (left + dx) + 'px';
      item.style.top = (top + dy) + 'px';
    }
    function onUp(ev){
      document.removeEventListener('mousemove', onMove);
      document.removeEventListener('mouseup', onUp);
      const x = parseFloat(item.style.left);
      const y = parseFloat(item.style.top);
      if (dotnetRef) dotnetRef.invokeMethodAsync('UpdatePosition', id, x, y);
    }
    document.addEventListener('mousemove', onMove);
    document.addEventListener('mouseup', onUp);
  }
  function init(ref){
    dotnetRef = ref;
    document.querySelectorAll('.design-item').forEach(el => {
      if (!el.dataset.init) {
        el.addEventListener('mousedown', onMouseDown);
        el.dataset.init = '1';
      }
    });
  }
  return { init };
})();
