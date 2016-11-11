using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine.AI
{
    public class MonteCarloAIPlayer : AIPlayer
    {
        protected override List<Card> GetCardsToPass(GameManager gameManager, int player)
        {
            List<Card> cards = new List<Card>();
            return cards;
        }

        protected override Card GetCardsToPlay(GameManager gameManager, int player)
        {
            Card card = null;
            return card;
        }
    }
}
