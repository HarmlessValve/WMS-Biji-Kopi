using System;
using Npgsql;
using CoffeeWMS.Data;

namespace CoffeeWMS.Repositories
{
    public class TransactionRepository
    {
        public void AddIncomingTransaction(int supplierId, int coffeeId, int quantity, int petugasId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_add_incoming_transaction(@s_id, @c_id, @qty, @p_id)", conn))
                {
                    cmd.Parameters.AddWithValue("s_id", supplierId);
                    cmd.Parameters.AddWithValue("c_id", coffeeId);
                    cmd.Parameters.AddWithValue("qty", quantity);
                    cmd.Parameters.AddWithValue("p_id", petugasId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
