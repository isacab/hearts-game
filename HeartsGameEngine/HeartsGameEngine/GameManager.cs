using HeartsGameEngine.DataObjects;
using HeartsGameEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeartsGameEngine
{
    public class GameManager
    {
        #region Fields

        public const string SaveFile = "save.xml"; // The file where Game is saved.

        private SaveHandler saveHandler;

        #endregion //Fields

        #region Constructor

        public GameManager(Game game, Rules rules)
        {
            if (game == null || rules == null)
                throw new ArgumentNullException();

            this.game = game;
            this.rules = rules;
            this.saveHandler = new SaveHandler(SaveFile, game);
        }

        #endregion //Constructor

        #region Events

        public event EventHandler<GameChangedEventArgs> GameChanged;

        #endregion //Events

        #region Properties

        private readonly Game game;
        public Game Game
        {
            get { return game; }
        }

        private readonly Rules rules;
        public Rules Rules
        {
            get { return rules; }
        }

        /// <summary>
        /// Is true when the state of Game is chagning.
        /// </summary>
        private bool isChanging = false;
        public bool IsChanging
        {
            get { return isChanging; }
            private set { isChanging = value; }
        }

        #endregion //Properties

        #region Game Actions

        /// <summary>
        /// Loads the game with data from an xml file.
        /// </summary>
        /// <exception cref="System.IO.IOException">Could not find/read the file</exception>
        /// <exception cref="HeartsGameEngine.SaveHandlerException">File has probably an invalid format/state</exception>
        public void Load()
        {
            saveHandler.Load();
            OnGameChanged(new GameChangedEventArgs(GameAction.Reset));
        }

        /// <summary>
        /// Save Game to an xml file.
        /// </summary>
        /// <exception cref="System.IO.IOException">Could not find/write the file</exception>
        /// <exception cref="HeartsGameEngine.SaveHandlerException">Game has probably an invalid state</exception>
        public void Save()
        {
            saveHandler.Save();
        }

        /// <summary>
        /// Set Game to it's initial state
        /// </summary>
        public void Reset()
        {
            IsChanging = true;
            game.Reset();
            IsChanging = false;
            OnGameChanged(new GameChangedEventArgs(GameAction.Reset));
        }

        /// <summary>
        /// Starts a new round and deal the cards.
        /// </summary>
        /// <returns>True on success</returns>
        public bool StartNewRound()
        {
            if (IsChanging || !Rules.CanStartNewRound())
                return false;

            IsChanging = true;

            game.RoundCounter += 1;
            game.CurrentPlayer = -1;
            game.CurrentTrick.Clear();
            game.TrickHistory.Clear();

            for(int i = 0; i < Game.NumberOfPlayers; i++)
            {
                Player p = game.GetPlayerByIndex(i);
                p.Score.Add(0);
                p.Hand.Clear();
                p.PassedCards.Clear();
            }

            Deal();

            Direction passDirection = Rules.PassDirection();

            if(passDirection == Direction.None)
            {
                game.Phase = HeartsPhase.Tricks;
            }
            else
            {
                game.Phase = HeartsPhase.PassCards;
            }

            IsChanging = false;

            OnGameChanged(new GameChangedEventArgs(GameAction.StartNewRound));

            return true;
        }

        /// <summary>
        /// Starts a new turn
        /// </summary>
        /// <returns>True on success</returns>
        public bool StartNewTurn()
        {
            if (IsChanging || !Rules.CanStartNewTurn())
                return false;

            IsChanging = true;

            game.CurrentPlayer = rules.NextPlayer();

            IsChanging = false;

            OnGameChanged(new GameChangedEventArgs(GameAction.StartNewTurn, game.CurrentPlayer));

            return true;
        }

        /// <summary>
        /// Pass three cards. When all players have passed the cards are moved from the players hand to another players hand. Which hand depends on the pass direction.
        /// </summary>
        /// <param name="playerIndex">Player that is making the pass</param>
        /// <param name="cards">The cards to play</param>
        /// <returns>True on success</returns>
        public bool PassCards(int playerIndex, IList<Card> cards)
        {
            if (IsChanging || !Rules.ValidPass(playerIndex, cards))
                return false;

            IsChanging = true;

            var players = game.GetPlayers();
            Player p = players[playerIndex];
            int numPlayers = players.Count;

            p.PassedCards.Clear();
            foreach(Card c in cards)
                p.PassedCards.Add(c);

            bool allPlayersHavePassed = game.GetPlayers().All(x => x.PassedCards.Count == 3);

            if (allPlayersHavePassed)
            {
                int positions = (int)Rules.PassDirection();

                for (int i = 0; i < numPlayers; i++)
                {
                    Player p1 = players[i];
                    Player p2 = players[HelperMethods.Rotate(i, positions, numPlayers)];

                    for (int j = 0; j < 3; j++)
                    {
                        Card c = p1.PassedCards[j];
                        p1.Hand.Remove(c);
                        p2.Hand.Add(c);
                    }
                }

                game.Phase = HeartsPhase.Tricks;
            }

            IsChanging = false;

            OnGameChanged(new GameChangedEventArgs(GameAction.PassCards, playerIndex));

            return true;
        }

        /// <summary>
        /// Play a card
        /// </summary>
        /// <param name="playerIndex">Player that is making the play</param>
        /// <param name="card">The card to play</param>
        /// <returns>True on success</returns>
        public bool Play(int playerIndex, Card card)
        {
            if (IsChanging || !Rules.ValidPlay(playerIndex, card))
                return false;

            IsChanging = true;

            game.CurrentPlayer = -1;

            Player player = game.GetPlayerByIndex(playerIndex);

            player.Hand.Remove(card);

            TrickItem trickItem = new TrickItem(playerIndex, card);

            game.CurrentTrick.Add(trickItem);

            IsChanging = false;

            OnGameChanged(new GameChangedEventArgs(GameAction.Play, playerIndex));

            return true;
        }

        /// <summary>
        /// Add current trick to trick history, clear current trick, update score and update game phase.
        /// </summary>
        /// <returns>True on success</returns>
        public bool ClearTrick()
        {
            if (IsChanging || !Rules.CanClearTrick())
                return false;

            IsChanging = true;
            
            int trickWinner = Rules.TrickWinner(game.CurrentTrick);

            game.TrickHistory.Add(new ObservableCollection<TrickItem>(game.CurrentTrick));
            game.CurrentTrick.Clear();
            UpdateRoundScore();

            bool roundIsOver = game.TrickHistory.Count >= 13;

            // Check if round is finnished
            if (roundIsOver)
                game.Phase = HeartsPhase.None;

            IsChanging = false;

            OnGameChanged(new GameChangedEventArgs(GameAction.ClearTrick, trickWinner));

            return true;
        }

        #endregion //Game Actions

        #region Private Helpers

        /// <summary>
        /// Deal cards to players hands from a shuffled deck.
        /// </summary>
        private void Deal()
        {
            List<Card> deck = DeckFactory.Make();
            Helpers.HelperMethods.Shuffle(deck);

            int numCardsPerHand = 13;

            for (int i = 0; i < numCardsPerHand; i++)
            {
                foreach (Player p in game.GetPlayers())
                {
                    p.Hand.Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }
        }

        private void UpdateRoundScore()
        {
            int[] roundScore = Rules.CalcRoundScore();

            // Assign roundScore to player score
            for (int i = 0; i < roundScore.Length; i++)
            {
                Player p = game.GetPlayerByIndex(i);
                int lastRound = p.Score.Count;
                if(lastRound > 0)
                    p.Score[lastRound - 1] = roundScore[i];
            }
        }

        private void OnGameChanged(GameChangedEventArgs e)
        {
            if (GameChanged != null)
                GameChanged(this, e);
        }

        #endregion //Private Helpers
    }
}
