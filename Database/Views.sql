-- /Database/Views.sql

-- 1. View untuk Log Aktivitas
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

-- 2. View untuk Suppliers
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

-- 3. View untuk Destinations
CREATE OR REPLACE VIEW vw_destinations AS
SELECT 
    destination_id,
    destination_name,
    address,
    is_active
FROM destinations
WHERE is_active = true
ORDER BY destination_name;
