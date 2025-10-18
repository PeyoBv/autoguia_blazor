-- Crear tablas principales de AutoGuía
CREATE TABLE IF NOT EXISTS "Marcas" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "LogoUrl" VARCHAR(500),
    "Descripcion" VARCHAR(1000),
    "FechaCreacion" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "EsActivo" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS "Productos" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL,
    "NumeroDeParte" VARCHAR(100) NOT NULL,
    "Descripcion" VARCHAR(1000),
    "ImagenUrl" VARCHAR(500),
    "FechaCreacion" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "EsActivo" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS "Talleres" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL,
    "Descripcion" VARCHAR(1000),
    "Direccion" VARCHAR(300) NOT NULL,
    "Ciudad" VARCHAR(100) NOT NULL,
    "Region" VARCHAR(100) NOT NULL,
    "CodigoPostal" VARCHAR(20),
    "Telefono" VARCHAR(20),
    "Email" VARCHAR(200),
    "SitioWeb" VARCHAR(300),
    "Latitud" DOUBLE PRECISION,
    "Longitud" DOUBLE PRECISION,
    "HorarioAtencion" VARCHAR(100),
    "CalificacionPromedio" DECIMAL,
    "TotalResenas" INTEGER NOT NULL DEFAULT 0,
    "Especialidades" VARCHAR(500),
    "ServiciosOfrecidos" VARCHAR(2000),
    "EsVerificado" BOOLEAN NOT NULL DEFAULT FALSE,
    "EsActivo" BOOLEAN NOT NULL DEFAULT TRUE,
    "FechaRegistro" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "FechaVerificacion" TIMESTAMPTZ
);

-- Insertar datos de prueba
INSERT INTO "Marcas" ("Nombre", "LogoUrl") VALUES
('Toyota', '/images/marcas/toyota.png'),
('Honda', '/images/marcas/honda.png'),
('Nissan', '/images/marcas/nissan.png'),
('Chevrolet', '/images/marcas/chevrolet.png'),
('Ford', '/images/marcas/ford.png')
ON CONFLICT DO NOTHING;

INSERT INTO "Productos" ("Nombre", "NumeroDeParte", "Descripcion", "ImagenUrl") VALUES
('Pastillas de Freno Delanteras', 'BP-1234', 'Pastillas de freno cerámicas para mayor durabilidad y menor ruido', '/images/productos/pastillas-freno-bosch.jpg'),
('Filtro de Aceite', 'FO-9012', 'Filtro de aceite de alta calidad para motor', '/images/productos/filtro-aceite-mann.jpg'),
('Amortiguador Delantero', 'AD-7890', 'Amortiguador de gas presurizado para mejor confort y control', '/images/productos/amortiguador-monroe.jpg'),
('Batería 12V 65Ah', 'BT-9753', 'Batería de arranque libre de mantenimiento', '/images/productos/bateria-bosch.jpg'),
('Aceite Motor 5W-30 Sintético', 'AM-2468', 'Aceite sintético premium para motores de alta performance', '/images/productos/aceite-castrol.jpg')
ON CONFLICT DO NOTHING;

INSERT INTO "Talleres" ("Nombre", "Descripcion", "Direccion", "Ciudad", "Region", "Telefono", "EsVerificado") VALUES
('Taller Mecánico San Miguel', 'Taller especializado en reparaciones generales y mantenimiento preventivo', 'Av. Santa Rosa 1234', 'Santiago', 'Metropolitana', '+56 2 2555-1234', TRUE),
('AutoService Las Condes', 'Centro de servicio automotriz con tecnología de punta', 'Av. Apoquindo 5678', 'Las Condes', 'Metropolitana', '+56 2 2333-5678', TRUE),
('Taller Rodriguez', 'Taller familiar con 30 años de experiencia', 'Calle Valparaíso 910', 'Valparaíso', 'Valparaíso', '+56 32 2111-910', FALSE)
ON CONFLICT DO NOTHING;