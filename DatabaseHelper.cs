using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LoginSystem_24590603
{
    /// <summary>
    /// All ADO.NET code lives here. Forms call these methods instead of
    /// opening a SqlConnection themselves.
    /// (Bonus task: "move all database code out of the forms")
    /// </summary>
    public static class DatabaseHelper
    {
        // Reads from App.config <connectionStrings> - never hard-coded.
        private static readonly string ConnStr =
            ConfigurationManager.ConnectionStrings["LoginDBConnection"].ConnectionString;

        /// <summary>
        /// Tries to open a connection and immediately close it.
        /// Returns true/false instead of letting the app crash.
        /// </summary>
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnStr))
                {
                    con.Open();
                } // using -> Dispose() -> connection closed even if something throws
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// ExecuteScalar is ideal for a single value like COUNT(*).
        /// Used to check "does this username already exist?" before inserting.
        /// </summary>
        public static bool UsernameExists(string username)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Users WHERE Username = @username", con))
            {
                cmd.Parameters.AddWithValue("@username", username);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Parameterized INSERT. ExecuteNonQuery returns rows affected.
        /// </summary>
        public static bool RegisterUser(string username, string passwordHash, string email, string fullName)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                @"INSERT INTO dbo.Users (Username, PasswordHash, Email, FullName)
                  VALUES (@username, @passwordHash, @email, @fullName)", con))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                // A NULL db value would come back as DBNull.Value; going the other
                // way (writing NULL), we must pass DBNull.Value ourselves if empty.
                cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                cmd.Parameters.AddWithValue("@fullName", string.IsNullOrWhiteSpace(fullName) ? (object)DBNull.Value : fullName);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Parameterized SELECT using SqlDataReader (fast, forward-only,
        /// best when reading a single row / small result to check).
        /// Returns the stored hash + full name so LoginForm can compare
        /// hashes and greet the user, without ever pulling back plain text.
        /// </summary>
        public static bool TryGetUserForLogin(string username, out string storedHash, out string fullName)
        {
            storedHash = null;
            fullName = null;

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT PasswordHash, FullName FROM dbo.Users WHERE Username = @username", con))
            {
                cmd.Parameters.AddWithValue("@username", username);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        storedHash = reader["PasswordHash"].ToString();
                        // FullName is nullable in the schema - check for DBNull
                        // before calling .ToString() on it.
                        fullName = reader["FullName"] == DBNull.Value
                            ? string.Empty
                            : reader["FullName"].ToString();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// SqlDataAdapter + DataTable - fills a whole result set in one go,
        /// which is exactly what a DataGridView wants.
        /// Deliberately does NOT select PasswordHash.
        /// </summary>
        public static DataTable GetAllUsers()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT UserID, Username, Email, CreatedAt FROM dbo.Users ORDER BY UserID", con))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
