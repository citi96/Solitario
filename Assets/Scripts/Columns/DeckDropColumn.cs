using System.Collections.Generic;
using Cards;
using Managers;
using UnityEngine;

namespace Columns {
    public class DeckDropColumn : Column {
        [SerializeField] private float spacing;

        public override float Spacing => spacing;

        public override bool AddCards(CardObject[] cardsToAdd) {
            cardsToAdd[0].transform.SetParent(transform);
            AddCardsToList(cardsToAdd, true);
            card = cardsToAdd[0].Card;
            cardsToAdd[0].Turn(false);
            return true;
        }

        public override void RemoveCards(IEnumerable<CardObject> card) {
            base.RemoveCards(card);
            GameManager.Instance.UpdateDeckDropColumnAfterCardRemove(this);
        }

        public override void UpdateColumn() { }
    }
}