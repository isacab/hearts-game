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
        public void MakeAction(GameManager gameManager, int player)
        {
            Rules rules = gameManager.Rules;

            if (rules.CanPassCards(player))
            {
                List<Card> cards = GetCardsToPass(gameManager, player);
                gameManager.PassCards(player, cards);
            }

            if (rules.CanPlay(player))
            {
                Card card = GetCardsToPlay(gameManager, player);
                gameManager.Play(player, card);
            }
        }

        private List<Card> GetCardsToPass(GameManager gameManager, int player)
        {
            Game game = gameManager.Game;
            IList<Card> validHand = gameManager.Rules.ValidCards(player);
            List<Card> cards = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                int rnd = HelperMethods.GetRandomNumber(0, validHand.Count - 1);
                cards.Add(validHand[rnd]);
                validHand.RemoveAt(rnd);
            }

            return cards;
        }

        private Card GetCardsToPlay(GameManager gameManager, int player)
        {
            Game game = gameManager.Game;
            IList<Card> validHand = gameManager.Rules.ValidCards(player);
            int rnd = HelperMethods.GetRandomNumber(0, validHand.Count() - 1);
            Card card = validHand.ElementAt(rnd);

            return card;
        }
    }
}
