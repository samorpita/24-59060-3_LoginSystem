# 24-59060-3 Login System

A WinForms + SQL Server application implementing Registration, Login, and
Logout, built for Lab 1.

## Environment

- **SQL Server**: LocalDB (`(localdb)\MSSQLLocalDB`) - change the `Data
  Source` in `App.config` if you use SQL Express (`.\SQLEXPRESS`) or a full
  instance instead.
- **Visual Studio**: 2022
- **.NET**: .NET Framework 4.7.2 (WinForms)
- **Connection string format** (in `App.config`, no real password committed
  since we use `Integrated Security=True`, i.e. Windows auth):
  ```
  Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=24-59060-3_LoginDB;Integrated Security=True;Connect Timeout=30;
  ```

## How the database was created

Ran `Schema.sql` in SSMS / Visual Studio's SQL Server Object Explorer. It:
1. Creates `24-59060-3_LoginDB` if it doesn't already exist.
2. Creates `dbo.Users` with `UserID` (identity PK), `Username` (unique),
   `PasswordHash`, `Email`, `FullName`, `CreatedAt` (defaults to `GETDATE()`).

## How Registration, Login and Logout work

**Classes:**
- `DatabaseHelper` - the only class that touches `SqlConnection`/`SqlCommand`.
  Every query is parameterized. Forms never build SQL themselves.
- `PasswordHasher` - SHA-256 hashing (`Hash`) and comparison (`Verify`).
- `LoginForm`, `RegistrationForm`, `HomeForm` - the three UI screens.

**Registration** (`RegistrationForm.btnRegister_Click`):
1. Validates: no empty fields, password ≥ 6 chars, password == confirm
   password, email contains `@` (if provided).
2. Calls `DatabaseHelper.UsernameExists` (an `ExecuteScalar` `COUNT(*)`
   query) to reject duplicate usernames before attempting an insert.
3. Hashes the password with `PasswordHasher.Hash` and calls
   `DatabaseHelper.RegisterUser`, which runs a parameterized
   `ExecuteNonQuery()` INSERT. The plain-text password is never sent to the
   database.
4. On success: shows a message box, clears the form, and closes back to
   `LoginForm`.

**Login** (`LoginForm.btnLogin_Click`):
1. Calls `DatabaseHelper.TryGetUserForLogin`, which runs a parameterized
   `SqlDataReader` query for the user's stored hash and full name.
2. Compares the typed password against the stored hash via
   `PasswordHasher.Verify` - plain text is never compared.
3. On success: opens `HomeForm` with `"Welcome, {FullName}"` and hides
   `LoginForm`.
4. On failure: shows a message box; after 3 failed attempts in a row,
   disables the Login button (`failedAttempts` counter in `LoginForm`).

**Logout** (`HomeForm.btnLogout_Click`):
- Closes `HomeForm` only (`this.Close()`), never calls `Application.Exit()`.
  `LoginForm` subscribes to `HomeForm.FormClosed`, which clears its textboxes
  and re-shows itself with focus on the username field - so the app returns
  to a clean login screen and keeps running, with no orphan forms.

## Password hashing

Passwords are hashed with **SHA-256** (`PasswordHasher.cs`) before ever
reaching the database. At registration we store only the hash; at login we
hash the typed password and compare hash-to-hash. Plain text is unacceptable
because if the database is ever leaked or read by anyone with access, plain
text passwords are immediately usable for every user's account (and likely
their other accounts, since people reuse passwords). A hash cannot be
reversed back into the original password.

## SQL Injection Demo (Task 6)

See `docs/injection-demo/HOW_TO_DEMO.md` for the vulnerable snippet, the
exploit input (`' OR '1'='1` in the password field), and full steps. Summary:

- **Vulnerable code**: builds the SQL by string concatenation of the raw
  textbox values.
- **Exploit**: typing `' OR '1'='1` as the password turns the WHERE clause
  into something that's always true, returning every row and logging in with
  no valid password.
- **Fixed code**: `DatabaseHelper.TryGetUserForLogin` uses a parameterized
  query (`@username`), and the password is verified via hash comparison in
  C#, not by concatenating it into SQL at all.
- **Why parameters stop it**: parameter values are sent to SQL Server
  separately from the SQL text and are never parsed as SQL syntax, so the
  same malicious input is just treated as literal data (a wrong value)
  instead of code.

Screenshots: `docs/screenshots/injection_before_exploit.png`,
`injection_vulnerable_code.png`, `injection_after_fix.png`.

## Bonus tasks attempted

- [x] Moved all database code out of the forms into `DatabaseHelper` - forms
      never touch `SqlConnection` directly.
- [ ] *(fill in second bonus task if you attempt one, e.g. change-password
  screen, or search/filter grid by username)*

## Problems hit and how they were solved

*(Fill this in honestly from your own experience running it - e.g. LocalDB
service not started, connection string mismatch, etc. Use the
Troubleshooting section of the assignment brief as a starting point, but
describe what actually happened on your machine.)*

## Screenshots

See `docs/screenshots/`.
