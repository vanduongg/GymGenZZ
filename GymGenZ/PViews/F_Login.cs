using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymGenZ.PViews
{
    public partial class F_Login : Form
    {
        public F_Login()
        {
            InitializeComponent();
            conn.Open();
        }

        private void F_Login_Load(object sender, EventArgs e)
        {

        }
        SQLiteConnection conn = new SQLiteConnection("Data Source = C:\\Data\\GYM.db");

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = ipUsername.Text;
            string password = ipPassword.Text; 

            using (SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM Staff WHERE username = @username AND password = @password", conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@username", username);
                da.SelectCommand.Parameters.AddWithValue("@password", password);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    this.Hide();
                    string valueToSend = username;
                    F_Main f = new F_Main(valueToSend);
                    f.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không hợp lệ");
                }
            }

        }
    }
}
