using System;
using System.Collections.Generic;

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
        public event Action<string>? OnComboChanged;
        public event Action<string>? OnEventMessage;

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

        private readonly Random _random = new();
        internal Random Random => _random;

        private List<string> _words = new() { "мама", "программа", "скорость", "интерфейс", "шифрование", "клавиатура", "разработка" };
        internal List<string> Words { get => _words; set => _words = value; }

        public List<string>? CityWords { get; private set; }

        public void SetWords(List<string> words)
        {
            if (words != null && words.Count > 0)
                _words = words;
        }

        public void SetCityWords(List<string> words) => CityWords = words;

        private int _wordIndex;
        internal int WordIndex { get => _wordIndex; set => _wordIndex = value; }
        private int _letterIndex;
        private int _currentWordBaseXP;
        private int _comboCount;
        private readonly EventManager _eventManager = new();
        private System.Windows.Forms.Timer _timer;

        public GameModel()
        {
            _timer = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };
            _timer.Tick += (s, e) => Tick();
            _eventManager.Register(new CityEvent());
        }

        public void StartGame()
        {
            TotalXP = 0;
            IsRunning = true;
            TimeLeft = TimeSpan.FromSeconds(30);
            _wordIndex = _random.Next(_words.Count);
            _comboCount = 0;
            ResetWordState();

            _eventManager.EndAll(this);

            OnXPChanged?.Invoke(TotalXP);
            OnWordChanged?.Invoke(GetDisplayWord());
            OnTimerChanged?.Invoke(TimeLeft);
            OnComboChanged?.Invoke("");
            OnEventMessage?.Invoke("");

            _timer.Start();
        }

        private void Tick()
        {
            TimeLeft = TimeLeft.Add(TimeSpan.FromSeconds(-1));
            OnTimerChanged?.Invoke(TimeLeft);

            _eventManager.OnTick(this);

            if (TimeLeft <= TimeSpan.Zero)
                EndGame();
        }

        private void EndGame()
        {
            _timer.Stop();
            IsRunning = false;
            _eventManager.EndAll(this);
            OnEventMessage?.Invoke("");
            OnGameEnded?.Invoke(TotalXP);
        }

        internal void ResetWordState()
        {
            _letterIndex = 0;
            _currentWordBaseXP = 0;
        }

        internal void ShowEventMessage(string message) => OnEventMessage?.Invoke(message);

        internal void RefreshWordDisplay() => OnWordChanged?.Invoke(GetDisplayWord());

        internal void AddTime(int seconds)
        {
            TimeLeft = TimeLeft.Add(TimeSpan.FromSeconds(seconds));
            OnTimerChanged?.Invoke(TimeLeft);
        }

        private void NextWord()
        {
            _wordIndex = _random.Next(_words.Count);
            ResetWordState();
            OnWordChanged?.Invoke(GetDisplayWord());
        }

        public void ProcessInput(char typedChar)
        {
            if (!IsRunning) return;

            char target = _words[_wordIndex][_letterIndex];
            if (!_letterRarity.TryGetValue(target, out double rarity)) rarity = 0.5;

            if (typedChar == target)
            {
                _comboCount++;
                double mult = GetComboMultiplier();
                double rarityBonus = _eventManager.ActiveEvent?.IsBonusActive == true ? 0.1 : 0;
                int gain = (int)Math.Round(5 * (1 + rarity + rarityBonus) * mult);
                _currentWordBaseXP += gain;
                TotalXP += gain;
                OnXPChanged?.Invoke(TotalXP);
                OnComboChanged?.Invoke(GetComboDisplayText());

                _letterIndex++;
                if (_letterIndex >= _words[_wordIndex].Length)
                {
                    double wordRar = _wordRarity.TryGetValue(_words[_wordIndex], out double wr) ? wr : 0.5;
                    int bonus = (int)Math.Round(_currentWordBaseXP * wordRar);

                    if (_eventManager.ActiveEvent?.IsBonusActive == true)
                        bonus *= 2;

                    TotalXP += bonus;
                    OnXPChanged?.Invoke(TotalXP);

                    if (_eventManager.ActiveEvent?.IsActive == true)
                    {
                        _eventManager.OnWordCompleted(this);
                        OnWordChanged?.Invoke(GetDisplayWord());
                    }
                    else if (!_eventManager.TryTrigger(this))
                    {
                        NextWord();
                    }
                    else
                    {
                        OnWordChanged?.Invoke(GetDisplayWord());
                    }
                }
                else
                {
                    OnWordChanged?.Invoke(GetDisplayWord());
                }
            }
            else
            {
                _comboCount = 0;
                OnComboChanged?.Invoke("");

                _eventManager.ActiveEvent?.OnPlayerMistake(this);

                int penalty = (int)Math.Round(2 * (1 - rarity));
                TotalXP = Math.Max(0, TotalXP - penalty);
                OnXPChanged?.Invoke(TotalXP);
            }
        }

        private double GetComboMultiplier()
        {
            if (_comboCount >= 50) return 1.3;
            if (_comboCount >= 20) return 1.2;
            if (_comboCount >= 10) return 1.1;
            return 1.0;
        }

        private string GetComboDisplayText()
        {
            if (_comboCount >= 50) return "КОМБО x50!!!";
            if (_comboCount >= 20) return "КОМБО x20!!";
            if (_comboCount >= 10) return "КОМБО x10!";
            return "";
        }

        private string GetDisplayWord()
        {
            string w = _words[_wordIndex];
            return w.Substring(0, _letterIndex) + "[" + w[_letterIndex] + "]" + w.Substring(_letterIndex + 1);
        }
    }
}
