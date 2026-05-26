using System.Collections.Generic;

namespace Type_faster.Model
{
    public class EventManager
    {
        private readonly List<GameEvent> _events = new();
        public GameEvent? ActiveEvent { get; private set; }

        public void Register(GameEvent e) => _events.Add(e);

        public bool TryTrigger(GameModel model)
        {
            foreach (var e in _events)
            {
                if (e.CanTrigger(model))
                {
                    e.Trigger(model);
                    ActiveEvent = e;
                    return true;
                }
            }
            return false;
        }

        public void OnWordCompleted(GameModel model)
        {
            ActiveEvent?.OnEventWordCompleted(model);
            if (ActiveEvent != null && !ActiveEvent.IsActive && !ActiveEvent.IsBonusActive)
                ActiveEvent = null;
        }

        public void OnTick(GameModel model)
        {
            ActiveEvent?.OnTick(model);
            if (ActiveEvent != null && !ActiveEvent.IsActive && !ActiveEvent.IsBonusActive)
                ActiveEvent = null;
        }

        public void EndAll(GameModel model)
        {
            foreach (var e in _events)
                e.End(model);
            ActiveEvent = null;
        }
    }
}
