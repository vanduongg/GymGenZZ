using GymGenZ.PControls;
using GymGenZ.PModels;
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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GymGenZ.PViews
{
    public partial class F_Customer : Form
    {

        private CCustomer _dataService;
        private List<MCustomer> _customers;
        private String idCustomer = null;

        public F_Customer()
        {
            InitializeComponent();
            _dataService = new CCustomer("Data Source=C:\\Data\\GYM.db");
            loadDataToGrid();
            dataGripViewCustomer.SelectionChanged += dataGripViewCustomer_SelectionChanged;
        }

        private void loadDataToGrid()
        {
            string searchText = tbFind.Text.Trim();
            _customers = _dataService.SearchCustomers(searchText);
            dataGripViewCustomer.DataSource = _customers;
        }

        private void dataGripViewCustomer_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGripViewCustomer.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGripViewCustomer.SelectedRows[0];
                idCustomer = selectedRow.Cells["CustomerID"].Value.ToString();
            }
        }

        private void tbFind_TextChanged(object sender, EventArgs e)
        {
            loadDataToGrid();
        }

        private void btn_Schedule_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Selected Customer ID: {idCustomer}");
        }

        private void F_Customer_Load(object sender, EventArgs e)
        {

        }
    }
}
