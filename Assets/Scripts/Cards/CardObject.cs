using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cards {
    public class CardObject : MonoBehaviour {
        [SerializeField] private Image back;
        [SerializeField] private Image[] suits;
        [SerializeField] private Image number;

        private Card _card;

        public Card Card {
            private get => _card;
            set {
                _card = value;
                InitCard();
            }
        }

        private void InitCard() {
            SetSuits();
            SetNumber();
        }

        private void SetNumber() {
            number.sprite = Resources.Load<Sprite>($"Cards/Numbers/{_card.Number}");
            PickNumberColor();
        }

        private void PickNumberColor() {
            switch (_card.SuitColor) {
                case CardEnum.SuitColor.Red:
                    number.color = Color.red;
                    break;
                case CardEnum.SuitColor.Black:
                    number.color = Color.black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSuits() {
            foreach (var suit in suits) {
                suit.sprite = Resources.Load<Sprite>($"Cards/Suits/{_card.Suit.ToString()}");
            }
        }

        public void Turn() {
            back.gameObject.SetActive(false);
            Card.IsVisible = true;
        }
    }
}