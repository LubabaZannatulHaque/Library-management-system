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
    public partial class StudentDashboard : Form
    {
        private DataAccess Da { set; get; }
        private LogIn Lg { set; get; }
        private string Id { set; get; }

        // Timer related variables
        private Timer borrowTimer;
        private int remainingSeconds = 60; // 1 minute = 60 seconds
        private decimal penaltyPerMinute = 10.00m; // 10 taka per minute penalty

        public StudentDashboard()
        {
            InitializeComponent();
            this.Da = new DataAccess();
            this.PopulateGridView();

            // Timer initialize koro
            InitializeTimer();
        }

        public StudentDashboard(LogIn lg, string id) : this()
        {
            this.Lg = lg;
            this.Id = id;
            this.BorrowedGridView();
        }

        private void InitializeTimer()
        {
            borrowTimer = new Timer();
            borrowTimer.Interval = 1000; // 1 second
            borrowTimer.Tick += BorrowTimer_Tick;
        }

        private void PopulateGridView(String sql = "select * from Book")
        {

            try
            {
                using (var da = new DataAccess())
                {
                    SqlDataAdapter sda = new SqlDataAdapter(sql, da.Connection);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    this.dgvBook.AutoGenerateColumns = true;
                    this.dgvBook.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        

        private void BorrowedGridView()
        {

            try
            {
                string sq = @"
            SELECT b.BorrowId, b.BookId, b.UserId, bk.Title, b.BorrowDate, b.ReturnDate, 
                   b.Status, b.BorrowedQuantity, b.Penalty, b.TimerActive,
                   CASE 
                       WHEN b.TimerActive = 1 THEN 'Timer Active'
                       WHEN b.Penalty > 0 THEN 'Penalty Applied'
                       ELSE 'Normal' 
                   END AS TimerStatus
            FROM Borrow b
            INNER JOIN Book bk ON b.BookId = bk.BookId
            WHERE b.UserId = @UserId
              AND b.Status = 'Borrowed'
            ORDER BY b.BorrowDate DESC";

                using (var da = new DataAccess())
                {
                    SqlDataAdapter sda = new SqlDataAdapter(sq, da.Connection);
                    sda.SelectCommand.Parameters.AddWithValue("@UserId", this.Id);

                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    dgvBooksBorrowed.AutoGenerateColumns = true;
                    dgvBooksBorrowed.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        

        private void StudentDashboard_Load(object sender, EventArgs e)
        {
            this.PopulateGridView();
            if (!string.IsNullOrEmpty(this.Id))
            {
                this.BorrowedGridView();
                CheckActiveTimer(); // Active timer check koro
            }
        }

        private void CheckActiveTimer()
        {

            try
            {
                int activeCount = 0;
                DateTime earliestEndTime;

                using (var da = new DataAccess())
                {
                    string sql = @"
                SELECT COUNT(*) AS ActiveCount
                FROM Borrow
                WHERE UserId = @UserId
                AND TimerActive = 1
                AND Status = 'Borrowed'";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@UserId", this.Id);

                    activeCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (activeCount > 0)
                {
                    using (var da = new DataAccess())
                    {
                        string timeSql = @"
                    SELECT MIN(TimerEndTime) AS EarliestEndTime
                    FROM Borrow
                    WHERE UserId = @UserId
                    AND TimerActive = 1
                    AND Status = 'Borrowed'";

                        SqlCommand timeCmd = new SqlCommand(timeSql, da.Connection);
                        timeCmd.Parameters.AddWithValue("@UserId", this.Id);

                        object result = timeCmd.ExecuteScalar();
                        earliestEndTime = Convert.ToDateTime(result);
                    }

                    // Time calculation
                    TimeSpan timeLeft = earliestEndTime - DateTime.Now;

                    if (timeLeft.TotalSeconds > 0)
                    {
                        remainingSeconds = (int)timeLeft.TotalSeconds;
                        borrowTimer.Start();
                        UpdateTimerDisplay();
                    }
                    else
                    {
                        ApplyTimerPenaltyToAll(); // time expire
                    }
                }
                else
                {
                    if (lblTimer != null)
                    {
                        lblTimer.Text = "No Active Timer";
                        lblTimer.ForeColor = Color.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Timer check error: " + ex.Message);
            }
        }

        

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Book WHERE Title LIKE '" + this.txtName.Text + "%';";
            this.PopulateGridView(sql);
        }

        private void txtAuthor_TextChanged(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Book WHERE Author LIKE '" + this.txtAuthor.Text + "%';";
            this.PopulateGridView(sql);
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            // Stop timer if running
            if (borrowTimer != null && borrowTimer.Enabled)
            {
                borrowTimer.Stop();
            }

            this.Hide();
            this.Lg?.Show();
        }

        private void StudentDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Stop timer if running
            if (borrowTimer != null && borrowTimer.Enabled)
            {
                borrowTimer.Stop();
            }

            this.Hide();
            this.Lg?.Show();
        }

        private void btnMyBorrowedBooks_Click(object sender, EventArgs e)
        {
            this.BorrowedGridView();
        }

        private void btnSearchBooks_Click(object sender, EventArgs e)
        {
            this.PopulateGridView();
        }

        private void gdvBook_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Book grid click handler
        }

        private void btnBorrow_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgvBook.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select at least one book to borrow.");
                    return;
                }

                int totalBorrowed = 0;

                foreach (DataGridViewRow row in dgvBook.SelectedRows)
                {
                    int bookId = Convert.ToInt32(row.Cells["BookId"].Value);

                    // (1) Already borrowed check
                    int borrowCount = 0;
                    using (var da = new DataAccess())
                    {
                        string countSql = "SELECT COUNT(*) FROM Borrow WHERE UserId=@UserId AND BookId=@BookId";
                        SqlCommand cmdCount = new SqlCommand(countSql, da.Connection);
                        cmdCount.Parameters.AddWithValue("@UserId", this.Id);
                        cmdCount.Parameters.AddWithValue("@BookId", bookId);
                        borrowCount = Convert.ToInt32(cmdCount.ExecuteScalar());
                    }

                    if (borrowCount >= 2)
                    {
                        MessageBox.Show($"Already borrowed this book twice! (BookId {bookId})");
                        continue;
                    }

                    // (2) Quantity check
                    int qty = 0;
                    using (var da = new DataAccess())
                    {
                        string qtySql = "SELECT Quantity FROM Book WHERE BookId=@BookId";
                        SqlCommand cmdQty = new SqlCommand(qtySql, da.Connection);
                        cmdQty.Parameters.AddWithValue("@BookId", bookId);
                        qty = Convert.ToInt32(cmdQty.ExecuteScalar());
                    }

                    if (qty <= 0)
                    {
                        MessageBox.Show($"Book {bookId} is out of stock!");
                        continue;
                    }

                    // (3) Generate BorrowId (00001)
                    string newBorrowId = "";
                    using (var da = new DataAccess())
                    {
                        string idSql = @"SELECT RIGHT('00000' + CAST(ISNULL(MAX(CAST(BorrowId AS INT)),0) + 1 AS VARCHAR(5)), 5)
                                 FROM Borrow";

                        SqlCommand idCmd = new SqlCommand(idSql, da.Connection);
                        newBorrowId = idCmd.ExecuteScalar().ToString();
                    }

                    // (4) Timer + Dates
                    DateTime borrowTime = DateTime.Now;
                    DateTime timerEndTime = borrowTime.AddMinutes(1);
                    DateTime returnDate = borrowTime.AddDays(7);

                    // (5) Insert Borrow
                    using (var da = new DataAccess())
                    {
                        string insertSql = @"
                INSERT INTO Borrow 
                (BorrowId, UserId, BookId, BorrowDate, ReturnDate, Status, BorrowedQuantity, TimerActive, TimerStartTime, TimerEndTime, Penalty)
                VALUES (@BorrowId, @UserId, @BookId, @BorrowDate, @ReturnDate, 'Borrowed', 1, 1, @StartTime, @EndTime, 0)";

                        SqlCommand cmd = new SqlCommand(insertSql, da.Connection);
                        cmd.Parameters.AddWithValue("@BorrowId", newBorrowId);
                        cmd.Parameters.AddWithValue("@UserId", this.Id);
                        cmd.Parameters.AddWithValue("@BookId", bookId);
                        cmd.Parameters.AddWithValue("@BorrowDate", borrowTime);
                        cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                        cmd.Parameters.AddWithValue("@StartTime", borrowTime);
                        cmd.Parameters.AddWithValue("@EndTime", timerEndTime);
                        cmd.ExecuteNonQuery();
                    }

                    // (6) Quantity Update
                    using (var da = new DataAccess())
                    {
                        string updateSql = "UPDATE Book SET Quantity = Quantity - 1 WHERE BookId=@BookId";
                        SqlCommand cmd = new SqlCommand(updateSql, da.Connection);
                        cmd.Parameters.AddWithValue("@BookId", bookId);
                        cmd.ExecuteNonQuery();
                    }

                    totalBorrowed++;
                }

                // (7) Final UI Update + Timer Start
                if (totalBorrowed > 0)
                {
                    if (!borrowTimer.Enabled)
                        StartBorrowTimer();

                    MessageBox.Show($"{totalBorrowed} book(s) borrowed successfully! Timer started.");

                    BorrowedGridView();
                    PopulateGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Borrow error: " + ex.Message);
            }
        }

        

        

        

        private void btnReturn_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgvBooksBorrowed.CurrentRow == null)
                {
                    MessageBox.Show("Please select a borrowed book to return.");
                    return;
                }

                int borrowId = Convert.ToInt32(dgvBooksBorrowed.CurrentRow.Cells["BorrowId"].Value);
                int bookId = Convert.ToInt32(dgvBooksBorrowed.CurrentRow.Cells["BookId"].Value);

                // (1) Mark as returned
                using (var da = new DataAccess())
                {
                    string returnSql = @"
                UPDATE Borrow 
                SET Status='Returned', BorrowedQuantity=0, TimerActive=0 
                WHERE BorrowId=@BorrowId";

                    SqlCommand cmd = new SqlCommand(returnSql, da.Connection);
                    cmd.Parameters.AddWithValue("@BorrowId", borrowId);
                    cmd.ExecuteNonQuery();
                }

                // (2) Increase book quantity
                using (var da = new DataAccess())
                {
                    string updateSql = @"UPDATE Book SET Quantity = Quantity + 1 WHERE BookId=@BookId";

                    SqlCommand cmd = new SqlCommand(updateSql, da.Connection);
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    cmd.ExecuteNonQuery();
                }

                // (3) Check active timer remaining
                int activeCount = 0;
                using (var da = new DataAccess())
                {
                    string checkSql = @"
                SELECT COUNT(*) 
                FROM Borrow 
                WHERE UserId=@UserId AND TimerActive=1 AND Status='Borrowed'";

                    SqlCommand cmd = new SqlCommand(checkSql, da.Connection);
                    cmd.Parameters.AddWithValue("@UserId", this.Id);

                    activeCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // (4) Stop timer if no active borrow
                if (activeCount == 0)
                {
                    if (borrowTimer != null && borrowTimer.Enabled)
                        borrowTimer.Stop();

                    if (lblTimer != null)
                    {
                        lblTimer.Text = "No Active Timer";
                        lblTimer.ForeColor = Color.Gray;
                    }
                }

                MessageBox.Show("Book returned successfully!");

                BorrowedGridView();
                PopulateGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Return error: " + ex.Message);
            }
        }

        

        private void StartBorrowTimer()
        {
            remainingSeconds = 60; // Reset to 1 minute
            borrowTimer.Start();
            UpdateTimerDisplay();
        }

        private void BorrowTimer_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;
            UpdateTimerDisplay();

            if (remainingSeconds <= 0)
            {
                // Time up! Apply penalty to all active borrows
                borrowTimer.Stop();
                ApplyTimerPenaltyToAll();
            }
        }

        private void UpdateTimerDisplay()
        {

            if (lblTimer == null)
                return;

            int activeCount = 0;

            // (1) Active borrowed books count নাও
            try
            {
                using (var da = new DataAccess())
                {
                    string sql = @"
                SELECT COUNT(*) 
                FROM Borrow 
                WHERE UserId=@UserId 
                AND TimerActive=1 
                AND Status='Borrowed'";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@UserId", this.Id);

                    activeCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                activeCount = 0;
            }

            // (2) Timer formatting
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            // (3) UI Display formatting
            if (activeCount > 0)
                lblTimer.Text = $"Time Left: {minutes:00}:{seconds:00} ({activeCount} books)";
            else
                lblTimer.Text = $"Time Left: {minutes:00}:{seconds:00}";

            // (4) Color logic (same as before)
            if (remainingSeconds <= 20)
                lblTimer.ForeColor = Color.Red;
            else if (remainingSeconds <= 40)
                lblTimer.ForeColor = Color.Orange;
            else
                lblTimer.ForeColor = Color.Green;
        }

        

        private void ApplyTimerPenaltyToAll()
        {

            try
            {
                int penalizedCount = 0;
                decimal penalty = penaltyPerMinute * 1; // 1 minute penalty

                using (var da = new DataAccess())
                {
                    string sql = @"
                UPDATE Borrow 
                SET Penalty = @Penalty, TimerActive = 0
                WHERE UserId = @UserId 
                AND TimerActive = 1 
                AND Status = 'Borrowed'";

                    SqlCommand cmd = new SqlCommand(sql, da.Connection);
                    cmd.Parameters.AddWithValue("@Penalty", penalty);
                    cmd.Parameters.AddWithValue("@UserId", this.Id);

                    penalizedCount = cmd.ExecuteNonQuery();
                }

                if (penalizedCount > 0)
                {
                    decimal totalPenalty = penalizedCount * penalty;
                    MessageBox.Show($"Time up! Penalty applied to {penalizedCount} book(s). Total Penalty: {totalPenalty} Taka");
                }

                // Timer label update
                if (lblTimer != null)
                {
                    lblTimer.Text = "Timer Expired - Penalty Applied";
                    lblTimer.ForeColor = Color.Red;
                }

                // Refresh Borrowed list
                this.BorrowedGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Penalty update error: " + ex.Message);
            }
        }

    

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.PopulateGridView();
            this.BorrowedGridView();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            if (borrowTimer != null && borrowTimer.Enabled)
            {
                borrowTimer.Stop();
            }

            this.Hide();
            this.Lg?.Show();
        }
    }
}