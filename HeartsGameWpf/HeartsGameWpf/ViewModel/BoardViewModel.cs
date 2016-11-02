using HeartsGameEngine;
using HeartsGameEngine.AI;
using HeartsGameEngine.DataObjects;
using HeartsGameWpf.Misc;
using HeartsGameWpf.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace HeartsGameWpf.ViewModel
{
    public class BoardViewModel : BaseViewModel
    {
        private readonly GameManager gameManager;
        private readonly RandomAiPlayer aiPlayer;
        private readonly FileSystemWatcher watcher;

        private const int clearTrickDelay = 1000;
        private const int aiDelay = 250;

        public BoardViewModel(GameManager gameManager)
        {
            if (gameManager == null)
                throw new ArgumentNullException();

            this.gameManager = gameManager;

            gameManager.GameChanged += OnGameCanged;

            player1 = new HumanPlayerViewModel(gameManager, 0);
            player2 = new PlayerViewModel(gameManager, 1);
            player3 = new PlayerViewModel(gameManager, 2);
            player4 = new PlayerViewModel(gameManager, 3);

            aiPlayer = new RandomAiPlayer();

            watcher = new FileSystemWatcher();
            watcher.Changed += OnWatcherChanged;
            watcher.Path = Directory.GetCurrentDirectory();
            watcher.Filter = "thefiletowatch.txt";
            watcher.EnableRaisingEvents = true;

            Update();
        }

        private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Game Game
        {
            get { return gameManager.Game; }
        }

        private readonly HumanPlayerViewModel player1;
        public HumanPlayerViewModel Player1
        {
            get { return player1; }
        }

        private readonly PlayerViewModel player2;
        public PlayerViewModel Player2
        {
            get { return player2; }
        }

        private readonly PlayerViewModel player3;
        public PlayerViewModel Player3
        {
            get { return player3; }
        }

        private readonly PlayerViewModel player4;
        public PlayerViewModel Player4
        {
            get { return player4; }
        }

        private int lastTrickWinner = -1;
        public int LastTrickWinner
        {
            get { return lastTrickWinner; }
            set { SetValue(ref lastTrickWinner, value); }
        }

        public ICommand StartNewTurn
        {
            get { return new ActionCommand<object>((x) => { gameManager.StartNewTurn(); }); }
        }

        public ICommand ClearTrick
        {
            get { return new ActionCommand<object>((x) => { gameManager.ClearTrick(); }); }
        }

        private void OnGameCanged(object sender, GameChangedEventArgs e)
        {
            // Cancel delayed action
            if (e.Action == GameAction.Reset)
            {
                delayedAction.Cancel();
            }

            gameManager.Save();

            if(e.Action != GameAction.PassCards)
            {
                Update();
            }
        }

        private DelayedAction delayedAction = new DelayedAction();
        private void Update()
        {
            Rules rules = gameManager.Rules;
            
            // Check if trick is finished
            if (gameManager.Rules.TrickIsFinished(Game.CurrentTrick))
            {
                LastTrickWinner = gameManager.Rules.TrickWinner(Game.CurrentTrick);
            }
            else
            {
                // Reset last trick winner when trick is cleared
                LastTrickWinner = -1;
            }

            // Make AI actions
            for (int i = 0; i < Game.NumberOfPlayers; i++)
            {
                int player = i;

                if (rules.CanPassCards(player))
                {
                    aiPlayer.MakeAction(gameManager, player);
                }

                if (rules.CanPlay(player))
                {
                    delayedAction = new DelayedAction();
                    delayedAction.Delay = aiDelay; //debug (rules.NextPlayer() == 0) ? 0 : 250;
                    delayedAction.Action = new Action(() =>
                    {
                        aiPlayer.MakeAction(gameManager, player);
                    });
                    delayedAction.Execute();
                }
            }

            // Check if waiting for new turn
            if (rules.CanStartNewTurn())
            {
                gameManager.StartNewTurn();
            }

            // Ceck if trick is finished
            if (rules.CanClearTrick())
            {
                delayedAction = new DelayedAction();
                delayedAction.Delay = clearTrickDelay;
                delayedAction.Action = new Action(() =>
                {
                    gameManager.ClearTrick();
                });
                delayedAction.Execute();
            }
        }
    }
}
