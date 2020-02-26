using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

namespace Columns {
    public class VerticalColumn : Column {
        [SerializeField] private int maxCoveredCards;
        protected override CardEnum.CardSuit Suit { get; set; }
        private CardEnum.SuitColor Color { get; set; }

        public override void AddCoveredCards(CardObject card) {
            card.transform.SetParent(transform);
            cards.Add(card);

            if (cards.Count == maxCoveredCards + 1) UpdateLastCard(card.Card);
        }

        public override void RemoveCards(IEnumerable<CardObject> cardsToRemove) {
            foreach (var cardToRemove in cardsToRemove) cards.Remove(cardToRemove);

            UpdateLastCard(cards[cards.Count - 1].Card);
            TurnTopCard();
        }

        private void UpdateLastCard(Card card) {
            Suit = card.Suit;
            Color = card.SuitColor;
            number = card.Number;
        }

        public override bool AddCards(Card[] cardsToAdd) {
            bool success = false;
            int childCount = cards.Count;

            if (number != 0 && childCount < 13)
                success = Color != cardsToAdd[0].SuitColor && number == cardsToAdd[0].Number + 1;
            else if (number == 0) success = cardsToAdd[0].Number == 13;

            if (success) UpdateLastCard(cardsToAdd[cardsToAdd.Length - 1]);

            return success;
        }

        public override void UpdateColumn() {
            var visibleCards = cards.Where(card => card.Card.IsVisible).ToList();
            int turnedCardsCount = cards.Count - visibleCards.Count;

            visibleCards.Sort();

            for (int i = 0; i < visibleCards.Count(); i++)
                visibleCards.ElementAt(i).transform.SetSiblingIndex(turnedCardsCount++);
        }

        public void TurnTopCard() {
            int childCount = cards.Count;

            if (childCount > 0 && !cards[childCount - 1].Card.IsVisible) cards[childCount - 1].Turn();
        }

        public bool IsInitPhaseFinished(CardObject cardObject) {
            return cards.Count == maxCoveredCards + 1 && cards[maxCoveredCards].transform == cardObject.transform;
        }
    }
}