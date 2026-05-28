using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Type_faster.Model;

namespace Type_faster.View
{
    public class DictionaryForm : Form
    {
        private readonly FileDictionaryRepository _repository;
        private Label lblDictStatus;
        private Label lblAddDictPrompt;
        private Label lblDictName;
        private TextBox txtNewDictName;
        private Label lblLoadInstructions;
        private TextBox txtWordsInput;
        private Button btnLoadFile;
        private Button btnSaveDictionary;
        private Label lblUsedDictionary;
        private ComboBox cmbDictionaries;
        private Button btnLoadDictionary;
        private PrivateFontCollection _fonts = new();
        private FontFamily? _pangolin;

        public DictionaryForm()
        {
            Text = "Настройки — Управление словарями";
            Size = new Size(900, 890);
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = Color.FromArgb(245, 245, 245);

            LoadFont();
            InitializeControls();

            string folder = Path.Combine(Application.StartupPath, "Dictionaries");
            _repository = new FileDictionaryRepository(folder);
            RefreshDictionaryList();
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

        private Font GetPangolinFont(float emSize, FontStyle style = FontStyle.Regular)
        {
            return _pangolin != null
                ? new Font(_pangolin, emSize, style)
                : new Font("Segoe UI", emSize * 0.6f, style);
        }

        private void InitializeControls()
        {
            lblDictStatus = new Label
            {
                Dock = DockStyle.Top,
                Height = 40,
                Text = "",
                Font = GetPangolinFont(16),
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblAddDictPrompt = new Label
            {
                Dock = DockStyle.Top,
                Height = 53,
                Text = "Добавьте собственный словарь.",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GetPangolinFont(20, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            lblDictName = new Label
            {
                Dock = DockStyle.Top,
                Height = 33,
                Text = "Название словаря:",
                Font = GetPangolinFont(18),
                ForeColor = Color.DimGray
            };
            txtNewDictName = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 35,
                Font = GetPangolinFont(18)
            };
            lblLoadInstructions = new Label
            {
                Dock = DockStyle.Top,
                Height = 33,
                Text = "Загрузите файл или вставьте слова в поле ввода.",
                Font = GetPangolinFont(16),
                ForeColor = Color.DimGray
            };
            txtWordsInput = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 200,
                Multiline = true,
                Font = GetPangolinFont(16),
                ScrollBars = ScrollBars.Vertical
            };
            btnLoadFile = new Button
            {
                Text = "Загрузить из файла",
                Dock = DockStyle.Top,
                Height = 60,
                Font = GetPangolinFont(20),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadFile.FlatAppearance.BorderSize = 0;
            btnLoadFile.MouseEnter += (s, e) => btnLoadFile.BackColor = Color.FromArgb(90, 150, 200);
            btnLoadFile.MouseLeave += (s, e) => btnLoadFile.BackColor = Color.FromArgb(70, 130, 180);

            var spacer = new Label { Dock = DockStyle.Top, Height = 20 };

            btnSaveDictionary = new Button
            {
                Text = "Сохранить словарь",
                Dock = DockStyle.Top,
                Height = 60,
                Font = GetPangolinFont(20),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveDictionary.FlatAppearance.BorderSize = 0;
            btnSaveDictionary.MouseEnter += (s, e) => btnSaveDictionary.BackColor = Color.FromArgb(90, 150, 200);
            btnSaveDictionary.MouseLeave += (s, e) => btnSaveDictionary.BackColor = Color.FromArgb(70, 130, 180);

            lblUsedDictionary = new Label
            {
                Dock = DockStyle.Top,
                Height = 33,
                Text = "Используемый словарь:",
                Font = GetPangolinFont(18),
                ForeColor = Color.DimGray
            };
            cmbDictionaries = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 35,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = GetPangolinFont(18)
            };
            btnLoadDictionary = new Button
            {
                Text = "Загрузить в игру",
                Dock = DockStyle.Top,
                Height = 60,
                Font = GetPangolinFont(20),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadDictionary.FlatAppearance.BorderSize = 0;
            btnLoadDictionary.MouseEnter += (s, e) => btnLoadDictionary.BackColor = Color.FromArgb(90, 150, 200);
            btnLoadDictionary.MouseLeave += (s, e) => btnLoadDictionary.BackColor = Color.FromArgb(70, 130, 180);

            Controls.Add(btnLoadDictionary);
            Controls.Add(cmbDictionaries);
            Controls.Add(lblUsedDictionary);
            Controls.Add(new Label { Dock = DockStyle.Top, Height = 15 });
            Controls.Add(btnSaveDictionary);
            Controls.Add(spacer);
            Controls.Add(btnLoadFile);
            Controls.Add(txtWordsInput);
            Controls.Add(lblLoadInstructions);
            Controls.Add(txtNewDictName);
            Controls.Add(lblDictName);
            Controls.Add(lblAddDictPrompt);
            Controls.Add(new Label { Dock = DockStyle.Top, Height = 5 });
            Controls.Add(lblDictStatus);

            btnLoadDictionary.Click += (s, e) => LoadSelectedDictionary();
            btnSaveDictionary.Click += (s, e) => SaveCurrentDictionary();
            btnLoadFile.Click += (s, e) => LoadFromFile();
        }

        private void RefreshDictionaryList()
        {
            cmbDictionaries.Items.Clear();

            var names = _repository.GetDictionaryNames().ToList();

            string lastDictPath = Path.Combine(Application.StartupPath, "last_dictionary.txt");
            string lastUsedName = null;
            if (File.Exists(lastDictPath))
            {
                lastUsedName = File.ReadAllText(lastDictPath).Trim();
            }

            if (lastUsedName != null && names.Contains(lastUsedName))
            {
                cmbDictionaries.Items.Add(lastUsedName);
                foreach (var name in names)
                    if (name != lastUsedName)
                        cmbDictionaries.Items.Add(name);
            }
            else
            {
                foreach (var name in names)
                    cmbDictionaries.Items.Add(name);
            }

            if (cmbDictionaries.Items.Count > 0)
                cmbDictionaries.SelectedIndex = 0;
        }

        public List<string>? SelectedWords { get; private set; }
        public string? SelectedDictionaryName { get; private set; }

        private void LoadSelectedDictionary()
        {
            if (cmbDictionaries.SelectedItem == null) return;
            string name = cmbDictionaries.SelectedItem.ToString();
            var words = _repository.LoadDictionary(name);
            txtWordsInput.Text = string.Join(Environment.NewLine, words);
            SelectedWords = words;
            SelectedDictionaryName = name;
            SetStatus("Загружен: " + name);
        }

        private void SaveCurrentDictionary()
        {
            string name = txtNewDictName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                SetError("Введите название словаря");
                return;
            }
            var words = WordParser.Parse(txtWordsInput.Text);
            _repository.SaveDictionary(name, words);
            RefreshDictionaryList();
            SetStatus("Словарь сохранён: " + name);
        }

        private void LoadFromFile()
        {
            using (var dialog = new OpenFileDialog { Filter = "Text Files|*.txt", Multiselect = false })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string content = File.ReadAllText(dialog.FileName);
                    var words = WordParser.Parse(content);
                    txtWordsInput.Text = string.Join(Environment.NewLine, words);
                    txtNewDictName.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
                    SetStatus("Загружен файл: " + dialog.FileName);
                }
            }
        }

        private void SetStatus(string msg)
        {
            lblDictStatus.ForeColor = Color.Green;
            lblDictStatus.Text = msg;
        }

        private void SetError(string msg)
        {
            lblDictStatus.ForeColor = Color.Red;
            lblDictStatus.Text = msg;
        }
    }
}
