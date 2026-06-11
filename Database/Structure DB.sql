DROP VIEW IF EXISTS stock_summary;
DROP VIEW IF EXISTS vw_user_roles;
DROP TABLE IF EXISTS activity_logs;
DROP TABLE IF EXISTS outgoing_transactions;
DROP TABLE IF EXISTS incoming_transactions;
DROP TABLE IF EXISTS coffee_products;
DROP TABLE IF EXISTS coffee_origins;
DROP TABLE IF EXISTS user_roles;
DROP TABLE IF EXISTS coffee_types;
DROP TABLE IF EXISTS coffee_categories;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS destinations;
DROP TABLE IF EXISTS suppliers;

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
    last_updated     TIMESTAMP DEFAULT CURRENT_TIMESTAMP
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