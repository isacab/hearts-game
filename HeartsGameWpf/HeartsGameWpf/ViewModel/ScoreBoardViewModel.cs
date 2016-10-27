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

            gameManager.Game.PropertyChanged += OnGamePropertyChanged;
        }

        private void OnGamePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "RoundCounter")
            {
                RaisePropertyChanged("Rounds");
            }
        }

        public IEnumerable<int> Rounds
        {
            get { return Enumerable.Range(1, gameManager.Game.RoundCounter); }
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
    }
}
