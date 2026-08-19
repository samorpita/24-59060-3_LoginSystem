# SQL Injection Demo (Task 6) - how to produce the 4 required screenshots

Do this in a **throwaway copy** of the project so the vulnerable code never
touches your real submission (concatenated SQL left in the submission is an
automatic -15).

## Steps

1. Copy the whole `24-59060-3_LoginSystem` folder somewhere else, e.g.
   `24-59060-3_LoginSystem_VULN_DEMO_ONLY`.

2. In that throwaway copy, open `LoginForm.cs` and temporarily replace the
   body of `btnLogin_Click` with the vulnerable version below (this mirrors
   bug #1 in the sample project - string concatenation instead of
   parameters):

   ```csharp
   // VULNERABLE - FOR DEMO ONLY, DO NOT SHIP THIS
   private void btnLogin_Click(object sender, EventArgs e)
   {
       using (var con = new System.Data.SqlClient.SqlConnection(
           System.Configuration.ConfigurationManager
               .ConnectionStrings["LoginDBConnection"].ConnectionString))
       {
           string sql = "SELECT * FROM dbo.Users WHERE Username='" + txtUsername.Text +
                        "' AND PasswordHash='" + txtPassword.Text + "'";
           var cmd = new System.Data.SqlClient.SqlCommand(sql, con);
           con.Open();
           var reader = cmd.ExecuteReader();
           if (reader.HasRows)
               MessageBox.Show("Login success (VULNERABLE)");
           else
               MessageBox.Show("Login failed");
       }
   }
   ```

3. Run it. In the Username box type any existing username (or a fake one),
   and in the Password box type:

   ```
   ' OR '1'='1
   ```

   **Screenshot (a):** the login succeeding with no valid password - this
   shows the bypass working.

4. **Screenshot (b):** the vulnerable code above, shown in your editor.

5. Revert `btnLogin_Click` back to the real, parameterized version (copy it
   back from the real project, which calls `DatabaseHelper.TryGetUserForLogin`
   and `PasswordHasher.Verify`).

6. Run it again with the *same* input (`' OR '1'='1` as the password).

   **Screenshot (c):** the login now correctly failing - the string is just
   treated as a wrong password, not as SQL.

7. Delete the throwaway copy entirely once you have your screenshots.

## Why parameters stop the attack (for your report/README)

When the SQL is built by concatenation, whatever the user types becomes part
of the SQL text itself. Typing `' OR '1'='1` closes the quoted string early
and adds a condition that is always true, so the WHERE clause matches every
row and the query returns results with no real password. With a parameterized
query (`@username`, `@passwordHash`), the value is sent to SQL Server
separately from the SQL text, in a data channel, and is never parsed as SQL
syntax - so the same input is just compared literally as a (wrong) password
and the login fails as expected.

Place your 4 screenshots in `docs/screenshots/` and reference them in your
README and Report.pdf.
