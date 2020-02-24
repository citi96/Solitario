using Cards;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Columns {
    public abstract class Column : MonoBehaviour {
        [SerializeField] protected VerticalLayoutGroup verticalLayoutGroup;
        public abstract CardEnum.CardSuit Suit { get; set; }
        public abstract CardEnum.SuitColor Color { get; set; }

        public VerticalLayoutGroup VerticalLayoutGroup => verticalLayoutGroup;
    }
}