using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagementSystem
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }
        private LogIn Lg { set; get; }

        public AdminDashboard(LogIn lg) : this()
        {
            this.Lg = lg;
        }
        private void btnManageBooks_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ManageBooks(this).Show();

        }
        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ManageUsers(this).Show();
        }
        private void btnBorrowRecords_Click(object sender, EventArgs e)
        {
            this.Hide();
            new BorrowRecords(this).Show();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            this.Hide();
            Lg.Show();
        }

        private void AdminDashboard_FormClosed(object sender, FormClosedEventArgs e) //**
        {
            //this.Hide();
            //Lg.Show();
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {

        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Lg.Show();
        }
    }
}
