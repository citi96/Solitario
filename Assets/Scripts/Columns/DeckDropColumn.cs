using System.Collections.Generic;
using System.Linq;
using Cards;
using Managers;
using Undo.Moves;
using UnityEngine;

namespace Columns {
    public class DeckDropColumn : Column {
        [SerializeField] private float spacing;

        public override float Spacing => spacing;

        public override bool AddCards(CardObject[] cardsToAdd) {
            cardsToAdd[0].transform.SetParent(transform);
            AddCardsToList(cardsToAdd, true);
            card = cardsToAdd[0].Card;
            cardsToAdd[0].Flip(false);
            return true;
        }

        public override void RemoveCards(IEnumerable<CardObject> card) {
            base.RemoveCards(card);
            DeckManager.Instance.UpdateDeckDropColumnAfterCardRemove(this);
        }

        public override void UpdateColumn() { }

        /// <summary>
        /// It's impossible to add a card directly to a DeckDropColumn.
        /// </summary>
        /// <param name="cardToAdd"></param>
        /// <returns>Always false</returns>
        public override bool CanAddCard(Card cardToAdd) {
            return false;
        }

        public override void InstantiateMoveToUndo(Column toColumn, CardObject[] cardsMoved) {
            GameManager.Instance.AddMoveToUndo(new FromDeckDropColumnMove(this, toColumn, cardsMoved));
        }

        /// <summary>
        /// The first visible card is the one that can be picked, so it must have its trigger enabled.
        /// Moreover in the first column the first visible card is the last one.
        /// </summary>
        /// <returns></returns>
        public override CardObject GetFirstVisibleCard() {
            return Cards.LastOrDefault(c => c.TriggerActive);
        }

        /// <summary>
        /// Does exactly what GetFirstVisibleCard does.
        /// </summary>
        /// <returns></returns>
        public override CardObject GetLastVisibleCard() {
            return GetFirstVisibleCard();
        }

        public override bool CanHaveHiddenCard() {
            return false;
        }
    }
}