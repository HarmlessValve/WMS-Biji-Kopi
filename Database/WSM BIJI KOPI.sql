----------------------------------------------------------------------
-- Drop Table
DROP VIEW IF EXISTS stock_summary;

DROP TABLE IF EXISTS activity_logs;
DROP TABLE IF EXISTS outgoing_transactions;
DROP TABLE IF EXISTS incoming_transactions;
DROP TABLE IF EXISTS stock;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS coffee_types;
DROP TABLE IF EXISTS coffee_categories;
DROP TABLE IF EXISTS destinations;
DROP TABLE IF EXISTS suppliers;
----------------------------------------------------------------------

----------------------------------------------------------------------
-- Tabel Roles 
CREATE TABLE roles (
    role_id   SERIAL PRIMARY KEY,
    role_name VARCHAR(20) UNIQUE NOT NULL,
    description TEXT
);

-- Tabel Users
CREATE TABLE users (
    user_id    SERIAL PRIMARY KEY,
    username   VARCHAR(50) UNIQUE NOT NULL,
    password   VARCHAR(100) NOT NULL,
    role_id    INT REFERENCES roles(role_id) ON DELETE RESTRICT,
    is_active  BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
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
    category_id   INT REFERENCES coffee_categories(category_id) ON DELETE SET NULL,
    minimum_stock INTEGER DEFAULT 20 CHECK (minimum_stock >= 0),
    is_active     BOOLEAN DEFAULT TRUE
);

-- Tabel Destinasi Ekspor/Impor
CREATE TABLE destinations (
    destination_id   SERIAL PRIMARY KEY,
    destination_name VARCHAR(100) NOT NULL,
    address          TEXT,
    is_active        BOOLEAN DEFAULT TRUE
);

-- Tabel Stok
CREATE TABLE stock (
    stock_id         SERIAL PRIMARY KEY,
    coffee_id        INT UNIQUE NOT NULL REFERENCES coffee_types(coffee_id) ON DELETE CASCADE,
    current_quantity INTEGER DEFAULT 0 CHECK (current_quantity >= 0),
    last_updated     TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel Transaksi Masuk
CREATE TABLE incoming_transactions (
    incoming_id  SERIAL PRIMARY KEY,
    supplier_id  INT REFERENCES suppliers(supplier_id) ON DELETE SET NULL,
    coffee_id    INT REFERENCES coffee_types(coffee_id) ON DELETE SET NULL,
    quantity     INTEGER NOT NULL CHECK (quantity > 0),
    received_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    petugas_id   INT REFERENCES users(user_id) ON DELETE SET NULL
);

-- Tabel Transaksi Keluar
CREATE TABLE outgoing_transactions (
    outgoing_id    SERIAL PRIMARY KEY,
    destination_id INT REFERENCES destinations(destination_id) ON DELETE SET NULL,
    coffee_id      INT REFERENCES coffee_types(coffee_id) ON DELETE SET NULL,
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
----------------------------------------------------------------------

----------------------------------------------------------------------
--Fungsi Trigger Update stok otomatis saat transaksi masuk
CREATE OR REPLACE FUNCTION fn_update_stock_on_incoming()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO stock (coffee_id, current_quantity, last_updated)
    VALUES (NEW.coffee_id, 0, CURRENT_TIMESTAMP)
    ON CONFLICT (coffee_id) DO NOTHING;

    UPDATE stock
    SET current_quantity = current_quantity + NEW.quantity,
        last_updated     = CURRENT_TIMESTAMP
    WHERE coffee_id = NEW.coffee_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

--Trigger update masuk stok
CREATE TRIGGER trg_incoming_update_stock
AFTER INSERT ON incoming_transactions
FOR EACH ROW EXECUTE FUNCTION fn_update_stock_on_incoming();
----------------------------------------------------------------------

----------------------------------------------------------------------
--Fungsi Trigger Update stok otomatis saat transaksi keluar
CREATE OR REPLACE FUNCTION fn_update_stock_on_outgoing()
RETURNS TRIGGER AS $$
DECLARE
    v_current INTEGER;
BEGIN
    -- Ambil stok saat ini
    SELECT current_quantity INTO v_current
    FROM stock
    WHERE coffee_id = NEW.coffee_id
    FOR UPDATE;  -- lock baris agar aman dari race condition

    -- Jika stok tidak cukup, batalkan transaksi
    IF v_current IS NULL OR v_current < NEW.quantity THEN
        RAISE EXCEPTION 'Stok tidak cukup. Stok tersedia: %, diminta: %',
            COALESCE(v_current, 0), NEW.quantity;
    END IF;

    -- Kurangi stok
    UPDATE stock
    SET current_quantity = current_quantity - NEW.quantity,
        last_updated     = CURRENT_TIMESTAMP
    WHERE coffee_id = NEW.coffee_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger update keluar stok
CREATE TRIGGER trg_outgoing_update_stock
BEFORE INSERT ON outgoing_transactions
FOR EACH ROW EXECUTE FUNCTION fn_update_stock_on_outgoing();
----------------------------------------------------------------------

----------------------------------------------------------------------
CREATE OR REPLACE PROCEDURE sp_add_incoming_transaction(
    p_supplier_id INT,
    p_coffee_id INT,
    p_quantity INT,
    p_petugas_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- 1. Validasi: Pastikan data ID yang dimasukkan ada di database
    IF NOT EXISTS (SELECT 1 FROM suppliers WHERE supplier_id = p_supplier_id) THEN
        RAISE EXCEPTION 'Error: Supplier dengan ID % tidak ditemukan.', p_supplier_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM coffee_types WHERE coffee_id = p_coffee_id) THEN
        RAISE EXCEPTION 'Error: Jenis Kopi dengan ID % tidak ditemukan.', p_coffee_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_petugas_id) THEN
        RAISE EXCEPTION 'Error: Petugas dengan ID % tidak ditemukan.', p_petugas_id;
    END IF;

    -- 2. Lakukan INSERT (Otomatis memicu trigger penambahan stok yang sudah kamu buat)
    INSERT INTO incoming_transactions (supplier_id, coffee_id, quantity, petugas_id)
    VALUES (p_supplier_id, p_coffee_id, p_quantity, p_petugas_id);

    -- 3. Catat ke Log Aktivitas (Opsional, memanfaatkan tabel activity_logs kamu)
    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_petugas_id, 'INCOMING_STOCK', 'Menambah stok kopi ID ' || p_coffee_id || ' sebanyak ' || p_quantity || ' kg.');

END;
$$;

CREATE OR REPLACE PROCEDURE sp_add_outgoing_transaction(
    p_destination_id INT,
    p_coffee_id INT,
    p_quantity INT,
    p_petugas_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- 1. Validasi: Pastikan data ID yang dimasukkan ada di database
    IF NOT EXISTS (SELECT 1 FROM destinations WHERE destination_id = p_destination_id) THEN
        RAISE EXCEPTION 'Error: Destinasi dengan ID % tidak ditemukan.', p_destination_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM coffee_types WHERE coffee_id = p_coffee_id) THEN
        RAISE EXCEPTION 'Error: Jenis Kopi dengan ID % tidak ditemukan.', p_coffee_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_petugas_id) THEN
        RAISE EXCEPTION 'Error: Petugas dengan ID % tidak ditemukan.', p_petugas_id;
    END IF;

    -- 2. Lakukan INSERT
    -- Jika stok kurang, trigger `trg_outgoing_update_stock` milikmu akan otomatis membatalkan proses ini.
    INSERT INTO outgoing_transactions (destination_id, coffee_id, quantity, petugas_id)
    VALUES (p_destination_id, p_coffee_id, p_quantity, p_petugas_id);

    -- 3. Catat ke Log Aktivitas
    INSERT INTO activity_logs (user_id, action, description)
    VALUES (p_petugas_id, 'OUTGOING_STOCK', 'Mengeluarkan stok kopi ID ' || p_coffee_id || ' sebanyak ' || p_quantity || ' kg.');

END;
$$;
----------------------------------------------------------------------

----------------------------------------------------------------------
-- View Ringkasan Stok
CREATE VIEW stock_summary AS
SELECT
    ct.coffee_name,
    cc.category_name,
    COALESCE(s.current_quantity, 0)  AS current_quantity,
    ct.minimum_stock,
    CASE
        WHEN COALESCE(s.current_quantity, 0) < ct.minimum_stock THEN 'LOW'
        ELSE 'SAFE'
    END AS status
FROM coffee_types ct
LEFT JOIN coffee_categories cc ON ct.category_id = cc.category_id
LEFT JOIN stock s              ON ct.coffee_id   = s.coffee_id
WHERE ct.is_active = TRUE;
----------------------------------------------------------------------

----------------------------------------------------------------------
-- 1. Isi Roles & Users
INSERT INTO roles (role_name, description) VALUES ('Admin', 'Kepala Gudang');
INSERT INTO users (username, password, role_id) VALUES ('budi_gudang', 'password123', 1);

-- 2. Isi Suppliers & Destinations
INSERT INTO suppliers (company_name, address, phone) VALUES ('Koperasi Tani Gayo', 'Aceh', '0812345');
INSERT INTO destinations (destination_name, address) VALUES ('Cafe Kenangan Jakarta', 'Jakarta');

-- 3. Isi Kategori & Jenis Kopi
INSERT INTO coffee_categories (category_name, description) VALUES ('Arabika', 'Kopi dataran tinggi');
INSERT INTO coffee_types (coffee_name, category_id) VALUES ('Arabica Gayo Clean Wash', 1);

-- Format: CALL sp_add_incoming_transaction(supplier_id, coffee_id, quantity, petugas_id);
CALL sp_add_incoming_transaction(1, 1, 150, 1);

-- Format: CALL sp_add_outgoing_transaction(destination_id, coffee_id, quantity, petugas_id);
CALL sp_add_outgoing_transaction(1, 1, 50, 1);
----------------------------------------------------------------------

----------------------------------------------------------------------
SELECT * FROM stock_summary;
SELECT * FROM activity_logs;
----------------------------------------------------------------------





