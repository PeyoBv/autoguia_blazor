namespace AutoGuia.Infrastructure.Caching
{
    /// <summary>
    /// Claves de caché centralizadas para evitar duplicación y errores
    /// </summary>
    public static class CacheKeys
    {
        // Talleres
        public const string TodosLosTalleres = "talleres_all";
        public const string TalleresPorCiudad = "talleres_ciudad_{0}";
        public const string TallerPorId = "taller_id_{0}";

        // Productos
        public const string TodosLosProductos = "productos_all";
        public const string ProductoPorId = "producto_id_{0}";
        public const string ProductosPorCategoria = "productos_categoria_{0}";
        public const string MarcasProductos = "productos_marcas";

        // Foro
        public const string PublicacionesPaginadas = "foro_publicaciones_page_{0}_size_{1}";
        public const string PublicacionPorId = "foro_publicacion_{0}";
        public const string RespuestasPorPublicacion = "foro_respuestas_{0}";

        // Categorías
        public const string TodasLasCategorias = "categorias_all";
        public const string CategoriaPorId = "categoria_id_{0}";
        public const string SubcategoriasPorCategoria = "subcategorias_categoria_{0}";

        // Vehículos
        public const string TodasLasMarcas = "vehiculos_marcas";
        public const string ModelosPorMarca = "vehiculos_modelos_marca_{0}";

        // APIs Externas
        public const string MercadoLibreBusqueda = "ml_search_{0}_{1}_{2}";
        public const string MercadoLibreProducto = "ml_product_{0}";
        public const string MercadoLibreCategorias = "ml_categorias_auto";
        public const string EbayBusqueda = "ebay_search_{0}_{1}_{2}";
        public const string EbayProducto = "ebay_product_{0}";
        public const string EbayCategorias = "ebay_categorias_auto";
        public const string EbayAccessToken = "ebay_access_token";

        /// <summary>
        /// Genera una clave de caché con formato
        /// </summary>
        public static string Format(string template, params object[] args)
        {
            return string.Format(template, args);
        }
    }
}
