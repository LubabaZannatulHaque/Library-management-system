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
    public partial class BorrowRecords : Form
    {
        private DataAccess Da { set; get; }
        private AdminDashboard Admin { set; get; }

        public BorrowRecords()
        {
            InitializeComponent();
            this.Da = new DataAccess();
            this.PopulateGridView();
            InitializeControls();
        }

        public BorrowRecords(AdminDashboard admin) : this()
        {
            this.Admin = admin;
        }

        private void InitializeControls()
        {
            
            cmbSearchby.Items.Clear();
            cmbSearchby.Items.Add("Status");
            cmbSearchby.Items.Add("UserId");
            cmbSearchby.Items.Add("BookId");
            cmbSearchby.Items.Add("BorrowId");
            cmbSearchby.SelectedIndex = 0;
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Borrowed");
            cmbStatus.Items.Add("Returned");

            SetupNumericControls();
        }

        private void SetupNumericControls()
        {
            // Setup penalty values
            PaneltyUpDown.Items.Clear();
            for (int i = 0; i <= 500; i += 10)
            {
                PaneltyUpDown.Items.Add(i.ToString());
            }

            // Setup quantity values
            BorrowedQuantityUpDown.Items.Clear();
            for (int i = 0; i <= 10; i++)
            {
                BorrowedQuantityUpDown.Items.Add(i.ToString());
            }
        }

        private void PopulateGridView(String sql = "SELECT * FROM Borrow ORDER BY BorrowDate DESC")
        {

            try
            {
                using (var da = new DataAccess())
                {
                    SqlDataAdapter sda = new SqlDataAdapter(sql, da.Connection);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    dgvBorrowed.DataSource = dt;

                    // Column formatting if data exists
                    if (dgvBorrowed.Columns.Count > 0)
                    {
                        if (dgvBorrowed.Columns.Contains("BorrowDate"))
                            dgvBorrowed.Columns["BorrowDate"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

                        if (dgvBorrowed.Columns.Contains("ReturnDate"))
                            dgvBorrowed.Columns["ReturnDate"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

                        if (dgvBorrowed.Columns.Contains("TimerStartTime"))
                            dgvBorrowed.Columns["TimerStartTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

                        if (dgvBorrowed.Columns.Contains("TimerEndTime"))
                            dgvBorrowed.Columns["TimerEndTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        

        private void ClearFields()
        {
            txtBorrowId.Clear();
            txtUserId.Clear();
            txtBookId.Clear();
            cmbStatus.SelectedIndex = -1;
            PaneltyUpDown.Text = "0";
            BorrowedQuantityUpDown.Text = "0";
            dtpBorrowDate.Value = DateTime.Now;
            dtpReturnDate.Value = DateTime.Now;
            dtpTimerStartTime.Value = DateTime.Now;
            dtpTimerEndTime.Value = DateTime.Now;
            chktrue.Checked = false;
            chkfalse.Checked = false;
        }

        private void LoadSelectedRecord()
        {
            if (dgvBorrowed.CurrentRow != null)
            {
                DataGridViewRow row = dgvBorrowed.CurrentRow;
                txtBorrowId.Text = row.Cells["BorrowId"].Value?.ToString() ?? "";
                txtUserId.Text = row.Cells["UserId"].Value?.ToString() ?? "";
                txtBookId.Text = row.Cells["BookId"].Value?.ToString() ?? "";
                cmbStatus.Text = row.Cells["Status"].Value?.ToString() ?? "";
                PaneltyUpDown.Text = row.Cells["Penalty"].Value?.ToString() ?? "0";
                BorrowedQuantityUpDown.Text = row.Cells["BorrowedQuantity"].Value?.ToString() ?? "0";
                if (row.Cells["BorrowDate"].Value != DBNull.Value && row.Cells["BorrowDate"].Value != null)
                    dtpBorrowDate.Value = Convert.ToDateTime(row.Cells["BorrowDate"].Value);

                if (row.Cells["ReturnDate"].Value != DBNull.Value && row.Cells["ReturnDate"].Value != null)
                    dtpReturnDate.Value = Convert.ToDateTime(row.Cells["ReturnDate"].Value);

                if (row.Cells["TimerStartTime"].Value != DBNull.Value && row.Cells["TimerStartTime"].Value != null)
                    dtpTimerStartTime.Value = Convert.ToDateTime(row.Cells["TimerStartTime"].Value);

                if (row.Cells["TimerEndTime"].Value != DBNull.Value && row.Cells["TimerEndTime"].Value != null)
                    dtpTimerEndTime.Value = Convert.ToDateTime(row.Cells["TimerEndTime"].Value);
                bool timerActive = Convert.ToBoolean(row.Cells["TimerActive"].Value ?? false);
                chktrue.Checked = timerActive;
                chkfalse.Checked = !timerActive;
            }
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtBorrowId.Text))
            {
                MessageBox.Show("BorrowId is required!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                MessageBox.Show("UserId is required!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBookId.Text))
            {
                MessageBox.Show("BookId is required!");
                return false;
            }

            if (cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Status is required!");
                return false;
            }

            return true;
        }

        private void btnBack_Click(object sender, EventArgs e) //***
        {
            //this.Hide();
            //this.Admin?.Show();
        }

        private void BorrowRecords_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            this.Admin?.Show();
        }

        private void dgvBorrowed_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                LoadSelectedRecord();
            }
        }

        private void cmbSearchby_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (cmbSearchby.SelectedItem == null || string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                PopulateGridView();
                return;
            }

            string searchBy = cmbSearchby.SelectedItem.ToString();
            string searchValue = txtSearch.Text;
            string sql = "";

            switch (searchBy)
            {
                case "Status":
                    sql = $"SELECT * FROM Borrow WHERE Status LIKE '%{searchValue}%' ORDER BY BorrowDate DESC";
                    break;
                case "UserId":
                    sql = $"SELECT * FROM Borrow WHERE UserId LIKE '%{searchValue}%' ORDER BY BorrowDate DESC";
                    break;
                case "BookId":
                    sql = $"SELECT * FROM Borrow WHERE BookId LIKE '%{searchValue}%' ORDER BY BorrowDate DESC";
                    break;
                case "BorrowId":
                    sql = $"SELECT * FROM Borrow WHERE BorrowId LIKE '%{searchValue}%' ORDER BY BorrowDate DESC";
                    break;
                default:
                    PopulateGridView();
                    return;
            }

            PopulateGridView(sql);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            try
            {
                if (!ValidateInput())
                    return;

                int borrowId = Convert.ToInt32(txtBorrowId.Text);
                int userId = Convert.ToInt32(txtUserId.Text);
                int bookId = Convert.ToInt32(txtBookId.Text);
                string status = cmbStatus.Text;
                decimal penalty = Convert.ToDecimal(PaneltyUpDown.Text);
                int borrowedQty = Convert.ToInt32(BorrowedQuantityUpDown.Text);
                bool timerActive = chktrue.Checked;

                string sql = @"
            UPDATE Borrow SET 
                UserId = @UserId,
                BookId = @BookId,
                Status = @Status,
                Penalty = @Penalty,
                BorrowedQuantity = @BorrowedQty,
                TimerActive = @TimerActive,
                BorrowDate = @BorrowDate,
                ReturnDate = @ReturnDate,
                TimerStartTime = @TimerStart,
                TimerEndTime = @TimerEnd
            WHERE BorrowId = @BorrowId";

                using (var da = new DataAccess())
                {
                    using (SqlCommand cmd = new SqlCommand(sql, da.Connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@BookId", bookId);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@Penalty", penalty);
                        cmd.Parameters.AddWithValue("@BorrowedQty", borrowedQty);
                        cmd.Parameters.AddWithValue("@TimerActive", timerActive ? 1 : 0);
                        cmd.Parameters.AddWithValue("@BorrowDate", dtpBorrowDate.Value);
                        cmd.Parameters.AddWithValue("@ReturnDate", dtpReturnDate.Value);
                        cmd.Parameters.AddWithValue("@TimerStart", dtpTimerStartTime.Value);
                        cmd.Parameters.AddWithValue("@TimerEnd", dtpTimerEndTime.Value);
                        cmd.Parameters.AddWithValue("@BorrowId", borrowId);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Record updated successfully!");
                            PopulateGridView();
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Update failed! Record not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update error: " + ex.Message);
            }
        }

        

        private void btnDelete_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(txtBorrowId.Text))
                {
                    MessageBox.Show("Please select a record to delete!");
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Are you sure you want to delete this borrow record?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                int borrowId = Convert.ToInt32(txtBorrowId.Text);

                string sql = @"DELETE FROM Borrow WHERE BorrowId = @BorrowId";

                using (var da = new DataAccess())
                using (SqlCommand cmd = new SqlCommand(sql, da.Connection))
                {
                    cmd.Parameters.AddWithValue("@BorrowId", borrowId);

                    int deleted = cmd.ExecuteNonQuery();

                    if (deleted > 0)
                    {
                        MessageBox.Show("Record deleted successfully!");
                        PopulateGridView();
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Delete failed! Record not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete error: " + ex.Message);
            }
        }

        

        private void chktrue_CheckedChanged(object sender, EventArgs e)
        {
            if (chktrue.Checked)
            {
                chkfalse.Checked = false;
            }
        }

        private void chkfalse_CheckedChanged(object sender, EventArgs e)
        {
            if (chkfalse.Checked)
            {
                chktrue.Checked = false;
            }
        }

        private void BorrowRecords_Load(object sender, EventArgs e)
        {
           
        }

       
        private void RefreshGrid()
        {
            PopulateGridView();
        }

        private void ShowAllRecords()
        {
            PopulateGridView();
            txtSearch.Clear();
        }

        private void BorrowRecords_Load_1(object sender, EventArgs e)
        {

        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Admin?.Show();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}