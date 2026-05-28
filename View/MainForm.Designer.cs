using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
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
        private PrivateFontCollection _fonts = new();
        private FontFamily? _pangolin;

        private void LoadFont()
        {
            string fontPath = Path.Combine(Application.StartupPath, "cosmetic", "Pangolin-Regular.ttf");
            if (File.Exists(fontPath))
            {
                _fonts.AddFontFile(fontPath);
                _pangolin = _fonts.Families[0];
            }
        }

        private Font GetPangolinFont(float emSize, FontStyle style = FontStyle.Regular)
        {
            return _pangolin != null
                ? new Font(_pangolin, emSize, style)
                : new Font("Segoe UI", emSize * 0.6f, style);
        }

        private void InitializeComponent()
        {
            this.KeyPreview = true;
            this.Size = new Size(1200, 900);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadFont();

            int cw = ClientSize.Width;

            var topPanel = new Panel { Height = 60, Dock = DockStyle.Top };

            lblTimer = new Label()
            {
                AutoSize = false,
                Font = GetPangolinFont(20),
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(200, 50),
                Location = new Point(50, 5),
                ForeColor = Color.DimGray,
                Text = "Время: 00:30"
            };

            lblBestScore = new Label()
            {
                AutoSize = false,
                Font = GetPangolinFont(18),
                TextAlign = ContentAlignment.MiddleRight,
                Size = new Size(260, 50),
                Location = new Point(cw - 310, 5),
                ForeColor = Color.DimGray,
                Text = "Рекорд: 0"
            };

            topPanel.Controls.Add(lblTimer);
            topPanel.Controls.Add(lblBestScore);

            lblXP = new Label()
            {
                AutoSize = false,
                Font = GetPangolinFont(32, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 80),
                Location = new Point((cw - 400) / 2, 80),
                ForeColor = Color.DodgerBlue,
                Text = "XP: 0"
            };

            lblWord = new Label()
            {
                AutoSize = false,
                Font = GetPangolinFont(52, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(800, 200),
                Location = new Point((cw - 800) / 2, 210),
                ForeColor = Color.Black
            };

            lblCombo = new Label()
            {
                AutoSize = false,
                Font = GetPangolinFont(28, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(600, 70),
                Location = new Point((cw - 600) / 2, 430),
                ForeColor = Color.DodgerBlue,
                Visible = false,
                Text = ""
            };

            lblEvent = new Label()
            {
                AutoSize = false,
                Font = GetPangolinFont(22, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(1000, 60),
                Location = new Point((cw - 1000) / 2, 510),
                ForeColor = Color.Black
            };

            btnStart = new Button()
            {
                Text = "Начать игру",
                Font = GetPangolinFont(24),
                Size = new Size(300, 70),
                Location = new Point((cw - 300) / 2, 600),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnStart.FlatAppearance.BorderSize = 0;

            btnStart.MouseEnter += (s, e) => btnStart.BackColor = Color.FromArgb(90, 150, 200);
            btnStart.MouseLeave += (s, e) => btnStart.BackColor = Color.FromArgb(70, 130, 180);

            txtDummy = new TextBox()
            {
                Location = new Point(0, 0),
                Size = new Size(100, 22),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(245, 245, 245),
                TabStop = false
            };

            txtDummy.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && btnStart.Text != "Начать игру")
                {
                    char expected = GetExpectedChar();
                    txtDummy.BackColor = e.KeyChar == expected
                        ? Color.LightGreen
                        : Color.LightCoral;
                    OnKeyPressed?.Invoke(e.KeyChar);
                }
            };

            Controls.Add(topPanel);
            Controls.Add(lblXP);
            Controls.Add(lblCombo);
            Controls.Add(lblEvent);
            Controls.Add(lblWord);
            Controls.Add(txtDummy);
            Controls.Add(btnStart);

            btnStart.Click += (s, e) =>
            {
                OnStartClicked?.Invoke();
                btnStart.Text = "Идёт игра";
                btnStart.Enabled = false;
                txtDummy.BackColor = Color.White;
                txtDummy.Focus();
                txtDummy.Select(0, 0);
            };
        }

        private char GetExpectedChar()
        {
            string text = lblWord.Text;
            int start = text.IndexOf('[');
            int end = text.IndexOf(']', start + 1);
            if (start >= 0 && end > start + 1)
                return text[start + 1];
            return '\0';
        }

        public event Action OnStartClicked = delegate { };
        public event Action<char> OnKeyPressed = delegate { };

        public void UpdateWord(string displayWord)
        {
            lblWord.Text = displayWord;
            txtDummy.BackColor = Color.White;
        }

        public void UpdateXP(int xp)
        {
            lblXP.Text = $"XP: {xp}";
            if (xp > _bestScore)
            {
                _bestScore = xp;
                lblBestScore.Text = "НОВЫЙ РЕКОРД!";
                lblBestScore.ForeColor = Color.DodgerBlue;
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

            using (var form = new GameOverForm(finalXP, isNewRecord ? _bestScore : prevBest, isNewRecord))
                form.ShowDialog(this);

            lblBestScore.Text = $"Рекорд: {(isNewRecord ? _bestScore : prevBest)}";
            lblBestScore.ForeColor = Color.DimGray;
            _bestScore = isNewRecord ? _bestScore : prevBest;
            btnStart.Text = "Начать игру";
            btnStart.Enabled = true;
            lblWord.Text = "Готово!";
            lblTimer.Text = "Время: 00:00";
            txtDummy.BackColor = Color.White;
        }
    }
}