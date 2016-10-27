using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HeartsGameEngine.DataObjects
{
    public class TrickItem : IEquatable<TrickItem>
    {
        public TrickItem(int player, Card card)
        {
            Player = player;
            Card = card;
        }

        public TrickItem(XElement e)
        {
            // Player
            XElement playerEl = e.Element("Player");
            Player = Int32.Parse(playerEl.Value);

            // Card
            XElement cardEl = e.Element("Card").Element("Card");
            Card = new Card(cardEl);
        }

        public int Player { get; private set; }

        public Card Card { get; private set; }

        public bool Equals(TrickItem other)
        {
            return this.Card.Equals(other.Card) && this.Player == other.Player;
        }

        public override int GetHashCode()
        {
            return this.Card.GetHashCode() * 17 + this.Player.GetHashCode();
        }

        public XElement GenerateXElement()
        {
            return new XElement("TrickItem",
                        new XElement("Player", Player),
                        new XElement("Card", Card.GenerateXElement())
                    );
        }
    }
}
