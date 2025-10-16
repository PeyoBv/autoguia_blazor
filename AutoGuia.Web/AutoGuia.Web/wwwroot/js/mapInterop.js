// AutoGuía - Google Maps Integration
// Funciones para la integración de Google Maps con Blazor

window.autoguiaMap = {
    map: null,
    markers: [],
    infoWindow: null,

    // Inicializa el mapa de Google Maps
    initMap: function (mapElement, talleresData, apiKey) {
        return new Promise((resolve, reject) => {
            try {
                // Verificar si Google Maps ya está cargado
                if (typeof google === 'undefined') {
                    // Cargar la API de Google Maps dinámicamente
                    const script = document.createElement('script');
                    script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&callback=autoguiaMap.initializeMapCallback`;
                    script.async = true;
                    script.defer = true;
                    
                    // Guardar los datos para usar en el callback
                    window.autoguiaMapData = {
                        mapElement: mapElement,
                        talleresData: talleresData,
                        resolve: resolve,
                        reject: reject
                    };
                    
                    script.onerror = () => reject('Error al cargar Google Maps API');
                    document.head.appendChild(script);
                } else {
                    // Si Google Maps ya está cargado, inicializar directamente
                    this.createMap(mapElement, talleresData);
                    resolve('Mapa inicializado correctamente');
                }
            } catch (error) {
                reject(`Error en initMap: ${error.message}`);
            }
        });
    },

    // Callback para cuando se carga la API de Google Maps
    initializeMapCallback: function () {
        try {
            const data = window.autoguiaMapData;
            autoguiaMap.createMap(data.mapElement, data.talleresData);
            data.resolve('Mapa inicializado correctamente');
            
            // Limpiar datos temporales
            delete window.autoguiaMapData;
        } catch (error) {
            window.autoguiaMapData.reject(`Error en callback: ${error.message}`);
        }
    },

    // Crea el mapa y configura los marcadores
    createMap: function (mapElement, talleresData) {
        // Configuración por defecto del mapa (centrado en Santiago, Chile)
        const defaultCenter = { lat: -33.4489, lng: -70.6693 };
        
        // Configuración del mapa
        const mapOptions = {
            center: defaultCenter,
            zoom: 12,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            styles: [
                {
                    featureType: "poi",
                    elementType: "labels",
                    stylers: [{ visibility: "off" }]
                }
            ]
        };

        // Crear el mapa
        this.map = new google.maps.Map(mapElement, mapOptions);
        
        // Crear InfoWindow para mostrar información de los talleres
        this.infoWindow = new google.maps.InfoWindow();

        // Agregar marcadores para cada taller
        if (talleresData && talleresData.length > 0) {
            this.addMarkers(talleresData);
            this.fitMapToMarkers();
        }
    },

    // Agrega marcadores al mapa
    addMarkers: function (talleresData) {
        // Limpiar marcadores existentes
        this.clearMarkers();

        talleresData.forEach(taller => {
            const marker = new google.maps.Marker({
                position: { lat: taller.latitud, lng: taller.longitud },
                map: this.map,
                title: taller.titulo,
                icon: this.getMarkerIcon(taller.esVerificado),
                animation: google.maps.Animation.DROP
            });

            // Crear contenido del InfoWindow
            const infoContent = this.createInfoWindowContent(taller);

            // Agregar evento click al marcador
            marker.addListener('click', () => {
                this.infoWindow.setContent(infoContent);
                this.infoWindow.open(this.map, marker);
            });

            // Guardar referencia del marcador
            this.markers.push(marker);
        });
    },

    // Crea el contenido HTML para el InfoWindow
    createInfoWindowContent: function (taller) {
        const estrellas = this.generateStars(taller.calificacionPromedio);
        const verificadoBadge = taller.esVerificado 
            ? '<span class="badge bg-success">✓ Verificado</span>' 
            : '<span class="badge bg-secondary">Sin verificar</span>';

        return `
            <div class="info-window-content" style="max-width: 300px;">
                <h6 class="mb-2">
                    <strong>${taller.titulo}</strong>
                    ${verificadoBadge}
                </h6>
                <div class="mb-2">
                    <small class="text-muted">Calificación:</small><br>
                    ${estrellas} <small>(${taller.calificacionPromedio.toFixed(1)})</small>
                </div>
                <div class="mb-2">
                    <small class="text-muted">Dirección:</small><br>
                    <small>${taller.direccion}</small>
                </div>
                ${taller.telefono ? `
                <div class="mb-2">
                    <small class="text-muted">Teléfono:</small><br>
                    <small><a href="tel:${taller.telefono}">${taller.telefono}</a></small>
                </div>
                ` : ''}
                ${taller.email ? `
                <div class="mb-2">
                    <small class="text-muted">Email:</small><br>
                    <small><a href="mailto:${taller.email}">${taller.email}</a></small>
                </div>
                ` : ''}
                <div class="mt-2">
                    <button class="btn btn-primary btn-sm" onclick="autoguiaMap.verDetalles(${taller.id})">
                        Ver Detalles
                    </button>
                    <button class="btn btn-outline-secondary btn-sm ms-1" onclick="autoguiaMap.comoLlegar(${taller.latitud}, ${taller.longitud})">
                        Cómo Llegar
                    </button>
                </div>
            </div>
        `;
    },

    // Genera HTML para mostrar estrellas de calificación
    generateStars: function (rating) {
        let stars = '';
        const fullStars = Math.floor(rating);
        const hasHalfStar = rating % 1 >= 0.5;
        
        for (let i = 0; i < fullStars; i++) {
            stars += '<i class="fas fa-star text-warning"></i>';
        }
        
        if (hasHalfStar) {
            stars += '<i class="fas fa-star-half-alt text-warning"></i>';
        }
        
        const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);
        for (let i = 0; i < emptyStars; i++) {
            stars += '<i class="far fa-star text-warning"></i>';
        }
        
        return stars;
    },

    // Obtiene el icono apropiado para el marcador
    getMarkerIcon: function (esVerificado) {
        return esVerificado ? {
            url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                    <circle cx="16" cy="16" r="12" fill="#28a745" stroke="#fff" stroke-width="2"/>
                    <path d="M12 16l3 3 7-7" stroke="#fff" stroke-width="2" fill="none"/>
                </svg>
            `),
            scaledSize: new google.maps.Size(32, 32),
            anchor: new google.maps.Point(16, 32)
        } : {
            url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                    <circle cx="16" cy="16" r="12" fill="#6c757d" stroke="#fff" stroke-width="2"/>
                    <circle cx="16" cy="12" r="2" fill="#fff"/>
                    <rect x="14" y="16" width="4" height="8" fill="#fff"/>
                </svg>
            `),
            scaledSize: new google.maps.Size(32, 32),
            anchor: new google.maps.Point(16, 32)
        };
    },

    // Ajusta el mapa para mostrar todos los marcadores
    fitMapToMarkers: function () {
        if (this.markers.length === 0) return;

        const bounds = new google.maps.LatLngBounds();
        this.markers.forEach(marker => {
            bounds.extend(marker.getPosition());
        });

        this.map.fitBounds(bounds);
        
        // Asegurar un zoom mínimo
        const listener = google.maps.event.addListener(this.map, "idle", () => {
            if (this.map.getZoom() > 15) this.map.setZoom(15);
            google.maps.event.removeListener(listener);
        });
    },

    // Centra el mapa en coordenadas específicas
    centerMap: function (lat, lng, zoom = 12) {
        if (this.map) {
            this.map.setCenter({ lat: lat, lng: lng });
            this.map.setZoom(zoom);
        }
    },

    // Limpia todos los marcadores del mapa
    clearMarkers: function () {
        this.markers.forEach(marker => {
            marker.setMap(null);
        });
        this.markers = [];
    },

    // Agrega un solo marcador
    addSingleMarker: function (tallerData) {
        const marker = new google.maps.Marker({
            position: { lat: tallerData.latitud, lng: tallerData.longitud },
            map: this.map,
            title: tallerData.titulo,
            icon: this.getMarkerIcon(tallerData.esVerificado),
            animation: google.maps.Animation.BOUNCE
        });

        const infoContent = this.createInfoWindowContent(tallerData);
        
        marker.addListener('click', () => {
            this.infoWindow.setContent(infoContent);
            this.infoWindow.open(this.map, marker);
        });

        this.markers.push(marker);
        
        // Mostrar automáticamente el InfoWindow del nuevo marcador
        setTimeout(() => {
            marker.setAnimation(null);
            this.infoWindow.setContent(infoContent);
            this.infoWindow.open(this.map, marker);
        }, 1000);
    },

    // Función para ver detalles del taller (callback a Blazor)
    verDetalles: function (tallerId) {
        if (window.blazorInterop && window.blazorInterop.verDetallesTaller) {
            window.blazorInterop.verDetallesTaller(tallerId);
        } else {
            console.log(`Ver detalles del taller ID: ${tallerId}`);
        }
    },

    // Función para obtener direcciones
    comoLlegar: function (lat, lng) {
        const url = `https://www.google.com/maps/dir/?api=1&destination=${lat},${lng}`;
        window.open(url, '_blank');
    },

    // Destruye el mapa y libera recursos
    destroy: function () {
        if (this.map) {
            this.clearMarkers();
            this.map = null;
            this.infoWindow = null;
        }
    }
};

// Hacer disponible el callback globalmente
window.autoguiaMap.initializeMapCallback = window.autoguiaMap.initializeMapCallback;