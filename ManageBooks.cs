using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LibraryManagementSystem
{
    public partial class ManageBooks : Form
    {
        private DataAccess Da { set; get; }
        public ManageBooks()
        {
            InitializeComponent();
            this.Da = new DataAccess();
            this.PopulateGridView();
        }

        private void ManageBooks_Load(object sender, EventArgs e)
        {

        }
        private AdminDashboard Admin { set; get; }
        public ManageBooks(AdminDashboard admin) : this() 
        {
            this.Admin = admin;
        }

        private void PopulateGridView(String sql = "select * from Book")
        {
            try
            {
                var da = new DataAccess();

                SqlDataAdapter sda = new SqlDataAdapter(sql, da.Connection);

                DataTable dt = new DataTable();
                sda.Fill(dt);

                this.gdvBook.AutoGenerateColumns = true;
                this.gdvBook.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


        }
        private void btnBack_Click(object sender, EventArgs e) 
        { 

        }
        private void ManageBooks_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            this.Hide(); Admin.Show(); //Application.Exit();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.Clear();

        }
        private void Clear()
        {
            this.txtAuthor.Text = "";
            this.txtCategory.Text = "";
            this.txtPublisher.Text = "";
            this.numericUpDown.Value = 0;
            this.txttitle.Text = "";
            this.txtBookId.Text = "";
            //this.txtSearchByName.Text = "";

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            try
            {
                using (var da = new DataAccess())
                {
                    string sql = @"INSERT INTO Book (BookId, Title, Author, Publisher, Category, Quantity) 
                           VALUES (@BookId, @Title, @Author, @Publisher, @Category, @Quantity)";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@BookId", txtBookId.Text);
                    cmd.Parameters.AddWithValue("@Title", txttitle.Text);
                    cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
                    cmd.Parameters.AddWithValue("@Publisher", txtPublisher.Text);
                    cmd.Parameters.AddWithValue("@Category", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@Quantity", numericUpDown.Value);

                    int a = cmd.ExecuteNonQuery();

                    if (a == 1)
                    {
                        PopulateGridView();
                        Clear();
                        MessageBox.Show("Inserted!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {

            try
            {
                using (var da = new DataAccess())
                {
                    string sql = @"UPDATE Book SET 
                            Title=@Title, 
                            Author=@Author, 
                            Publisher=@Publisher, 
                            Category=@Category, 
                            Quantity=@Quantity
                           WHERE BookId=@BookId";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@Title", txttitle.Text);
                    cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
                    cmd.Parameters.AddWithValue("@Publisher", txtPublisher.Text);
                    cmd.Parameters.AddWithValue("@Category", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@Quantity", numericUpDown.Value);
                    cmd.Parameters.AddWithValue("@BookId", txtBookId.Text);

                    int a = cmd.ExecuteNonQuery();

                    if (a == 1)
                    {
                        PopulateGridView();
                        MessageBox.Show("Updated!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        
        private void btnDelete_Click(object sender, EventArgs e)
        {

            try
            {
                using (var da = new DataAccess())
                {
                    string sql = "DELETE FROM Book WHERE BookId=@BookId";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@BookId", txtBookId.Text);

                    int a = cmd.ExecuteNonQuery();

                    if (a == 1)
                    {
                        MessageBox.Show("Deleted!");
                        PopulateGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        
        private void gdvBook_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txtBookId.Text= this.gdvBook.CurrentRow.Cells["BookId"].Value.ToString();
            this.txttitle.Text= this.gdvBook.CurrentRow.Cells["Title"].Value.ToString();
            this.txtAuthor.Text= this.gdvBook.CurrentRow.Cells["Author"].Value.ToString();
            this.txtPublisher.Text= this.gdvBook.CurrentRow.Cells["Publisher"].Value.ToString();
            this.txtCategory.Text= this.gdvBook.CurrentRow.Cells["Category"].Value.ToString();
            this.numericUpDown.Value = Convert.ToDecimal(this.gdvBook.CurrentRow.Cells["Quantity"].Value);
           
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string sql = "select * from Book where [" + this.txtSearchBy.Text + "] like '" + this.txtSearch.Text + "%';";
            PopulateGridView(sql);
        }

        private void txtSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearchBy.Text) || string.IsNullOrEmpty(this.txtSearch.Text))
            {
                PopulateGridView();
                return;
            }
            string sql = "select * from Book where [" + this.txtSearchBy.Text + "] like '" + this.txtSearch.Text + "%';";
            PopulateGridView(sql);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateGridView();
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Hide(); Admin.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void gdvBook_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
