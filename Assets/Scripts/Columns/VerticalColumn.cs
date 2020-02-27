using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Columns {
    public class VerticalColumn : StandardColumn {
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        [SerializeField] private int maxCoveredCards;
        protected override CardEnum.CardSuit Suit { get; set; }

        public override float Spacing => layoutGroup.spacing;

        public bool LayoutGroupEnabled {
            set => layoutGroup.enabled = value;
        }

        public void AddCoveredCards(CardObject card) {
            card.transform.SetParent(transform);
            Cards.Add(card);

            if (Cards.Count == maxCoveredCards + 1) UpdateLastCard(card.Card);
        }

        public override void RemoveCards(IEnumerable<CardObject> cardsToRemove) {
            base.RemoveCards(cardsToRemove);
            if (Cards.Count > 0) {
                TurnTopCard();
            }
        }

        protected override void UpdateCardOperation() { }


        public override bool AddCards(CardObject[] cardsToAdd) {
            bool success = false;
            var cardToAdd = cardsToAdd[0].Card;

            if (Cards.Count > 0 && !IsComplete)
                success = card.SuitColor != cardToAdd.SuitColor && card.Number == cardToAdd.Number + 1;
            else if (Cards.Count == 0) success = cardToAdd.Number == 13;

            AddCardsToList(cardsToAdd, success);
            UpdateScore(cardsToAdd, success);

            return success;
        }

        public override void UpdateColumn() {
            var visibleCards = Cards.Where(card => card.Card.IsVisible).ToList();
            int turnedCardsCount = Cards.Count - visibleCards.Count;

            visibleCards.Sort();

            for (int i = 0; i < visibleCards.Count; i++)
                visibleCards.ElementAt(i).transform.SetSiblingIndex(turnedCardsCount++);
        }

        public void TurnTopCard() {
            int childCount = Cards.Count;

            if (childCount > 0 && !Cards[childCount - 1].Card.IsVisible) {
                Cards[childCount - 1].Flip(false);
            }
        }

        public bool IsInitPhaseFinished(CardObject cardObject) {
            return Cards.Count == maxCoveredCards + 1 && Cards[maxCoveredCards].transform == cardObject.transform;
        }
    }
}