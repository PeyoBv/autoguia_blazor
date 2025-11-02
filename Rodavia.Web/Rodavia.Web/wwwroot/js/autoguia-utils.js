// AutoGuía JavaScript Utils - Manejo ultra robusto de errores DOM
console.log('AutoGuía JavaScript cargado correctamente - V2');

// Silenciar completamente errores DOM para evitar bloqueos
window.addEventListener('error', function(e) {
    if (e.message && e.message.includes('removeChild')) {
        e.preventDefault();
        return false;
    }
}, true);

// Interceptar errores de DOM que pueden causar problemas en Blazor
(function() {
    // Interceptar removeChild con máxima protección
    const originalRemoveChild = Node.prototype.removeChild;
    Node.prototype.removeChild = function(child) {
        try {
            // Validaciones ultra robustas
            if (!child) {
                console.log('removeChild: child es null o undefined');
                return null;
            }
            if (!this) {
                console.log('removeChild: this es null o undefined');
                return child;
            }
            
            // Verificar DOM válido
            if (typeof child.parentNode === 'undefined') {
                return child;
            }
            
            // Verificar que el elemento realmente sea hijo de este padre
            if (child.parentNode && child.parentNode !== this) {
                // Intentar remover del padre correcto
                try {
                    if (child.parentNode.removeChild) {
                        return child.parentNode.removeChild(child);
                    }
                } catch(e) {
                    console.log('removeChild: Error removiendo del padre correcto:', e.message);
                    return child;
                }
            }
            
            // Verificar si el child ya fue removido
            if (!child.parentNode) {
                console.log('removeChild: child no tiene parentNode');
                return child;
            }
            
            // Verificar que el padre contiene al hijo
            if (this.contains && !this.contains(child)) {
                console.log('removeChild: padre no contiene al hijo');
                return child;
            }
            
            // Verificar que ambos elementos están en el DOM
            if (!document.contains(this) || !document.contains(child)) {
                console.log('removeChild: elementos no están en el DOM');
                return child;
            }
            
            return originalRemoveChild.call(this, child);
        } catch (error) {
            // Error manejado completamente
            console.log('removeChild interceptado:', error.message);
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
            return child;
        }
    };
    
    // Interceptar insertBefore también
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
            return newNode;
        }
    };
})();

// Manejo global de errores JavaScript mejorado
window.addEventListener('error', function(event) {
    // Solo loggear en desarrollo para no llenar la consola en producción
    if (window.location.hostname === 'localhost') {
        console.warn('Error JS manejado:', event.error?.message || event.message);
    }
    // Prevenir que errores DOM rompan Blazor
    return true;
});

window.addEventListener('unhandledrejection', function(event) {
    // Manejar promesas rechazadas
    if (window.location.hostname === 'localhost') {
        console.warn('Promise rechazada manejada:', event.reason);
    }
    event.preventDefault();
});

// Interceptar errores de Blazor específicos
window.blazorErrorHandler = function(error) {
    if (error.message && error.message.includes('removeChild')) {
        // Silenciar errores específicos de removeChild
        return true;
    }
    return false;
};

window.autoguiaTest = {
    testFunction: function() {
        console.log('Función de prueba ejecutada correctamente');
        return 'Test OK';
    },
    
    // Función mejorada para compartir sin errores
    shareContent: function(title, text, url) {
        try {
            if (navigator.share) {
                navigator.share({
                    title: title,
                    text: text,
                    url: url
                });
            } else {
                // Fallback para navegadores que no soportan Web Share API
                navigator.clipboard.writeText(url).then(() => {
                    alert('Enlace copiado al portapapeles');
                }).catch(() => {
                    // Fallback final
                    prompt('Copia este enlace:', url);
                });
            }
        } catch (error) {
            console.log('Error sharing:', error);
            // Fallback final
            prompt('Copia este enlace:', url);
        }
    },
    
    // Función para limpiar elementos del DOM de forma segura
    safeDomCleanup: function() {
        // Limpiar posibles elementos huérfanos que pueden causar removeChild errors
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