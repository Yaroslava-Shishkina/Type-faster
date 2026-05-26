using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WordGame
{
    public partial class Form1 : Form
    {
        // Путь к папке со словарями (рядом с .exe)
        private readonly string _dictionariesFolder = Path.Combine(Application.StartupPath, "Dictionaries");

        // Текущий словарь, который будет использоваться в игре
        public List<string> currentDictionary = new List<string>();

        public Form1()
        {
            InitializeComponent();
            InitializeDictionaries();
            RefreshDictionaryList();
        }

        // 1. Инициализация папки и встроенного словаря
        private void InitializeDictionaries()
        {
            Directory.CreateDirectory(_dictionariesFolder);

            string standardPath = Path.Combine(_dictionariesFolder, "Стандартный.txt");
        }

        // 2. Обновление списка в ComboBox
        private void RefreshDictionaryList()
        {
            cmbDictionaries.Items.Clear();
            var files = Directory.GetFiles(_dictionariesFolder, "*.txt");
            foreach (var file in files)
            {
                cmbDictionaries.Items.Add(Path.GetFileNameWithoutExtension(file));
            }

            if (cmbDictionaries.Items.Count > 0)
                cmbDictionaries.SelectedIndex = 0;
        }

        // 3. Очистка и парсинг слов
        private List<string> ParseWords(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText)) return new List<string>();

            // Заменяем переносы строк и табы на пробелы
            string normalized = rawText.Replace('\n', ' ')
                                       .Replace('\r', ' ')
                                       .Replace('\t', ' ');

            // Разбиваем по пробелам
            string[] parts = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var part in parts)
            {
                // Оставляем только буквы (в т.ч. кириллицу), цифры и дефисы
                string clean = Regex.Replace(part, @"[^\p{L}\p{N}-]", "");
                if (!string.IsNullOrEmpty(clean))
                    result.Add(clean); 
            }

            return result;
        }

        // 4. Загрузка слов из файла в поле ввода
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Текстовые файлы|*.txt|Все файлы|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtWordInput.Text = File.ReadAllText(ofd.FileName);
                    lblStatus.Text = $"Файл загружен: {Path.GetFileName(ofd.FileName)}";
                }
            }
        }

        // 5. Сохранение нового словаря
        private void btnSaveDict_Click(object sender, EventArgs e)
        {
            string name = txtDictName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название словаря", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Убираем запрещённые символы из имени файла
            string safeName = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
            string filePath = Path.Combine(_dictionariesFolder, safeName + ".txt");

            var words = ParseWords(txtWordInput.Text);
            if (words.Count == 0)
            {
                MessageBox.Show("Не найдено ни одного слова. Проверьте ввод.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            File.WriteAllLines(filePath, words);
            txtDictName.Text = "";
            txtWordInput.Text = "";
            RefreshDictionaryList();
            cmbDictionaries.SelectedItem = safeName;
            lblStatus.Text = $"Словарь \"{safeName}\" сохранён ({words.Count} слов)";
        }

        // 6. Загрузка выбранного словаря в currentDictionary
        private void btnLoadToGame_Click(object sender, EventArgs e)
        {
            if (cmbDictionaries.SelectedItem == null) return;

            string selectedName = cmbDictionaries.SelectedItem.ToString();
            string filePath = Path.Combine(_dictionariesFolder, selectedName + ".txt");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл словаря не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string content = File.ReadAllText(filePath);
            currentDictionary = ParseWords(content);
            lblStatus.Text = $"Загружен словарь: {selectedName} | Слов: {currentDictionary.Count}";

        }

    }
}