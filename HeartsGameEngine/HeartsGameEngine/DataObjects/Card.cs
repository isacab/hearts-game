using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HeartsGameEngine.DataObjects
{
    public enum CardSuit
    {
        Hearts,
        Clubs,
        Diamonds,
        Spades
    }

    public enum CardValue
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public class Card : IEquatable<Card>
    {
        public Card(CardSuit suit, CardValue value)
        {
            this.suit = suit;
            this.value = value;
        }

        public Card(XElement e)
        {
            // Suit
            XElement suitEl = e.Element("Suit");
            suit = (CardSuit)Enum.Parse(typeof(CardSuit), suitEl.Value);

            // Value
            XElement valueEl = e.Element("Value");
            value = (CardValue)Enum.Parse(typeof(CardValue), valueEl.Value);
        }

        private readonly CardSuit suit;
        public CardSuit Suit 
        {
            get { return suit; }
        }

        private readonly CardValue value;
        public CardValue Value
        {
            get { return value; }
        }

        public bool Equals(Card other)
        {
            return this.Suit == other.Suit && this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return this.Suit.GetHashCode() * 17 + this.Value.GetHashCode();
        }

        public override string ToString()
        {
            string str = "" + this.Suit.ToString()[0];

            if ((int)this.Value < 11)
                str += this.Value.ToString("d");
            else
                str += this.Value.ToString()[0];

            return str;
        }

        public XElement GenerateXElement()
        {
            return new XElement("Card",
                       new XElement("Suit", Suit),
                       new XElement("Value", Value)
                       );
        }
    }
}
