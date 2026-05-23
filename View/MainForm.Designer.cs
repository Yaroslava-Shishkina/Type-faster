using System;
using System.Windows.Forms;

namespace Type_faster.View
{
    public partial class MainForm : Form, IGameView
    {
        private Label lblWord;
        private Label lblXP;
        private Label lblTimer;
        private Button btnStart;
        private TextBox txtDummy;

        private void InitializeComponent()
        {
            this.KeyPreview = true;
            lblWord = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 24), TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 60 };
            lblXP = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 18), TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 40, Text = "XP: 0" };
            lblTimer = new Label() { AutoSize = false, Font = new System.Drawing.Font("Consolas", 16), TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 35, Text = "Время: 05:00" };
            btnStart = new Button() { Text = "Начать игру", Font = new System.Drawing.Font("Consolas", 14), Dock = DockStyle.Bottom, Height = 50 };
            txtDummy = new TextBox()
            {
                Visible = true,
                TabStop = false 
            };

            Controls.Add(lblWord);
            Controls.Add(lblXP);
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

                txtDummy.KeyPress += (s, e) =>
                {
                    if (!char.IsControl(e.KeyChar) && btnStart.Text != "Начать игру")
                        OnKeyPressed?.Invoke(e.KeyChar);
                };

                //txtDummy.KeyPress += (s, e) =>
                //{
                //    MessageBox.Show($"Нажата: {e.KeyChar}"); 
                //};
            };
        }

        //protected override void OnKeyPress(KeyPressEventArgs e)
        //{
        //    base.OnKeyPress(e);
        //    if (!char.IsControl(e.KeyChar) && btnStart.Text != "Начать игру") 
        //        OnKeyPressed?.Invoke(e.KeyChar);
        //}

        public event Action OnStartClicked = delegate { };
        public event Action<char> OnKeyPressed = delegate { };

        public void UpdateWord(string displayWord) => lblWord.Text = displayWord;
        public void UpdateXP(int xp) => lblXP.Text = $"XP: {xp}";
        public void UpdateTimer(TimeSpan timeLeft) => lblTimer.Text = $"Время: {timeLeft:mm\\:ss}";

        public void ShowEndScreen(int finalXP)
        {
            MessageBox.Show($"Раунд окончен!\nВаш итоговый опыт: {finalXP}", "Игра завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnStart.Text = "Начать игру";
            btnStart.Enabled = true;
            lblWord.Text = "Готово!";
            lblTimer.Text = "Время: 00:00";
        }
    }
}