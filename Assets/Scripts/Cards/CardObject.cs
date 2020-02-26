using System;
using System.Linq;
using Columns;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Cards {
    public class CardObject : MonoBehaviour, IComparable {
        private Card _card;
        private DraggedCards _draggedCards;
        private Column _draggedColumn;
        [SerializeField] private Image back;
        [SerializeField] private Image number;
        [SerializeField] private Image[] suits;

        public Card Card {
            get => _card;
            set {
                _card = value;
                InitCard();
            }
        }

        public int CompareTo(object obj) {
            if (obj is CardObject other)
                return other.Card.Number - _card.Number;
            return 0;
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
            foreach (var suit in suits) suit.sprite = Resources.Load<Sprite>($"Cards/Suits/{_card.Suit.ToString()}");
        }

        public void Turn() {
            back.gameObject.SetActive(false);
            Card.IsVisible = true;
        }

        public void OnCardSelected() {
            // Do not enable dragging operation if others are still pending
            if (GameManager.Instance.DraggedCards.transform.childCount > 0)
                return;

            _draggedCards = GameManager.Instance.DraggedCards;
            _draggedColumn = GetComponentInParent<Column>();
            var cardsToDrag = _draggedColumn.PickCardsFromColumn(this);

            _draggedCards.gameObject.SetActive(true);

            foreach (var card in cardsToDrag) {
                card.transform.SetParent(_draggedCards.transform);
                var destination = GameManager.GetCardDestinationPosition(_draggedCards.transform, _draggedCards.Spacing);
                card.transform.position = destination;
            }
        }

        public void OnCardMoved() {
            // Drag cards only if selected
            if (_draggedCards != null)
                _draggedCards.MoveCards();
        }

        public void OnCardDropped() {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) {
                var hitCollider = hit.collider.GetComponent<ColumnCollider>();
                var cardsToAdd = _draggedCards.transform.Cast<Transform>().Select(child => child.GetComponent<CardObject>()).ToArray();
                if (hitCollider.CanAddCards(cardsToAdd.Select(c => c.Card).ToArray())) {
                    _draggedColumn.RemoveCards(cardsToAdd);
                    _draggedCards.AddCardToColumn(hitCollider.Column);
                    GameManager.Instance.NotifyMoveToColumns();
                    return;
                }
            }

            _draggedCards.MoveCardsToColumn(GameManager.GetCardDestinationPosition(_draggedColumn.transform, _draggedColumn.VerticalLayoutGroup.spacing),
                0.02f, _draggedColumn);

            _draggedCards = null;
            _draggedColumn = null;
        }
    }
}