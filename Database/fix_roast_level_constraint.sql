-- Hapus constraint lama yang membatasi kombinasi Jenis Kopi, Kategori, dan Asal (Tanpa Roast Level)
ALTER TABLE coffee_products 
DROP CONSTRAINT IF EXISTS coffee_products_coffee_id_category_id_origin_id_key;

-- Buat Unique Index baru yang juga mempertimbangkan roast_level_id
-- Menggunakan COALESCE agar nilai NULL tetap bisa dianggap sebagai nilai unik (0) saat mengecek duplikasi
CREATE UNIQUE INDEX IF NOT EXISTS idx_coffee_products_unique_with_roast 
ON coffee_products (
    coffee_id, 
    COALESCE(category_id, 0), 
    COALESCE(origin_id, 0), 
    COALESCE(roast_level_id, 0)
);

-- Update view stock_summary agar memuat informasi Roast Level
DROP VIEW IF EXISTS stock_summary CASCADE;
CREATE OR REPLACE VIEW stock_summary AS
SELECT
    cp.product_id,
    cp.coffee_id,
    ct.coffee_name,
    cc.category_name,
    co.origin_name,
    rl.roast_level_name,
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
LEFT JOIN roast_levels rl      ON cp.roast_level_id = rl.roast_level_id
WHERE ct.is_active = TRUE;

-- Update view vw_coffee_products agar memuat informasi Roast Level secara konsisten
CREATE OR REPLACE VIEW vw_coffee_products AS
SELECT 
    cp.product_id,
    cp.coffee_id,
    ct.coffee_name,
    cp.category_id,
    COALESCE(cc.category_name, 'Tanpa Kategori') AS category_name,
    cp.origin_id,
    COALESCE(co.origin_name, 'Tanpa Origin') AS origin_name,
    cp.roast_level_id,
    COALESCE(rl.roast_level_name, '-') AS roast_level_name,
    COALESCE(cp.current_quantity, 0) AS current_quantity,
    cp.minimum_stock,
    ct.is_active
FROM coffee_products cp
LEFT JOIN coffee_types ct      ON cp.coffee_id = ct.coffee_id
LEFT JOIN coffee_categories cc ON cp.category_id = cc.category_id
LEFT JOIN coffee_origins co    ON cp.origin_id = co.origin_id
LEFT JOIN roast_levels rl      ON cp.roast_level_id = rl.roast_level_id
WHERE ct.is_active = true AND cp.is_active = true
ORDER BY ct.coffee_name;
