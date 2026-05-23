using System;
using System.Windows.Forms;
using Type_faster.Model;
using Type_faster.Presenter;
using Type_faster.View;

namespace Type_faster
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var model = new GameModel();
            var view = new MainForm();
            var presenter = new GamePresenter(view, model);

            Application.Run(view);
        }
    }
}
