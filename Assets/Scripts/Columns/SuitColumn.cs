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
            bool success = false;
            var cardToAdd = cardsToAdd[0].Card;

            if (Suit == cardToAdd.Suit) {
                if (!IsComplete && Cards.Count > 0) {
                    success = cardToAdd.Number == card.Number + 1;
                }
                else if (Cards.Count == 0) {
                    success = cardToAdd.Number == 1;
                }
            }

            AddCardsToList(cardsToAdd, success);
            UpdateScore(cardsToAdd, success);

            return success;
        }

        public override void InstantiateMoveToUndo(Column toColumn, CardObject[] cardsMoved) {
            GameManager.Instance.AddMoveToUndo(new FromColumnMove(this, toColumn, cardsMoved));
        }

        protected override void AddCardsToList(IEnumerable<CardObject> cardsToAdd, bool success) {
            var cardsArray = cardsToAdd as CardObject[] ?? cardsToAdd.ToArray();
            base.AddCardsToList(cardsArray, success);
            if (success) {
                StartCoroutine(GameManager.Instance.MoveCardsOnBoard(cardsArray[0], GameManager.Instance.GetCardDestinationPosition(transform, Spacing)));
            }
        }

        public override void RemoveCards(IEnumerable<CardObject> cardsToRemove) {
            base.RemoveCards(cardsToRemove);
            GameManager.Instance.UpdateScore(removingScore);
        }

        protected override void UpdateCardOperation() {
            GameManager.Instance.UpdateScore(addingScore - 5);
        }
    }
}