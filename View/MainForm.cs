using System.IO;
using Type_faster.Model;

namespace Type_faster.View
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadBestScore();
        }

        private void LoadBestScore()
        {
            var repo = new ScoreRepository(
                Path.Combine(Application.StartupPath, "Dictionaries"));
            _bestScore = repo.LoadBestScore();
            lblBestScore.Text = $"Рекорд: {_bestScore}";
        }
    }
}
