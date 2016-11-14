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
            IList<Card> playedCards = GetPlayedCards(game);
            IList<Card> oponentsCards = GetOponentsCards(game, playerIndex);
            var currentTrick = game.CurrentTrick;
            Player potentialMoonShooter = PotentialMoonShooter(game);
            bool heartsHasBeenPlayed = gameManager.Rules.HeartsHasBeenPlayed();

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
                    foreach(Card c in highSpades)
                    {
                        if (CanDumpCard(game, c))
                            card = c;
                    }
                }
            }

            if (card == null)
            {
                if (CanBreakMoonShooter(potentialMoonShooter, player, validHand))
                {
                    card = validHand.Where(x => x.Suit == CardSuit.Hearts).OrderByDescending(x => x.Value).FirstOrDefault();
                }
                else
                {
                    Card qsCard = new Card(CardSuit.Spades, CardValue.Queen);
                    bool hasQueenOfSpades = hand.Any(x => x.Equals(qsCard));
                    TrickItem high = High(game.CurrentTrick);

                    if (hasQueenOfSpades)
                    {
                        if (high == null)
                            card = validHand.Where(x => x.Suit != CardSuit.Spades).OrderBy(x => x.Value).FirstOrDefault();
                        else
                            card = validHand.Where(x => x.Value < high.Card.Value || x.Suit != high.Card.Suit && !x.Equals(qsCard))
                                                    .OrderByDescending(x => x.Value)
                                                    .FirstOrDefault();
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
            }

            if(card == null)
            {
                if (game.CurrentTrick.Count == 3)
                {
                    card = validHand.OrderByDescending(x => x.Value).First();
                }
                else
                {
                    int rnd = HelperMethods.GetRandomNumber(0, validHand.Count() - 1);
                    card = validHand.ElementAt(rnd);
                }
            }

            return card;
        }

        private bool QueenOfSpadesHasBeenPlayed(Game game)
        {
            return game.TrickHistory.Any(x => x.Any(y => y.Card.Suit == CardSuit.Spades))
                    || game.CurrentTrick.Any(x => x.Card.Suit == CardSuit.Spades);
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

        private bool CanBreakMoonShooter(Player potentialMoonShooter, Player player, IList<Card> validHand)
        {
            return potentialMoonShooter != null
                && potentialMoonShooter != player
                && validHand.Any(x => x.Suit == CardSuit.Hearts || x.Equals(new Card(CardSuit.Spades, CardValue.Queen)));
                    
        }

        private IList<Card> GetOponentsCards(Game game, int playerIndex)
        {
            var players = game.GetPlayers();
            Player player = game.GetPlayerByIndex(playerIndex);
            IList<Card> cards = (from p in players
                                 from c in p.Hand
                                 where p != player
                                 select c).ToList();
            return cards;
        }

        private IList<Card> GetPlayedCards(Game game)
        {
            IList<Card> cards = (from ti in game.CurrentTrick
                                select ti.Card)
                                .Union(from trick in game.TrickHistory
                                       from ti in trick
                                       select ti.Card)
                                .ToList();
            return cards;
        }

        private Card GetDefensiveCard(Game game, IList<Card> validHand, int player)
        {
            TrickItem high = High(game.CurrentTrick);
            Card card = null;

            var hand = game.GetPlayerByIndex(player).Hand;

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

        private Player PotentialMoonShooter(Game game)
        {
            var players = game.GetPlayers();

            if (players.Count(x => x.Score.Last() > 0) != 1)
                return null;

            Player potentialMoonShooter = players.First(x => x.Score.Last() > 0);

            return potentialMoonShooter;
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
