using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace Type_faster.View
{
    public class GameOverForm : Form
    {
        private static PrivateFontCollection _fonts = new();
        private static FontFamily? _pangolin;

        static GameOverForm()
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

        public GameOverForm(int finalXP, int bestScore, bool isNewRecord)
        {
            Text = "Игра завершена";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.FromArgb(245, 245, 245);

            var lblTitle = new Label
            {
                Text = "Раунд окончен!",
                Font = GetPangolinFont(36, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 120,
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            var lblScore = new Label
            {
                Text = $"Ваш итоговый опыт: {finalXP}",
                Font = GetPangolinFont(28),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                ForeColor = Color.DodgerBlue
            };

            var lblRecord = new Label
            {
                Text = $"Рекорд: {bestScore}",
                Font = GetPangolinFont(28),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                ForeColor = Color.DimGray
            };

            var btnOk = new Button
            {
                Text = "OK",
                Font = GetPangolinFont(24),
                Size = new Size(240, 100),
                Location = new Point((ClientSize.Width - 240) / 2, 420),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) => Close();

            if (isNewRecord)
            {
                var lblNewRecord = new Label
                {
                    Text = "НОВЫЙ РЕКОРД!",
                    Font = GetPangolinFont(30, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Height = 90,
                    ForeColor = Color.DodgerBlue
                };
                Controls.Add(lblNewRecord);
            }

            Controls.Add(lblTitle);
            Controls.Add(lblScore);
            Controls.Add(lblRecord);
            Controls.Add(btnOk);
        }
    }
}
