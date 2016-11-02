using HeartsGameEngine;
using HeartsGameEngine.AI;
using HeartsGameEngine.DataObjects;
using HeartsGameWpf.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HeartsGameWpf.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private BoardViewModel board;
        private ScoreModalViewModel scoreModal;
        private Visibility showScoreModal = Visibility.Collapsed;
        private GameManager gameManager;

        public MainViewModel()
        {
            gameManager = new GameManager();
            gameManager.Load();
            board = new BoardViewModel(gameManager);
            scoreModal = new ScoreModalViewModel(gameManager);
            gameManager.GameChanged += OnGameChanged;

            InitDelayedActions();

            gameManager.ClearTrick();
            gameManager.StartNewTurn();
            UpdateShowScoreModal();
        }

        public BoardViewModel Board
        {
            get { return this.board; }
        }

        public ScoreModalViewModel ScoreModal
        {
            get { return scoreModal; }
        }

        public Visibility ShowScoreModal
        {
            get { return showScoreModal; }
            private set { SetValue(ref showScoreModal, value); RaisePropertyChanged("EnableMenu"); }
        }

        public bool EnableMenu
        {
            get { return ShowScoreModal != Visibility.Visible; }
        }

        public ICommand NewGameCommand
        {
            get { return new ActionCommand<object>(NewGame); }
        }

        public ICommand ScoreCommand
        {
            get { return new ActionCommand<object>(Score); }
        }

        public ICommand SettingsCommand
        {
            get { return new ActionCommand<object>(Settings); }
        }

        public ICommand CloseScoreModalCommand
        {
            get { return new ActionCommand<object>(CloseScoreModal); }
        }

        public ICommand CloseSaveHandlerErrorModalCommand
        {
            get { return new ActionCommand<object>(CloseSaveHandlerErrorModal); }
        }

        private void NewGame(object obj)
        {
            CloseModals();
            gameManager.Reset();
            gameManager.StartNewRound();
        }

        private void Score(object obj)
        {
            CloseModals();
            ShowScoreModal = Visibility.Visible;
        }

        private void Settings(object obj)
        {
            gameManager.Load();
        }

        private void CloseScoreModal(object obj)
        {
            CloseModals();

            if (!gameManager.IsChanging)
            {
                Rules rules = gameManager.Rules;

                if (rules.GameOver())
                    gameManager.Reset();

                if (rules.CanStartNewRound())
                    gameManager.StartNewRound();
            }
        }

        private void CloseSaveHandlerErrorModal(object obj)
        {
            // TODO CloseModals();
        }

        private void CloseModals()
        {
            ShowScoreModal = Visibility.Collapsed;
        }

        private void OnGameChanged(object sender, GameChangedEventArgs e)
        {
            UpdateShowScoreModal();
        }

        private void InitDelayedActions()
        {
            
        }

        private void UpdateShowScoreModal()
        {
            Rules rules = gameManager.Rules;
            Game game = gameManager.Game;

            // Check if round is over
            if ((rules.CanStartNewRound() && game.RoundCounter > 0) || rules.GameOver())
            {
                ShowScoreModal = Visibility.Visible;
            }
        }
    }
}
