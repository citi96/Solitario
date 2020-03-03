using Cards;
using UnityEngine;

namespace Columns {
    public class ColumnCollider : MonoBehaviour {
        [SerializeField] private StandardColumn column;
        [SerializeField] private float canvasDefaultScaling;

        public StandardColumn Column => column;

        private void Update() {
            int columnChildCount = Column.transform.childCount;

            transform.position = columnChildCount > 0 ? Column.transform.GetChild(columnChildCount - 1).position : column.transform.position;
        }

        public bool CanAddCards(CardObject[] cards) {
            return Column.AddCards(cards);
        }
    }
}