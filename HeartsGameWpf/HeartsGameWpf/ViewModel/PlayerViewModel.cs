using HeartsGameEngine;
using HeartsGameEngine.DataObjects;
using HeartsGameWpf.Misc;
using HeartsGameWpf.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace HeartsGameWpf.ViewModel
{
    public class PlayerViewModel : BaseViewModel
    {
        protected readonly GameManager gameManager;
        protected readonly Player player;

        public PlayerViewModel(GameManager gameManager, int playerIndex)
        {
            if (gameManager == null)
                throw new ArgumentNullException();

            this.gameManager = gameManager;
            this.playerIndex = playerIndex;

            this.player = gameManager.Game.GetPlayerByIndex(playerIndex);

            UpdateAll(); // Set all logic based properties

            SetEventHandlers();
        }

        private void SetEventHandlers()
        {
            player.Score.CollectionChanged += OnScoreChanged;
            gameManager.GameChanged += OnReseted;
        }

        public string Name
        {
            get { return Settings.Default.PlayerNames[playerIndex]; }
        }

        private ICollectionView sortedHand;
        public ICollectionView SortedHand
        {
            get 
            { 
                if(sortedHand == null)
                {
                    sortedHand = CollectionViewSource.GetDefaultView(player.Hand);
                    sortedHand.SortDescriptions.Add(new SortDescription("Suit", ListSortDirection.Ascending));
                    sortedHand.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));
                }

                return sortedHand; 
            }
        }

        private ICollectionView score;
        public ICollectionView Score
        {
            get
            {
                if (score == null)
                {
                    score = CollectionViewSource.GetDefaultView(player.Score);
                }

                return score;
            }
        }
        
        private readonly int playerIndex;
        public int PlayerIndex
        {
            get { return playerIndex; }
        }

        private int totalScore;
        public int TotalScore
        {
            get { return totalScore; }
            private set { SetValue(ref totalScore, value); }
        }

        private int currentRoundScore;
        public int CurrentRoundScore
        {
            get { return currentRoundScore; }
            private set { SetValue(ref currentRoundScore, value); }
        }

        private void OnScoreChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCurrentRoundScore();
            UpdateTotalScore();
        }

        private void OnReseted(object sender, EventArgs e)
        {
            UpdateAll();
        }

        protected virtual void UpdateAll()
        {
            UpdateCurrentRoundScore();
            UpdateTotalScore();
        }

        protected void UpdateTotalScore()
        {
            TotalScore = player.TotalScore();
        }

        protected void UpdateCurrentRoundScore()
        {
            CurrentRoundScore = player.Score.LastOrDefault();
        }
    }
}
