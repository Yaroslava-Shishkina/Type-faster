using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace Type_faster.View
{
    public class SettingsForm : Form
    {
        private Button btnDictManager;
        private Button btnResetData;
        private static PrivateFontCollection _fonts = new();
        private static FontFamily? _pangolin;

        static SettingsForm()
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

        public string? SelectedDictionaryName { get; private set; }
        public List<string>? SelectedWords { get; private set; }

        public SettingsForm()
        {
            Text = "Настройки";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.FromArgb(245, 245, 245);

            InitializeControls();
        }

        private void InitializeControls()
        {
            var lblTitle = new Label
            {
                Text = "Настройки",
                Font = GetPangolinFont(36, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 170,
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            btnDictManager = new Button
            {
                Text = "Управление словарями",
                Font = GetPangolinFont(24),
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDictManager.FlatAppearance.BorderSize = 0;
            btnDictManager.MouseEnter += (s, e) => btnDictManager.BackColor = Color.FromArgb(90, 150, 200);
            btnDictManager.MouseLeave += (s, e) => btnDictManager.BackColor = Color.FromArgb(70, 130, 180);

            btnResetData = new Button
            {
                Text = "Сбросить данные игрока",
                Font = GetPangolinFont(24),
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnResetData.FlatAppearance.BorderSize = 0;
            btnResetData.MouseEnter += (s, e) => btnResetData.BackColor = Color.FromArgb(90, 150, 200);
            btnResetData.MouseLeave += (s, e) => btnResetData.BackColor = Color.FromArgb(70, 130, 180);

            Controls.Add(btnResetData);
            Controls.Add(btnDictManager);
            Controls.Add(lblTitle);

            btnDictManager.Click += BtnDictManager_Click;
            btnResetData.Click += BtnResetData_Click;
        }

        private void BtnDictManager_Click(object sender, EventArgs e)
        {
            using (var dictForm = new DictionaryForm())
            {
                dictForm.ShowDialog(this);
                if (dictForm.SelectedDictionaryName != null)
                {
                    SelectedDictionaryName = dictForm.SelectedDictionaryName;
                    SelectedWords = dictForm.SelectedWords;
                }
            }
        }

        private void BtnResetData_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Вы уверены, что хотите сбросить рекорд?",
                "Сброс данных",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            string scorePath = Path.Combine(Application.StartupPath, "Dictionaries", "best_score.txt");
            if (File.Exists(scorePath))
                File.Delete(scorePath);

            MessageBox.Show("Рекорд сброшен.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
