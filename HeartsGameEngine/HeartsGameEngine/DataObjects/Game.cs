using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HeartsGameEngine.DataObjects
{
    using Trick = ObservableCollection<TrickItem>;

    public enum HeartsPhase
    {
        None, PassCards, Tricks
    }

    public class Game : DataObject
    {
        #region Fields

        public const int NumberOfPlayers = 4;

        private readonly List<Player> players = new List<Player>() 
        { 
            new Player(), new Player(), new Player(), new Player() 
        };

        #endregion // Fields

        #region Constructor

        public Game() {}

        public Game(XElement e)
        {
            Load(e);
        }

        #endregion // Constructor

        #region Properties

        private int currentPlayer = -1;
        public int CurrentPlayer 
        {
            get { return currentPlayer; }
            set { SetValue(ref currentPlayer, value); }
        }

        private int roundCounter = 0;
        public int RoundCounter
        {
            get { return roundCounter; }
            set { SetValue(ref roundCounter, value); }
        }

        private HeartsPhase phase = HeartsPhase.None;
        public HeartsPhase Phase
        {
            get { return phase; }
            set { SetValue(ref phase, value); }
        }

        /*private readonly ObservableCollection<Player> players2 = new ObservableCollection<Player>();
        public ObservableCollection<Player> Players
        {
            get { return players2; }
        }*/

        public Player Player1
        {
            get { return players[0]; }
        }

        public Player Player2
        {
            get { return players[1]; }
        }

        public Player Player3
        {
            get { return players[2]; }
        }

        public Player Player4
        {
            get { return players[3]; }
        }

        private Trick currentTrick = new Trick();
        public Trick CurrentTrick
        {
            get { return currentTrick; }
            set { SetValue(ref currentTrick, value); }
        }

        private ObservableCollection<Trick> trickHistory = new ObservableCollection<Trick>();
        public ObservableCollection<Trick> TrickHistory
        {
            get { return trickHistory; }
            set { SetValue(ref trickHistory, value); }
        }

        #endregion // Properties

        #region Methods

        public int GetIndexByPlayer(Player player)
        {
            return players.IndexOf(player);
        }

        public Player GetPlayerByIndex(int index)
        {
            return players[index];
        }

        public List<Player> GetPlayers()
        {
            return new List<Player>(players);
        }

        public override XElement GenerateXElement()
        {
            // Game
            XElement rootEl = new XElement("Game");

            // Current Player
            XElement currentPlayerEl = new XElement("CurrentPlayer", CurrentPlayer);
            rootEl.Add(currentPlayerEl);

            // RoundCounter
            XElement roundCounterEl = new XElement("RoundCounter", RoundCounter);
            rootEl.Add(roundCounterEl);

            // Phase
            XElement phaseEl = new XElement("Phase", Phase.ToString());
            rootEl.Add(phaseEl);

            // Player1
            XElement player1El = new XElement("Player1", Player1.GenerateXElement());
            rootEl.Add(player1El);

            // Player2
            XElement player2El = new XElement("Player2", Player2.GenerateXElement());
            rootEl.Add(player2El);

            // Player3
            XElement player3El = new XElement("Player3", Player3.GenerateXElement());
            rootEl.Add(player3El);

            // Player4
            XElement player4El = new XElement("Player4", Player4.GenerateXElement());
            rootEl.Add(player4El);

            // CurrentTrick
            XElement currentTrickEl = new XElement("CurrentTrick", (from item in currentTrick 
                                                                    select item.GenerateXElement()).ToList());
            rootEl.Add(currentTrickEl);

            // TrickHistory
            XElement trickHistoryEl = new XElement("TrickHistory", (from trick in trickHistory
                                                                    select new XElement("Trick",
                                                                        from item  in trick
                                                                        select item.GenerateXElement())).ToList());
            rootEl.Add(trickHistoryEl);

            return rootEl;
        }

        public override void Load(XElement e)
        {
            // CurrentPlayer
            XElement currentPlayerEl = e.Element("CurrentPlayer");
            this.CurrentPlayer = Int32.Parse(currentPlayerEl.Value);

            // RoundCounter
            XElement roundCounterEl = e.Element("RoundCounter");
            this.RoundCounter = Int32.Parse(roundCounterEl.Value);

            // Phase
            XElement phaseEl = e.Element("Phase");
            this.Phase = (HeartsPhase)Enum.Parse(typeof(HeartsPhase), phaseEl.Value);

            // Player 1
            XElement player1El = e.Element("Player1").Element("Player");
            this.Player1.Load(player1El);

            // Player 2
            XElement player2El = e.Element("Player2").Element("Player");
            this.Player2.Load(player2El);

            // Player 3
            XElement player3El = e.Element("Player3").Element("Player");
            this.Player3.Load(player3El);

            // Player 4
            XElement player4El = e.Element("Player4").Element("Player");
            this.Player4.Load(player4El);

            // CurrentTrick
            XElement currentTrickEl = e.Element("CurrentTrick");
            var currentTrick = (from item in currentTrickEl.Elements()
                                select new TrickItem(item)).ToList();
            Helpers.HelperMethods.UpdateList(this.CurrentTrick, currentTrick);

            // TrickHistory
            XElement trickHistoryEl = e.Element("TrickHistory");
            var trickHistory = (from trick in trickHistoryEl.Elements()
                                select new Trick(
                                    from item in trick.Elements()
                                    select new TrickItem(item)
                                    )
                                );
            this.TrickHistory.Clear();
            foreach (var t in trickHistory)
                this.TrickHistory.Add(t);
        }

        public void Reset()
        {
            this.RoundCounter = 0;
            this.CurrentPlayer = -1;
            this.Phase = HeartsPhase.None;
            this.CurrentTrick.Clear();
            this.TrickHistory.Clear();
            foreach (var p in this.players)
                p.Reset();
        }

        #endregion // Methods
    }
}
