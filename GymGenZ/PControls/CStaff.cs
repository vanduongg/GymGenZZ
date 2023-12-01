using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymGenZ.PModels;
using System.Data.SqlClient;
using System.Windows.Forms;

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

        public List<MStaff> ShowAvailableStaff(string shiftCode, string date)
{
            List<MStaff> staffList = new List<MStaff>();

            using (SQLiteConnection con = new SQLiteConnection(_conn))
            { 
                string query = "SELECT DISTINCT s.fullName AS FullName, s.numberPhone AS NumberPhone " +
                               "FROM Staff s " +
                               "WHERE NOT EXISTS (" +
                               "    SELECT 1 FROM TrainingSessions t " +
                               "    WHERE t.trainerID = s.id " +
                               $"      AND (t.ShiftCode = '{shiftCode}' AND t.Date = '{date}')" +
                               ")";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MStaff staff = new MStaff
                            {
                                fullname = reader["FullName"].ToString(),
                                numberPhone = reader["NumberPhone"].ToString()
                            };

                            staffList.Add(staff);
                        }
                    }
                }
            }

            return staffList;
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
