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
    public partial class LogIn : Form
    {
        //DataAccess Da = new DataAccess();
        public LogIn()
        {
            InitializeComponent();
            //DataAccess Da = new DataAccess();
        }

        private void LogIn_Load(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true; 
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }
        private void Clear()
        {
            this.txtUserID.Text = "";
            this.txtPassword.Text = "";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    string sql = "SELECT UserId, Password, Name, Role FROM [User] WHERE UserId=@id AND Password=@pass";

                    using (SqlCommand cmd = new SqlCommand(sql, da.Connection))
                    {
                        cmd.Parameters.AddWithValue("@id", txtUserID.Text);
                        cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader["Role"].ToString();
                                string id = reader["UserId"].ToString();

                                if (role == "Admin")
                                {
                                    this.Hide();
                                    new AdminDashboard(this).Show();
                                }
                                else if (role == "Student")
                                {
                                    this.Hide();
                                    new StudentDashboard(this, id).Show();
                                }
                                else
                                {
                                    MessageBox.Show("Unknown Role Detected!");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid Credentials");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }








        private void txtUserID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
