using System;
using System.Windows.Forms;

namespace LoginSystem_24590603
{
    public partial class HomeForm : Form
    {
        private readonly string loggedInUsername;

        public HomeForm(string fullName, string username)
        {
            InitializeComponent();
            loggedInUsername = username;
            string displayName = string.IsNullOrWhiteSpace(fullName) ? username : fullName;
            lblWelcome.Text = $"Welcome, {displayName}";
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                gridUsers.DataSource = DatabaseHelper.GetAllUsers();
                // Defensive: never show password/hash columns even if the
                // query above is ever changed to select more columns.
                foreach (DataGridViewColumn col in gridUsers.Columns)
                {
                    if (col.Name.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        col.Name.IndexOf("hash", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        col.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load users:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Closing this form (not exiting the app) is what returns
            // control to LoginForm - see LoginForm's FormClosed handler.
            this.Close();
        }
    }
}
