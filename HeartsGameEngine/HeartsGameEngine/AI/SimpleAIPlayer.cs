using HeartsGameEngine.DataObjects;
using HeartsGameEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine.AI
{
    public class SimpleAIPlayer : AIPlayer
    {
        protected override List<Card> GetCardsToPass(GameManager gameManager, int player)
        {
            Game game = gameManager.Game;
            IList<Card> validHand = gameManager.Rules.ValidCards(player);
            List<Card> cards = new List<Card>();

            for (int i = 0; i < 3; i++)
            {
                Card card = validHand.FirstOrDefault(x => x.Suit == CardSuit.Spades && x.Value >= CardValue.Queen);

                if(card == null)
                {
                    card = validHand.OrderByDescending(x => x.Value).First();
                }

                cards.Add(card);
                validHand.Remove(card);
            }

            return cards;
        }

        protected override Card GetCardsToPlay(GameManager gameManager, int playerIndex)
        {
            Card card = null;
            Game game = gameManager.Game;
            Player player = game.GetPlayerByIndex(playerIndex);
            IList<Card> validHand = gameManager.Rules.ValidCards(playerIndex);
            IList<Card> hand = game.GetPlayerByIndex(playerIndex).Hand;
            Card qsCard = new Card(CardSuit.Spades, CardValue.Queen);

            if(game.TrickHistory.Count == 0) //first trick
            {
                card = validHand.OrderByDescending(x => x.Value).First();
            }
            else if(!QueenOfSpadesHasBeenPlayed(game))
            {
                List<Card> highSpades = validHand.Where(x => x.Suit == CardSuit.Spades && x.Value >= CardValue.Queen).ToList();
                if (highSpades.Count == 0 && validHand.Any(x => x.Suit == CardSuit.Spades) && BleedingOutSpades(game, player))
                {
                    card = validHand.Where(x => x.Suit == CardSuit.Spades).First();
                }
                else if(highSpades.Count > 0)
                {
                    foreach(Card c in highSpades.OrderByDescending(x => x.Value))
                    {
                        if (CanDumpCard(gameManager, c))
                            card = c;
                    }
                }
            }

            if (card == null)
            {
                TrickItem high = High(game.CurrentTrick);
                bool hasHighSpades = hand.Any(x => x.Suit == CardSuit.Spades && x.Value >= CardValue.Queen);

                if (hasHighSpades && high == null)
                {
                    card = validHand.Where(x => x.Suit != CardSuit.Spades).OrderBy(x => x.Value).FirstOrDefault();
                }
                else
                {
                    if (high == null)
                        card = validHand.OrderBy(x => x.Value).FirstOrDefault();
                    else
                        card = validHand.Where(x => x.Value < high.Card.Value || x.Suit != high.Card.Suit)
                                                .OrderByDescending(x => x.Value)
                                                .FirstOrDefault();
                }
            }

            if (card == null && game.CurrentTrick.Count == 3)
            {
                card = validHand.OrderByDescending(x => x.Value).FirstOrDefault(x => !x.Equals(qsCard));
            }

            if(card == null)
            {
                card = validHand.OrderBy(x => x.Value).First();
            }

            return card;
        }

        #region Private helpers

        private bool QueenOfSpadesHasBeenPlayed(Game game)
        {
            Card qsCard = new Card(CardSuit.Spades, CardValue.Queen);
            return game.TrickHistory.Any(x => x.Any(y => y.Equals(qsCard)))
                    || game.CurrentTrick.Any(x => x.Equals(qsCard));
        }

        private bool BleedingOutSpades(Game game, Player player)
        {
            if (game.CurrentTrick.Count != 0)
                return false;

            int thisPlayerScore = player.TotalScore();
            int minScore = game.GetPlayers().Min(x => x.TotalScore());
            bool rv = (thisPlayerScore - minScore) < 12;
            return rv;
        }

        private bool CanDumpCard(GameManager gameManager, Card card)
        {
            var currentTrick = gameManager.Game.CurrentTrick;
            var rules = gameManager.Rules;

            if (currentTrick.Count == 0)
                return false;

            if (currentTrick[0].Card.Suit != card.Suit)
                return true;

            if (currentTrick.Any(x => x.Card.Suit == card.Suit && x.Card.Value > card.Value))
                return true;

            if (currentTrick.Count == 3 && !rules.IsPenaltyCard(card) && !currentTrick.Any(x => rules.IsPenaltyCard(x.Card)))
                return true;

            return false;
        }

        private TrickItem High(IList<TrickItem> trick)
        {
            TrickItem high = null;
            if(trick.Count > 0)
            {
                CardSuit leadingSuit = trick[0].Card.Suit;
                high = trick.Where(x => x.Card.Suit == leadingSuit)
                                    .OrderBy(x => x.Card.Value)
                                    .LastOrDefault();
            }
            return high;
        }

        #endregion Private helpers
    }
}
