using System;
using Type_faster.Model;
using Type_faster.View;

namespace Type_faster.Presenter
{
    public class GamePresenter
    {
        private readonly IGameView _view;
        private readonly GameModel _model;

        public GamePresenter(IGameView view, GameModel model)
        {
            _view = view;
            _model = model;
            WireEvents();
        }

        private void WireEvents()
        {
            _view.OnStartClicked += _model.StartGame;
            _view.OnKeyPressed += _model.ProcessInput;

            _model.OnXPChanged += _view.UpdateXP;
            _model.OnWordChanged += _view.UpdateWord;
            _model.OnTimerChanged += _view.UpdateTimer;
            _model.OnComboChanged += _view.UpdateCombo;
            _model.OnEventMessage += _view.UpdateEventMessage;
            _model.OnGameEnded += _view.ShowEndScreen;

        }
    }


}