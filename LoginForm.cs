using System;
using System.Windows.Forms;

namespace LoginSystem_24590603
{
    public partial class LoginForm : Form
    {
        private int failedAttempts = 0;
        private const int MaxFailedAttempts = 3;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Verify the DB is reachable as soon as the form opens, instead
            // of waiting for the user to click Login and get a crash.
            if (!DatabaseHelper.TestConnection(out string error))
            {
                MessageBox.Show(
                    "Could not connect to the database.\n\n" + error,
                    "Connection Problem",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            ResetForm();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblStatus.Text = "Please enter both username and password.";
                return;
            }

            try
            {
                bool found = DatabaseHelper.TryGetUserForLogin(username, out string storedHash, out string fullName);

                if (found && PasswordHasher.Verify(password, storedHash))
                {
                    failedAttempts = 0;
                    HomeForm home = new HomeForm(fullName, username);
                    home.FormClosed += (s, args) =>
                    {
                        // When HomeForm closes (Logout), come back to a clean LoginForm.
                        ResetForm();
                        this.Show();
                        txtUsername.Focus();
                    };
                    this.Hide();
                    home.Show();
                }
                else
                {
                    failedAttempts++;
                    int remaining = MaxFailedAttempts - failedAttempts;

                    if (remaining > 0)
                    {
                        MessageBox.Show(
                            $"Invalid username or password. {remaining} attempt(s) remaining.",
                            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        lblStatus.Text = $"Invalid credentials. {remaining} attempt(s) left.";
                    }
                    else
                    {
                        btnLogin.Enabled = false;
                        lblStatus.Text = "Too many failed attempts. Login disabled.";
                        MessageBox.Show(
                            "Too many failed login attempts. The Login button has been disabled.",
                            "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A database error occurred:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGoToRegister_Click(object sender, EventArgs e)
        {
            RegistrationForm reg = new RegistrationForm();
            reg.FormClosed += (s, args) =>
            {
                ResetForm();
                this.Show();
            };
            this.Hide();
            reg.Show();
        }

        /// <summary>
        /// Clears the form and re-enables Login. Called on startup and
        /// whenever we return here from Registration or Logout.
        /// </summary>
        public void ResetForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            lblStatus.Text = "";
            failedAttempts = 0;
            btnLogin.Enabled = true;
            txtUsername.Focus();
        }
    }
}
