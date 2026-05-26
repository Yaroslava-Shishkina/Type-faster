using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Type_faster.Model
{
    public interface IDictionaryRepository
    {
        IEnumerable<string> GetDictionaryNames();
        List<string> LoadDictionary(string name);
        void SaveDictionary(string name, List<string> words);
        bool DictionaryExists(string name);
    }

    public class FileDictionaryRepository : IDictionaryRepository
    {
        private readonly string _folder;

        public FileDictionaryRepository(string folderPath)
        {
            _folder = folderPath;
            Directory.CreateDirectory(_folder);
            EnsureDefaultDictionary();
        }

        private void EnsureDefaultDictionary()
        {
            string defaultPath = Path.Combine(_folder, "Стандартный.txt");
            if (!File.Exists(defaultPath))
            {
                var defaultWords = new[] { "яблоко", "груша", "банан", "виноград", "апельсин" };
                File.WriteAllLines(defaultPath, defaultWords);
            }

            string cityPath = Path.Combine(_folder, "Сыграем в города.txt");
            if (!File.Exists(cityPath))
            {
                var cityWords = new[]
                {
                    "Москва", "Париж", "Лондон", "Токио", "Нью-Йорк",
                    "Берлин", "Рим", "Мадрид", "Вена", "Прага",
                    "Афины", "Каир", "Пекин", "Сеул", "Сидней",
                    "Оттава", "Бразилиа", "Дели", "Стокгольм", "Осло",
                    "Хельсинки", "Дублин", "Копенгаген", "Варшава", "Будапешт",
                    "Лиссабон", "Бангкок", "Сингапур", "Куала-Лумпур", "Джакарта"
                };
                File.WriteAllLines(cityPath, cityWords);
            }
        }

        public IEnumerable<string> GetDictionaryNames()
        {
            return Directory.GetFiles(_folder, "*.txt")
                            .Select(f => Path.GetFileNameWithoutExtension(f));
        }

        public List<string> LoadDictionary(string name)
        {
            string path = Path.Combine(_folder, name + ".txt");
            if (!File.Exists(path)) return new List<string>();
            return File.ReadAllLines(path)
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .ToList();
        }

        public void SaveDictionary(string name, List<string> words)
        {
            string safeName = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
            string path = Path.Combine(_folder, safeName + ".txt");
            File.WriteAllLines(path, words);
        }

        public bool DictionaryExists(string name)
        {
            string safeName = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
            return File.Exists(Path.Combine(_folder, safeName + ".txt"));
        }
    }
}