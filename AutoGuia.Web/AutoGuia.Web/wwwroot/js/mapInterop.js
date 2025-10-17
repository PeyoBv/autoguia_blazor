// AutoGuía - Google Maps Integration (Versión Segura y Simplificada)
// Compatible con ciclo de vida de Blazor - Evita conflictos DOM

window.autoguiaMap = {
    map: null,
    markers: [],
    infoWindow: null,
    isInitializing: false,

    // Inicializa el mapa de Google Maps con manejo seguro de errores
    initMap: function (mapElementId, talleresData, apiKey) {
        return new Promise((resolve, reject) => {
            try {
                console.log(`Intentando inicializar mapa con ID: ${mapElementId}`);
                
                // Verificar si ya se está inicializando
                if (this.isInitializing) {
                    console.log('Mapa ya se está inicializando, resolviendo...');
                    resolve('Mapa ya inicializándose');
                    return;
                }
                
                this.isInitializing = true;

                // Buscar elemento del mapa de manera más flexible
                let mapElement = this.findMapElement(mapElementId);
                
                if (!mapElement) {
                    // Si no encontramos el elemento, esperamos un poco más para Blazor
                    setTimeout(() => {
                        mapElement = this.findMapElement(mapElementId);
                        if (!mapElement) {
                            console.warn('Elemento de mapa no encontrado, continuando sin mapa');
                            this.isInitializing = false;
                            resolve('Sin elemento de mapa disponible');
                            return;
                        }
                        this.loadGoogleMapsAPI(mapElement, talleresData, apiKey, resolve, reject);
                    }, 1000);
                } else {
                    this.loadGoogleMapsAPI(mapElement, talleresData, apiKey, resolve, reject);
                }
                
            } catch (error) {
                console.warn('Error controlado en initMap:', error.message);
                this.isInitializing = false;
                // Resolvemos en lugar de rechazar para no romper la aplicación
                resolve('Error manejado: ' + error.message);
            }
        });
    },

    // Busca el elemento del mapa de manera flexible
    findMapElement: function(mapElementId) {
        // Intentar múltiples selectores
        let element = document.getElementById(mapElementId);
        if (element) return element;
        
        element = document.querySelector(`[id="${mapElementId}"]`);
        if (element) return element;
        
        element = document.querySelector('.mapa-contenedor');
        if (element) return element;
        
        element = document.querySelector('#mapa-talleres');
        if (element) return element;
        
        // Si nada funciona, crear un elemento placeholder
        console.log('Creando elemento placeholder para mapa');
        const placeholder = document.createElement('div');
        placeholder.id = mapElementId;
        placeholder.style.height = '400px';
        placeholder.style.background = '#f0f0f0';
        placeholder.style.display = 'flex';
        placeholder.style.alignItems = 'center';
        placeholder.style.justifyContent = 'center';
        placeholder.innerHTML = '<p class="text-muted">Mapa no disponible en este momento</p>';
        
        // Buscar un contenedor donde insertarlo
        const container = document.querySelector('.container') || document.body;
        if (container) {
            container.appendChild(placeholder);
        }
        
        return placeholder;
    },

    // Carga la API de Google Maps
    loadGoogleMapsAPI: function(mapElement, talleresData, apiKey, resolve, reject) {
        try {
            // Verificar si Google Maps ya está disponible
            if (typeof google !== 'undefined' && google.maps) {
                console.log('Google Maps ya disponible, creando mapa...');
                this.createSimpleMap(mapElement, talleresData);
                this.isInitializing = false;
                resolve('Mapa creado exitosamente');
                return;
            }

            // Verificar si ya hay un script cargándose
            const existingScript = document.querySelector('script[src*="maps.googleapis.com"]');
            if (existingScript) {
                console.log('Script de Google Maps ya cargándose...');
                this.waitForGoogleMaps(mapElement, talleresData, resolve, reject);
                return;
            }

            // Cargar Google Maps API
            console.log('Cargando Google Maps API...');
            const script = document.createElement('script');
            script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&libraries=places`;
            script.async = true;
            script.defer = true;
            
            script.onload = () => {
                console.log('Google Maps API cargada');
                this.waitForGoogleMaps(mapElement, talleresData, resolve, reject);
            };
            
            script.onerror = () => {
                console.warn('Error cargando Google Maps API');
                this.isInitializing = false;
                resolve('Error al cargar Google Maps - continuando sin mapa');
            };
            
            // Agregar al head de manera segura
            const head = document.head || document.getElementsByTagName('head')[0];
            if (head) {
                head.appendChild(script);
            }
            
        } catch (error) {
            console.warn('Error en loadGoogleMapsAPI:', error.message);
            this.isInitializing = false;
            resolve('Error cargando API: ' + error.message);
        }
    },

    // Espera a que Google Maps esté disponible
    waitForGoogleMaps: function(mapElement, talleresData, resolve, reject) {
        let attempts = 0;
        const maxAttempts = 50; // 5 segundos máximo
        
        const checkGoogle = setInterval(() => {
            attempts++;
            
            if (typeof google !== 'undefined' && google.maps) {
                clearInterval(checkGoogle);
                console.log('Google Maps disponible, creando mapa...');
                this.createSimpleMap(mapElement, talleresData);
                this.isInitializing = false;
                resolve('Mapa inicializado correctamente');
            } else if (attempts >= maxAttempts) {
                clearInterval(checkGoogle);
                console.warn('Timeout esperando Google Maps');
                this.isInitializing = false;
                resolve('Timeout - continuando sin mapa');
            }
        }, 100);
    },

    // Crea un mapa simple y seguro
    createSimpleMap: function(mapElement, talleresData) {
        try {
            if (!mapElement || !google || !google.maps) {
                console.warn('No se puede crear el mapa - requisitos no cumplidos');
                return;
            }

            // Configuración básica del mapa
            const mapOptions = {
                center: { lat: -33.4489, lng: -70.6693 }, // Santiago, Chile
                zoom: 12,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };

            // Crear mapa
            this.map = new google.maps.Map(mapElement, mapOptions);
            console.log('Mapa base creado exitosamente');

            // Crear InfoWindow
            this.infoWindow = new google.maps.InfoWindow();

            // Agregar marcadores si hay datos
            if (talleresData && Array.isArray(talleresData) && talleresData.length > 0) {
                this.addSimpleMarkers(talleresData);
            } else {
                console.log('No hay datos de talleres para mostrar');
            }

        } catch (error) {
            console.warn('Error creando mapa simple:', error.message);
        }
    },

    // Agrega marcadores de manera segura
    addSimpleMarkers: function(talleresData) {
        try {
            if (!this.map || !Array.isArray(talleresData)) {
                console.warn('No se pueden agregar marcadores - mapa o datos no disponibles');
                return;
            }

            // Limpiar marcadores existentes
            this.clearMarkers();

            talleresData.forEach((taller, index) => {
                try {
                    if (taller.latitud && taller.longitud) {
                        const marker = new google.maps.Marker({
                            position: { lat: parseFloat(taller.latitud), lng: parseFloat(taller.longitud) },
                            map: this.map,
                            title: taller.titulo || `Taller ${index + 1}`
                        });

                        // Evento click simple
                        marker.addListener('click', () => {
                            const content = `
                                <div style="max-width: 200px;">
                                    <h6><strong>${taller.titulo || 'Taller'}</strong></h6>
                                    <p><small>${taller.direccion || 'Dirección no disponible'}</small></p>
                                </div>
                            `;
                            this.infoWindow.setContent(content);
                            this.infoWindow.open(this.map, marker);
                        });

                        this.markers.push(marker);
                    }
                } catch (markerError) {
                    console.warn(`Error creando marcador ${index}:`, markerError.message);
                }
            });

            console.log(`${this.markers.length} marcadores agregados exitosamente`);

        } catch (error) {
            console.warn('Error agregando marcadores:', error.message);
        }
    },

    // Limpia marcadores de manera segura
    clearMarkers: function() {
        try {
            if (Array.isArray(this.markers)) {
                this.markers.forEach(marker => {
                    if (marker && typeof marker.setMap === 'function') {
                        marker.setMap(null);
                    }
                });
            }
            this.markers = [];
        } catch (error) {
            console.warn('Error limpiando marcadores:', error.message);
            this.markers = [];
        }
    },

    // Limpia recursos
    destroy: function() {
        try {
            this.clearMarkers();
            if (this.infoWindow && typeof this.infoWindow.close === 'function') {
                this.infoWindow.close();
            }
            this.map = null;
            this.infoWindow = null;
            this.isInitializing = false;
            console.log('Recursos de mapa liberados');
        } catch (error) {
            console.warn('Error destruyendo mapa:', error.message);
        }
    }
};

// Manejo global de errores para el mapa
window.addEventListener('error', function(event) {
    if (event.error && event.error.message && event.error.message.includes('google')) {
        console.warn('Error de Google Maps capturado y controlado:', event.error.message);
        return true; // Prevenir que rompa la aplicación
    }
});

console.log('AutoGuía Maps - Script seguro cargado correctamente');