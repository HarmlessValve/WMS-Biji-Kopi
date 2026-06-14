-- ============================================================================
-- KONFIGURASI SCHEMA BARU
-- ============================================================================
-- Ganti "coffee_warehouse" di bawah ini dengan nama schema yang Anda mau.
-- Schema akan dibuat (jika belum ada), lalu search_path session ini diarahkan
-- ke schema tersebut terlebih dahulu (fallback ke public).
--
-- Akibatnya: SEMUA objek di bawah ini (table, view, function, procedure,
-- trigger, index) akan otomatis dibuat di schema "coffee_warehouse" --
-- TANPA perlu mengubah satu pun nama objek di seluruh script ini.
-- ============================================================================

CREATE SCHEMA IF NOT EXISTS coffee_warehouse;

SET search_path TO coffee_warehouse, public;


-- ============================================================================
-- COFFEE WAREHOUSE MANAGEMENT SYSTEM
-- Schema lengkap: DDL + Triggers + Views + Functions + Stored Procedures + Seed Data
-- Versi: Perbaikan gabungan (struktur bersih, sequence aman, konsisten)
-- ============================================================================


-- ============================================================================
-- SECTION 0: DROP SEMUA OBJEK (urutan terbalik agar tidak ada dependency error)
-- Jalankan bagian ini bila ingin reset total. Hapus bagian ini bila tidak perlu.
-- ============================================================================

DROP TRIGGER  IF EXISTS trg_log_outgoing_transaction    ON outgoing_transactions;
DROP TRIGGER  IF EXISTS trg_log_incoming_transaction    ON incoming_transactions;
DROP TRIGGER  IF EXISTS trg_outgoing_update_stock       ON outgoing_transactions;
DROP TRIGGER  IF EXISTS trg_incoming_update_stock       ON incoming_transactions;

DROP FUNCTION IF EXISTS fn_log_outgoing_transaction()    CASCADE;
DROP FUNCTION IF EXISTS fn_log_incoming_transaction()    CASCADE;
DROP FUNCTION IF EXISTS fn_update_stock_on_outgoing()    CASCADE;
DROP FUNCTION IF EXISTS fn_update_stock_on_incoming()    CASCADE;
DROP FUNCTION IF EXISTS fn_get_stock_by_product(INT)     CASCADE;
DROP FUNCTION IF EXISTS fn_is_stock_sufficient(INT, INT) CASCADE;
DROP FUNCTION IF EXISTS fn_get_low_stock_items()         CASCADE;
DROP FUNCTION IF EXISTS fn_get_cascading_jenis_kopi()    CASCADE;
DROP FUNCTION IF EXISTS fn_get_cascading_kategori(INT)   CASCADE;
DROP FUNCTION IF EXISTS fn_get_cascading_origin(INT, INT) CASCADE;
DROP FUNCTION IF EXISTS fn_get_cascading_roast_level(INT, INT, INT) CASCADE;
DROP FUNCTION IF EXISTS fn_get_or_create_product(INT, INT, INT, INT) CASCADE;
DROP FUNCTION IF EXISTS fn_get_laporan_penerimaan(DATE, DATE) CASCADE;
DROP FUNCTION IF EXISTS fn_get_laporan_pengiriman(DATE, DATE) CASCADE;

DROP PROCEDURE IF EXISTS sp_add_user(INT, VARCHAR, VARCHAR, INT[]);
DROP PROCEDURE IF EXISTS sp_update_user(INT, INT, VARCHAR, VARCHAR, BOOLEAN, INT[]);
DROP PROCEDURE IF EXISTS sp_soft_delete_user(INT, INT);
DROP PROCEDURE IF EXISTS sp_add_supplier(INT, VARCHAR, TEXT, VARCHAR);
DROP PROCEDURE IF EXISTS sp_update_supplier(INT, INT, VARCHAR, TEXT, VARCHAR, BOOLEAN);
DROP PROCEDURE IF EXISTS sp_soft_delete_supplier(INT, INT);
DROP PROCEDURE IF EXISTS sp_add_destination(INT, VARCHAR, TEXT);
DROP PROCEDURE IF EXISTS sp_update_destination(INT, INT, VARCHAR, TEXT, BOOLEAN);
DROP PROCEDURE IF EXISTS sp_soft_delete_destination(INT, INT);
DROP PROCEDURE IF EXISTS sp_add_coffee_type(INT, VARCHAR);
DROP PROCEDURE IF EXISTS sp_soft_delete_coffee_type(INT, INT);
DROP PROCEDURE IF EXISTS sp_add_coffee_product(INT, INT, INT, INT, INT, INT);
DROP PROCEDURE IF EXISTS sp_update_coffee_product(INT, INT, INT, INT, INT, INT, INT, BOOLEAN);
DROP PROCEDURE IF EXISTS sp_soft_delete_coffee_product(INT, INT);
DROP PROCEDURE IF EXISTS sp_add_coffee_origin(INT, VARCHAR, VARCHAR, TEXT);
DROP PROCEDURE IF EXISTS sp_add_incoming_transaction(INT, INT, INT, INT);
DROP PROCEDURE IF EXISTS sp_add_outgoing_transaction(INT, INT, INT, INT);

DROP VIEW IF EXISTS vw_dashboard_summary       CASCADE;
DROP VIEW IF EXISTS vw_outgoing_transactions   CASCADE;
DROP VIEW IF EXISTS vw_incoming_transactions   CASCADE;
DROP VIEW IF EXISTS vw_coffee_products         CASCADE;
DROP VIEW IF EXISTS vw_destinations            CASCADE;
DROP VIEW IF EXISTS vw_suppliers               CASCADE;
DROP VIEW IF EXISTS vw_logs                    CASCADE;
DROP VIEW IF EXISTS stock_summary              CASCADE;
DROP VIEW IF EXISTS vw_user_roles              CASCADE;
DROP VIEW IF EXISTS vw_roles                   CASCADE;
DROP VIEW IF EXISTS vw_coffee_types            CASCADE;
DROP VIEW IF EXISTS vw_coffee_categories       CASCADE;
DROP VIEW IF EXISTS vw_active_roast_levels     CASCADE;
DROP VIEW IF EXISTS vw_active_coffee_origins   CASCADE;
DROP VIEW IF EXISTS vw_active_suppliers        CASCADE;
DROP VIEW IF EXISTS vw_active_destinations     CASCADE;

DROP INDEX IF EXISTS idx_coffee_products_unique_with_roast;

DROP TABLE IF EXISTS activity_logs           CASCADE;
DROP TABLE IF EXISTS outgoing_transactions   CASCADE;
DROP TABLE IF EXISTS incoming_transactions   CASCADE;
DROP TABLE IF EXISTS coffee_products         CASCADE;
DROP TABLE IF EXISTS roast_levels            CASCADE;
DROP TABLE IF EXISTS coffee_origins          CASCADE;
DROP TABLE IF EXISTS coffee_types            CASCADE;
DROP TABLE IF EXISTS coffee_categories       CASCADE;
DROP TABLE IF EXISTS destinations            CASCADE;
DROP TABLE IF EXISTS suppliers               CASCADE;
DROP TABLE IF EXISTS user_roles              CASCADE;
DROP TABLE IF EXISTS users                   CASCADE;
DROP TABLE IF EXISTS roles                   CASCADE;


-- ============================================================================
-- SECTION 1: DDL — TABEL
-- ============================================================================

-- 1.1 Roles
CREATE TABLE roles (
    role_id     SERIAL PRIMARY KEY,
    role_name   VARCHAR(20) UNIQUE NOT NULL,
    description TEXT
);

-- 1.2 Users
CREATE TABLE users (
    user_id    SERIAL PRIMARY KEY,
    username   VARCHAR(50) UNIQUE NOT NULL,
    password   VARCHAR(100) NOT NULL,
    is_active  BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 1.3 User Roles (junction table many-to-many)
CREATE TABLE user_roles (
    id      SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(user_id)  ON DELETE CASCADE,
    role_id INT NOT NULL REFERENCES roles(role_id)  ON DELETE CASCADE,
    UNIQUE (user_id, role_id)
);

-- 1.4 Suppliers
CREATE TABLE suppliers (
    supplier_id  SERIAL PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    address      TEXT,
    phone        VARCHAR(20),
    is_active    BOOLEAN DEFAULT TRUE
);

-- 1.5 Kategori Kopi (Cherry → Gabah → Green Bean → Roasted Bean → Ground Coffee)
CREATE TABLE coffee_categories (
    category_id   SERIAL PRIMARY KEY,
    category_name VARCHAR(50) UNIQUE NOT NULL,
    description   TEXT
);

-- 1.6 Jenis Kopi (Arabika, Robusta, dst.)
CREATE TABLE coffee_types (
    coffee_id   SERIAL PRIMARY KEY,
    coffee_name VARCHAR(100) NOT NULL,
    is_active   BOOLEAN DEFAULT TRUE
);

-- 1.7 Asal Daerah Kopi
CREATE TABLE coffee_origins (
    origin_id   SERIAL PRIMARY KEY,
    origin_name VARCHAR(100) NOT NULL,
    region      VARCHAR(100),
    description TEXT,
    is_active   BOOLEAN DEFAULT TRUE
);

-- 1.8 Tingkat Sangrai (hanya relevan untuk kategori Roasted Bean)
CREATE TABLE roast_levels (
    roast_level_id   SERIAL PRIMARY KEY,
    roast_level_name VARCHAR(100) NOT NULL,
    description      TEXT,
    is_active        BOOLEAN DEFAULT TRUE
);

-- 1.9 Destinasi
CREATE TABLE destinations (
    destination_id   SERIAL PRIMARY KEY,
    destination_name VARCHAR(100) NOT NULL,
    address          TEXT,
    is_active        BOOLEAN DEFAULT TRUE
);

-- 1.10 Produk Kopi (master gabungan)
--      roast_level_id boleh NULL (hanya diisi bila category = Roasted Bean)
CREATE TABLE coffee_products (
    product_id       SERIAL PRIMARY KEY,
    coffee_id        INT NOT NULL  REFERENCES coffee_types(coffee_id)       ON DELETE CASCADE,
    category_id      INT           REFERENCES coffee_categories(category_id) ON DELETE SET NULL,
    origin_id        INT           REFERENCES coffee_origins(origin_id)      ON DELETE SET NULL,
    roast_level_id   INT           REFERENCES roast_levels(roast_level_id)   ON DELETE SET NULL,
    current_quantity INTEGER DEFAULT 0  CHECK (current_quantity >= 0),
    minimum_stock    INTEGER DEFAULT 20 CHECK (minimum_stock >= 0),
    last_updated     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active        BOOLEAN DEFAULT TRUE
);

-- Unique index yang memperhitungkan roast_level_id (NULL dianggap sebagai 0)
CREATE UNIQUE INDEX idx_coffee_products_unique_with_roast
ON coffee_products (
    coffee_id,
    COALESCE(category_id,    0),
    COALESCE(origin_id,      0),
    COALESCE(roast_level_id, 0)
);

-- 1.11 Transaksi Masuk
CREATE TABLE incoming_transactions (
    incoming_id SERIAL PRIMARY KEY,
    supplier_id INT REFERENCES suppliers(supplier_id)      ON DELETE SET NULL,
    product_id  INT REFERENCES coffee_products(product_id) ON DELETE SET NULL,
    quantity    INTEGER NOT NULL CHECK (quantity > 0),
    received_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    petugas_id  INT REFERENCES users(user_id)              ON DELETE SET NULL,
    is_active   BOOLEAN DEFAULT true
);

-- 1.12 Transaksi Keluar
CREATE TABLE outgoing_transactions (
    outgoing_id    SERIAL PRIMARY KEY,
    destination_id INT REFERENCES destinations(destination_id)    ON DELETE SET NULL,
    product_id     INT REFERENCES coffee_products(product_id)     ON DELETE SET NULL,
    quantity       INTEGER NOT NULL CHECK (quantity > 0),
    shipped_at     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    petugas_id     INT REFERENCES users(user_id)                  ON DELETE SET NULL,
    is_active   BOOLEAN DEFAULT true
);

-- 1.13 Log Aktivitas
CREATE TABLE activity_logs (
    log_id      SERIAL PRIMARY KEY,
    user_id     INT REFERENCES users(user_id) ON DELETE SET NULL,
    action      VARCHAR(100),
    description TEXT,
    log_time    TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


-- ============================================================================
-- SECTION 2: SEED DATA
-- INSERT dengan ID eksplisit, lalu sequence di-reset agar INSERT berikutnya aman
-- ============================================================================

-- 2a. Roles
INSERT INTO roles (role_id, role_name, description) VALUES
    (1, 'Admin',   'Sistem Administrator / Kepala Gudang'),
    (2, 'Manager', 'Manager Gudang'),
    (3, 'Petugas', 'Petugas Input Data')
ON CONFLICT (role_name) DO UPDATE SET description = EXCLUDED.description;

SELECT setval(pg_get_serial_sequence('roles', 'role_id'), MAX(role_id)) FROM roles;

-- 2b. Kategori Kopi (alur pemrosesan kopi dari hulu ke hilir)
INSERT INTO coffee_categories (category_id, category_name, description) VALUES
    (1, 'Cherry',       'Buah kopi segar yang baru dipetik dari pohon, biasanya berwarna merah matang.'),
    (2, 'Gabah',        'Biji kopi yang sudah dikupas dari daging buahnya namun masih terbungkus cangkang keras.'),
    (3, 'Green Bean',   'Biji kopi mentah yang sudah dikupas dari kulit tanduk dan dikeringkan, belum disangrai.'),
    (4, 'Roasted Bean', 'Biji kopi hijau yang telah disangrai dan berubah cokelat dengan aroma khas.'),
    (5, 'Ground Coffee','Roasted bean yang sudah digiling menjadi bubuk, siap untuk diseduh.')
ON CONFLICT (category_name) DO UPDATE SET description = EXCLUDED.description;

SELECT setval(pg_get_serial_sequence('coffee_categories', 'category_id'), MAX(category_id)) FROM coffee_categories;

-- 2c. Tingkat Sangrai (hanya berlaku bila kategori = Roasted Bean)
INSERT INTO roast_levels (roast_level_id, roast_level_name, description, is_active) VALUES
    (1, 'Light Roast',   'Sangrai ringan, keasaman tinggi, aroma bunga/buah lebih menonjol.', true),
    (2, 'Medium Roast',  'Sangrai sedang, keseimbangan antara keasaman dan body.', true),
    (3, 'Medium Dark',   'Antara medium dan dark, sedikit minyak di permukaan biji.', true),
    (4, 'Dark Roast',    'Sangrai gelap, rasa pahit lebih dominan, aroma smoky.', true)
ON CONFLICT (roast_level_id) DO UPDATE SET
    roast_level_name = EXCLUDED.roast_level_name,
    description      = EXCLUDED.description,
    is_active        = EXCLUDED.is_active;

SELECT setval(pg_get_serial_sequence('roast_levels', 'roast_level_id'), MAX(roast_level_id)) FROM roast_levels;

-- 2d. Jenis Kopi
INSERT INTO coffee_types (coffee_id, coffee_name, is_active) VALUES
    (1, 'Arabika',  true),
    (2, 'Robusta',  true),
    (3, 'Liberika', true),
    (4, 'Excelsa',  true)
ON CONFLICT (coffee_id) DO UPDATE SET
    coffee_name = EXCLUDED.coffee_name,
    is_active   = EXCLUDED.is_active;

SELECT setval(pg_get_serial_sequence('coffee_types', 'coffee_id'), MAX(coffee_id)) FROM coffee_types;

-- 2e. Asal Daerah
INSERT INTO coffee_origins (origin_id, origin_name, region, description, is_active) VALUES
    (1, 'Gayo',        'Sumatera', 'Aceh Tengah',               true),
    (2, 'Toraja',      'Sulawesi', 'Tana Toraja',               true),
    (3, 'Lampung',     'Sumatera', 'Tanggamus / Lampung Barat', true),
    (4, 'Mandailing',  'Sumatera', 'Sumatera Utara',            true),
    (5, 'Lintong',     'Sumatera', 'Humbang Hasundutan',        true),
    (6, 'Sidikalang',  'Sumatera', 'Dairi, Sumatera Utara',     true),
    (7, 'Kerinci',     'Sumatera', 'Gunung Kerinci, Jambi',     true),
    (8, 'Preanger',    'Jawa',     'Jawa Barat',                true),
    (9, 'Ijen',        'Jawa',     'Bondowoso / Banyuwangi',    true),
    (10, 'Temanggung', 'Jawa',     'Sindoro Sumbing',           true),
    (11, 'Kintamani',  'Bali',     'Gunung Batur, Bangli',      true),
    (12, 'Bajawa',     'Flores',   'Ngada, NTT',                true),
    (13, 'Manggarai',  'Flores',   'Manggarai, NTT',            true),
    (14, 'Kalosi',     'Sulawesi', 'Enrekang',                  true),
    (15, 'Wamena',     'Papua',    'Lembah Baliem, Jayawijaya', true),
    (16, 'Moanemani',  'Papua',    'Dogiyai',                   true)
ON CONFLICT (origin_id) DO UPDATE SET
    origin_name = EXCLUDED.origin_name,
    region      = EXCLUDED.region,
    description = EXCLUDED.description,
    is_active   = EXCLUDED.is_active;

SELECT setval(pg_get_serial_sequence('coffee_origins', 'origin_id'), MAX(origin_id)) FROM coffee_origins;

-- 2f. Akun Admin Default
INSERT INTO users (username, password, is_active)
VALUES ('admin', 'admin123', true)
ON CONFLICT (username) DO NOTHING;

SELECT setval(pg_get_serial_sequence('users', 'user_id'), MAX(user_id)) FROM users;

INSERT INTO user_roles (user_id, role_id)
SELECT u.user_id, r.role_id
FROM users u, roles r
WHERE u.username = 'admin' AND r.role_name = 'Admin'
ON CONFLICT (user_id, role_id) DO NOTHING;

INSERT INTO activity_logs (user_id, action, description)
SELECT user_id, 'SYSTEM_SEED', 'System generated initial Admin account'
FROM users WHERE username = 'admin';

-- 2g. Produk Kopi Contoh
--     Gayo Green Bean dari Aceh Tengah (belum disangrai, roast_level_id = NULL) - As Arabika
INSERT INTO coffee_products (coffee_id, category_id, origin_id, roast_level_id, minimum_stock)
SELECT
    (SELECT coffee_id  FROM coffee_types      WHERE coffee_name  = 'Arabika'    LIMIT 1),
    (SELECT category_id FROM coffee_categories WHERE category_name = 'Green Bean' LIMIT 1),
    (SELECT origin_id  FROM coffee_origins    WHERE origin_name  = 'Aceh Tengah' LIMIT 1),
    NULL,
    20
WHERE NOT EXISTS (
    SELECT 1 FROM coffee_products cp
    WHERE cp.coffee_id    = (SELECT coffee_id  FROM coffee_types      WHERE coffee_name  = 'Arabika'     LIMIT 1)
      AND cp.category_id  = (SELECT category_id FROM coffee_categories WHERE category_name = 'Green Bean' LIMIT 1)
      AND cp.origin_id    = (SELECT origin_id  FROM coffee_origins    WHERE origin_name  = 'Aceh Tengah' LIMIT 1)
      AND cp.roast_level_id IS NULL
);

--     Toraja Roasted Bean Medium Roast dari Tana Toraja - As Arabika
INSERT INTO coffee_products (coffee_id, category_id, origin_id, roast_level_id, minimum_stock)
SELECT
    (SELECT coffee_id   FROM coffee_types       WHERE coffee_name    = 'Arabika'      LIMIT 1),
    (SELECT category_id FROM coffee_categories  WHERE category_name  = 'Roasted Bean' LIMIT 1),
    (SELECT origin_id   FROM coffee_origins     WHERE origin_name    = 'Tana Toraja'  LIMIT 1),
    (SELECT roast_level_id FROM roast_levels    WHERE roast_level_name = 'Medium Roast' LIMIT 1),
    15
WHERE NOT EXISTS (
    SELECT 1 FROM coffee_products cp
    WHERE cp.coffee_id       = (SELECT coffee_id   FROM coffee_types       WHERE coffee_name    = 'Arabika'       LIMIT 1)
      AND cp.category_id     = (SELECT category_id FROM coffee_categories  WHERE category_name  = 'Roasted Bean'  LIMIT 1)
      AND cp.origin_id       = (SELECT origin_id   FROM coffee_origins     WHERE origin_name    = 'Tana Toraja'   LIMIT 1)
      AND cp.roast_level_id  = (SELECT roast_level_id FROM roast_levels    WHERE roast_level_name = 'Medium Roast' LIMIT 1)
);

--     Lampung Roasted Bean Dark Roast dari Tanggamus - As Robusta
INSERT INTO coffee_products (coffee_id, category_id, origin_id, roast_level_id, minimum_stock)
SELECT
    (SELECT coffee_id   FROM coffee_types       WHERE coffee_name    = 'Robusta'     LIMIT 1),
    (SELECT category_id FROM coffee_categories  WHERE category_name  = 'Roasted Bean' LIMIT 1),
    (SELECT origin_id   FROM coffee_origins     WHERE origin_name    = 'Tanggamus'   LIMIT 1),
    (SELECT roast_level_id FROM roast_levels    WHERE roast_level_name = 'Dark Roast' LIMIT 1),
    30
WHERE NOT EXISTS (
    SELECT 1 FROM coffee_products cp
    WHERE cp.coffee_id       = (SELECT coffee_id   FROM coffee_types       WHERE coffee_name    = 'Robusta'      LIMIT 1)
      AND cp.category_id     = (SELECT category_id FROM coffee_categories  WHERE category_name  = 'Roasted Bean' LIMIT 1)
      AND cp.origin_id       = (SELECT origin_id   FROM coffee_origins     WHERE origin_name    = 'Tanggamus'    LIMIT 1)
      AND cp.roast_level_id  = (SELECT roast_level_id FROM roast_levels    WHERE roast_level_name = 'Dark Roast'  LIMIT 1)
);

SELECT setval(pg_get_serial_sequence('coffee_products', 'product_id'), MAX(product_id)) FROM coffee_products;


-- ============================================================================
-- SECTION 3: TRIGGER FUNCTIONS
-- ============================================================================

-- 3a. Update stok saat penerimaan
CREATE OR REPLACE FUNCTION fn_update_stock_on_incoming()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE coffee_products
    SET current_quantity = current_quantity + NEW.quantity,
        last_updated     = CURRENT_TIMESTAMP
    WHERE product_id = NEW.product_id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 3b. Validasi & kurangi stok saat pengiriman
CREATE OR REPLACE FUNCTION fn_update_stock_on_outgoing()
RETURNS TRIGGER AS $$
DECLARE
    v_current INTEGER;
BEGIN
    SELECT current_quantity INTO v_current
    FROM coffee_products
    WHERE product_id = NEW.product_id
    FOR UPDATE;

    IF v_current IS NULL OR v_current < NEW.quantity THEN
        RAISE EXCEPTION 'Stok tidak cukup. Stok tersedia: %, diminta: %',
            COALESCE(v_current, 0), NEW.quantity;
    END IF;

    UPDATE coffee_products
    SET current_quantity = current_quantity - NEW.quantity,
        last_updated     = CURRENT_TIMESTAMP
    WHERE product_id = NEW.product_id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 3c. Auto-log transaksi masuk
CREATE OR REPLACE FUNCTION fn_log_incoming_transaction()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO activity_logs (user_id, action, description)
    VALUES (
        NEW.petugas_id,
        'INCOMING_STOCK',
        'Auto-log: Penerimaan produk ID ' || NEW.product_id
            || ' sebanyak ' || NEW.quantity
            || ' kg dari supplier ID ' || COALESCE(NEW.supplier_id::TEXT, 'N/A')
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 3d. Auto-log transaksi keluar
CREATE OR REPLACE FUNCTION fn_log_outgoing_transaction()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO activity_logs (user_id, action, description)
    VALUES (
        NEW.petugas_id,
        'OUTGOING_STOCK',
        'Auto-log: Pengiriman produk ID ' || NEW.product_id
            || ' sebanyak ' || NEW.quantity
            || ' kg ke destinasi ID ' || COALESCE(NEW.destination_id::TEXT, 'N/A')
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- ============================================================================
-- SECTION 4: TRIGGERS
-- ============================================================================

CREATE OR REPLACE TRIGGER trg_incoming_update_stock
AFTER INSERT ON incoming_transactions
FOR EACH ROW EXECUTE FUNCTION fn_update_stock_on_incoming();

CREATE OR REPLACE TRIGGER trg_outgoing_update_stock
BEFORE INSERT ON outgoing_transactions
FOR EACH ROW EXECUTE FUNCTION fn_update_stock_on_outgoing();

CREATE OR REPLACE TRIGGER trg_log_incoming_transaction
AFTER INSERT ON incoming_transactions
FOR EACH ROW EXECUTE FUNCTION fn_log_incoming_transaction();

CREATE OR REPLACE TRIGGER trg_log_outgoing_transaction
AFTER INSERT ON outgoing_transactions
FOR EACH ROW EXECUTE FUNCTION fn_log_outgoing_transaction();


-- ============================================================================
-- SECTION 5: VIEWS
-- ============================================================================

-- 5a. User + Roles
CREATE OR REPLACE VIEW vw_user_roles AS
SELECT
    u.user_id,
    u.username,
    u.is_active,
    u.created_at,
    STRING_AGG(r.role_name, ', ') AS roles_string
FROM users u
LEFT JOIN user_roles ur ON u.user_id = ur.user_id
LEFT JOIN roles r       ON ur.role_id = r.role_id
GROUP BY u.user_id, u.username, u.is_active, u.created_at;

-- 5a.1 Roles
CREATE OR REPLACE VIEW vw_roles AS
SELECT role_id, role_name, description
FROM roles;

-- 5b. Ringkasan Stok
CREATE OR REPLACE VIEW stock_summary AS
SELECT
    cp.product_id,
    cp.coffee_id,
    ct.coffee_name,
    cc.category_name,
    co.origin_name,
    rl.roast_level_name,
    COALESCE(cp.current_quantity, 0) AS current_quantity,
    cp.minimum_stock,
    CASE
        WHEN COALESCE(cp.current_quantity, 0) < cp.minimum_stock THEN 'LOW'
        ELSE 'SAFE'
    END AS status
FROM coffee_products cp
LEFT JOIN coffee_types       ct ON cp.coffee_id      = ct.coffee_id
LEFT JOIN coffee_categories  cc ON cp.category_id    = cc.category_id
LEFT JOIN coffee_origins     co ON cp.origin_id      = co.origin_id
LEFT JOIN roast_levels       rl ON cp.roast_level_id = rl.roast_level_id
WHERE ct.is_active = TRUE AND cp.is_active = TRUE;

-- 5c. Log Aktivitas
CREATE OR REPLACE VIEW vw_logs AS
SELECT
    al.log_id,
    COALESCE(u.username, 'System') AS actor,
    al.action,
    al.description,
    al.log_time
FROM activity_logs al
LEFT JOIN users u ON al.user_id = u.user_id
ORDER BY al.log_time DESC;

-- 5d. Semua Supplier
CREATE OR REPLACE VIEW vw_suppliers AS
SELECT supplier_id, company_name, address, phone, is_active
FROM suppliers
ORDER BY company_name;

-- 5e. Semua Destinasi
CREATE OR REPLACE VIEW vw_destinations AS
SELECT destination_id, destination_name, address, is_active
FROM destinations
ORDER BY destination_name;

-- 5f. Produk Kopi (untuk combobox & grid di aplikasi)
CREATE OR REPLACE VIEW vw_coffee_products AS
SELECT
    cp.product_id,
    cp.coffee_id,
    ct.coffee_name,
    cp.category_id,
    COALESCE(cc.category_name,    'Tanpa Kategori') AS category_name,
    cp.origin_id,
    COALESCE(co.origin_name,      'Tanpa Origin')   AS origin_name,
    cp.roast_level_id,
    COALESCE(rl.roast_level_name, '-')              AS roast_level_name,
    COALESCE(cp.current_quantity, 0)                AS current_quantity,
    cp.minimum_stock,
    cp.is_active
FROM coffee_products cp
LEFT JOIN coffee_types      ct ON cp.coffee_id      = ct.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id    = cc.category_id
LEFT JOIN coffee_origins    co ON cp.origin_id      = co.origin_id
LEFT JOIN roast_levels      rl ON cp.roast_level_id = rl.roast_level_id
ORDER BY ct.coffee_name;

-- 5f.1 Coffee Types
CREATE OR REPLACE VIEW vw_coffee_types AS
SELECT coffee_id, coffee_name, is_active
FROM coffee_types
ORDER BY coffee_name;

-- 5f.2 Coffee Categories
CREATE OR REPLACE VIEW vw_coffee_categories AS
SELECT category_id, category_name, description
FROM coffee_categories
ORDER BY category_name;

-- 5f.3 Active Roast Levels
CREATE OR REPLACE VIEW vw_active_roast_levels AS
SELECT roast_level_id, roast_level_name, description, is_active
FROM roast_levels
WHERE is_active = true
ORDER BY roast_level_id;

-- 5f.4 Active Coffee Origins
CREATE OR REPLACE VIEW vw_active_coffee_origins AS
SELECT origin_id, origin_name, region, description, is_active
FROM coffee_origins
WHERE is_active = true
ORDER BY origin_name;

-- 5g. Transaksi Masuk
CREATE OR REPLACE VIEW vw_incoming_transactions AS
SELECT
    t.incoming_id,
    t.received_at AS tanggal,
    COALESCE(s.company_name, 'N/A') AS supplier,
    COALESCE(ct.coffee_name,  'N/A') || ' - '
        || COALESCE(cc.category_name, 'N/A') || ' - '
        || COALESCE(co.origin_name,   'N/A') AS jenis_kopi,
    t.quantity    AS jumlah,
    COALESCE(u.username, 'N/A') AS petugas
FROM incoming_transactions t
LEFT JOIN suppliers         s  ON t.supplier_id  = s.supplier_id
LEFT JOIN coffee_products  cp  ON t.product_id   = cp.product_id
LEFT JOIN coffee_types     ct  ON cp.coffee_id   = ct.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id = cc.category_id
LEFT JOIN coffee_origins    co ON cp.origin_id   = co.origin_id
LEFT JOIN users             u  ON t.petugas_id   = u.user_id
ORDER BY t.received_at DESC;

-- 5h. Transaksi Keluar
CREATE OR REPLACE VIEW vw_outgoing_transactions AS
SELECT
    t.outgoing_id,
    t.shipped_at AS tanggal,
    COALESCE(d.destination_name, 'N/A') AS destinasi,
    COALESCE(ct.coffee_name,  'N/A') || ' - '
        || COALESCE(cc.category_name, 'N/A') || ' - '
        || COALESCE(co.origin_name,   'N/A') AS jenis_kopi,
    t.quantity    AS jumlah,
    COALESCE(u.username, 'N/A') AS petugas
FROM outgoing_transactions t
LEFT JOIN destinations      d  ON t.destination_id = d.destination_id
LEFT JOIN coffee_products  cp  ON t.product_id     = cp.product_id
LEFT JOIN coffee_types     ct  ON cp.coffee_id     = ct.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id   = cc.category_id
LEFT JOIN coffee_origins    co ON cp.origin_id     = co.origin_id
LEFT JOIN users             u  ON t.petugas_id     = u.user_id
ORDER BY t.shipped_at DESC;

-- 5i. Dashboard Summary
CREATE OR REPLACE VIEW vw_dashboard_summary AS
SELECT
    (SELECT COUNT(*) FROM coffee_types  WHERE is_active = true) AS total_coffee_types,
    (SELECT COUNT(*) FROM suppliers     WHERE is_active = true) AS total_suppliers,
    (SELECT COUNT(*) FROM destinations  WHERE is_active = true) AS total_destinations,
    (SELECT COUNT(*) FROM incoming_transactions)                AS total_incoming,
    (SELECT COUNT(*) FROM outgoing_transactions)                AS total_outgoing,
    (SELECT COUNT(*) FROM stock_summary WHERE status = 'LOW')  AS total_low_stock;


-- ============================================================================
-- SECTION 6: UTILITY FUNCTIONS
-- ============================================================================

-- 6a. Ambil stok produk
CREATE OR REPLACE FUNCTION fn_get_stock_by_product(p_product_id INT)
RETURNS INTEGER AS $$
DECLARE v_qty INTEGER;
BEGIN
    SELECT current_quantity INTO v_qty
    FROM coffee_products WHERE product_id = p_product_id;
    RETURN COALESCE(v_qty, 0);
END;
$$ LANGUAGE plpgsql;

-- 6b. Cek apakah stok cukup
CREATE OR REPLACE FUNCTION fn_is_stock_sufficient(p_product_id INT, p_quantity INT)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN fn_get_stock_by_product(p_product_id) >= p_quantity;
END;
$$ LANGUAGE plpgsql;

-- 6c. Daftar produk dengan stok rendah
CREATE OR REPLACE FUNCTION fn_get_low_stock_items()
RETURNS TABLE (
    product_id    INT,
    coffee_name   VARCHAR,
    current_qty   INT,
    minimum_stock INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        cp.product_id,
        ct.coffee_name,
        COALESCE(cp.current_quantity, 0) AS current_qty,
        cp.minimum_stock
    FROM coffee_products cp
    LEFT JOIN coffee_types ct ON cp.coffee_id = ct.coffee_id
    WHERE ct.is_active = true
      AND COALESCE(cp.current_quantity, 0) < cp.minimum_stock;
END;
$$ LANGUAGE plpgsql;


-- ============================================================================
-- SECTION 7: STORED PROCEDURES — USER MANAGEMENT
-- ============================================================================

-- 7a. Tambah User
CREATE OR REPLACE PROCEDURE sp_add_user(
    p_admin_id    INT,
    p_username    VARCHAR,
    p_password    VARCHAR,
    p_roles_array INT[]
)
LANGUAGE plpgsql AS $$
DECLARE
    new_user_id INT;
    r_id        INT;
BEGIN
    INSERT INTO users (username, password, is_active)
    VALUES (p_username, p_password, true)
    RETURNING user_id INTO new_user_id;

    IF p_roles_array IS NOT NULL THEN
        FOREACH r_id IN ARRAY p_roles_array LOOP
            INSERT INTO user_roles (user_id, role_id) VALUES (new_user_id, r_id);
        END LOOP;
    END IF;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'CREATE_USER', 'Created user: ' || p_username);
END;
$$;

-- 7b. Update User
CREATE OR REPLACE PROCEDURE sp_update_user(
    p_admin_id    INT,
    p_user_id     INT,
    p_username    VARCHAR,
    p_password    VARCHAR,
    p_is_active   BOOLEAN,
    p_roles_array INT[]
)
LANGUAGE plpgsql AS $$
DECLARE r_id INT;
BEGIN
    IF p_password IS NULL OR p_password = '' THEN
        UPDATE users SET username = p_username, is_active = p_is_active
        WHERE user_id = p_user_id;
    ELSE
        UPDATE users SET username = p_username, password = p_password, is_active = p_is_active
        WHERE user_id = p_user_id;
    END IF;

    DELETE FROM user_roles WHERE user_id = p_user_id;

    IF p_roles_array IS NOT NULL THEN
        FOREACH r_id IN ARRAY p_roles_array LOOP
            INSERT INTO user_roles (user_id, role_id) VALUES (p_user_id, r_id);
        END LOOP;
    END IF;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'UPDATE_USER', 'Updated user_id: ' || p_user_id);
END;
$$;

-- 7c. Soft Delete User
CREATE OR REPLACE PROCEDURE sp_soft_delete_user(
    p_admin_id INT,
    p_user_id  INT
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE users SET is_active = FALSE WHERE user_id = p_user_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'SOFT_DELETE_USER', 'Deactivated user_id: ' || p_user_id);
END;
$$;


-- ============================================================================
-- SECTION 8: STORED PROCEDURES — MASTER DATA
-- ============================================================================

-- 8a. Tambah Supplier
CREATE OR REPLACE PROCEDURE sp_add_supplier(
    p_admin_id    INT,
    p_company_name VARCHAR,
    p_address     TEXT,
    p_phone       VARCHAR
)
LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO suppliers (company_name, address, phone)
    VALUES (p_company_name, p_address, p_phone);

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'CREATE_SUPPLIER', 'Added supplier: ' || p_company_name);
END;
$$;

-- 8b. Soft Delete Supplier
CREATE OR REPLACE PROCEDURE sp_soft_delete_supplier(
    p_admin_id   INT,
    p_supplier_id INT
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE suppliers SET is_active = FALSE WHERE supplier_id = p_supplier_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'SOFT_DELETE_SUPPLIER', 'Deactivated supplier_id: ' || p_supplier_id);
END;
$$;

-- 8b.1 Update Supplier
CREATE OR REPLACE PROCEDURE sp_update_supplier(
    p_admin_id    INT,
    p_supplier_id INT,
    p_company_name VARCHAR,
    p_address     TEXT,
    p_phone       VARCHAR,
    p_is_active   BOOLEAN
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE suppliers 
    SET company_name = p_company_name, 
        address = p_address, 
        phone = p_phone, 
        is_active = p_is_active 
    WHERE supplier_id = p_supplier_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'UPDATE_SUPPLIER', 'Updated supplier_id: ' || p_supplier_id);
END;
$$;

-- 8c. Tambah Destinasi
CREATE OR REPLACE PROCEDURE sp_add_destination(
    p_admin_id         INT,
    p_destination_name VARCHAR,
    p_address          TEXT
)
LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO destinations (destination_name, address)
    VALUES (p_destination_name, p_address);

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'CREATE_DESTINATION', 'Added destination: ' || p_destination_name);
END;
$$;

-- 8d. Soft Delete Destinasi
CREATE OR REPLACE PROCEDURE sp_soft_delete_destination(
    p_admin_id     INT,
    p_destination_id INT
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE destinations SET is_active = FALSE WHERE destination_id = p_destination_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'SOFT_DELETE_DESTINATION', 'Deactivated destination_id: ' || p_destination_id);
END;
$$;

-- 8d.1 Update Destination
CREATE OR REPLACE PROCEDURE sp_update_destination(
    p_admin_id       INT,
    p_destination_id INT,
    p_destination_name VARCHAR,
    p_address        TEXT,
    p_is_active      BOOLEAN
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE destinations 
    SET destination_name = p_destination_name, 
        address = p_address, 
        is_active = p_is_active 
    WHERE destination_id = p_destination_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'UPDATE_DESTINATION', 'Updated destination_id: ' || p_destination_id);
END;
$$;

-- 8e. Tambah Jenis Kopi
CREATE OR REPLACE PROCEDURE sp_add_coffee_type(
    p_admin_id   INT,
    p_coffee_name VARCHAR
)
LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO coffee_types (coffee_name) VALUES (p_coffee_name);

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'CREATE_COFFEE_TYPE', 'Added coffee type: ' || p_coffee_name);
END;
$$;

-- 8f. Soft Delete Jenis Kopi
CREATE OR REPLACE PROCEDURE sp_soft_delete_coffee_type(
    p_admin_id INT,
    p_coffee_id INT
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE coffee_types SET is_active = FALSE WHERE coffee_id = p_coffee_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'SOFT_DELETE_COFFEE_TYPE', 'Deactivated coffee_id: ' || p_coffee_id);
END;
$$;

-- 8g. Tambah Produk Kopi
--     p_roast_level_id boleh NULL (kirim NULL kalau bukan Roasted Bean)
CREATE OR REPLACE PROCEDURE sp_add_coffee_product(
    p_admin_id      INT,
    p_coffee_id     INT,
    p_category_id   INT,
    p_origin_id     INT,
    p_roast_level_id INT,   -- NULL bila bukan Roasted Bean
    p_minimum_stock INT
)
LANGUAGE plpgsql AS $$
DECLARE v_product_id INT;
BEGIN
    INSERT INTO coffee_products (coffee_id, category_id, origin_id, roast_level_id, minimum_stock)
    VALUES (p_coffee_id, p_category_id, p_origin_id, p_roast_level_id, COALESCE(p_minimum_stock, 20))
    RETURNING product_id INTO v_product_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'CREATE_COFFEE_PRODUCT', 'Added coffee product_id: ' || v_product_id);
END;
$$;

-- 8h. Soft Delete Produk Kopi
CREATE OR REPLACE PROCEDURE sp_soft_delete_coffee_product(
    p_admin_id  INT,
    p_product_id INT
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE coffee_products SET is_active = FALSE WHERE product_id = p_product_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'SOFT_DELETE_COFFEE_PRODUCT', 'Deactivated product_id: ' || p_product_id);
END;
$$;

-- 8i. Update Produk Kopi
CREATE OR REPLACE PROCEDURE sp_update_coffee_product(
    p_admin_id      INT,
    p_product_id    INT,
    p_coffee_id     INT,
    p_category_id   INT,
    p_origin_id     INT,
    p_roast_level_id INT,
    p_minimum_stock INT,
    p_is_active     BOOLEAN
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE coffee_products
    SET coffee_id = p_coffee_id,
        category_id = p_category_id,
        origin_id = p_origin_id,
        roast_level_id = CASE WHEN p_roast_level_id > 0 THEN p_roast_level_id ELSE NULL END,
        minimum_stock = p_minimum_stock,
        is_active = p_is_active,
        last_updated = CURRENT_TIMESTAMP
    WHERE product_id = p_product_id;

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'UPDATE_COFFEE_PRODUCT', 'Updated product_id: ' || p_product_id);
END;
$$;

-- 8j. Add Coffee Origin
CREATE OR REPLACE PROCEDURE sp_add_coffee_origin(
    p_admin_id   INT,
    p_origin_name VARCHAR,
    p_region      VARCHAR,
    p_description TEXT
)
LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO coffee_origins (origin_name, region, description, is_active)
    VALUES (p_origin_name, p_region, p_description, true);

    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_admin_id, 'CREATE_COFFEE_ORIGIN', 'Added coffee origin: ' || p_origin_name);
END;
$$;


-- ============================================================================
-- SECTION 9: STORED PROCEDURES — TRANSAKSI
-- ============================================================================

-- 9a. Transaksi Masuk
CREATE OR REPLACE PROCEDURE sp_add_incoming_transaction(
    p_supplier_id INT,
    p_product_id  INT,
    p_quantity    INT,
    p_petugas_id  INT
)
LANGUAGE plpgsql AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM suppliers      WHERE supplier_id = p_supplier_id AND is_active = true) THEN
        RAISE EXCEPTION 'Supplier ID % tidak ditemukan atau tidak aktif.', p_supplier_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM coffee_products WHERE product_id = p_product_id AND is_active = true) THEN
        RAISE EXCEPTION 'Produk ID % tidak ditemukan atau tidak aktif.', p_product_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users          WHERE user_id = p_petugas_id    AND is_active = true) THEN
        RAISE EXCEPTION 'Petugas ID % tidak ditemukan atau tidak aktif.', p_petugas_id;
    END IF;

    -- Trigger stok & log berjalan otomatis
    INSERT INTO incoming_transactions (supplier_id, product_id, quantity, petugas_id)
    VALUES (p_supplier_id, p_product_id, p_quantity, p_petugas_id);
END;
$$;

-- 9b. Transaksi Keluar
CREATE OR REPLACE PROCEDURE sp_add_outgoing_transaction(
    p_destination_id INT,
    p_product_id     INT,
    p_quantity       INT,
    p_petugas_id     INT
)
LANGUAGE plpgsql AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM destinations    WHERE destination_id = p_destination_id AND is_active = true) THEN
        RAISE EXCEPTION 'Destinasi ID % tidak ditemukan atau tidak aktif.', p_destination_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM coffee_products WHERE product_id = p_product_id AND is_active = true) THEN
        RAISE EXCEPTION 'Produk ID % tidak ditemukan atau tidak aktif.', p_product_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users           WHERE user_id = p_petugas_id   AND is_active = true) THEN
        RAISE EXCEPTION 'Petugas ID % tidak ditemukan atau tidak aktif.', p_petugas_id;
    END IF;

    -- Trigger stok (cek & kurangi) + log berjalan otomatis
    INSERT INTO outgoing_transactions (destination_id, product_id, quantity, petugas_id)
    VALUES (p_destination_id, p_product_id, p_quantity, p_petugas_id);
END;
$$;


-- ============================================================================
-- SECTION 10: VIEWS & FUNCTIONS FOR MVC REFACTORING
-- ============================================================================

-- 10.1. Views untuk Data Master Statis
CREATE OR REPLACE VIEW vw_active_suppliers AS
SELECT supplier_id, company_name 
FROM suppliers 
WHERE is_active = true
ORDER BY company_name;

CREATE OR REPLACE VIEW vw_active_destinations AS
SELECT destination_id, destination_name 
FROM destinations 
WHERE is_active = true
ORDER BY destination_name;

-- 10.2. Functions untuk Cascading Dropdown (Dynamic)
CREATE OR REPLACE FUNCTION fn_get_cascading_jenis_kopi()
RETURNS TABLE (coffee_id INT, coffee_name VARCHAR) AS $$
BEGIN
    RETURN QUERY
    SELECT DISTINCT ct.coffee_id, ct.coffee_name 
    FROM coffee_products cp 
    JOIN coffee_types ct ON cp.coffee_id = ct.coffee_id 
    WHERE cp.is_active = true AND ct.is_active = true 
    ORDER BY ct.coffee_name;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION fn_get_cascading_kategori(p_coffee_id INT)
RETURNS TABLE (category_id INT, category_name VARCHAR) AS $$
BEGIN
    RETURN QUERY
    SELECT DISTINCT cc.category_id, cc.category_name 
    FROM coffee_products cp 
    JOIN coffee_categories cc ON cp.category_id = cc.category_id 
    WHERE cp.coffee_id = p_coffee_id AND cp.is_active = true 
    ORDER BY cc.category_name;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION fn_get_cascading_origin(p_coffee_id INT, p_category_id INT)
RETURNS TABLE (origin_id INT, origin_name VARCHAR) AS $$
BEGIN
    RETURN QUERY
    SELECT DISTINCT co.origin_id, co.origin_name 
    FROM coffee_products cp 
    JOIN coffee_origins co ON cp.origin_id = co.origin_id 
    WHERE cp.coffee_id = p_coffee_id AND cp.category_id = p_category_id 
      AND cp.is_active = true AND co.is_active = true 
    ORDER BY co.origin_name;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION fn_get_cascading_roast_level(p_coffee_id INT, p_category_id INT, p_origin_id INT)
RETURNS TABLE (roast_level_id INT, roast_level_name VARCHAR) AS $$
BEGIN
    RETURN QUERY
    SELECT DISTINCT rl.roast_level_id, rl.roast_level_name 
    FROM coffee_products cp 
    JOIN roast_levels rl ON cp.roast_level_id = rl.roast_level_id 
    WHERE cp.coffee_id = p_coffee_id AND cp.category_id = p_category_id AND cp.origin_id = p_origin_id 
      AND cp.is_active = true AND rl.is_active = true 
    ORDER BY rl.roast_level_name;
END;
$$ LANGUAGE plpgsql;

-- 10.3. Function untuk Mendapatkan / Membuat Produk
CREATE OR REPLACE FUNCTION fn_get_or_create_product(
    p_coffee_id INT,
    p_category_id INT,
    p_origin_id INT,
    p_roast_level_id INT
) RETURNS INT AS $$
DECLARE
    v_product_id INT;
BEGIN
    -- Coba cari produk yang sudah ada (termasuk pengecekan NULL)
    SELECT product_id INTO v_product_id
    FROM coffee_products
    WHERE coffee_id = p_coffee_id
      AND category_id = p_category_id
      AND origin_id = p_origin_id
      AND (
          (roast_level_id = p_roast_level_id AND p_roast_level_id > 0)
          OR (roast_level_id IS NULL AND (p_roast_level_id = 0 OR p_roast_level_id IS NULL))
      )
    LIMIT 1;

    -- Jika belum ada, buat baru
    IF v_product_id IS NULL THEN
        INSERT INTO coffee_products (
            coffee_id, category_id, origin_id, roast_level_id, minimum_stock
        )
        VALUES (
            p_coffee_id, 
            p_category_id, 
            p_origin_id, 
            CASE WHEN p_roast_level_id > 0 THEN p_roast_level_id ELSE NULL END, 
            20
        )
        RETURNING product_id INTO v_product_id;
    END IF;

    RETURN v_product_id;
END;
$$ LANGUAGE plpgsql;

-- 10.4. Functions untuk Laporan (Filter Tanggal)
CREATE OR REPLACE FUNCTION fn_get_laporan_penerimaan(p_start_date DATE, p_end_date DATE)
RETURNS TABLE (
    "Tanggal" TIMESTAMP,
    "Supplier" VARCHAR,
    "JenisKopi" TEXT,
    "Jumlah" INT,
    "Petugas" VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tanggal AS "Tanggal",
        supplier::VARCHAR AS "Supplier",
        jenis_kopi::TEXT AS "JenisKopi",
        jumlah AS "Jumlah",
        petugas::VARCHAR AS "Petugas"
    FROM vw_incoming_transactions
    WHERE tanggal >= p_start_date AND tanggal < p_end_date
    ORDER BY tanggal DESC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION fn_get_laporan_pengiriman(p_start_date DATE, p_end_date DATE)
RETURNS TABLE (
    "Tanggal" TIMESTAMP,
    "Destinasi" VARCHAR,
    "JenisKopi" TEXT,
    "Jumlah" INT,
    "Petugas" VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tanggal AS "Tanggal",
        destinasi::VARCHAR AS "Destinasi",
        jenis_kopi::TEXT AS "JenisKopi",
        jumlah AS "Jumlah",
        petugas::VARCHAR AS "Petugas"
    FROM vw_outgoing_transactions
    WHERE tanggal >= p_start_date AND tanggal < p_end_date
    ORDER BY tanggal DESC;
END;
$$ LANGUAGE plpgsql;
-- ============================================================================
-- SECTION 11: CARA MENGALIHKAN APLIKASI KE SCHEMA BARU INI (PostgreSQL)
-- ============================================================================
ALTER ROLE postgres IN DATABASE smg_kopi SET search_path TO coffee_warehouse, public;
SHOW search_path;

-- Setelah script di atas dijalankan, semua objek (tabel, view, function,
-- procedure, trigger, index) berada di schema "coffee_warehouse", BUKAN di
-- "public". Database lama di "public" tetap aman, tidak tersentuh.
--
-- Agar aplikasi Anda otomatis memanggil objek-objek di "coffee_warehouse"
-- TANPA mengubah kode program (nama tabel/SP/fungsi tetap sama), atur
-- search_path untuk role/user database yang dipakai aplikasi:
--
--     ALTER ROLE username_aplikasi_kamu SET search_path TO coffee_warehouse, public;
--
-- Cara kerja:
--   - Setiap koneksi baru dari user tersebut akan memiliki search_path
--     = coffee_warehouse, public secara default.
--   - Saat aplikasi memanggil sp_add_incoming_transaction(), vw_logs,
--     stock_summary, dll (tanpa prefix schema), PostgreSQL akan mencari di
--     "coffee_warehouse" dahulu. Jika ditemukan, itulah yang dijalankan.
--   - Jika suatu objek TIDAK ada di "coffee_warehouse" (misal Anda belum
--     migrasi sebagian objek), PostgreSQL otomatis fallback mencari ke
--     "public".
--
-- Untuk verifikasi search_path yang aktif untuk user tertentu:
--     SELECT rolname, rolconfig FROM pg_roles WHERE rolname = 'username_aplikasi_kamu';
--
-- Untuk mengecek search_path pada koneksi yang sedang berjalan:
--     SHOW search_path;
--
-- Jika suatu saat ingin rollback (kembali pakai schema "public" saja):
--     ALTER ROLE username_aplikasi_kamu RESET search_path;
-- ============================================================================