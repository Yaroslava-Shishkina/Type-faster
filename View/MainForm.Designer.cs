using System;
using System.Windows.Forms;

namespace Type_faster.View
{
    public partial class MainForm : Form, IGameView
    {
        private Label lblWord;
        private Label lblCombo;
        private Label lblEvent;
        private Label lblXP;
        private Label lblBestScore;
        private Label lblTimer;
        private Button btnStart;
        private TextBox txtDummy;
        private int _bestScore;

        private void InitializeComponent()
        {
            this.KeyPreview = true;
            this.Size = new System.Drawing.Size(400, 340);
            lblWord = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 24), 
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 60 };
            lblCombo = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 20, System.Drawing.FontStyle.Bold), 
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 45, ForeColor = System.Drawing.Color.Goldenrod, Visible = false, Text = "" };
            lblEvent = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 14, System.Drawing.FontStyle.Bold), 
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 35, ForeColor = System.Drawing.Color.OrangeRed, Visible = false, Text = "" };
            lblXP = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 18), 
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 40, Text = "XP: 0" };
            lblBestScore = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 14), 
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 25, Text = "Рекорд: 0" };
            lblTimer = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 16), 
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 35, Text = "Время: 00:30" };
            btnStart = new Button() { Text = "Начать игру", Font = new System.Drawing.Font("Consolas", 14), 
                Dock = DockStyle.Bottom, Height = 50 };
            txtDummy = new TextBox()
            {
                Visible = true,
                TabStop = false
            };
            txtDummy.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && btnStart.Text != "Начать игру")
                    OnKeyPressed?.Invoke(e.KeyChar);
            };

            Controls.Add(lblWord);
            Controls.Add(lblCombo);
            Controls.Add(lblEvent);
            Controls.Add(lblXP);
            Controls.Add(lblBestScore);
            Controls.Add(lblTimer);
            Controls.Add(btnStart);
            Controls.Add(txtDummy);

            btnStart.Click += (s, e) =>
            {
                OnStartClicked?.Invoke();
                btnStart.Text = "Идёт игра";
                btnStart.Enabled = false;

                txtDummy.Focus();
                txtDummy.Select(0, 0);
            };
        }

        public event Action OnStartClicked = delegate { };
        public event Action<char> OnKeyPressed = delegate { };

        public void UpdateWord(string displayWord) => lblWord.Text = displayWord;
        public void UpdateXP(int xp)
        {
            lblXP.Text = $"XP: {xp}";
            if (xp > _bestScore)
            {
                _bestScore = xp;
                lblBestScore.Text = "НОВЫЙ РЕКОРД!";
            }
        }
        public void UpdateCombo(string comboText)
        {
            lblCombo.Text = comboText;
            lblCombo.Visible = !string.IsNullOrEmpty(comboText);
        }

        public void UpdateEventMessage(string message)
        {
            lblEvent.Text = message;
            lblEvent.Visible = !string.IsNullOrEmpty(message);
        }

        public void UpdateTimer(TimeSpan timeLeft) => lblTimer.Text = $"Время: {timeLeft:mm\\:ss}";

        public void ShowEndScreen(int finalXP)
        {
            var repo = new Type_faster.Model.ScoreRepository(
                System.IO.Path.Combine(Application.StartupPath, "Dictionaries"));
            int prevBest = repo.LoadBestScore();
            bool isNewRecord = _bestScore > prevBest;
            if (isNewRecord)
                repo.SaveBestScore(_bestScore);

            string message = $"Раунд окончен!\nВаш итоговый опыт: {finalXP}\nРекорд: {(isNewRecord ? _bestScore : prevBest)}";
            if (isNewRecord)
                message += "\n\nНОВЫЙ РЕКОРД!";

            MessageBox.Show(message, "Игра завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);

            lblBestScore.Text = $"Рекорд: {(isNewRecord ? _bestScore : prevBest)}";
            _bestScore = isNewRecord ? _bestScore : prevBest;
            btnStart.Text = "Начать игру";
            btnStart.Enabled = true;
            lblWord.Text = "Готово!";
            lblTimer.Text = "Время: 00:00";
        }
    }
}