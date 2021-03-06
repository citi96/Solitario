using System;
using System.Collections;
using System.Linq;
using Columns;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cards {
    public class CardObject : MonoBehaviour, IComparable {
        [SerializeField] private Image back;
        [SerializeField] private Image number;
        [SerializeField] private Image[] suits;
        [SerializeField] private EventTrigger trigger;

        private Card _card;
        private DraggedCards _draggedCards;
        private Column _draggedColumn;
        private GameManager _gm;

        public bool IsInDeck { get; set; } = false;

        public Card Card {
            get => _card;
            set {
                _card = value;
                InitCard();
            }
        }

        public bool TriggerActive {
            get => trigger.gameObject.activeSelf;
            set => trigger.gameObject.SetActive(value);
        }

        private void Start() {
            _gm = GameManager.Instance;
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

        /// <summary>
        /// Flip the card up or down.
        /// </summary>
        /// <param name="down">True = card is facing down (back visible); False = card is facing up.</param>
        /// <param name="animated">Disable animation during rollback</param>
        public void Flip(bool down, bool animated = true) {
            if (animated)
                StartCoroutine(PlayFlipAnimation(down ? 0 : 180, down, down ? -1 : 1));
            else
                FlipNoAnimation(down ? 0 : 180, down, down ? -1 : 1);

            Card.IsVisible = !down;
        }

        private void FlipNoAnimation(int y, bool down, int direction) {
            while (Math.Abs(transform.eulerAngles.y - y) > 1f) {
                transform.Rotate(Vector3.up, 10 * direction);
                if (Math.Abs(transform.eulerAngles.y - 90) < 1f) {
                    back.gameObject.SetActive(down);
                }
            }
        }

        private IEnumerator PlayFlipAnimation(float y, bool down, int direction) {
            while (Math.Abs(transform.eulerAngles.y - y) > 1f) {
                transform.Rotate(Vector3.up, 10 * direction);
                if (Math.Abs(transform.eulerAngles.y - 90) < 1f) {
                    back.gameObject.SetActive(down);
                }

                yield return new WaitForSeconds(0.02f);
            }
        }

        public void OnCardSelected() {
            // Do not enable dragging operation if others are still pending
            if (_gm.DraggedCards.transform.childCount > 0 && _gm.Waiting)
                return;

            _draggedCards = _gm.DraggedCards;
            _draggedColumn = GetComponentInParent<Column>();
            var cardsToDrag = _draggedColumn.PickCardsFromColumn(this);

            foreach (var card in cardsToDrag) {
                card.transform.SetParent(_draggedCards.transform);
                var destination = _gm.GetCardDestinationPosition(_draggedCards.transform, _draggedCards.Spacing);
                card.transform.position = destination;
            }
        }

        public void OnCardMoved() {
            // Drag cards only if selected
            if (_draggedCards != null)
                _draggedCards.MoveCards();
        }

        public void OnCardDropped() {
            if (_draggedCards == null) return;

            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            ColumnCollider hitCollider;

            if (hit.collider != null && (hitCollider = hit.collider.GetComponent<ColumnCollider>()) != null) {
                var cardsToAdd = _draggedCards.transform.Cast<Transform>().Select(child => child.GetComponent<CardObject>()).ToArray();
                if (hitCollider.CanAddCards(cardsToAdd)) {
                    _draggedCards.AddCardToColumn(hitCollider.Column);
                    _draggedColumn.RemoveCards(cardsToAdd);
                    //_gm.NotifyMoveToColumns();
                    _draggedColumn.InstantiateMoveToUndo(hitCollider.Column, cardsToAdd);
                    return;
                }
            }

            _draggedCards.MoveCardsToColumn(_gm.GetCardDestinationPosition(_draggedColumn.transform, _draggedColumn.Spacing, 1),
                0.02f, _draggedColumn);

            StartCoroutine(_gm.StartWaiting(0.2f));
            _draggedCards = null;
            _draggedColumn = null;
        }

        public void TurnCardFromDeck() {
            if (IsInDeck) {
                DeckManager.Instance.PickCardFromDeckAfterClick(this);
            }
        }
    }
}