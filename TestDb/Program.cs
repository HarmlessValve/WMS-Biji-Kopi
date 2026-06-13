using System;
using Npgsql;

class Program {
    static void Main() {
        try {
            string connStr = "Host=localhost;Port=5432;Database=prakasli;Username=postgres;Password=supercoy";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();
            string sql1 = @"
DROP FUNCTION IF EXISTS fn_get_laporan_penerimaan(DATE, DATE);
CREATE OR REPLACE FUNCTION fn_get_laporan_penerimaan(p_start_date DATE, p_end_date DATE)
RETURNS TABLE (
    ""Tanggal"" TIMESTAMP,
    ""Supplier"" VARCHAR,
    ""JenisKopi"" TEXT,
    ""Jumlah"" INT,
    ""Petugas"" VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tanggal AS ""Tanggal"",
        supplier::VARCHAR AS ""Supplier"",
        jenis_kopi::TEXT AS ""JenisKopi"",
        jumlah AS ""Jumlah"",
        petugas::VARCHAR AS ""Petugas""
    FROM vw_incoming_transactions
    WHERE tanggal >= p_start_date AND tanggal < p_end_date
    ORDER BY tanggal DESC;
END;
$$ LANGUAGE plpgsql;
";
            string sql2 = @"
DROP FUNCTION IF EXISTS fn_get_laporan_pengiriman(DATE, DATE);
CREATE OR REPLACE FUNCTION fn_get_laporan_pengiriman(p_start_date DATE, p_end_date DATE)
RETURNS TABLE (
    ""Tanggal"" TIMESTAMP,
    ""Destinasi"" VARCHAR,
    ""JenisKopi"" TEXT,
    ""Jumlah"" INT,
    ""Petugas"" VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tanggal AS ""Tanggal"",
        destinasi::VARCHAR AS ""Destinasi"",
        jenis_kopi::TEXT AS ""JenisKopi"",
        jumlah AS ""Jumlah"",
        petugas::VARCHAR AS ""Petugas""
    FROM vw_outgoing_transactions
    WHERE tanggal >= p_start_date AND tanggal < p_end_date
    ORDER BY tanggal DESC;
END;
$$ LANGUAGE plpgsql;
";
            using (var cmd = new NpgsqlCommand(sql1, conn)) { cmd.ExecuteNonQuery(); }
            using (var cmd = new NpgsqlCommand(sql2, conn)) { cmd.ExecuteNonQuery(); }
            Console.WriteLine("Functions updated successfully.");
            
            using (var cmd = new NpgsqlCommand("SELECT * FROM fn_get_laporan_penerimaan('2026-06-12'::date, '2026-06-15'::date);", conn)) {
                using var reader = cmd.ExecuteReader();
                int count = 0;
                while(reader.Read()) count++;
                Console.WriteLine("Rows found: " + count);
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.ToString());
        }
    }
}
