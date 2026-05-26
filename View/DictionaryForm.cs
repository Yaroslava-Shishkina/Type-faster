using System;
using System.Collections.Generic;
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

        public DictionaryForm()
        {
            Text = "Настройки — Управление словарями";
            Size = new System.Drawing.Size(450, 460);
            StartPosition = FormStartPosition.CenterParent;

            InitializeControls();

            string folder = Path.Combine(Application.StartupPath, "Dictionaries");
            _repository = new FileDictionaryRepository(folder);
            RefreshDictionaryList();
        }

        private void InitializeControls()
        {
            lblDictStatus = new Label { Dock = DockStyle.Top, Height = 20, Text = "" };
            lblAddDictPrompt = new Label { Dock = DockStyle.Top, Height = 30, Text = "Добавьте собственный словарь.", TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            lblDictName = new Label { Dock = DockStyle.Top, Height = 20, Text = "Название словаря:" };
            txtNewDictName = new TextBox { Dock = DockStyle.Top, Height = 30 };
            lblLoadInstructions = new Label { Dock = DockStyle.Top, Height = 20, Text = "Загрузите файл или вставьте слова в поле ввода." };
            txtWordsInput = new TextBox { Dock = DockStyle.Top, Height = 80, Multiline = true };
            btnLoadFile = new Button { Text = "Загрузить из файла", Dock = DockStyle.Top, Height = 30 };
            var spacer = new Label { Dock = DockStyle.Top, Height = 20 };
            btnSaveDictionary = new Button { Text = "Сохранить словарь", Dock = DockStyle.Top, Height = 30 };
            lblUsedDictionary = new Label { Dock = DockStyle.Top, Height = 20, Text = "Используемый словарь:" };
            cmbDictionaries = new ComboBox { Dock = DockStyle.Top, Height = 30, DropDownStyle = ComboBoxStyle.DropDownList };
            btnLoadDictionary = new Button { Text = "Загрузить в игру", Dock = DockStyle.Top, Height = 30 };

            Controls.Add(btnLoadDictionary);
            Controls.Add(cmbDictionaries);
            Controls.Add(lblUsedDictionary);
            Controls.Add(btnSaveDictionary);
            Controls.Add(spacer);
            Controls.Add(btnLoadFile);
            Controls.Add(txtWordsInput);
            Controls.Add(lblLoadInstructions);
            Controls.Add(txtNewDictName);
            Controls.Add(lblDictName);
            Controls.Add(lblAddDictPrompt);
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
            lblDictStatus.ForeColor = System.Drawing.Color.Green;
            lblDictStatus.Text = msg;
        }

        private void SetError(string msg)
        {
            lblDictStatus.ForeColor = System.Drawing.Color.Red;
            lblDictStatus.Text = msg;
        }
    }
}
