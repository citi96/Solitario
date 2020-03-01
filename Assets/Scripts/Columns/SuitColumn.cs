using System.Collections.Generic;
using System.Linq;
using Cards;
using Managers;
using Undo.Moves;
using UnityEngine;
using UnityEngine.UI;

namespace Columns {
    public class SuitColumn : StandardColumn {
        [SerializeField] private int removingScore;
        [SerializeField] private float spacing;
        [SerializeField] protected CardEnum.CardSuit suit;
        [SerializeField] protected Image suitImage;

        protected override CardEnum.CardSuit Suit {
            get => suit;
            set => suit = value;
        }

        public override float Spacing => spacing;

        private void Awake() {
            suitImage.sprite = Resources.Load<Sprite>($"Cards/Suits/{Suit.ToString()}");
        }

        public override bool AddCards(CardObject[] cardsToAdd) {
            bool success;
            var cardToAdd = cardsToAdd[0].Card;
            success = CanAddCard(cardToAdd);
            AddCardsToList(cardsToAdd, success);

            return success;
        }

        public override bool CanAddCard(Card cardToAdd) {
            bool success = false;
            if (Suit == cardToAdd.Suit) {
                if (!IsComplete && Cards.Count > 0) {
                    success = cardToAdd.Number == card.Number + 1;
                } else if (Cards.Count == 0) {
                    success = cardToAdd.Number == 1;
                }
            }

            return success;
        }

        public override void InstantiateMoveToUndo(StandardColumn toColumn, CardObject[] cardsMoved) {
            GameManager.Instance.AddMoveToUndo(new FromColumnMove(this, toColumn, cardsMoved,
                cardsMoved.Select(cardObject => cardObject.Card.HasBeenInVerticalColumn).ToList()));
            base.InstantiateMoveToUndo(toColumn, cardsMoved);
        }

        protected override void AddCardsToList(IEnumerable<CardObject> cardsToAdd, bool success) {
            var cardsArray = cardsToAdd as CardObject[] ?? cardsToAdd.ToArray();
            base.AddCardsToList(cardsArray, success);
            if (success) {
                StartCoroutine(GameManager.MoveCardsOnBoard(cardsArray[0], GameManager.Instance.GetCardDestinationPosition(transform, Spacing)));
            }
        }

        public override bool CanHaveHiddenCard() {
            return false;
        }

        public override void RemoveCards(IEnumerable<CardObject> cardsToRemove, bool removePoints = true) {
            base.RemoveCards(cardsToRemove, removePoints);
            if (removePoints)
                GameManager.Instance.UpdateScore(removingScore);
        }

        protected override void AddCardAlreadyAddedScore() {
            GameManager.Instance.UpdateScore(addingScore - 5);
        }
    }
}