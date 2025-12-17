using ParkingApp.Forms;
using System;
using System.Windows.Forms;

namespace ParkingApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new LoginForm());
            // hoặc DashboardForm()
        }
    }
}