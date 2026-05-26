using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Type_faster.View
{
    public class SettingsForm : Form
    {
        private Button btnDictManager;
        private Button btnResetData;

        public string? SelectedDictionaryName { get; private set; }
        public List<string>? SelectedWords { get; private set; }

        public SettingsForm()
        {
            Text = "Настройки";
            Size = new System.Drawing.Size(300, 200);
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            InitializeControls();
        }

        private void InitializeControls()
        {
            btnDictManager = new Button
            {
                Text = "Управление словарями",
                Font = new System.Drawing.Font("Consolas", 14),
                Dock = DockStyle.Top,
                Height = 50
            };

            btnResetData = new Button
            {
                Text = "Сбросить данные игрока",
                Font = new System.Drawing.Font("Consolas", 14),
                Dock = DockStyle.Top,
                Height = 50
            };

            Controls.Add(btnResetData);
            Controls.Add(btnDictManager);

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
