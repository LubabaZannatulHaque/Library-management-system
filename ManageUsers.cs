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

namespace LibraryManagementSystem
{
    public partial class ManageUsers : Form
    {
        private DataAccess Da { set; get; }
        public ManageUsers()
        {
            InitializeComponent();
            this.Da = new DataAccess();
            PopulateGridView();
            
        }
        private void PopulateGridView(String sql = "select * from [User]")
        {
            try
            {
                using (var da = new DataAccess())
                {
                    SqlDataAdapter sda = new SqlDataAdapter(sql, da.Connection);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    dgvUser.AutoGenerateColumns = true;
                    dgvUser.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
        private AdminDashboard Admin { set; get; }

        public ManageUsers(AdminDashboard admin):this()
        {
            this.Admin = admin;
        }
        private void ManageUsers_Load(object sender, EventArgs e)
        {
            this.PopulateGridView();
        }

        private void btnBack_Click(object sender, EventArgs e) //*****
        {
            //this.Hide();
            //this.Admin.Show();
        }

        private void ManageUsers_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Admin.Show();
            //Application.Exit();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateGridView();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            try
            {
                using (var da = new DataAccess())
                {
                    string sql = @"INSERT INTO [User]  (UserId, Name, Password, Role)
                           VALUES (@UserId, @Name, @Password, @Role)";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@UserId", txtID.Text);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@Role", txtRole.Text);

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

        
        private void Clear()
        {
            txtName.Text = "";
            txtPassword.Text = "";
            txtRole.Text = "";
            txtsearch.Text = "";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            try
            {
                using (var da = new DataAccess())
                {
                    string sql = @"UPDATE [User] 
                           SET Name=@Name, Password=@Password, Role=@Role
                           WHERE UserId=@UserId";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@Role", txtRole.Text);
                    cmd.Parameters.AddWithValue("@UserId", txtID.Text);

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
                    string sql = @"DELETE FROM [User]  WHERE UserId=@UserId";
                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@UserId", txtID.Text);

                    int a = cmd.ExecuteNonQuery();
                    if (a == 1)
                    {
                        PopulateGridView();
                        Clear();
                        MessageBox.Show("Deleted!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvUser_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Tag= this.dgvUser.CurrentRow.Cells[0].Value.ToString();
            txtName.Tag= this.dgvUser.CurrentRow.Cells[1].Value.ToString();
            txtPassword.Tag= this.dgvUser.CurrentRow.Cells[2].Value.ToString();
            txtRole.Tag= this.dgvUser.CurrentRow.Cells[3].Value.ToString();
        }
        private void dgvUser_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txtName.Text = this.dgvUser.CurrentRow.Cells["Name"].Value.ToString();
            this.txtID.Text = this.dgvUser.CurrentRow.Cells["UserId"].Value.ToString();
            this.txtPassword.Text = this.dgvUser.CurrentRow.Cells["Password"].Value.ToString();
            this.txtRole.Text = this.dgvUser.CurrentRow.Cells["Role"].Value.ToString();
        }

        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearchBy.Text) || string.IsNullOrEmpty(this.txtsearch.Text))
            {
                PopulateGridView();
                return;
            }
            string sql = "select * from [User]  where [" + txtSearchBy.Text+"] like '" + txtsearch.Text + "%';";
            PopulateGridView(sql);
        }

        private void txtSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sql = "select * from [User]  where [" + txtSearchBy.Text + "] like '" + txtsearch.Text + "%';";
            PopulateGridView(sql);
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Admin.Show();
        }
    }
}
