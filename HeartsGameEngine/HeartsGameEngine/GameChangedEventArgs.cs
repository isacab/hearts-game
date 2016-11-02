using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartsGameEngine
{
    public enum GameAction
    {
        Reset, PassCards, Play, StartNewTurn, StartNewRound, ClearTrick
    }

    public class GameChangedEventArgs : EventArgs
    {
        public GameChangedEventArgs(GameAction action, int player = -1)
        {
            Action = action;
            Player = player;
        }

        public GameAction Action { get; set; }

        public int Player { get; set; }
    }
}
