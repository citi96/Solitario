using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Columns {
    public abstract class Column : MonoBehaviour {
        protected readonly List<CardObject> cards = new List<CardObject>();
        protected int number;
        [SerializeField] protected VerticalLayoutGroup verticalLayoutGroup;

        protected abstract CardEnum.CardSuit Suit { get; set; }

        public VerticalLayoutGroup VerticalLayoutGroup => verticalLayoutGroup;

        public IEnumerable<CardObject> PickCardsFromColumn(CardObject cardObject) {
            var children = GetComponentsInChildren<CardObject>();
            int i = 0;

            for (; i < children.Length; i++)
                if (children[i].Equals(cardObject)) {
                    break;
                    ;
                }

            return children.Skip(i).ToArray();
        }

        public abstract bool AddCards(Card[] cardsToAdd);

        /// <summary>
        ///     Used in VerticalColumn to restore cards order after a dragging operation with multiple cards.
        ///     Sometimes order is lost after such operation, this method prevents this possibility.
        /// </summary>
        public abstract void UpdateColumn();

        public abstract void AddCoveredCards(CardObject card);
        public abstract void RemoveCards(IEnumerable<CardObject> cards);
    }
}