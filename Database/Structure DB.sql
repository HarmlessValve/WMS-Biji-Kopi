-- Tabel Roles 
CREATE TABLE roles (
    role_id     SERIAL PRIMARY KEY,
    role_name   VARCHAR(20) UNIQUE NOT NULL,
    description TEXT
);

-- Tabel Users (tidak lagi langsung relasi ke roles)
CREATE TABLE users (
    user_id    SERIAL PRIMARY KEY,
    username   VARCHAR(50) UNIQUE NOT NULL,
    password   VARCHAR(100) NOT NULL,
    is_active  BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel User Roles (satu-satunya jembatan antara users dan roles)
CREATE TABLE user_roles (
    id      SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    role_id INT NOT NULL REFERENCES roles(role_id) ON DELETE CASCADE,
    UNIQUE (user_id, role_id)
);

-- Tabel Suppliers
CREATE TABLE suppliers (
    supplier_id  SERIAL PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    address      TEXT,
    phone        VARCHAR(20),
    is_active    BOOLEAN DEFAULT TRUE
);

-- Tabel Kategori Kopi
CREATE TABLE coffee_categories (
    category_id   SERIAL PRIMARY KEY,
    category_name VARCHAR(50) UNIQUE NOT NULL,
    description   TEXT
);

-- Tabel Jenis Kopi
CREATE TABLE coffee_types (
    coffee_id     SERIAL PRIMARY KEY,
    coffee_name   VARCHAR(100) NOT NULL,
    is_active     BOOLEAN DEFAULT TRUE
);

-- Tabel Coffee Origins (asal daerah kopi)
CREATE TABLE coffee_origins (
    origin_id   SERIAL PRIMARY KEY,
    origin_name VARCHAR(100) NOT NULL,
    region      VARCHAR(100),
    description TEXT,
    is_active   BOOLEAN DEFAULT TRUE
);

-- Tabel Destinasi Ekspor/Impor
CREATE TABLE destinations (
    destination_id   SERIAL PRIMARY KEY,
    destination_name VARCHAR(100) NOT NULL,
    address          TEXT,
    is_active        BOOLEAN DEFAULT TRUE
);

-- Tabel Coffee Products (Gabungan Master Kopi)
CREATE TABLE coffee_products (
    product_id       SERIAL PRIMARY KEY,
    coffee_id        INT NOT NULL REFERENCES coffee_types(coffee_id) ON DELETE CASCADE,
    category_id      INT REFERENCES coffee_categories(category_id) ON DELETE SET NULL,
    origin_id        INT REFERENCES coffee_origins(origin_id) ON DELETE SET NULL,
    current_quantity INTEGER DEFAULT 0 CHECK (current_quantity >= 0),
    minimum_stock    INTEGER DEFAULT 20 CHECK (minimum_stock >= 0),
    last_updated     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active        BOOLEAN DEFAULT TRUE,
    UNIQUE (coffee_id, category_id, origin_id)
);

-- Tabel lanjutan untuk Roasted bean
CREATE TABLE roast_levels (
    roast_level_id SERIAL PRIMARY KEY,
    roast_level_name VARCHAR(100) NOT NULL,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE
);

-- Tabel Transaksi Masuk
CREATE TABLE incoming_transactions (
    incoming_id SERIAL PRIMARY KEY,
    supplier_id INT REFERENCES suppliers(supplier_id) ON DELETE SET NULL,
    product_id  INT REFERENCES coffee_products(product_id) ON DELETE SET NULL,
    quantity    INTEGER NOT NULL CHECK (quantity > 0),
    received_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    petugas_id  INT REFERENCES users(user_id) ON DELETE SET NULL
);

-- Tabel Transaksi Keluar
CREATE TABLE outgoing_transactions (
    outgoing_id    SERIAL PRIMARY KEY,
    destination_id INT REFERENCES destinations(destination_id) ON DELETE SET NULL,
    product_id     INT REFERENCES coffee_products(product_id) ON DELETE SET NULL,
    quantity       INTEGER NOT NULL CHECK (quantity > 0),
    shipped_at     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    petugas_id     INT REFERENCES users(user_id) ON DELETE SET NULL
);

-- Tabel Log Aktivitas
CREATE TABLE activity_logs (
    log_id      SERIAL PRIMARY KEY,
    user_id     INT REFERENCES users(user_id) ON DELETE SET NULL,
    action      VARCHAR(100),
    description TEXT,
    log_time    TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);




--==============================================================================
--BIMA NAMBAHIN roast level, diatas juga ada tabel baru namanya roast_level
--==============================================================================
INSERT INTO coffee_types (coffee_id, coffee_name, is_active)
VALUES 
(1, 'Arabika', true),
(2, 'Robusta', true),
(3, 'Liberika', true),
(4, 'Excelsa', true)
ON CONFLICT (coffee_id) 
DO UPDATE SET
    coffee_name = EXCLUDED.coffee_name,
    is_active = EXCLUDED.is_active;


INSERT INTO coffee_categories (category_id, category_name, description)
VALUES
(1, 'Cherry', 
 'Buah kopi yang baru dipetik dari pohon, biasanya berwarna merah matang.'),

(2, 'Gabah', 
 'Biji kopi yang sudah dikupas dari daging buahnya namun masih terbungkus lapisan cangkang keras.'),

(3, 'Green Bean', 
 'Biji kopi mentah yang sudah dikupas dari kulit tanduknya dan dikeringkan, namun belum melalui proses pemanggangan.'),

(4, 'Roasted Bean', 
 'Biji kopi hijau yang telah dipanggang (roasting) dan berubah warna menjadi cokelat dengan aroma khas.'),

(5, 'Ground Coffee', 
 'Roasted bean yang sudah digiling (dihaluskan) menjadi bubuk agar siap diseduh.')

ON CONFLICT (category_id)
DO UPDATE SET
    category_name = EXCLUDED.category_name,
    description = EXCLUDED.description;


INSERT INTO roast_levels (roast_level_id, roast_level_name, description, is_active)
VALUES
(1, 'Light Roast', 'Sangrai ringan', true),
(2, 'Medium Roast', 'Sangrai sedang', true),
(3, 'Medium Dark', 'Antara medium dan dark', true),
(4, 'Dark Roast', 'Sangrai gelap', true)
ON CONFLICT (roast_level_id)
DO UPDATE SET
    roast_level_name = EXCLUDED.roast_level_name,
    description = EXCLUDED.description,
    is_active = EXCLUDED.is_active;

SELECT setval(
    pg_get_serial_sequence('roast_levels', 'roast_level_id'),
    (SELECT MAX(roast_level_id) FROM roast_levels)
);


ALTER TABLE coffee_products
ADD COLUMN roast_level_id INTEGER;

ALTER TABLE coffee_products
ADD CONSTRAINT fk_coffee_products_roast_level
FOREIGN KEY (roast_level_id)
REFERENCES roast_levels(roast_level_id);