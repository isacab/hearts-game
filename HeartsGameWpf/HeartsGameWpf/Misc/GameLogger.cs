using HeartsGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameWpf.Misc
{
    class GameLogger
    {
        GameManager gameManager;

        public GameLogger(GameManager gameManager)
        {
            this.gameManager = gameManager;

            SetEventHandlers();
        }

        private void OnInitializedNextDeal(object sender, EventArgs e)
        {
            SetEventHandlers();
        }

        private void SetEventHandlers()
        {
            /*gameManager.NewRound += OnNewRound;
            gameManager.NewTurn += OnNewTurn;
            gameManager.Played += OnPlayed;
            gameManager.PassedCards += OnPassedCards;
            gameManager.Reseted += OnReseted;*/
        }

        /*private void OnPassedCards(object sender, PassedCardsEventArgs e)
        {
            Console.WriteLine("Passed cards, player: {0}, cards: {1}, {2}, {3}", e.Player, e.Cards[0], e.Cards[1], e.Cards[2]);

            if(e.AllPlayersHavePassed)
                Console.WriteLine("All players have passed.");

            Console.WriteLine("");
        }

        private void OnPlayed(object sender, PlayedEventArgs e)
        {
            Console.WriteLine("Played, player: {0}, card: {1}\n", e.Player, e.Card.ToString());
        }

        public void OnNewRound(object sender, NewRoundEventArgs e)
        {
            Console.WriteLine("New round started, round: {0}.\n", e.Round);
        }

        public void OnNewTurn(object sender, NewTurnEventArgs e)
        {
            Console.WriteLine("New turn, current player: {0}\n", e.CurrentPlayer);
        }

        public void OnReseted(object sender, EventArgs e)
        {
            Console.WriteLine("Reseted");
        }*/
    }
}
