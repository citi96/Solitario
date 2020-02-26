using System;
using System.Collections.Generic;
using Cards;
using UnityEngine;

namespace Columns {
    public class SuitColumn : Column {
        [SerializeField] protected CardEnum.CardSuit suit;

        protected override CardEnum.CardSuit Suit {
            get => suit;
            set => suit = value;
        }

        public override bool AddCards(Card[] cardsToAdd) {
            throw new NotImplementedException();
        }

        public override void UpdateColumn() { }

        public override void AddCoveredCards(CardObject card) {
            throw new NotImplementedException();
        }

        public override void RemoveCards(IEnumerable<CardObject> cards) {
            throw new NotImplementedException();
        }
    }
}