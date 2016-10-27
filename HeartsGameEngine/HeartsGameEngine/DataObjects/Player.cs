using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HeartsGameEngine.Helpers;

namespace HeartsGameEngine.DataObjects
{
    public class Player : DataObject
    {
        #region Constructor

        public Player() {}

        public Player(XElement element)
        {
            Load(element);
        }

        # endregion // Constructor

        #region Properties

        private readonly ObservableCollection<Card> hand = new ObservableCollection<Card>();
        public ObservableCollection<Card> Hand
        {
            get { return hand; }
        }

        private readonly ObservableCollection<Card> passedCards = new ObservableCollection<Card>();
        public ObservableCollection<Card> PassedCards
        {
            get { return passedCards; }
        }

        private readonly ObservableCollection<int> score = new ObservableCollection<int>();
        public ObservableCollection<int> Score
        {
            get { return score; }
        }

        #endregion // Properties

        #region Methods

        public int TotalScore()
        {
            int totalScore = 0;

            foreach (int roundScore in score)
            {
                totalScore += roundScore;
            }

            return totalScore;
        }

        public override XElement GenerateXElement()
        {
            // Hand
            XElement handEl = new XElement("Hand", from card in hand
                                                   select card.GenerateXElement());

            // PassedCards
            XElement passedCardsEl = new XElement("PassedCards", from card in passedCards
                                                                 select card.GenerateXElement());

            // Score
            XElement scoreEl = new XElement("Score", from rs in score
                                                     select new XElement("RoundScore", rs));

            // Player
            XElement rootEl = new XElement("Player");
            rootEl.Add(handEl);
            rootEl.Add(passedCardsEl);
            rootEl.Add(scoreEl);

            return rootEl;
        }

        public override void Load(XElement e)
        {
            // Hand
            XElement handEl = e.Element("Hand");
            var hand = (from card in handEl.Elements()
                        select new Card(card)).ToList();
            HelperMethods.UpdateList(this.hand, hand);

            // PassedCards
            XElement passedCardsEl = e.Element("PassedCards");
            var passedCards = (from card in passedCardsEl.Elements()
                               select new Card(card)).ToList();
            HelperMethods.UpdateList(this.passedCards, passedCards);

            // Score
            XElement scoreEl = e.Element("Score");
            var score = (from rs in scoreEl.Elements()
                         select Int32.Parse(rs.Value)).ToList();
            HelperMethods.UpdateList(this.score, score);
        }

        #endregion //Methods

        internal void Reset()
        {
            this.hand.Clear();
            this.passedCards.Clear();
            this.score.Clear();
        }
    }
}
