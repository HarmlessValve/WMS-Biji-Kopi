-- /Database/UserManagement.sql

-- 1. Pastikan roles tersedia
INSERT INTO roles (role_name, description) VALUES ('Admin', 'Sistem Administrator') ON CONFLICT (role_name) DO NOTHING;
INSERT INTO roles (role_name, description) VALUES ('Manager', 'Manager Gudang') ON CONFLICT (role_name) DO NOTHING;
INSERT INTO roles (role_name, description) VALUES ('Petugas', 'Petugas Input') ON CONFLICT (role_name) DO NOTHING;

-- 2. View user_roles untuk melihat daftar user beserta rolenya dalam satu string
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

-- 3. Stored Procedure: Tambah Pengguna beserta Roles
-- p_roles_array dipassing sebagai ID role list (contoh: '{1,2}')
CREATE OR REPLACE PROCEDURE sp_add_user(
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
    -- Insert dan ambil ID user yang terbuat
    INSERT INTO users (username, password, is_active)
    VALUES (p_username, p_password, true)
    RETURNING user_id INTO new_user_id;

    -- Insert ke tabel user_roles
    IF p_roles_array IS NOT NULL THEN
        FOREACH r_id IN ARRAY p_roles_array
        LOOP
            INSERT INTO user_roles (user_id, role_id) VALUES (new_user_id, r_id);
        END LOOP;
    END IF;
    
    -- Catat di activity_logs (System Action, non-user spesific)
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (NULL, 'CREATE_USER', 'Created user: ' || p_username);
END;
$$;

-- 4. Stored Procedure: Update Pengguna dan Roles
CREATE OR REPLACE PROCEDURE sp_update_user(
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
    -- Update User Info
    IF p_password = '' OR p_password IS NULL THEN
        UPDATE users 
        SET username = p_username, is_active = p_is_active
        WHERE user_id = p_user_id;
    ELSE
        UPDATE users 
        SET username = p_username, password = p_password, is_active = p_is_active
        WHERE user_id = p_user_id;
    END IF;

    -- Hapus relasi role lama (karena ON DELETE CASCADE tidak aktif untuk UPDATE ID)
    DELETE FROM user_roles WHERE user_id = p_user_id;

    -- Masukkan relasi role baru
    IF p_roles_array IS NOT NULL THEN
        FOREACH r_id IN ARRAY p_roles_array
        LOOP
            INSERT INTO user_roles (user_id, role_id) VALUES (p_user_id, r_id);
        END LOOP;
    END IF;
    
    -- Catat di activity_logs
    INSERT INTO activity_logs (user_id, action, description) 
    VALUES (NULL, 'UPDATE_USER', 'Updated user_id: ' || p_user_id);
END;
$$;
