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

        public abstract bool AddCards(CardObject[] cardsToAdd);

        public virtual void RemoveCards(IEnumerable<CardObject> cards) {
            foreach (var cardToRemove in cards) {
                Cards.Remove(cardToRemove);
            }
        }

        /// <summary>
        /// Used in VerticalColumn to restore cards order after a dragging operation with multiple cards.
        /// Sometimes order is lost after such operation, this method prevents this possibility. 
        /// </summary>
        public abstract void UpdateColumn();

        public void RollbackCard(CardObject[] cards) {
            AddCardsToList(cards, true);
            foreach (var cardObject in cards) {
                Transform cardTransform;
                (cardTransform = cardObject.transform).SetParent(transform);
                cardTransform.position = transform.position;
            }
        }

        public abstract void InstantiateMoveToUndo(Column toColumn, CardObject[] cardsMoved);

        protected virtual void AddCardsToList(IEnumerable<CardObject> cardsToAdd, bool success) {
            if (success) {
                foreach (var cardObject in cardsToAdd) {
                    Cards.Add(cardObject);
                }
            }
        }
    }
}