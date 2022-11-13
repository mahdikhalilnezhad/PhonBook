using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;


namespace PhonBook
{
    public partial class FrmManiPhonBook : Form
    {

        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataSet ds = new DataSet();
        CurrencyManager currencymanager;



        public FrmManiPhonBook()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            conn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
                                    AttachDbFilename=E:\C_sharp_practice\PhonBook\PhonBook\PhonBookDB.mdf;
                                    Integrated Security=True";
            conn.Open();

            FillGrid();
        }

        /// <summary>
        /// Query the Data
        /// </summary>
        /// <param name="s">The Query that defult is "select * from phonetable</param>
        public void FillGrid(string s="select firstname,lastname,phone,address from PhoneTable")
        {
            cmd.CommandText = s;
            cmd.Connection = conn;
            adapter.SelectCommand = cmd;
            adapter.Fill(ds, "TempTable");
            DataGridView.DataBindings.Clear();
            DataGridView.DataBindings.Add("datasource", ds, "Temptable");
            txtFirstName.DataBindings.Clear();
            txtLastName.DataBindings.Clear();
            txtPhone.DataBindings.Clear();
            txtAddress.DataBindings.Clear();
            txtFirstName.DataBindings.Add("text", ds, "temptable.firstname");
            txtLastName.DataBindings.Add("text", ds, "temptable.lastname");
            txtPhone.DataBindings.Add("text", ds, "temptable.phone");
            txtAddress.DataBindings.Add("text", ds, "temptable.address");

            currencymanager = (CurrencyManager)this.BindingContext[ds, "temptable"];
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currencymanager.Position ++;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            currencymanager.Position = 0;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            currencymanager.Position --;
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            currencymanager.Position = currencymanager.Count-1;
        }
    }
}