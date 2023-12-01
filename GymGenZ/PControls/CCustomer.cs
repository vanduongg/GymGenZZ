﻿using GymGenZ.PModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

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

                string query = @"SELECT DISTINCT
                            c.id AS ID,
                            c.name AS CustomerName,
                            c.gender AS Gender,
                            c.cccd AS CCCD,
                            c.phone AS PhoneNumber,
                            c.start AS Start,
                            c.end AS End,
                            c.address AS Address,
                            p.name AS PackageName,
                            st.fullName AS TrainerName
                        FROM Customer c
                        LEFT JOIN Package p ON c.idPackage = p.id
                        LEFT JOIN TrainingSessions ts ON c.id = ts.customerID
                        LEFT JOIN Staff st ON ts.trainerID = st.id
                        WHERE c.name LIKE @searchText OR
                              c.phone LIKE @searchText OR
                              c.cccd LIKE @searchText";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MCustomer customer = new MCustomer
                            {
                                CustomerID = reader["ID"].ToString(),
                                CustomerName = reader["CustomerName"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                CCCD = reader["CCCD"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Start = reader["Start"].ToString(),
                                End = reader["End"].ToString(),
                                Address = reader["Address"].ToString(),
                                PackageName = reader["PackageName"].ToString(),
                                TrainerName = reader["TrainerName"].ToString(),
                            };

                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }






        public Tuple<int, string, string, string, string, string> GetCustomerInfo(int customerId)
        {
            Tuple<int, string, string, string, string, string> customerInfo = null;

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                string query = @"SELECT DISTINCT
                        c.id AS CustomerID,
                        c.name AS CustomerName,
                        p.name AS PackageName,
                        c.start AS StartDate,
                        c.end AS EndDate,
                        st.fullName AS TrainerName
                     FROM Customer c
                     JOIN Package p ON c.idPackage = p.id
                     JOIN TrainingSessions sch ON c.id = sch.customerID
                     JOIN Staff st ON sch.trainerID = st.id
                     WHERE c.id = @customerId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customerId", customerId);

                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idCustomer = reader.GetInt32(0);
                            string nameCustomer = reader.GetString(1);
                            string packageName = reader.GetString(2);
                            string startDate = reader.IsDBNull(3) ? "N/A" : reader.GetString(3);
                            string endDate = reader.IsDBNull(4) ? "N/A" : reader.GetString(4);
                            string trainerName = reader.IsDBNull(5) ? "N/A" : reader.GetString(5);

                            customerInfo = Tuple.Create(idCustomer, nameCustomer, packageName, startDate, endDate, trainerName);
                        }
                        else
                        {
                            Console.WriteLine("Customer not found.");
                        }
                    }
                }
            }

            return customerInfo;
        }




        public bool SignPTrainer(string idCustomer, string idStaff, List<string> lstDate, string shiftCode, int duration)
        {
            try
            {
                _conn.Open();
                try
                {
                    foreach (string date in lstDate)
                    {
                        string insertQuery = "INSERT INTO TrainingSessions (trainerID, customerID, date, shiftCode, duration) " +
                                             "VALUES (@TrainerID, @CustomerID, @Date, @ShiftCode, @Duration)";
                        using (SQLiteCommand insertPTrainer = new SQLiteCommand(insertQuery, _conn))
                        {
                            insertPTrainer.Parameters.AddWithValue("@TrainerID", idStaff);
                            insertPTrainer.Parameters.AddWithValue("@CustomerID", idCustomer);
                            insertPTrainer.Parameters.AddWithValue("@Date", date);
                            insertPTrainer.Parameters.AddWithValue("@ShiftCode", shiftCode);
                            insertPTrainer.Parameters.AddWithValue("@Duration", duration);
                            insertPTrainer.ExecuteNonQuery();
       
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool signCustomer(string name, string phone, string cccd, string packageID, string address, string gender)
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

                            string insertCustomerQuery = "INSERT INTO Customer(name, phone, cccd, idPackage, start, end, address, gender)"
                                                            + "VALUES(@Name, @Phone, @CCCD, @packageID, @registrationDate, @expirationDate, @address, @gender)";
                            using (SQLiteCommand insertCustomerCmd = new SQLiteCommand(insertCustomerQuery, _conn))
                            {
                                insertCustomerCmd.Parameters.AddWithValue("@Name", name);
                                insertCustomerCmd.Parameters.AddWithValue("@Phone", phone);
                                insertCustomerCmd.Parameters.AddWithValue("@CCCD", cccd);
                                insertCustomerCmd.Parameters.AddWithValue("@packageID", packageID);
                                insertCustomerCmd.Parameters.AddWithValue("@registrationDate", currentDate.ToString("yyyy-MM-dd"));
                                insertCustomerCmd.Parameters.AddWithValue("@expirationDate", expirationDate.ToString("yyyy-MM-dd"));
                                insertCustomerCmd.Parameters.AddWithValue("@address", address);
                                insertCustomerCmd.Parameters.AddWithValue("@gender", gender);
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
