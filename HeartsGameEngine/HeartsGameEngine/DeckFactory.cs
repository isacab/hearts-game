using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine
{
    static class DeckFactory
    {
        public static List<Card> Make()
        {
            List<Card> deck = new List<Card>();

            for (int s = 0; s < 4; s++)
            {
                for (int v = 2; v <= 14; v++)
                {
                    Card card = new Card((CardSuit)s, (CardValue)v);
                    deck.Add(card);
                }
            }

            return deck;
        }
    }
}
