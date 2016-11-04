using HeartsGameEngine;
using HeartsGameEngine.AI;
using HeartsGameEngine.DataObjects;
using HeartsGameWpf.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private GameManager gameManager;
        private BoardViewModel board;
        private ScoreModalViewModel scoreModal;
        private SaveHandlerErrorModalViewModel saveHandlerErrorModal;
        private bool showScoreModal = false;
        private bool showSaveHandlerErrorModal = false;
        private FileSystemWatcher watcher;
        private Action lastSaveFileAction;

        public MainViewModel()
        {
            gameManager = new GameManager();
            gameManager.GameChanged += OnGameChanged;
            board = new BoardViewModel(gameManager);
            scoreModal = new ScoreModalViewModel(gameManager);
            saveHandlerErrorModal = new SaveHandlerErrorModalViewModel();

            watcher = new FileSystemWatcher();
            watcher.Changed += OnWatcherChanged;
            watcher.Path = Directory.GetCurrentDirectory();
            watcher.Filter = GameManager.SaveFile;
            watcher.EnableRaisingEvents = true;

            if (File.Exists(GameManager.SaveFile))
                LoadGame();
            else
                NewGame(null);

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

        public SaveHandlerErrorModalViewModel SaveHandlerErrorModal
        {
            get { return saveHandlerErrorModal; }
        }

        public bool ShowScoreModal
        {
            get { return showScoreModal; }
            private set { SetValue(ref showScoreModal, value); RaisePropertyChanged("EnableMenu"); }
        }

        public bool ShowSaveHandlerErrorModal
        {
            get { return showSaveHandlerErrorModal; }
            private set { SetValue(ref showSaveHandlerErrorModal, value); RaisePropertyChanged("EnableMenu"); }
        }

        public bool EnableMenu
        {
            get { return !ShowScoreModal && !ShowSaveHandlerErrorModal; }
        }

        public ICommand NewGameCommand
        {
            get { return new ActionCommand<object>(NewGame); }
        }

        public ICommand ScoreCommand
        {
            get { return new ActionCommand<object>(Score); }
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
            ShowScoreModal = true;
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
            CloseModals();

            int? result = obj as int?;

            if(result == 0) // retry
            {
                if(lastSaveFileAction != null)
                    lastSaveFileAction();
            }
            else if(result == 1) // new game
            {
                NewGame(null);
            }
        }

        private void CloseModals()
        {
            ShowScoreModal = false;
            ShowSaveHandlerErrorModal = false;
        }

        private void OnGameChanged(object sender, GameChangedEventArgs e)
        {
            if(watcher.EnableRaisingEvents)
                SaveGame();
            UpdateShowScoreModal();
        }

        private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                /* Wait one sec until loading the game
                 * because NotePad will keep the file open for a short period
                 * after write operations */
                DelayedAction da = new DelayedAction();
                da.Delay = 1000;
                da.Action = (Action)delegate
                {
                    LoadGame();
                };
                // Sync with UI thread
                App.Current.Dispatcher.Invoke(da.Execute);
            }
        }

        private void UpdateShowScoreModal()
        {
            Rules rules = gameManager.Rules;
            Game game = gameManager.Game;

            // Check if round is over
            if ((rules.CanStartNewRound() && game.RoundCounter > 0) || rules.GameOver())
            {
                CloseModals();
                ShowScoreModal = true;
            }
        }

        private void LoadGame()
        {
            ShowSaveHandlerErrorModal = false;
            lastSaveFileAction = LoadGame;
            watcher.EnableRaisingEvents = false;

            try
            {
                gameManager.Load();
            }
            catch (Exception ex)
            {
                CloseModals();
                ShowSaveHandlerErrorModal = true;
                SaveHandlerErrorModal.Title = "Load Error";
                SaveHandlerErrorModal.Message = ex.Message;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                watcher.EnableRaisingEvents = true;
            }
        }

        private void SaveGame()
        {
            ShowSaveHandlerErrorModal = false;
            lastSaveFileAction = SaveGame;
            watcher.EnableRaisingEvents = false;

            try
            {
                gameManager.Save();
            }
            catch (Exception ex)
            {
                CloseModals();
                ShowSaveHandlerErrorModal = true;
                SaveHandlerErrorModal.Title = "Save Error";
                SaveHandlerErrorModal.Message = ex.Message;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                watcher.EnableRaisingEvents = true;
            }
        }
    }
}
