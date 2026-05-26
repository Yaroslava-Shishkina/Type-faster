using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Type_faster.Model;
using Type_faster.Presenter;

namespace Type_faster.View
{
    public class MainMenuForm : Form
    {
        private Button btnStartGame;
        private Button btnSettings;
        private List<string>? _selectedWords;

        private static string LastDictPath =>
            Path.Combine(Application.StartupPath, "last_dictionary.txt");

        public MainMenuForm()
        {
            Text = "Type Faster";
            Size = new System.Drawing.Size(300, 200);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            InitializeControls();
            LoadLastDictionary();
        }

        private void LoadLastDictionary()
        {
            if (!File.Exists(LastDictPath)) return;
            string name = File.ReadAllText(LastDictPath).Trim();
            if (string.IsNullOrEmpty(name)) return;
            var repo = new FileDictionaryRepository(
                Path.Combine(Application.StartupPath, "Dictionaries"));
            if (repo.DictionaryExists(name))
                _selectedWords = repo.LoadDictionary(name);
        }

        private void InitializeControls()
        {
            var lblTitle = new Label
            {
                Text = "Type Faster!",
                Font = new System.Drawing.Font("Consolas", 20, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            btnStartGame = new Button
            {
                Text = "Начать игру",
                Font = new System.Drawing.Font("Consolas", 14),
                Dock = DockStyle.Top,
                Height = 50
            };

            btnSettings = new Button
            {
                Text = "Настройки",
                Font = new System.Drawing.Font("Consolas", 14),
                Dock = DockStyle.Top,
                Height = 50
            };

            Controls.Add(btnSettings);
            Controls.Add(btnStartGame);
            Controls.Add(lblTitle);

            btnStartGame.Click += BtnStartGame_Click;
            btnSettings.Click += BtnSettings_Click;
        }

        private void BtnStartGame_Click(object sender, EventArgs e)
        {
            var model = new GameModel();
            if (_selectedWords != null)
                model.SetWords(_selectedWords);

            var repo = new FileDictionaryRepository(
                Path.Combine(Application.StartupPath, "Dictionaries"));
            var cityWords = repo.LoadDictionary("Сыграем в города");
            if (cityWords.Count > 0)
                model.SetCityWords(cityWords);

            var view = new MainForm();
            var presenter = new GamePresenter(view, model);
            view.ShowDialog(this);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog(this);
                if (settingsForm.SelectedDictionaryName != null)
                {
                    _selectedWords = settingsForm.SelectedWords;
                    File.WriteAllText(LastDictPath, settingsForm.SelectedDictionaryName);
                }
            }
        }
    }
}
