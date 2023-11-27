using GymGenZ.PModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;

namespace GymGenZ.PControls
{
    internal class CCustomer
    {
        private string _connectionString;
        private SQLiteConnection _conn;

        public CCustomer(string connectionString)
        {
            _connectionString = connectionString;
            _conn = new SQLiteConnection(_connectionString);
        }

        public List<MCustomer> SearchCustomers(string searchText)
        {
            List<MCustomer> customers = new List<MCustomer>();

            using (SQLiteConnection con = new SQLiteConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT Customer.id AS CustomerID, Customer.name AS CustomerName, " +
                               "Customer.phone AS PhoneNumber, Customer.cccd AS CCCD, " +
                               "Customer.start AS Start, Customer.end AS End " +
                               "FROM Customer " +
                               "WHERE Customer.name LIKE @searchText OR " +
                               "Customer.phone LIKE @searchText OR Customer.cccd LIKE @searchText";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MCustomer customer = new MCustomer
                            {
                                CustomerID = reader["CustomerID"].ToString(),
                                CustomerName = reader["CustomerName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                CCCD = reader["CCCD"].ToString(),
                                Start = reader["Start"].ToString(),
                                End = reader["End"].ToString()
                            };

                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }

        public bool signCustomer(string name, string phone, string cccd, string packageID)
        {
            try
            {
                _conn.Open();

                string selectPackageQuery = "SELECT * FROM Package WHERE id = @packageID";
                using (SQLiteCommand selectPackageCmd = new SQLiteCommand(selectPackageQuery, _conn))
                {
                    selectPackageCmd.Parameters.AddWithValue("@packageID", packageID);

                    using (SQLiteDataReader packageReader = selectPackageCmd.ExecuteReader())
                    {
                        if (packageReader.Read())
                        {
                            int packageId = packageReader.GetInt32(packageReader.GetOrdinal("id"));
                            string packageName = packageReader.GetString(packageReader.GetOrdinal("name"));
                            int packageTime = packageReader.GetInt32(packageReader.GetOrdinal("time"));
                            DateTime currentDate = DateTime.Now;
                            DateTime expirationDate = currentDate.AddDays(packageTime);

                            string insertCustomerQuery = "INSERT INTO Customer (name, phone, cccd, idPackage, start, end) " +
                                                        "VALUES (@Name, @Phone, @CCCD, @packageID, @registrationDate, @expirationDate)";
                            using (SQLiteCommand insertCustomerCmd = new SQLiteCommand(insertCustomerQuery, _conn))
                            {
                                insertCustomerCmd.Parameters.AddWithValue("@Name", name);
                                insertCustomerCmd.Parameters.AddWithValue("@Phone", phone);
                                insertCustomerCmd.Parameters.AddWithValue("@CCCD", cccd);
                                insertCustomerCmd.Parameters.AddWithValue("@packageID", packageID);
                                insertCustomerCmd.Parameters.AddWithValue("@registrationDate", currentDate.ToString("yyyy-MM-dd"));
                                insertCustomerCmd.Parameters.AddWithValue("@expirationDate", expirationDate.ToString("yyyy-MM-dd"));

                                insertCustomerCmd.ExecuteNonQuery();
                                return true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy thông tin gói tập.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                return false;
            }
            finally
            {
                _conn.Close();
            }
        }
    }
}
