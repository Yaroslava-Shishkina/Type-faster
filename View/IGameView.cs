using System;

namespace Type_faster.View
{
    public interface IGameView
    {
        void UpdateWord(string displayWord);
        void UpdateXP(int xp);
        void UpdateTimer(TimeSpan timeLeft);
        void ShowEndScreen(int finalXP);

        event Action OnStartClicked;
        event Action<char> OnKeyPressed;
    }
}