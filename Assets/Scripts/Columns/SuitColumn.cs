using System.Collections.Generic;
using System.Linq;
using Cards;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Columns {
    public class SuitColumn : StandardColumn {
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

            return success;
        }

        protected override void AddCardsToList(IEnumerable<CardObject> cardsToAdd, bool success) {
            var cardsArray = cardsToAdd as CardObject[] ?? cardsToAdd.ToArray();
            base.AddCardsToList(cardsArray, success);
            if (success) {
                StartCoroutine(GameManager.MoveCardsOnBoard(cardsArray[0], GameManager.GetCardDestinationPosition(transform, Spacing)));
            }
        }

        public override void UpdateColumn() { }
    }
}