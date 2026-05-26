namespace Type_faster.Model
{
    public abstract class GameEvent
    {
        public bool IsActive { get; protected set; }
        public bool IsBonusActive { get; protected set; }
        public abstract string Name { get; }

        public abstract bool CanTrigger(GameModel model);
        public abstract void Trigger(GameModel model);
        public virtual void OnPlayerMistake(GameModel model) { }
        public abstract void OnEventWordCompleted(GameModel model);
        public virtual void OnTick(GameModel model) { }
        public abstract void End(GameModel model);
    }
}
