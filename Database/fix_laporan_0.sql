-- Karena sebelumnya kita menggunakan DROP VIEW IF EXISTS stock_summary CASCADE;
-- maka view vw_dashboard_summary ikut terhapus otomatis oleh database.
-- Mari kita buat kembali vw_dashboard_summary menggunakan COUNT(*) sesuai permintaan "berapa kali transaksi"

CREATE OR REPLACE VIEW vw_dashboard_summary AS
SELECT
    (SELECT COUNT(*) FROM coffee_types WHERE is_active = true)    AS total_coffee_types,
    (SELECT COUNT(*) FROM suppliers WHERE is_active = true)       AS total_suppliers,
    (SELECT COUNT(*) FROM destinations WHERE is_active = true)    AS total_destinations,
    (SELECT COUNT(*) FROM incoming_transactions)                  AS total_incoming,
    (SELECT COUNT(*) FROM outgoing_transactions)                  AS total_outgoing,
    (SELECT COUNT(*) FROM stock_summary WHERE status = 'LOW')     AS total_low_stock;
