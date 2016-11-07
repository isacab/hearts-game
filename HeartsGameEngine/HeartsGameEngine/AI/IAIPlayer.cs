using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine.AI
{
    public interface IAIPlayer
    {
        void MakeAction(GameManager gameManager, int player);
    }
}
