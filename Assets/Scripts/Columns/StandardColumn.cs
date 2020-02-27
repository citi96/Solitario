using System.Collections.Generic;
using Cards;
using Managers;
using UnityEngine;

namespace Columns {
    public abstract class StandardColumn : Column {
        [SerializeField] protected int addingScore;
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

        protected void UpdateScore(CardObject[] cardsToAdd, bool success) {
            if (success) {
                foreach (var cardObject in cardsToAdd) {
                    if (!cardObject.Card.HasBeenInVerticalColumn) {
                        GameManager.Instance.UpdateScore(addingScore);
                        cardObject.Card.HasBeenInVerticalColumn = true;
                    }
                    else {
                        UpdateCardOperation();
                    }
                }

                GameManager.Instance.UpdateMove();
            }
        }

        protected abstract void UpdateCardOperation();
    }
}