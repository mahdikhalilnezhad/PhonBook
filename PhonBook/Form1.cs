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
        int beforEdit;


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
        /// filling the Gridview and the Textbox
        /// </summary>
        /// <param name="s">The Query that defult is "select * from phonetable</param>
        public void FillGrid(string s="select * from PhoneTable")
        {
            ds.Clear();

            cmd.CommandText = s;
            cmd.Connection = conn;

            adapter.SelectCommand = cmd;
            adapter.Fill(ds, "TempTable");

            DataGridView.DataBindings.Clear();
            DataGridView.DataBindings.Add("datasource", ds, "Temptable");

            txtId.DataBindings.Clear();
            txtFirstName.DataBindings.Clear();
            txtLastName.DataBindings.Clear();
            txtPhone.DataBindings.Clear();
            txtAddress.DataBindings.Clear();

            txtId.DataBindings.Add("text", ds, "temptable.Id");
            txtFirstName.DataBindings.Add("text", ds, "temptable.firstname");
            txtLastName.DataBindings.Add("text", ds, "temptable.lastname");
            txtPhone.DataBindings.Add("text", ds, "temptable.phone");
            txtAddress.DataBindings.Add("text", ds, "temptable.address");

            currencymanager = (CurrencyManager)this.BindingContext[ds, "temptable"];

        }
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            currencymanager.Position = e.RowIndex;
        }

        private void SetCurrentRecord(int currentRecord)
        {
            if (currentRecord < 0 || currentRecord > currencymanager.Count+1)
                return;
            currencymanager.Position = currentRecord;

            DataGridView.CurrentCell = DataGridView.Rows[currentRecord].Cells[DataGridView.CurrentCell.ColumnIndex];
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SetCurrentRecord(currencymanager.Position + 1);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            SetCurrentRecord(0);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            SetCurrentRecord(currencymanager.Position - 1);
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            SetCurrentRecord(currencymanager.Count - 1);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtAddress.ReadOnly = false;
            txtFirstName.ReadOnly = false;
            txtLastName.ReadOnly = false;
            txtPhone.ReadOnly = false;
            txtId.ReadOnly = true;

            txtId.Text = "";
            txtPhone.Text = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtAddress.Text = "";

            btnNew.Enabled = false;
            btnSave.Enabled = true;

            txtFirstName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            
            cmd.CommandText = "insert into phonetable values (@fn, @ln, @ph, @ad)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("fn", txtFirstName.Text);
            cmd.Parameters.AddWithValue("ln", txtLastName.Text);
            cmd.Parameters.AddWithValue("ph", txtPhone.Text);
            cmd.Parameters.AddWithValue("ad", txtAddress.Text);

            cmd.Connection = conn;

            cmd.ExecuteNonQuery(); ///Now Data inserted into the Database

            btnSave.Enabled = false;
            btnNew.Enabled = true;

            txtId.ReadOnly = true;
            txtAddress.ReadOnly = true;
            txtFirstName.ReadOnly = true;
            txtLastName.ReadOnly = true;
            txtPhone.ReadOnly = true;

            FillGrid();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            DialogResult dialog;

            cmd.CommandText = "delete from phonetable where id=@id";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id", txtId.Text);

            dialog = MessageBox.Show("Do you want to delete the '" + txtFirstName.Text + " " + txtLastName.Text + "' ?"
                                    , "Delete"
                                    , MessageBoxButtons.YesNoCancel);
            if (dialog == DialogResult.No || dialog == DialogResult.Cancel)
                return;

            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            FillGrid();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "Edit")
            {
                txtAddress.ReadOnly = false;
                txtFirstName.ReadOnly = false;
                txtLastName.ReadOnly = false;
                txtPhone.ReadOnly = false;
                txtId.ReadOnly = true;

                btnEdit.Text = "Apply";
                txtFirstName.Focus();

                beforEdit = currencymanager.Position;
            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                DialogResult dialog;

                cmd.CommandText = "update phonetable set firstname=@fn, lastname=@ln, phone=@ph, address=@ad where id=@id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("fn", txtFirstName.Text);
                cmd.Parameters.AddWithValue("ln", txtLastName.Text);
                cmd.Parameters.AddWithValue("ph", txtPhone.Text);
                cmd.Parameters.AddWithValue("ad", txtAddress.Text);
                cmd.Parameters.AddWithValue("id", txtId.Text);

                dialog = MessageBox.Show("Do you want to edit the person with Id '" + txtId.Text + "' ?"
                            , "Edit"
                            , MessageBoxButtons.YesNoCancel);
                if (dialog == DialogResult.No || dialog == DialogResult.Cancel)
                    return;

                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                FillGrid();

                SetCurrentRecord(beforEdit);

                txtId.ReadOnly = true;
                txtAddress.ReadOnly = true;
                txtFirstName.ReadOnly = true;
                txtLastName.ReadOnly = true;
                txtPhone.ReadOnly = true;

                btnEdit.Text = "Edit";

            }


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string query;
            query = "select * from PhoneTable where " + cmbSearchBy.Text + " like '" + txtSearchFor.Text + "%'";

            FillGrid(query);
        }

        private void txtSearchFor_TextChanged(object sender, EventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void DataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            SetCurrentRecord(DataGridView.CurrentCell.RowIndex);

        }


    }
}