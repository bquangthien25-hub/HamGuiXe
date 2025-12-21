using ParkingApp.Forms;
using System;
using System.Windows.Forms;

namespace ParkingApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            // hoặc DashboardForm nếu muốn vào thẳng
        }
    }
}
