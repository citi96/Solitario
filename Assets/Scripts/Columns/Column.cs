using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

namespace Columns {
    public abstract class Column : MonoBehaviour {
        protected Card card;

        public abstract float Spacing { get; }
        public List<CardObject> Cards { get; } = new List<CardObject>();

        public IEnumerable<CardObject> PickCardsFromColumn(CardObject cardObject) {
            var children = GetComponentsInChildren<CardObject>();
            int i = 0;

            for (; i < children.Length; i++)
                if (children[i].Equals(cardObject)) {
                    break;
                }

            return children.Skip(i).ToArray();
        }

        /// <summary>
        /// Use this method to add cards to column, use CanAddsCard to check if a card can be added to the column without actually adding it.
        /// </summary>
        /// <param name="cardsToAdd">Cards to be added to the column if possible</param>
        /// <returns>Return if this operation has success</returns>
        public abstract bool AddCards(CardObject[] cardsToAdd);

        public virtual void RemoveCards(IEnumerable<CardObject> cards, bool removePoints = true) {
            foreach (var cardToRemove in cards) {
                Cards.Remove(cardToRemove);
            }
        }

        /// <summary>
        /// Used in VerticalColumn to restore cards order after a dragging operation with multiple cards.
        /// Sometimes order is lost after such operation, this method prevents this possibility. 
        /// </summary>
        public abstract void UpdateColumn();

        /// <summary>
        /// Check whenever or not a card can be added to the column without adding it the list.
        /// </summary>
        /// <param name="cardToAdd">Te card to check</param>
        /// <returns>The result of the check</returns>
        public abstract bool CanAddCard(Card cardToAdd);

        public void RollbackCard(CardObject[] cards) {
            AddCardsToList(cards, true);
            foreach (var cardObject in cards) {
                Transform cardTransform;
                (cardTransform = cardObject.transform).SetParent(transform);
                cardTransform.position = transform.position;
            }
        }

        public abstract void InstantiateMoveToUndo(StandardColumn toColumn, CardObject[] cardsMoved);

        protected virtual void AddCardsToList(IEnumerable<CardObject> cardsToAdd, bool success) {
            if (success) {
                foreach (var cardObject in cardsToAdd) {
                    Cards.Add(cardObject);
                }
            }
        }

        public virtual CardObject GetFirstVisibleCard() {
            return Cards.FirstOrDefault(c => c.Card.IsVisible);
        }

        public virtual CardObject GetLastVisibleCard() {
            return Cards.LastOrDefault(c => c.Card.IsVisible);
        }

        public abstract bool CanHaveHiddenCard();
    }
}