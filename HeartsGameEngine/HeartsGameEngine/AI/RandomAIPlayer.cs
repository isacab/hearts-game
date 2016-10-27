using HeartsGameEngine.DataObjects;
using HeartsGameEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine.AI
{
    public class RandomAiPlayer
    {
        public RandomAiPlayer(GameManager gameManager = null, int player = -1, int delay = 0) 
        {
            GameManager = gameManager;
            Player = player;
            Delay = delay;

            MakeAction();
        }

        public int Delay
        {
            get;
            set;
        }

        public int Player
        {
            get;
            set;
        }

        private GameManager gameManager;
        public GameManager GameManager
        {
            get { return this.gameManager; }
            set
            {
                gameManager = value;

                if(gameManager != null)
                {
                    //gameManager.Game.PropertyChanged += OnGamePropertyChanged;
                    //gameManager.Game.Players[Player].PassedCards.CollectionChanged += OnPassedCardsChanged;
                    gameManager.GameChanged += OnEvent;
                }
            }
        }

        private void OnEvent(object sender, GameChangedEventArgs e)
        {
            if(e.Action == GameAction.Reset || e.Action == GameAction.StartNewRound || e.Action == GameAction.StartNewTurn)
                MakeAction();
        }

        private void MakeAction()
        {
            Rules rules = gameManager.Rules;

            if (rules.CanPassCards(Player))
            {
                List<Card> cards = PickCardsToPass();
                gameManager.PassCards(Player, cards);
            }

            if (rules.CanPlay(Player))
            {
                Card card = PickCardsToPlay();
                gameManager.Play(Player, card);
            }
        }

        private List<Card> PickCardsToPass()
        {
            Game game = gameManager.Game;
            IList<Card> validHand = gameManager.Rules.ValidCards(this.Player);
            List<Card> cards = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                int rnd = HelperMethods.GetRandomNumber(0, validHand.Count - 1);
                cards.Add(validHand[rnd]);
                validHand.RemoveAt(rnd);
            }

            return cards;
        }

        private Card PickCardsToPlay()
        {
            Game game = gameManager.Game;
            IList<Card> validHand = gameManager.Rules.ValidCards(this.Player);
            int rnd = HelperMethods.GetRandomNumber(0, validHand.Count() - 1);
            Card card = validHand.ElementAt(rnd);

            return card;
        }
    }
}
