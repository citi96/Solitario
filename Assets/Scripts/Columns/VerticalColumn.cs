using Cards;
using UnityEngine;

namespace Columns {
    public class VerticalColumn : Column {
        public override CardEnum.CardSuit Suit { get; set; }
        public override CardEnum.SuitColor Color { get; set; }
    }
}