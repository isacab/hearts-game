using HeartsGameEngine;
using HeartsGameEngine.DataObjects;
using HeartsGameWpf.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HeartsGameWpf.ViewModel
{
    public class HumanPlayerViewModel : PlayerViewModel
    {
        public HumanPlayerViewModel(GameManager gameManager, int playerIndex)
            : base(gameManager, playerIndex)
        {
            gameManager.Game.PropertyChanged += OnGamePropertyChanged;
        }

        private bool canMakeAction;
        public bool CanMakeAction
        {
            get { return canMakeAction; }
            private set { SetValue(ref canMakeAction, value); }
        }

        private IList<Card> validCards = new List<Card>();
        public IList<Card> ValidCards
        {
            get { return validCards; }
            set { SetValue(ref validCards, value); }
        }

        public ICommand SubmitCommand
        {
            get { return new ActionCommand<IList<Card>>(Submit, CanSubmit); }
        }

        private int expectedNumberOfCards;
        public int ExpectedNumberOfCards
        {
            get { return expectedNumberOfCards; }
            private set { SetValue(ref expectedNumberOfCards, value); }
        }

        private bool showPassButton = false;
        public bool ShowPassButton
        {
            get { return showPassButton; }
            private set { SetValue(ref showPassButton, value); }
        }

        private string submitLabel;
        public string SubmitLabel
        {
            get { return submitLabel; }
            private set { SetValue(ref submitLabel, value); }
        }

        protected void Submit(object obj)
        {
            IList<Card> cards = (IList<Card>)obj;

            HeartsPhase phase = gameManager.Game.Phase;

            if (phase == HeartsPhase.PassCards)
            {
                gameManager.PassCards(PlayerIndex, cards);
            }
            else if (phase == HeartsPhase.Tricks)
            {
                gameManager.Play(PlayerIndex, cards.FirstOrDefault());
            }
        }

        protected bool CanSubmit(object obj)
        {
            IList<Card> cards = obj as IList<Card>;

            bool ok = cards != null && cards.Count == ExpectedNumberOfCards;

            return ok;
        }

        private void OnGamePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Game game = gameManager.Game;

            if (e.PropertyName == "CurrentPlayer" || e.PropertyName == "Phase")
            {
                UpdateCanMakeAction();
                UpdateValidCards();
            }

            if (e.PropertyName == "Phase")
            {
                UpdateExpectedNumberOfCards();
                UpdateShowPassButton();
                UpdateValidCards();
            }

            if (e.PropertyName == "RoundCounter")
            {
                UpdateSubmitLabel();
            }
        }

        protected override void UpdateAll()
        {
            base.UpdateAll();

            UpdateCanMakeAction();
            UpdateValidCards();
            UpdateExpectedNumberOfCards();
            UpdateSubmitLabel();
            UpdateShowPassButton();
        }

        protected void UpdateSubmitLabel()
        {
            string dir = gameManager.Rules.PassDirection().ToString().ToLower();
            SubmitLabel = "Pass " + dir;
        }

        protected void UpdateExpectedNumberOfCards()
        {
            HeartsPhase phase = gameManager.Game.Phase;
            int num = 0;

            if (phase == HeartsPhase.PassCards)
                num = 3;
            else if (phase == HeartsPhase.Tricks)
                num = 1;

            ExpectedNumberOfCards = num;
        }

        protected void UpdateShowPassButton()
        {
            if (gameManager.Game.Phase == HeartsPhase.PassCards)
                ShowPassButton = true;
            else
                ShowPassButton = false;
        }

        protected void UpdateCanMakeAction()
        {
            Rules rules = gameManager.Rules;

            CanMakeAction = rules.CanPlay(PlayerIndex) || rules.CanPassCards(PlayerIndex);
        }

        protected void UpdateValidCards()
        {
            ValidCards = gameManager.Rules.ValidCards(PlayerIndex);
        }
    }
}
