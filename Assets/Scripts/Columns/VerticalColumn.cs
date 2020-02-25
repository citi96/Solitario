using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

namespace Columns {
    public class VerticalColumn : Column {
        public override CardEnum.CardSuit Suit { get; set; }
        public override CardEnum.SuitColor Color { get; set; }

        public override bool AddCards(Card card) {
            bool success = false;
            int childCount = transform.childCount;
            var lastCard = childCount > 0 ? transform.GetChild(childCount - 1).GetComponent<CardObject>().Card : null;

            if (lastCard != null && childCount < 13) {
                success = lastCard.SuitColor != card.SuitColor && lastCard.Number == card.Number + 1;
            }
            else if (lastCard == null) {
                success = card.Number == 13;
            }

            return success;
        }

        public override void UpdateColumn() {
            var visibleCards = transform.Cast<Transform>().Select(child => child.GetComponent<CardObject>()).
                Where(card => card.Card.IsVisible).ToList();
            int turnedCardsCount = transform.childCount - visibleCards.Count;
            
            visibleCards.Sort();

            for (int i = 0; i < visibleCards.Count(); i++)
                visibleCards.ElementAt(i).transform.SetSiblingIndex(turnedCardsCount++);
            
        }

        public void TurnTopCard() {
            if (transform.childCount > 0 && !transform.GetChild(transform.childCount - 1).GetComponent<CardObject>().Card.IsVisible) {
                transform.GetChild(transform.childCount - 1).GetComponent<CardObject>().Turn();
            }
        }
    }
}