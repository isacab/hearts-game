using HeartsGameEngine;
using HeartsGameWpf.Misc;
using HeartsGameWpf.Properties;
using System;
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
    public class ScoreModalViewModel : BaseViewModel
    {
        private GameManager gameManager;

        public ScoreModalViewModel(GameManager gameManager)
        {
            if (gameManager == null)
                throw new ArgumentNullException();

            this.gameManager = gameManager;
            this.scoreBoard = new ScoreBoardViewModel(gameManager);

            gameManager.GameChanged += OnEvent;

            UpdateAll();
        }

        private ScoreBoardViewModel scoreBoard;
        public ScoreBoardViewModel ScoreBoard
        {
            get { return scoreBoard; }
            set { SetValue(ref scoreBoard, value); }
        }

        private string title = "Score";
        public string Title
        {
            get { return title; }
            set { SetValue(ref title, value); }
        }

        private string closeButtonText;
        public string CloseButtonText
        {
            get { return closeButtonText; }
            set { SetValue(ref closeButtonText, value); }
        }

        private void OnEvent(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            UpdateTitle();
            UpdateCloseButtonText();
        }

        private void UpdateTitle()
        {
            Rules rules = gameManager.Rules;
            string title = "Score";

            if (rules.GameOver())
                title = "Game Over";

            Title = title;
        }

        private void UpdateCloseButtonText()
        {
            Rules rules = gameManager.Rules;
            string text = "Close";

            if (rules.GameOver())
                text = "New Game";
            else if (rules.CanStartNewRound())
                text = "Start Next Round";

            CloseButtonText = text;
        }
    }
}
