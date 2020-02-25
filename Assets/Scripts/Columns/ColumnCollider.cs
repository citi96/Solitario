using System;
using Cards;
using UnityEngine;

namespace Columns {
    public class ColumnCollider : MonoBehaviour{
        [SerializeField] private Column column;
        [SerializeField] private Collider2D collider;

        public Column Column => column;

        private void Update() {
            int columnChildCount = Column.transform.childCount;

            if (columnChildCount > 0) {
                transform.position = Column.transform.GetChild(columnChildCount - 1).position;
            }
        }

        public bool CanAddCards(Card card) {
            return Column.AddCards(card);
        }
    }
}