using System;
using System.Collections.Generic;

namespace Type_faster.Model
{
    public class CityEvent : GameEvent
    {
        private bool _hadMistake;
        private int _bonusTimeLeft;
        private List<string> _savedWords;

        public override string Name => "Город";

        public override bool CanTrigger(GameModel model)
        {
            if (IsActive || IsBonusActive) return false;
            return model.CityWords != null && model.CityWords.Count > 0
                && model.Random.Next(6) == 0;
        }

        public override void Trigger(GameModel model)
        {
            IsActive = true;
            IsBonusActive = false;
            _hadMistake = false;
            _savedWords = new List<string>(model.Words);

            model.Words = new List<string> { "город" };
            model.WordIndex = 0;
            model.ResetWordState();
            model.ShowEventMessage("Случайное событие! Введите «город» без ошибок!");
        }

        public override void OnPlayerMistake(GameModel model)
        {
            _hadMistake = true;
        }

        public override void OnEventWordCompleted(GameModel model)
        {
            if (!_hadMistake)
            {
                IsActive = false;
                IsBonusActive = true;
                _bonusTimeLeft = 7;

                model.AddTime(7);
                model.Words = model.CityWords;
                model.WordIndex = model.Random.Next(model.Words.Count);
                model.ResetWordState();
                model.ShowEventMessage("+7 секунд! Режим городов включён!");
            }
            else
            {
                model.ShowEventMessage("Не в этот раз...");
                model.Words = _savedWords;
                model.WordIndex = model.Random.Next(model.Words.Count);
                model.ResetWordState();
                End(model);
            }
        }

        public override void OnTick(GameModel model)
        {
            if (IsBonusActive)
            {
                _bonusTimeLeft--;
                if (_bonusTimeLeft > 0)
                    model.ShowEventMessage($"Режим городов: {_bonusTimeLeft} сек");
                else
                {
                    model.ShowEventMessage("Режим городов завершён");
                    End(model);
                }
            }
        }

        public override void End(GameModel model)
        {
            if (IsActive || IsBonusActive)
            {
                if (IsBonusActive)
                {
                    model.Words = _savedWords;
                    model.WordIndex = model.Random.Next(model.Words.Count);
                    model.ResetWordState();
                }
                IsActive = false;
                IsBonusActive = false;
                model.RefreshWordDisplay();
                model.ShowEventMessage("");
            }
        }
    }
}
