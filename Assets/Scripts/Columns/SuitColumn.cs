using Cards;
using UnityEngine;

namespace Columns {
    public class SuitColumn : Column{
        [SerializeField] protected CardEnum.CardSuit suit;
        [SerializeField] protected CardEnum.SuitColor color;


        public override CardEnum.CardSuit Suit {
            get => suit;
            set => suit = value;
        }

        public override CardEnum.SuitColor Color {
            get => color;
            set => color = value;
        }
    }
}