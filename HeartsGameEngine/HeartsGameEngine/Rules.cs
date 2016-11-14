using HeartsGameEngine.DataObjects;
using HeartsGameEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeartsGameEngine
{
    public enum Direction
    {
        None, Left, Across, Right
    }

    public class Rules
    {
        public const int EndScore = 100;

        public Rules(Game game)
        {
            Game = game;
        }

        private Game game;
        public Game Game
        {
            get { return game; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                game = value;
            }
        }

        public bool ValidPlay(int playerIndex, Card card)
        {
            return ValidCards(playerIndex).Contains(card);
        }

        public bool ValidPass(int playerIndex, IList<Card> cards)
        {
            return ValidCards(playerIndex).Intersect(cards).Distinct().Count() == 3;
        }

        public IList<Card> ValidCards(int playerIndex)
        {
            Player player = game.GetPlayerByIndex(playerIndex);
            List<Card> validCards = new List<Card>(player.Hand);

            if (game.Phase == HeartsPhase.Tricks)
            {
                var trick = game.CurrentTrick;
                var trickHistory = game.TrickHistory;

                bool firstTrick = trickHistory.Count == 0;
                bool firstPlayInTrick = trick.Count == 0;

                if (game.CurrentPlayer != playerIndex || game.Phase == HeartsPhase.None)
                {
                    validCards = new List<Card>();
                }
                else if (firstTrick && firstPlayInTrick)
                {
                    validCards = validCards.Where(x => x.Suit == CardSuit.Clubs && x.Value == CardValue.Two).ToList();
                }
                else if (firstPlayInTrick)
                {
                    if (!HeartsHasBeenPlayed())
                    {
                        List<Card> filteredHand = validCards.Where(x => x.Suit != CardSuit.Hearts).ToList();

                        if (filteredHand.Any())
                            validCards = filteredHand;
                    }
                }
                else
                {
                    CardSuit leadingSuit = trick[0].Card.Suit;

                    //Filter hand by leadingSuit
                    List<Card> filteredHand = validCards.Where(x => x.Suit == leadingSuit).ToList();

                    if (firstTrick && filteredHand.Count == 0)
                        filteredHand = validCards.Where(x => !IsPenaltyCard(x)).ToList();

                    if (filteredHand.Count != 0)
                        validCards = filteredHand;
                }
            }

            return validCards;
        }

        private bool IsPenaltyCard(Card card)
        {
            //true if Suit equals Hearts or if the card is queen of spades
            return card.Suit == CardSuit.Hearts || (card.Suit == CardSuit.Spades && card.Value == CardValue.Queen);
        }

        public bool HeartsHasBeenPlayed()
        {
            return Game.TrickHistory.Any(t => ContainsSuit(t, CardSuit.Hearts));
        }

        private bool ContainsSuit(IEnumerable<TrickItem> trick, CardSuit suit)
        {
            return trick.Any(x => x.Card.Suit == suit);
        }

        public bool CanStartNewRound()
        {
            return game.Phase == HeartsPhase.None && GameOver() == false;
        }

        public bool CanStartNewTurn()
        {
            return game.CurrentPlayer == -1 &&
                   game.Phase == HeartsPhase.Tricks &&
                   !TrickIsFinished(game.CurrentTrick) &&
                   !CanStartNewRound();
        }

        public Direction PassDirection()
        {
            int i = game.RoundCounter % Game.NumberOfPlayers;

            switch (i)
            {
                case 1: return Direction.Left;
                case 2: return Direction.Right;
                case 3: return Direction.Across;
                default: return Direction.None;
            }
        }

        public bool CanPassCards(int playerIndex)
        {
            Player player = game.GetPlayerByIndex(playerIndex);

            return game.Phase == HeartsPhase.PassCards && player.PassedCards.Count != 3;
        }

        public bool CanPlay(int playerIndex)
        {
            return game.CurrentPlayer == playerIndex && game.Phase == HeartsPhase.Tricks;
        }

        public bool CanClearTrick()
        {
            return TrickIsFinished(game.CurrentTrick);
        }

        public int TrickWinner(IList<TrickItem> trick)
        {
            int winner = -1;

            if (TrickIsFinished(trick))
            {
                CardSuit leadingSuit = trick[0].Card.Suit;
                winner = trick.Where(x => x.Card.Suit == leadingSuit)
                                .OrderBy(x => x.Card.Value)
                                .Last()
                                .Player;
            }

            return winner;
        }

        public bool TrickIsFinished(IList<TrickItem> trick)
        {
            return trick.Count >= Game.NumberOfPlayers;
        }

        public int[] CalcRoundScore()
        {
            int[] roundScore = new int[Game.NumberOfPlayers];

            foreach (var trick in game.TrickHistory)
            {
                int trickWinner = TrickWinner(trick);

                int numHearts = trick.Where(x => x.Card.Suit == CardSuit.Hearts).Count();
                int numQueenOfSpades = trick.Where(x => x.Card.Suit == CardSuit.Spades && x.Card.Value == CardValue.Queen).Count();

                int points = numHearts + numQueenOfSpades * 13;

                // Add points to trickwinner
                roundScore[trickWinner] += points;

                // Check if trickWinner shoots the moon
                if (roundScore[trickWinner] == 26)
                {
                    for (int i = 0; i < roundScore.Length; i++)
                    {
                        if (i == trickWinner)
                            roundScore[i] = 0;
                        else
                            roundScore[i] = 26;
                    }
                }
            }

            return roundScore;
        }

        public bool GameOver()
        {
            return game.Phase == HeartsPhase.None && game.GetPlayers().Any(x => x.TotalScore() >= EndScore);
        }

        public int Winner()
        {
            if (!GameOver())
                return -1;

            int winner = -1;
            var players = game.GetPlayers();
            int minScore = players.Min(x => x.TotalScore());
            IEnumerable<Player> winners = players.Where(x => x.TotalScore() == minScore);

            if (winners.Count() == 1)
                winner = game.GetIndexByPlayer(winners.First());

            return winner;
        }

        public int NextPlayer()
        {
            if (!CanStartNewTurn())
                return -1;

            int nextPlayer;

            bool firstTrick = game.TrickHistory.Count == 0;
            bool firstPlayInTrick = game.CurrentTrick.Count == 0;

            if (firstTrick && firstPlayInTrick)
            {
                List<Player> players = game.GetPlayers();
                Card c2 = new Card(CardSuit.Clubs, CardValue.Two); // clubs, 2
                Player p = players.First(x => x.Hand.Contains(c2)); // player with 2C
                nextPlayer = players.IndexOf(p);
            }
            else if (firstPlayInTrick)
            {
                var lastTrick = game.TrickHistory.Last();
                int lastTrickWinner = TrickWinner(lastTrick);
                nextPlayer = lastTrickWinner;
            }
            else
            {
                int lastPlayer = game.CurrentTrick.Last().Player;
                nextPlayer = HelperMethods.Rotate(lastPlayer, 1, Game.NumberOfPlayers);
            }

            return nextPlayer;
        }
    }
}
