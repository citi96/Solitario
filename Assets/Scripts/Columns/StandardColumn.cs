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

        public override void RemoveCards(IEnumerable<CardObject> cardsToRemove, bool removePoints = false) {
            base.RemoveCards(cardsToRemove, removePoints);
            if (Cards.Count > 0) {
                UpdateLastCard(Cards[Cards.Count - 1].Card);
            } else {
                card = null;
            }
        }

        // Update last card ultimate check
        public override void UpdateColumn() {
            Card newCard;
            if (Cards.Count > 0 && Cards[Cards.Count - 1].Card != (newCard = card)) {
                card = newCard;
            }
        }

        public override void InstantiateMoveToUndo(StandardColumn toColumn, CardObject[] cardsMoved) {
            toColumn.UpdateScore(cardsMoved);
        }

        public void UpdateScore(CardObject[] cardsToAdd) {
            foreach (var cardObject in cardsToAdd) {
                if (!cardObject.Card.HasBeenInVerticalColumn) {
                    GameManager.Instance.UpdateScore(addingScore);
                    cardObject.Card.HasBeenInVerticalColumn = true;
                } else {
                    AddCardAlreadyAddedScore();
                }
            }

            GameManager.Instance.UpdateMove();
        }

        protected abstract void AddCardAlreadyAddedScore();
    }
}