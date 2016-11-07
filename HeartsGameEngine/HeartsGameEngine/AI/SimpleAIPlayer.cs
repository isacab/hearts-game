using HeartsGameEngine.DataObjects;
using HeartsGameEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine.AI
{
    public class SimpleAIPlayer : IAIPlayer
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
                Card card = validHand.FirstOrDefault(x => x.Suit == CardSuit.Spades && x.Value == CardValue.Queen);

                if(card == null)
                {
                    card = validHand.OrderByDescending(x => x.Value).First();
                }

                cards.Add(card);
                validHand.Remove(card);
            }

            return cards;
        }

        private Card GetCardsToPlay(GameManager gameManager, int player)
        {
            Game game = gameManager.Game;
            IList<Card> validHand = gameManager.Rules.ValidCards(player);
            var currentTrick = game.CurrentTrick;
            int rnd = HelperMethods.GetRandomNumber(0, validHand.Count() - 1);
            Card card = validHand.ElementAt(rnd);

            Card queenOfSpades = validHand.FirstOrDefault(x => x.Suit == CardSuit.Spades && x.Value == CardValue.Queen);

            if(CanShootTheMoon(gameManager, player))
            {
                card = GetOffensiveCard(gameManager, player);
            }
            else if (queenOfSpades != null && CanDumpCard(game, queenOfSpades))
            {
                card = queenOfSpades;
            }
            else if(CanBreakMoonShooter(game, validHand, player))
            {
                card = validHand.Where(x => x.Suit == CardSuit.Hearts).OrderByDescending(x => x.Value).First();
            }
            else
            {
                card = GetDefensiveCard(game, validHand, player);
            }

            return card;
        }

        private Card GetDefensiveCard(Game game, IList<Card> validHand, int player)
        {
            TrickItem high = High(game.CurrentTrick);
            Card card = null;

            if (high == null)
                card = validHand.OrderBy(x => x.Value).First();
            else
                card = validHand.Where(x => x.Value < high.Card.Value || x.Suit != high.Card.Suit)
                                        .OrderByDescending(x => x.Value)
                                        .FirstOrDefault();

            if (card == null)
            {
                if (game.CurrentTrick.Count == 3)
                    card = validHand.OrderByDescending(x => x.Value).First();
                else
                {
                    int rnd = HelperMethods.GetRandomNumber(0, validHand.Count() - 1);
                    card = validHand.ElementAt(rnd);
                }
            }

            return card;
        }

        private bool CanBreakMoonShooter(Game game, IList<Card> validHand, int player)
        {
            var players = game.GetPlayers();

            if (players.Count(x => x.Score.Last() > 0) != 1)
                return false;

            Player potentialMoonShooter = players.First(x => x.Score.Last() > 0);
            int potentialMoonShooterIndex = game.GetIndexByPlayer(potentialMoonShooter);

            TrickItem high = High(game.CurrentTrick);

            bool rv = potentialMoonShooterIndex != player
                && high != null
                && high.Player != potentialMoonShooterIndex
                && validHand.Any(x => x.Suit == CardSuit.Hearts);

            return rv;
        }

        private Card GetOffensiveCard(GameManager gameManager, int player)
        {
            throw new NotImplementedException();
        }

        private bool CanDumpCard(Game game, Card card)
        {
            var currentTrick = game.CurrentTrick;

            if(currentTrick.Count == 0)
                return false;

            if(currentTrick[0].Card.Suit != card.Suit)
                return true;

            if(currentTrick.Any(x => x.Card.Suit == card.Suit && x.Card.Value > card.Value))
                return true;

            return false;
        }

        private bool CanShootTheMoon(GameManager gameManager, int player)
        {
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
    }
}
