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

namespace HeartsGameWpf.ViewModel
{
    public class BoardViewModel : BaseViewModel
    {
        private readonly GameManager gameManager;

        private readonly RandomAiPlayer[] aiPlayers = new RandomAiPlayer[4];

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

            int delay = 100;// Settings.Default.AIDelay;
            aiPlayers[0] = new RandomAiPlayer(gameManager, 1, delay);
            aiPlayers[1] = new RandomAiPlayer(gameManager, 2, delay);
            aiPlayers[2] = new RandomAiPlayer(gameManager, 3, delay);
            aiPlayers[3] = new RandomAiPlayer(gameManager, 0, delay);
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
            if(e.Action == GameAction.Play && gameManager.Rules.TrickIsFinished(Game.CurrentTrick))
            {
                LastTrickWinner = gameManager.Rules.TrickWinner(Game.CurrentTrick);
            }
            if(e.Action == GameAction.ClearTrick || e.Action == GameAction.Reset)
            {
                LastTrickWinner = -1;
            }
        }
    }
}
