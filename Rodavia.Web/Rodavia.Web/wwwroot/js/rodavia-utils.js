// Rodavia JavaScript Utils - Manejo robusto de errores DOM
console.log('Rodavia JavaScript cargado correctamente - V3.0');

// Interceptar errores de DOM que pueden causar problemas en Blazor
(function() {
    // Interceptar removeChild con protección robusta
    const originalRemoveChild = Node.prototype.removeChild;
    Node.prototype.removeChild = function(child) {
        try {
            if (!child || !this || !child.parentNode) {
                return child;
            }
            
            // Verificar que el elemento realmente sea hijo de este padre
            if (child.parentNode !== this) {
                if (child.parentNode && child.parentNode.removeChild) {
                    return child.parentNode.removeChild(child);
                }
                return child;
            }
            
            // Verificar que el padre contiene al hijo
            if (this.contains && !this.contains(child)) {
                return child;
            }
            
            // Verificar que ambos elementos están en el DOM
            if (!document.contains(this) || !document.contains(child)) {
                return child;
            }
            
            return originalRemoveChild.call(this, child);
        } catch (error) {
            console.warn('removeChild interceptado:', error.message);
            return child;
        }
    };
    
    // Interceptar appendChild
    const originalAppendChild = Node.prototype.appendChild;
    Node.prototype.appendChild = function(child) {
        try {
            if (!child || !this) {
                return child;
            }
            
            // Si el child ya tiene padre, removerlo primero
            if (child.parentNode && child.parentNode !== this) {
                try {
                    child.parentNode.removeChild(child);
                } catch(e) {
                    // Silenciar errores
                }
            }
            
            return originalAppendChild.call(this, child);
        } catch (error) {
            console.warn('appendChild error:', error.message);
            return child;
        }
    };
    
    // Interceptar insertBefore
    const originalInsertBefore = Node.prototype.insertBefore;
    Node.prototype.insertBefore = function(newNode, referenceNode) {
        try {
            if (!newNode || !this) {
                return newNode;
            }
            
            // Si el newNode ya tiene padre, removerlo primero
            if (newNode.parentNode && newNode.parentNode !== this) {
                try {
                    newNode.parentNode.removeChild(newNode);
                } catch(e) {
                    // Silenciar errores
                }
            }
            
            return originalInsertBefore.call(this, newNode, referenceNode);
        } catch (error) {
            console.warn('insertBefore error:', error.message);
            return newNode;
        }
    };
})();

// Manejo global de errores JavaScript
window.addEventListener('error', function(event) {
    if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
        console.warn('Error JS:', event.error?.message || event.message);
    }
    return true;
});

window.addEventListener('unhandledrejection', function(event) {
    if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
        console.warn('Promise rechazada:', event.reason);
    }
    event.preventDefault();
});

// Utilidades de Rodavia
window.rodaviaUtils = {
    // Función para compartir contenido
    shareContent: function(title, text, url) {
        try {
            if (navigator.share) {
                navigator.share({ title, text, url });
            } else {
                navigator.clipboard.writeText(url).then(() => {
                    alert('Enlace copiado al portapapeles');
                }).catch(() => {
                    prompt('Copia este enlace:', url);
                });
            }
        } catch (error) {
            console.warn('Error sharing:', error);
            prompt('Copia este enlace:', url);
        }
    },
    
    // Función para limpiar elementos del DOM de forma segura
    safeDomCleanup: function() {
        const orphanElements = document.querySelectorAll('[data-cleanup]');
        orphanElements.forEach(el => {
            try {
                if (el.parentNode) {
                    el.parentNode.removeChild(el);
                }
            } catch (e) {
                console.warn('Elemento no pudo ser removido:', e);
            }
        });
    }
};
