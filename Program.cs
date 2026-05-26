using System;
using System.Windows.Forms;
using Type_faster.View;

namespace Type_faster
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainMenuForm());
        }
    }
}
