-- ============================================================================
-- SCRIPT PEMBERSIHAN TOTAL (DROP SCRIPT) — CoffeeWMS
-- Jalankan file ini untuk menghapus seluruh objek database secara aman.
-- Urutan penghapusan disesuaikan berdasarkan dependensi (Foreign Keys & Views).
-- ============================================================================

-- ============================================================================
-- SECTION 1: DROP TRIGGERS
-- (Harus dihapus terlebih dahulu sebelum menghapus tabel atau fungsi terkait)
-- ============================================================================
DROP TRIGGER IF EXISTS trg_incoming_update_stock ON incoming_transactions;
DROP TRIGGER IF EXISTS trg_outgoing_update_stock ON outgoing_transactions;
DROP TRIGGER IF EXISTS trg_log_incoming_transaction ON incoming_transactions;
DROP TRIGGER IF EXISTS trg_log_outgoing_transaction ON outgoing_transactions;

-- ============================================================================
-- SECTION 2: DROP VIEWS
-- (Harus dihapus sebelum tabel induknya dihapus)
-- ============================================================================
DROP VIEW IF EXISTS vw_dashboard_summary;
DROP VIEW IF EXISTS vw_outgoing_transactions;
DROP VIEW IF EXISTS vw_incoming_transactions;
DROP VIEW IF EXISTS vw_coffee_types;
DROP VIEW IF EXISTS vw_destinations;
DROP VIEW IF EXISTS vw_suppliers;
DROP VIEW IF EXISTS vw_logs;
DROP VIEW IF EXISTS vw_stock_summary; -- <-- Tambahkan baris ini agar kehapus dengan benar
DROP VIEW IF EXISTS stock_summary;    -- Tetap biarkan ini untuk jaga-jaga versi lama
DROP VIEW IF EXISTS vw_user_roles;

-- ============================================================================
-- SECTION 3: DROP STORED PROCEDURES
-- ============================================================================
-- Manajemen Pengguna (User Management)
DROP PROCEDURE IF EXISTS sp_add_user(VARCHAR, VARCHAR, INT[]); -- Signature lama
DROP PROCEDURE IF EXISTS sp_add_user(INT, VARCHAR, VARCHAR, INT[]); -- Signature baru
DROP PROCEDURE IF EXISTS sp_update_user(INT, INT, VARCHAR, VARCHAR, BOOLEAN, INT[]);
DROP PROCEDURE IF EXISTS sp_soft_delete_user(INT, INT);

-- Manajemen Data Master (Master Data Management)
DROP PROCEDURE IF EXISTS sp_add_supplier(INT, VARCHAR, TEXT, VARCHAR);
DROP PROCEDURE IF EXISTS sp_soft_delete_supplier(INT, INT);
DROP PROCEDURE IF EXISTS sp_add_destination(INT, VARCHAR, TEXT);
DROP PROCEDURE IF EXISTS sp_soft_delete_destination(INT, INT);
DROP PROCEDURE IF EXISTS sp_add_coffee_type(INT, VARCHAR, INT, INT);
DROP PROCEDURE IF EXISTS sp_soft_delete_coffee_type(INT, INT);

-- Transaksi (Transactions)
DROP PROCEDURE IF EXISTS sp_add_incoming_transaction(INT, INT, INT, INT);
DROP PROCEDURE IF EXISTS sp_add_outgoing_transaction(INT, INT, INT, INT);

-- ============================================================================
-- SECTION 4: DROP FUNCTIONS (Trigger & Utility Functions)
-- ============================================================================
-- Fungsi Trigger
DROP FUNCTION IF EXISTS fn_update_stock_on_incoming();
DROP FUNCTION IF EXISTS fn_update_stock_on_outgoing();
DROP FUNCTION IF EXISTS fn_log_incoming_transaction();
DROP FUNCTION IF EXISTS fn_log_outgoing_transaction();

-- Fungsi Utilitas
DROP FUNCTION IF EXISTS fn_get_stock_by_coffee(INT);
DROP FUNCTION IF EXISTS fn_is_stock_sufficient(INT, INT);
DROP FUNCTION IF EXISTS fn_get_low_stock_items();

-- ============================================================================
-- SECTION 5: DROP TABLES
-- (Dihapus dari tabel anak/junction terlebih dahulu untuk menghindari error Foreign Key)
-- ============================================================================
DROP TABLE IF EXISTS activity_logs;
DROP TABLE IF EXISTS outgoing_transactions;
DROP TABLE IF EXISTS incoming_transactions;
DROP TABLE IF EXISTS stock;
DROP TABLE IF EXISTS coffee_origins;
DROP TABLE IF EXISTS user_roles;
DROP TABLE IF EXISTS coffee_types;
DROP TABLE IF EXISTS coffee_categories;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS destinations;
DROP TABLE IF EXISTS suppliers;

-- ============================================================================
-- SELESAI — Database bersih total dan siap untuk dipasang ulang (re-run schema)
-- ============================================================================
