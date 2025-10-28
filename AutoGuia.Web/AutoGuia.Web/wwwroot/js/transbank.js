// Función para enviar formulario POST a Transbank con el token
window.submitTransbankForm = function(url, token) {
    console.log('🔵 submitTransbankForm llamado');
    console.log('URL:', url);
    console.log('Token:', token);
    
    // Crear formulario dinámicamente
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = url;
    
    // Agregar el token como campo oculto
    const tokenInput = document.createElement('input');
    tokenInput.type = 'hidden';
    tokenInput.name = 'TBK_TOKEN';
    tokenInput.value = token;
    form.appendChild(tokenInput);
    
    // Agregar formulario al DOM y enviarlo
    document.body.appendChild(form);
    
    console.log('✅ Formulario creado, enviando...');
    form.submit();
};
