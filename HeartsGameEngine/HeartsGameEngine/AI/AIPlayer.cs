using HeartsGameEngine.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartsGameEngine.AI
{
    public abstract class AIPlayer : IAIPlayer
    {
        public virtual void MakeAction(GameManager gameManager, int player)
        {
            Rules rules = gameManager.Rules;

            if (rules.CanPassCards(player))
            {
                List<Card> cards = GetCardsToPass(gameManager, player);
                gameManager.PassCards(player, cards);
            }

            if (rules.CanPlay(player))
            {
                Card card = GetCardsToPlay(gameManager, player);
                gameManager.Play(player, card);
            }
        }

        protected abstract Card GetCardsToPlay(GameManager gameManager, int player);

        protected abstract List<Card> GetCardsToPass(GameManager gameManager, int player);
    }
}
