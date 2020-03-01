using System.Collections.Generic;
using System.Linq;
using Cards;
using Managers;
using Undo.Moves;
using UnityEngine;
using UnityEngine.UI;

namespace Columns {
    public class VerticalColumn : StandardColumn {
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        [SerializeField] private int maxCoveredCards;

        private bool _hasFlippedCard;

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

        public override void RemoveCards(IEnumerable<CardObject> cardsToRemove, bool removePoints = true) {
            base.RemoveCards(cardsToRemove);
            if (Cards.Count > 0) {
                FlipLastCard();
            }
        }

        protected override void AddCardAlreadyAddedScore() { }

        public override bool AddCards(CardObject[] cardsToAdd) {
            bool success;
            var cardToAdd = cardsToAdd[0].Card;
            success = CanAddCard(cardToAdd);
            AddCardsToList(cardsToAdd, success);

            return success;
        }

        public override bool CanAddCard(Card cardToAdd) {
            bool success = false;
            if (Cards.Count(c => c.Card.IsVisible) > 0 && !IsComplete)
                success = card.SuitColor != cardToAdd.SuitColor && card.Number == cardToAdd.Number + 1;
            else if (Cards.Count == 0) success = cardToAdd.Number == 13;
            return success;
        }

        public override void UpdateColumn() {
            var visibleCards = Cards.Where(cardObject => cardObject.Card.IsVisible).ToList();
            int turnedCardsCount = Cards.Count - visibleCards.Count;

            visibleCards.Sort();

            for (int i = 0; i < visibleCards.Count; i++)
                visibleCards.ElementAt(i).transform.SetSiblingIndex(turnedCardsCount++);
        }

        public override void InstantiateMoveToUndo(StandardColumn toColumn, CardObject[] cardsMoved) {
            GameManager.Instance.AddMoveToUndo(new FromVerticalColumnMove(this, toColumn, cardsMoved, _hasFlippedCard,
                cardsMoved.Select(cardObject => cardObject.Card.HasBeenInVerticalColumn).ToList()));
            _hasFlippedCard = false;
            base.InstantiateMoveToUndo(toColumn, cardsMoved);
        }

        public override bool CanHaveHiddenCard() {
            return true;
        }

        public void FlipLastCard(bool visible = true, bool animated = true) {
            int childCount = Cards.Count;

            if (childCount > 0 && !Cards[childCount - 1].Card.IsVisible == visible) {
                Cards[childCount - 1].Flip(!visible, animated);
                _hasFlippedCard = visible;
            }
        }

        public bool IsInitPhaseFinished(CardObject cardObject) {
            return Cards.Count == maxCoveredCards + 1 && Cards[maxCoveredCards].transform == cardObject.transform;
        }
    }
}