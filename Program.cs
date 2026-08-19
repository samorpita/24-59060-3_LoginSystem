using System;
using System.Windows.Forms;

namespace LoginSystem_24590603
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // The app should NOT exit when a user logs out - it should only
            // exit when the LoginForm itself is closed (e.g. window X button).
            Application.Run(new LoginForm());
        }
    }
}
