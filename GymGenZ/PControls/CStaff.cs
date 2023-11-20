using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymGenZ.PControls
{
    internal class CStaff
    {
        private SQLiteConnection _conn;

        public CStaff(string connectionString)
        {
            _conn = new SQLiteConnection(connectionString);
            _conn.Open();
        }

        public bool Login(string username, string password)
        {
            using (SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Staff WHERE username = @username AND password = @password", _conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@username", username);
                da.SelectCommand.Parameters.AddWithValue("@password", password);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt.Rows.Count > 0;
            }
        }

        public bool CheckUSer(string username, string numberPhone)
        {
            using (SQLiteConnection con = new SQLiteConnection(_conn))
            {
                string query = "SELECT * FROM Staff WHERE username = @username AND numberPhone = @numberPhone";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@numberPhone", numberPhone);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
        }

        public bool ChangePassword(string username, string newPassword)
        {
            using (SQLiteConnection con = new SQLiteConnection(_conn))
            {
                string updateQuery = "UPDATE Staff SET password = @newPassword WHERE username = @username";
                using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, con))
                {
                    updateCmd.Parameters.AddWithValue("@newPassword", newPassword);
                    updateCmd.Parameters.AddWithValue("@username", username);
                    updateCmd.ExecuteNonQuery();
                    return true;
                }
            }
        }
    }
}
