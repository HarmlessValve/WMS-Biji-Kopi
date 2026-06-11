-- 1a. Fungsi: Update stok otomatis saat transaksi masuk (incoming)
CREATE OR REPLACE FUNCTION fn_update_stock_on_incoming()
RETURNS TRIGGER AS $$
BEGIN
    -- Tambahkan quantity ke stok di coffee_products
    UPDATE coffee_products
    SET current_quantity = current_quantity + NEW.quantity,
        last_updated     = CURRENT_TIMESTAMP
    WHERE product_id = NEW.product_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 1b. Fungsi: Update stok otomatis saat transaksi keluar (outgoing)
CREATE OR REPLACE FUNCTION fn_update_stock_on_outgoing()
RETURNS TRIGGER AS $$
DECLARE
    v_current INTEGER;
BEGIN
    -- Ambil stok saat ini dengan row-level lock
    SELECT current_quantity INTO v_current
    FROM coffee_products
    WHERE product_id = NEW.product_id
    FOR UPDATE;

    -- Jika stok tidak cukup, batalkan transaksi
    IF v_current IS NULL OR v_current < NEW.quantity THEN
        RAISE EXCEPTION 'Stok tidak cukup. Stok tersedia: %, diminta: %',
            COALESCE(v_current, 0), NEW.quantity;
    END IF;

    -- Kurangi stok
    UPDATE coffee_products
    SET current_quantity = current_quantity - NEW.quantity,
        last_updated     = CURRENT_TIMESTAMP
    WHERE product_id = NEW.product_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 1c. Fungsi: Auto-log saat transaksi masuk
CREATE OR REPLACE FUNCTION fn_log_incoming_transaction()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO activity_logs (user_id, action, description)
    VALUES (
        NEW.petugas_id,
        'INCOMING_STOCK',
        'Auto-log: Penerimaan produk ID ' || NEW.product_id || ' sebanyak ' || NEW.quantity || ' kg dari supplier ID ' || COALESCE(NEW.supplier_id::TEXT, 'N/A')
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 1d. Fungsi: Auto-log saat transaksi keluar
CREATE OR REPLACE FUNCTION fn_log_outgoing_transaction()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO activity_logs (user_id, action, description)
    VALUES (
        NEW.petugas_id,
        'OUTGOING_STOCK',
        'Auto-log: Pengiriman produk ID ' || NEW.product_id || ' sebanyak ' || NEW.quantity || ' kg ke destinasi ID ' || COALESCE(NEW.destination_id::TEXT, 'N/A')
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- ============================================================================
-- SECTION 2: TRIGGERS
-- Trigger yang memanggil fungsi-fungsi di atas secara otomatis
-- ============================================================================

-- 2a. Trigger: Update stok saat ada penerimaan baru
CREATE OR REPLACE TRIGGER trg_incoming_update_stock
AFTER INSERT ON incoming_transactions
FOR EACH ROW EXECUTE FUNCTION fn_update_stock_on_incoming();

-- 2b. Trigger: Cek & kurangi stok saat ada pengiriman baru
CREATE OR REPLACE TRIGGER trg_outgoing_update_stock
BEFORE INSERT ON outgoing_transactions
FOR EACH ROW EXECUTE FUNCTION fn_update_stock_on_outgoing();

-- 2c. Trigger: Auto-log setiap penerimaan baru
CREATE OR REPLACE TRIGGER trg_log_incoming_transaction
AFTER INSERT ON incoming_transactions
FOR EACH ROW EXECUTE FUNCTION fn_log_incoming_transaction();

-- 2d. Trigger: Auto-log setiap pengiriman baru
CREATE OR REPLACE TRIGGER trg_log_outgoing_transaction
AFTER INSERT ON outgoing_transactions
FOR EACH ROW EXECUTE FUNCTION fn_log_outgoing_transaction();


-- ============================================================================
-- SECTION 3: VIEWS
-- View untuk query data gabungan yang sering digunakan oleh aplikasi
-- ============================================================================

-- 3a. View: Daftar user beserta role-nya (string aggregation)
CREATE OR REPLACE VIEW vw_user_roles AS
SELECT 
    u.user_id,
    u.username,
    u.is_active,
    u.created_at,
    STRING_AGG(r.role_name, ', ') AS roles_string
FROM users u
LEFT JOIN user_roles ur ON u.user_id = ur.user_id
LEFT JOIN roles r ON ur.role_id = r.role_id
GROUP BY u.user_id, u.username, u.is_active, u.created_at;

-- 3b. View: Ringkasan stok per produk kopi
CREATE OR REPLACE VIEW stock_summary AS
SELECT
    cp.product_id,
    ct.coffee_name,
    cc.category_name,
    co.origin_name,
    COALESCE(cp.current_quantity, 0)  AS current_quantity,
    cp.minimum_stock,
    CASE
        WHEN COALESCE(cp.current_quantity, 0) < cp.minimum_stock THEN 'LOW'
        ELSE 'SAFE'
    END AS status
FROM coffee_products cp
LEFT JOIN coffee_types ct      ON cp.coffee_id = ct.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id = cc.category_id
LEFT JOIN coffee_origins co    ON cp.origin_id = co.origin_id
WHERE ct.is_active = TRUE;

-- 3c. View: Log aktivitas (join ke username)
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

-- 3d. View: Supplier aktif
CREATE OR REPLACE VIEW vw_suppliers AS
SELECT 
    supplier_id,
    company_name,
    address,
    phone,
    is_active
FROM suppliers
WHERE is_active = true
ORDER BY company_name;

-- 3e. View: Destinasi aktif
CREATE OR REPLACE VIEW vw_destinations AS
SELECT 
    destination_id,
    destination_name,
    address,
    is_active
FROM destinations
WHERE is_active = true
ORDER BY destination_name;

-- 3f. View: Produk kopi beserta kategori dan origin (untuk combobox & grid di aplikasi)
CREATE OR REPLACE VIEW vw_coffee_products AS
SELECT 
    cp.product_id,
    ct.coffee_name,
    cp.category_id,
    COALESCE(cc.category_name, 'Tanpa Kategori') AS category_name,
    cp.origin_id,
    COALESCE(co.origin_name, 'Tanpa Origin') AS origin_name,
    COALESCE(cp.current_quantity, 0) AS current_quantity,
    cp.minimum_stock,
    ct.is_active
FROM coffee_products cp
LEFT JOIN coffee_types ct      ON cp.coffee_id = ct.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id = cc.category_id
LEFT JOIN coffee_origins co    ON cp.origin_id = co.origin_id
WHERE ct.is_active = true AND cp.is_active = true
ORDER BY ct.coffee_name;

-- 3g. View: Transaksi masuk (join supplier + kopi + petugas)
CREATE OR REPLACE VIEW vw_incoming_transactions AS
SELECT 
    t.incoming_id,
    t.received_at        AS tanggal,
    COALESCE(s.company_name, 'N/A')  AS supplier,
    COALESCE(c.coffee_name, 'N/A') || ' - ' || COALESCE(cc.category_name, 'N/A') || ' - ' || COALESCE(co.origin_name, 'N/A') AS jenis_kopi,
    t.quantity            AS jumlah,
    COALESCE(u.username, 'N/A')      AS petugas
FROM incoming_transactions t
LEFT JOIN suppliers s        ON t.supplier_id = s.supplier_id
LEFT JOIN coffee_products cp ON t.product_id  = cp.product_id
LEFT JOIN coffee_types c     ON cp.coffee_id  = c.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id = cc.category_id
LEFT JOIN coffee_origins co    ON cp.origin_id = co.origin_id
LEFT JOIN users u            ON t.petugas_id  = u.user_id
ORDER BY t.received_at DESC;

-- 3h. View: Transaksi keluar (join destinasi + kopi + petugas)
CREATE OR REPLACE VIEW vw_outgoing_transactions AS
SELECT 
    t.outgoing_id,
    t.shipped_at          AS tanggal,
    COALESCE(d.destination_name, 'N/A') AS destinasi,
    COALESCE(c.coffee_name, 'N/A') || ' - ' || COALESCE(cc.category_name, 'N/A') || ' - ' || COALESCE(co.origin_name, 'N/A') AS jenis_kopi,
    t.quantity             AS jumlah,
    COALESCE(u.username, 'N/A')         AS petugas
FROM outgoing_transactions t
LEFT JOIN destinations d     ON t.destination_id = d.destination_id
LEFT JOIN coffee_products cp ON t.product_id     = cp.product_id
LEFT JOIN coffee_types c     ON cp.coffee_id     = c.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id = cc.category_id
LEFT JOIN coffee_origins co    ON cp.origin_id = co.origin_id
LEFT JOIN users u            ON t.petugas_id     = u.user_id
ORDER BY t.shipped_at DESC;

-- 3i. View: Dashboard summary (statistik ringkasan)
CREATE OR REPLACE VIEW vw_dashboard_summary AS
SELECT
    (SELECT COUNT(*) FROM coffee_types WHERE is_active = true)    AS total_coffee_types,
    (SELECT COUNT(*) FROM suppliers WHERE is_active = true)       AS total_suppliers,
    (SELECT COUNT(*) FROM destinations WHERE is_active = true)    AS total_destinations,
    (SELECT COUNT(*) FROM incoming_transactions)                  AS total_incoming,
    (SELECT COUNT(*) FROM outgoing_transactions)                  AS total_outgoing,
    (SELECT COUNT(*) FROM stock_summary WHERE status = 'LOW')     AS total_low_stock;


-- ============================================================================
-- SECTION 4: UTILITY FUNCTIONS
-- Fungsi bantuan yang bisa dipanggil dari aplikasi atau SP lain
-- ============================================================================

-- 4a. Fungsi: Ambil jumlah stok untuk product_id tertentu
CREATE OR REPLACE FUNCTION fn_get_stock_by_product(p_product_id INT)
RETURNS INTEGER AS $$
DECLARE
    v_qty INTEGER;
BEGIN
    SELECT current_quantity INTO v_qty
    FROM coffee_products
    WHERE product_id = p_product_id;
    
    RETURN COALESCE(v_qty, 0);
END;
$$ LANGUAGE plpgsql;

-- 4b. Fungsi: Cek apakah stok cukup untuk pengiriman
CREATE OR REPLACE FUNCTION fn_is_stock_sufficient(p_product_id INT, p_quantity INT)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN fn_get_stock_by_product(p_product_id) >= p_quantity;
END;
$$ LANGUAGE plpgsql;

-- 4c. Fungsi: Ambil daftar produk dengan stok rendah (di bawah minimum)
CREATE OR REPLACE FUNCTION fn_get_low_stock_items()
RETURNS TABLE (
    product_id     INT,
    coffee_name    VARCHAR,
    current_qty    INT,
    minimum_stock  INT
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
-- SECTION 5: STORED PROCEDURES — USER MANAGEMENT
-- Prosedur CRUD untuk manajemen pengguna dan role
-- ============================================================================

-- 5a. SP: Tambah Pengguna beserta Role-nya
CREATE OR REPLACE PROCEDURE sp_add_user(
    p_admin_id INT,
    p_username VARCHAR,
    p_password VARCHAR,
    p_roles_array INT[]
)
LANGUAGE plpgsql
AS $$
DECLARE
    new_user_id INT;
    r_id INT;
BEGIN
    INSERT INTO users (username, password, is_active)
    VALUES (p_username, p_password, true)
    RETURNING user_id INTO new_user_id;

    IF p_roles_array IS NOT NULL THEN
        FOREACH r_id IN ARRAY p_roles_array
        LOOP
            INSERT INTO user_roles (user_id, role_id) VALUES (new_user_id, r_id);
        END LOOP;
    END IF;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'CREATE_USER', 'Created user: ' || p_username);
END;
$$;

-- 5b. SP: Update Pengguna dan Role-nya
CREATE OR REPLACE PROCEDURE sp_update_user(
    p_admin_id INT,
    p_user_id INT,
    p_username VARCHAR,
    p_password VARCHAR,
    p_is_active BOOLEAN,
    p_roles_array INT[]
)
LANGUAGE plpgsql
AS $$
DECLARE
    r_id INT;
BEGIN
    IF p_password = '' OR p_password IS NULL THEN
        UPDATE users 
        SET username = p_username, is_active = p_is_active
        WHERE user_id = p_user_id;
    ELSE
        UPDATE users 
        SET username = p_username, password = p_password, is_active = p_is_active
        WHERE user_id = p_user_id;
    END IF;

    DELETE FROM user_roles WHERE user_id = p_user_id;

    IF p_roles_array IS NOT NULL THEN
        FOREACH r_id IN ARRAY p_roles_array
        LOOP
            INSERT INTO user_roles (user_id, role_id) VALUES (p_user_id, r_id);
        END LOOP;
    END IF;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'UPDATE_USER', 'Updated user_id: ' || p_user_id);
END;
$$;

-- 5c. SP: Soft Delete Pengguna (nonaktifkan)
CREATE OR REPLACE PROCEDURE sp_soft_delete_user(
    p_admin_id INT,
    p_user_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE users 
    SET is_active = FALSE
    WHERE user_id = p_user_id;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'SOFT_DELETE_USER', 'Deactivated user_id: ' || p_user_id);
END;
$$;


-- ============================================================================
-- SECTION 6: STORED PROCEDURES — MASTER DATA MANAGEMENT
-- Prosedur CRUD untuk supplier, destinasi, dan jenis kopi
-- ============================================================================

-- 6a. SP: Tambah Supplier
CREATE OR REPLACE PROCEDURE sp_add_supplier(
    p_admin_id INT,
    p_company_name VARCHAR,
    p_address TEXT,
    p_phone VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO suppliers (company_name, address, phone)
    VALUES (p_company_name, p_address, p_phone);
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'CREATE_SUPPLIER', 'Added supplier: ' || p_company_name);
END;
$$;

-- 6b. SP: Soft Delete Supplier
CREATE OR REPLACE PROCEDURE sp_soft_delete_supplier(
    p_admin_id INT,
    p_supplier_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE suppliers SET is_active = FALSE WHERE supplier_id = p_supplier_id;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'SOFT_DELETE_SUPPLIER', 'Deactivated supplier_id: ' || p_supplier_id);
END;
$$;

-- 6c. SP: Tambah Destinasi
CREATE OR REPLACE PROCEDURE sp_add_destination(
    p_admin_id INT,
    p_destination_name VARCHAR,
    p_address TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO destinations (destination_name, address)
    VALUES (p_destination_name, p_address);
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'CREATE_DESTINATION', 'Added destination: ' || p_destination_name);
END;
$$;

-- 6d. SP: Soft Delete Destinasi
CREATE OR REPLACE PROCEDURE sp_soft_delete_destination(
    p_admin_id INT,
    p_destination_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE destinations SET is_active = FALSE WHERE destination_id = p_destination_id;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'SOFT_DELETE_DESTINATION', 'Deactivated destination_id: ' || p_destination_id);
END;
$$;

-- 6e. SP: Tambah Jenis Kopi
CREATE OR REPLACE PROCEDURE sp_add_coffee_type(
    p_admin_id INT,
    p_coffee_name VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO coffee_types (coffee_name)
    VALUES (p_coffee_name);
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'CREATE_COFFEE_TYPE', 'Added coffee type: ' || p_coffee_name);
END;
$$;

-- 6g. SP: Tambah Produk Kopi
CREATE OR REPLACE PROCEDURE sp_add_coffee_product(
    p_admin_id INT,
    p_coffee_id INT,
    p_category_id INT,
    p_origin_id INT,
    p_minimum_stock INT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_product_id INT;
BEGIN
    -- Insert ke produk kopi
    INSERT INTO coffee_products (coffee_id, category_id, origin_id, minimum_stock)
    VALUES (p_coffee_id, p_category_id, p_origin_id, COALESCE(p_minimum_stock, 20))
    RETURNING product_id INTO v_product_id;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'CREATE_COFFEE_PRODUCT', 'Added coffee product with product_id: ' || v_product_id);
END;
$$;

-- 6h. SP: Soft Delete Produk Kopi
CREATE OR REPLACE PROCEDURE sp_soft_delete_coffee_product(
    p_admin_id INT,
    p_product_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE coffee_products SET is_active = FALSE WHERE product_id = p_product_id;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'SOFT_DELETE_COFFEE_PRODUCT', 'Deactivated product_id: ' || p_product_id);
END;
$$;

-- 6f. SP: Soft Delete Jenis Kopi
CREATE OR REPLACE PROCEDURE sp_soft_delete_coffee_type(
    p_admin_id INT,
    p_coffee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE coffee_types SET is_active = FALSE WHERE coffee_id = p_coffee_id;
    
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (p_admin_id, 'SOFT_DELETE_COFFEE_TYPE', 'Deactivated coffee_id: ' || p_coffee_id);
END;
$$;


-- ============================================================================
-- SECTION 7: STORED PROCEDURES — TRANSAKSI
-- Prosedur untuk mencatat penerimaan dan pengiriman kopi
-- ============================================================================

-- 7a. SP: Tambah Transaksi Masuk (Penerimaan)
-- Trigger trg_incoming_update_stock akan otomatis menambah stok
-- Trigger trg_log_incoming_transaction akan otomatis mencatat log
CREATE OR REPLACE PROCEDURE sp_add_incoming_transaction(
    p_supplier_id INT,
    p_product_id INT,
    p_quantity INT,
    p_petugas_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validasi: Pastikan data yang direferensikan ada
    IF NOT EXISTS (SELECT 1 FROM suppliers WHERE supplier_id = p_supplier_id AND is_active = true) THEN
        RAISE EXCEPTION 'Error: Supplier dengan ID % tidak ditemukan atau tidak aktif.', p_supplier_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM coffee_products WHERE product_id = p_product_id) THEN
        RAISE EXCEPTION 'Error: Produk Kopi dengan ID % tidak ditemukan.', p_product_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_petugas_id AND is_active = true) THEN
        RAISE EXCEPTION 'Error: Petugas dengan ID % tidak ditemukan atau tidak aktif.', p_petugas_id;
    END IF;

    -- Insert transaksi (trigger stok & log akan berjalan otomatis)
    INSERT INTO incoming_transactions (supplier_id, product_id, quantity, petugas_id)
    VALUES (p_supplier_id, p_product_id, p_quantity, p_petugas_id);
END;
$$;

-- 7b. SP: Tambah Transaksi Keluar (Pengiriman)
-- Trigger trg_outgoing_update_stock akan otomatis mengecek & mengurangi stok
-- Trigger trg_log_outgoing_transaction akan otomatis mencatat log
CREATE OR REPLACE PROCEDURE sp_add_outgoing_transaction(
    p_destination_id INT,
    p_product_id INT,
    p_quantity INT,
    p_petugas_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validasi: Pastikan data yang direferensikan ada
    IF NOT EXISTS (SELECT 1 FROM destinations WHERE destination_id = p_destination_id AND is_active = true) THEN
        RAISE EXCEPTION 'Error: Destinasi dengan ID % tidak ditemukan atau tidak aktif.', p_destination_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM coffee_products WHERE product_id = p_product_id) THEN
        RAISE EXCEPTION 'Error: Produk Kopi dengan ID % tidak ditemukan.', p_product_id;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_petugas_id AND is_active = true) THEN
        RAISE EXCEPTION 'Error: Petugas dengan ID % tidak ditemukan atau tidak aktif.', p_petugas_id;
    END IF;

    -- Insert transaksi (trigger stok & log akan berjalan otomatis)
    INSERT INTO outgoing_transactions (destination_id, product_id, quantity, petugas_id)
    VALUES (p_destination_id, p_product_id, p_quantity, p_petugas_id);
END;
$$;


-- ============================================================================
-- SECTION 8: SEED DATA (PERBAIKAN KATEGORI)
-- Data awal yang disesuaikan dengan kategori tingkat sangrai baru
-- ============================================================================

-- 8a. Roles default
INSERT INTO roles (role_name, description) VALUES ('Admin', 'Sistem Administrator / Kepala Gudang') ON CONFLICT (role_name) DO NOTHING;
INSERT INTO roles (role_name, description) VALUES ('Manager', 'Manager Gudang')                     ON CONFLICT (role_name) DO NOTHING;
INSERT INTO roles (role_name, description) VALUES ('Petugas', 'Petugas Input Data')                 ON CONFLICT (role_name) DO NOTHING;

-- 8b. Kategori kopi default (Tingkat Sangrai)
INSERT INTO coffee_categories (category_name, description) VALUES ('Greenbeans', 'Kopi yang belum di sangrai') ON CONFLICT (category_name) DO NOTHING;
INSERT INTO coffee_categories (category_name, description) VALUES ('Light Roast', 'Kopi yang di sangrai dengan suhu rendah') ON CONFLICT (category_name) DO NOTHING;
INSERT INTO coffee_categories (category_name, description) VALUES ('Medium Roast', 'Kopi yang di sangrai dengan suhu sedang') ON CONFLICT (category_name) DO NOTHING;
INSERT INTO coffee_categories (category_name, description) VALUES ('Dark Roast', 'Kopi yang di sangrai dengan suhu tinggi') ON CONFLICT (category_name) DO NOTHING;

-- 8c. Akun Admin Default & Mapping Role
INSERT INTO users (username, password, is_active) 
VALUES ('admin', 'admin123', true)
ON CONFLICT (username) DO NOTHING;

INSERT INTO user_roles (user_id, role_id)
SELECT u.user_id, r.role_id 
FROM users u, roles r 
WHERE u.username = 'admin' AND r.role_name = 'Admin'
ON CONFLICT (user_id, role_id) DO NOTHING;

INSERT INTO activity_logs (user_id, action, description)
SELECT user_id, 'SYSTEM_SEED', 'System generated initial Admin account'
FROM users WHERE username = 'admin';

-- 8d. Jenis kopi default (coffee_types)
INSERT INTO coffee_types (coffee_name)
SELECT 'Gayo' WHERE NOT EXISTS (SELECT 1 FROM coffee_types WHERE coffee_name = 'Gayo');
INSERT INTO coffee_types (coffee_name)
SELECT 'Toraja' WHERE NOT EXISTS (SELECT 1 FROM coffee_types WHERE coffee_name = 'Toraja');
INSERT INTO coffee_types (coffee_name)
SELECT 'Lampung' WHERE NOT EXISTS (SELECT 1 FROM coffee_types WHERE coffee_name = 'Lampung');

-- 8e. Asal daerah default (coffee_origins)
INSERT INTO coffee_origins (origin_name, region, description)
SELECT 'Aceh Tengah', 'Sumatera', 'Dataran Tinggi Gayo' WHERE NOT EXISTS (SELECT 1 FROM coffee_origins WHERE origin_name = 'Aceh Tengah');
INSERT INTO coffee_origins (origin_name, region, description)
SELECT 'Tana Toraja', 'Sulawesi', 'Pegunungan Toraja' WHERE NOT EXISTS (SELECT 1 FROM coffee_origins WHERE origin_name = 'Tana Toraja');
INSERT INTO coffee_origins (origin_name, region, description)
SELECT 'Tanggamus', 'Sumatera', 'Dataran Rendah Lampung' WHERE NOT EXISTS (SELECT 1 FROM coffee_origins WHERE origin_name = 'Tanggamus');

-- ============================================================================
-- 8f. Produk kopi gabungan (coffee_products)
-- Menghubungkan Jenis Kopi + Kategori Roast Baru + Asal Daerah
-- ============================================================================

-- Menghubungkan Gayo (Greenbeans) + Aceh Tengah
INSERT INTO coffee_products (coffee_id, category_id, origin_id, minimum_stock)
SELECT 
    (SELECT coffee_id FROM coffee_types WHERE coffee_name = 'Gayo' LIMIT 1),
    (SELECT category_id FROM coffee_categories WHERE category_name = 'Greenbeans' LIMIT 1), -- Diubah ke Greenbeans
    (SELECT origin_id FROM coffee_origins WHERE origin_name = 'Aceh Tengah' LIMIT 1),
    20
WHERE NOT EXISTS (
    SELECT 1 FROM coffee_products cp 
    JOIN coffee_types ct ON cp.coffee_id = ct.coffee_id 
    JOIN coffee_categories cc ON cp.category_id = cc.category_id
    JOIN coffee_origins co ON cp.origin_id = co.origin_id 
    WHERE ct.coffee_name = 'Gayo' 
      AND cc.category_name = 'Greenbeans'
      AND co.origin_name = 'Aceh Tengah'
);

-- Menghubungkan Toraja (Medium Roast) + Tana Toraja
INSERT INTO coffee_products (coffee_id, category_id, origin_id, minimum_stock)
SELECT 
    (SELECT coffee_id FROM coffee_types WHERE coffee_name = 'Toraja' LIMIT 1),
    (SELECT category_id FROM coffee_categories WHERE category_name = 'Medium Roast' LIMIT 1), -- Diubah ke Medium Roast
    (SELECT origin_id FROM coffee_origins WHERE origin_name = 'Tana Toraja' LIMIT 1),
    15
WHERE NOT EXISTS (
    SELECT 1 FROM coffee_products cp 
    JOIN coffee_types ct ON cp.coffee_id = ct.coffee_id 
    JOIN coffee_categories cc ON cp.category_id = cc.category_id
    JOIN coffee_origins co ON cp.origin_id = co.origin_id 
    WHERE ct.coffee_name = 'Toraja' 
      AND cc.category_name = 'Medium Roast'
      AND co.origin_name = 'Tana Toraja'
);

-- Menghubungkan Lampung (Dark Roast) + Tanggamus
INSERT INTO coffee_products (coffee_id, category_id, origin_id, minimum_stock)
SELECT 
    (SELECT coffee_id FROM coffee_types WHERE coffee_name = 'Lampung' LIMIT 1),
    (SELECT category_id FROM coffee_categories WHERE category_name = 'Dark Roast' LIMIT 1), -- Diubah ke Dark Roast
    (SELECT origin_id FROM coffee_origins WHERE origin_name = 'Tanggamus' LIMIT 1),
    30
WHERE NOT EXISTS (
    SELECT 1 FROM coffee_products cp 
    JOIN coffee_types ct ON cp.coffee_id = ct.coffee_id 
    JOIN coffee_categories cc ON cp.category_id = cc.category_id
    JOIN coffee_origins co ON cp.origin_id = co.origin_id 
    WHERE ct.coffee_name = 'Lampung' 
      AND cc.category_name = 'Dark Roast'
      AND co.origin_name = 'Tanggamus'
);