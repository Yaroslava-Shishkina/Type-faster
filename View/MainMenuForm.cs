using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
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
        private PrivateFontCollection _fonts = new();
        private FontFamily? _pangolin;

        private static string LastDictPath =>
            Path.Combine(Application.StartupPath, "last_dictionary.txt");

        public MainMenuForm()
        {
            Text = "Type Faster";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.FromArgb(245, 245, 245);

            LoadFont();
            InitializeControls();
            LoadLastDictionary();
        }

        private void LoadFont()
        {
            string fontPath = Path.Combine(Application.StartupPath, "cosmetic", "Pangolin-Regular.ttf");
            if (File.Exists(fontPath))
            {
                _fonts.AddFontFile(fontPath);
                _pangolin = _fonts.Families[0];
            }
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

        private Font GetPangolinFont(float emSize, FontStyle style = FontStyle.Regular)
        {
            return _pangolin != null
                ? new Font(_pangolin, emSize, style)
                : new Font("Segoe UI", emSize * 0.6f, style);
        }

        private void InitializeControls()
        {
            int formW = ClientSize.Width;
            int btnW = 400;
            int btnH = 100;
            int titleH = 120;
            int x = (formW - btnW) / 2;

            var lblTitle = new Label
            {
                Text = "Type Faster!",
                Font = GetPangolinFont(48, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(btnW, titleH),
                Location = new Point(x, 80),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            btnStartGame = new Button
            {
                Text = "Начать игру",
                Font = GetPangolinFont(26),
                Size = new Size(btnW, btnH),
                Location = new Point(x, 240),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnStartGame.FlatAppearance.BorderSize = 0;

            btnSettings = new Button
            {
                Text = "Настройки",
                Font = GetPangolinFont(26),
                Size = new Size(btnW, btnH),
                Location = new Point(x, 360),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSettings.FlatAppearance.BorderSize = 0;

            btnStartGame.MouseEnter += Button_MouseEnter;
            btnStartGame.MouseLeave += Button_MouseLeave;
            btnSettings.MouseEnter += Button_MouseEnter;
            btnSettings.MouseLeave += Button_MouseLeave;

            Controls.Add(lblTitle);
            Controls.Add(btnStartGame);
            Controls.Add(btnSettings);

            btnStartGame.Click += BtnStartGame_Click;
            btnSettings.Click += BtnSettings_Click;
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = Color.FromArgb(90, 150, 200);
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = Color.FromArgb(70, 130, 180);
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
