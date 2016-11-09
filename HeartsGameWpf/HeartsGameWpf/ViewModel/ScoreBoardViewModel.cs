using HeartsGameEngine;
using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameWpf.ViewModel
{
    public class ScoreBoardViewModel : BaseViewModel
    {
        private GameManager gameManager;

        public ScoreBoardViewModel(GameManager gameManager)
        {
            this.gameManager = gameManager;

            player1 = new PlayerViewModel(gameManager, 0);
            player2 = new PlayerViewModel(gameManager, 1);
            player3 = new PlayerViewModel(gameManager, 2);
            player4 = new PlayerViewModel(gameManager, 3);

            gameManager.GameChanged += OnGameChanged;
        }

        private void OnGameChanged(object sender, GameChangedEventArgs e)
        {
            UpdateAll();
        }

        private IEnumerable<int> rounds;
        public IEnumerable<int> Rounds
        {
            get { return rounds; }
            private set { SetValue(ref rounds, value); }
        }

        private readonly PlayerViewModel player1;
        public PlayerViewModel Player1
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

        private string winner;
        public string Winner
        {
            get { return winner; }
            private set { SetValue(ref winner, value); }
        }

        private bool gameOver;
        public bool GameOver
        {
            get { return gameOver; }
            private set { SetValue(ref gameOver, value); }
        }

        private void UpdateAll()
        {
            UpdateRounds();
            UpdateGameOver();
            UpdateWinner();
        }

        private void UpdateRounds()
        {
            Game game = gameManager.Game;

            if(Rounds == null || Rounds.Count() != game.RoundCounter)
            {
                Rounds = Enumerable.Range(1, game.RoundCounter);
            }
        }

        private void UpdateGameOver()
        {
            GameOver = gameManager.Rules.GameOver();
        }

        private void UpdateWinner()
        {
            Rules rules = gameManager.Rules;
            string w = "";

            if (rules.GameOver())
            {
                switch(rules.Winner())
                {
                    case 0: w = Player1.Name;
                        break;
                    case 1: w = Player2.Name;
                        break;
                    case 2: w = Player3.Name;
                        break;
                    case 3: w = Player4.Name;
                        break;
                    default: w = "Tie";
                        break;
                }
            }

            Winner = w;
        }
    }
}
