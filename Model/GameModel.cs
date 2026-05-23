using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Type_faster.Model
{
    public class GameModel
    {
        public int TotalXP { get; private set; }
        public bool IsRunning { get; private set; }
        public TimeSpan TimeLeft { get; private set; }

        public event Action<int>? OnXPChanged;
        public event Action<string>? OnWordChanged;
        public event Action<TimeSpan>? OnTimerChanged;
        public event Action<int>? OnGameEnded;

        private readonly Dictionary<char, double> _letterRarity = new()
        {
            {'е', 0.1}, {'о', 0.1}, {'а', 0.15}, {'и', 0.2}, {'н', 0.2}, {'т', 0.2},
            {'с', 0.25}, {'р', 0.3}, {'в', 0.3}, {'л', 0.3}, {'к', 0.4}, {'м', 0.4},
            {'д', 0.5}, {'п', 0.5}, {'у', 0.5}, {'я', 0.6}, {'ы', 0.6}, {'з', 0.7},
            {'ь', 0.8}, {'б', 0.8}, {'г', 0.8}, {'ч', 0.9}, {'й', 0.9}, {'х', 0.9},
            {'ж', 1.0}, {'ш', 1.0}, {'ц', 1.0}, {'щ', 1.0}, {'ф', 1.0}, {'э', 1.0}, {'ю', 1.0}, {'ъ', 1.0}
        };

        private readonly Dictionary<string, double> _wordRarity = new()
        {
            {"мама", 0.1}, {"программа", 0.7}, {"скорость", 0.8}, {"интерфейс", 0.9}, {"шифрование", 1.0}
        };

        private readonly List<string> _words = new() { "мама", "программа", "скорость", "интерфейс", "шифрование", "клавиатура", "разработка" };

        private int _wordIndex;
        private int _letterIndex;
        private int _currentWordBaseXP;
        private System.Windows.Forms.Timer _timer;

        public GameModel()
        {
            _timer = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };
            _timer.Tick += (s, e) => Tick();
        }

        public void StartGame()
        {
            TotalXP = 0;
            IsRunning = true;
            TimeLeft = TimeSpan.FromMinutes(5);
            _wordIndex = 0;
            ResetWordState();

            OnXPChanged?.Invoke(TotalXP);
            OnWordChanged?.Invoke(GetDisplayWord());
            OnTimerChanged?.Invoke(TimeLeft);

            _timer.Start();
        }

        private void Tick()
        {
            TimeLeft = TimeLeft.Add(TimeSpan.FromSeconds(-1));
            OnTimerChanged?.Invoke(TimeLeft);

            if (TimeLeft <= TimeSpan.Zero)
                EndGame();
        }

        private void EndGame()
        {
            _timer.Stop();
            IsRunning = false;
            OnGameEnded?.Invoke(TotalXP);
        }

        private void ResetWordState()
        {
            _letterIndex = 0;
            _currentWordBaseXP = 0;
        }

        private void NextWord()
        {
            _wordIndex = (_wordIndex + 1) % _words.Count;
            ResetWordState();
            OnWordChanged?.Invoke(GetDisplayWord());
        }

        public void ProcessInput(char typedChar)
        {
            if (!IsRunning) return;

            char target = _words[_wordIndex][_letterIndex];
            if (!_letterRarity.TryGetValue(target, out double rarity)) rarity = 0.5;

            if (char.ToLower(typedChar) == target)
            {
                int gain = (int)Math.Round(5 * (1 + rarity));
                _currentWordBaseXP += gain;
                TotalXP += gain;
                OnXPChanged?.Invoke(TotalXP);

                _letterIndex++;
                if (_letterIndex >= _words[_wordIndex].Length)
                {
                    double wordRar = _wordRarity.TryGetValue(_words[_wordIndex], out double wr) ? wr : 0.5;
                    int bonus = (int)Math.Round(_currentWordBaseXP * wordRar);
                    TotalXP += bonus;
                    OnXPChanged?.Invoke(TotalXP);

                    NextWord();
                }
                else
                {
                    OnWordChanged?.Invoke(GetDisplayWord());
                }
            }
            else
            {
                int penalty = (int)Math.Round(2 * (1 - rarity));
                TotalXP = Math.Max(0, TotalXP - penalty);
                OnXPChanged?.Invoke(TotalXP);
            }
        }

        private string GetDisplayWord()
        {
            string w = _words[_wordIndex];
            return w.Substring(0, _letterIndex) + "[" + w[_letterIndex] + "]" + w.Substring(_letterIndex + 1);
        }
    }
}
