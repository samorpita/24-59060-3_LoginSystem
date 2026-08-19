using System;
using System.Windows.Forms;

namespace LoginSystem_24590603
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // ---- Validation ----
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                lblStatus.Text = "Username and password fields cannot be empty.";
                return;
            }

            if (password.Length < 6)
            {
                lblStatus.Text = "Password must be at least 6 characters long.";
                return;
            }

            if (password != confirmPassword)
            {
                lblStatus.Text = "Password and Confirm Password do not match.";
                return;
            }

            if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
            {
                lblStatus.Text = "Please enter a valid email address.";
                return;
            }

            try
            {
                // Check for duplicate username before inserting.
                if (DatabaseHelper.UsernameExists(username))
                {
                    lblStatus.Text = "Username already taken.";
                    return;
                }

                string passwordHash = PasswordHasher.Hash(password);
                bool success = DatabaseHelper.RegisterUser(username, passwordHash, email, fullName);

                if (success)
                {
                    MessageBox.Show("Registration successful! You can now log in.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    this.Close(); // returns control to LoginForm via FormClosed handler
                }
                else
                {
                    lblStatus.Text = "Registration failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                // Covers e.g. "Violation of UNIQUE KEY constraint" as a safety net
                // even though we already checked UsernameExists above.
                lblStatus.Text = "Database error: " + ex.Message;
            }
        }

        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            ClearForm();
            this.Close();
        }

        private void ClearForm()
        {
            txtFullName.Clear();
            txtUsername.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            lblStatus.Text = "";
        }
    }
}
