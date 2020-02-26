using System.Collections.Generic;
using Cards;

namespace Columns {
    public abstract class StandardColumn : Column {
        protected abstract CardEnum.CardSuit Suit { get; set; }

        public bool IsComplete => (Cards.Count == 13);

        protected virtual void UpdateLastCard(Card newCard) {
            card = newCard;
        }

        protected override void AddCardsToList(IEnumerable<CardObject> cardsToAdd, bool success) {
            base.AddCardsToList(cardsToAdd, success);
            if (success)
                UpdateLastCard(Cards[Cards.Count - 1].Card);
        }

        public override void RemoveCards(IEnumerable<CardObject> cardsToRemove) {
            base.RemoveCards(cardsToRemove);
            if (Cards.Count > 0) {
                UpdateLastCard(Cards[Cards.Count - 1].Card);
            }
            else {
                card = null;
            }
        }
    }
}