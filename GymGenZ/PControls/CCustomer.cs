﻿using GymGenZ.PModels;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GymGenZ.PControls
{
    internal class CCustomer
    {
        private string _connectionString;

        public CCustomer(string connectionString)
        {
            _connectionString = connectionString;
        }

        //private void SignPackage(string idCustomer, string caTap, string idPakage)
        //{
        //    using (SQLiteConnection con = new SQLiteConnection(_connectionString))
        //    {
        //        string updateQuery = "UPDATE Customer SET ";
        //        using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, con))
        //        {
        //            updateCmd.Parameters.AddWithValue("@newPassword", newPassword);
        //            updateCmd.Parameters.AddWithValue("@username", username);
        //            updateCmd.ExecuteNonQuery();
        //            return true;
        //        }
        //    }
        //}
        
        public List<MCustomer> SearchCustomers(string searchText)
        {
            List<MCustomer> customers = new List<MCustomer>();

            using (SQLiteConnection con = new SQLiteConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT Customer.id AS CustomerID, Customer.name AS CustomerName, " +
                               "Customer.phone AS PhoneNumber, Customer.cccd AS CCCD, " +
                               "Package.name AS PackageName, Schedule.shift AS Shift " +
                               "FROM Customer " +
                               "LEFT JOIN Schedule ON Customer.id = Schedule.idCustomer " +
                               "LEFT JOIN Package ON Schedule.idPackage = Package.id " +
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
                                PackageName = reader["PackageName"].ToString(),
                                Shift = reader["Shift"].ToString()
                            };

                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }
    }
}
