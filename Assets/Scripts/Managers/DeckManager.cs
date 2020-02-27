using System;
using Cards;
using Columns;
using UnityEngine;

namespace Managers {
    public class DeckManager : MonoBehaviour {
        [SerializeField] private Column[] deckDropColumns;

        private bool _isDrawThree;
        private int _dropTransformCount;

        public static DeckManager Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start() {
            _isDrawThree = Convert.ToBoolean(PlayerPrefs.GetInt("DrawThree"));
        }

        public void PickCardFromDeckAfterClick(CardObject card) {
            if (!GameManager.Instance.Waiting) {
                StartCoroutine(GameManager.Instance.StartWaiting());

                if (_isDrawThree)
                    PickCardFromDeckDrawThree(card);
                else
                    PickCardFromDeckClassic(card);

                GameManager.Instance.UpdateMove();
            }
        }

        private void PickCardFromDeckDrawThree(CardObject card) {
            if (deckDropColumns[2].Cards.Count > 0) {
                MoveDroppedCardsToPreviousColumn(50);
            }

            if (deckDropColumns[1].Cards.Count > 0) {
                MoveDroppedCardsToPreviousColumn(50);
            }

            for (int i = 0; i < 3; i++) {
                if (transform.childCount == 0) {
                    return;
                }

                if (i != 0) {
                    card = transform.GetChild(transform.childCount - 1).GetComponent<CardObject>();
                }

                PickCardFromDeck(card);
            }
        }

        private void PickCardFromDeckClassic(CardObject card) {
            if (NeedToTranslateCard(50, _dropTransformCount)) {
                _dropTransformCount = 2;
            }

            PickCardFromDeck(card);
        }

        private void PickCardFromDeck(CardObject card) {
            _dropTransformCount %= 3;
            var deckDrop = deckDropColumns[_dropTransformCount];
            ManagePreviousColumnCardTrigger(false);
            deckDrop.AddCards(new[] {card});
            _dropTransformCount++;
            var destination = GameManager.GetCardDestinationPosition(deckDrop.transform, 0);

            StartCoroutine(GameManager.MoveCardsOnBoard(card, destination, 500));
        }

        private bool NeedToTranslateCard(float transitionSpeed, int dropTransformCount) {
            if (dropTransformCount > 0 && dropTransformCount % 3 == 0) {
                MoveDroppedCardsToPreviousColumn(transitionSpeed);
                return true;
            }

            return false;
        }

        private void ManagePreviousColumnCardTrigger(bool active) {
            if (_dropTransformCount > 0) {
                var deckDropColumn = deckDropColumns[_dropTransformCount - 1];
                var lastCardInColumn = deckDropColumn.Cards[deckDropColumn.Cards.Count - 1];
                lastCardInColumn.TriggerActive = active;
            }
            else if (deckDropColumns[0].Cards.Count > 0 && deckDropColumns[1].Cards.Count == 0) {
                deckDropColumns[0].Cards[deckDropColumns[0].Cards.Count - 1].TriggerActive = active;
            }
        }

        private void MoveDroppedCardsToPreviousColumn(float speed) {
            for (int i = 1; i < deckDropColumns.Length; i++) {
                MoveDroppedCard(speed, i, i - 1);
            }
        }

        private void MoveDroppedCard(float speed, int formIndex, int toIndex) {
            if (deckDropColumns[formIndex].transform.childCount > 0) {
                var card = deckDropColumns[formIndex].transform.GetChild(0).GetComponent<CardObject>();
                var destination = GameManager.GetCardDestinationPosition(deckDropColumns[toIndex].transform, 0);
                deckDropColumns[toIndex].AddCards(new[] {card});
                deckDropColumns[formIndex].RemoveCards(new[] {card});
                StartCoroutine(GameManager.MoveCardsOnBoard(card, destination, speed));
            }
        }

        public void UpdateDeckDropColumnAfterCardRemove(Column column) {
            for (int i = 0; i < deckDropColumns.Length; i++) {
                if (deckDropColumns[i] == column && (i != 0 || deckDropColumns[i].Cards.Count == 0)) {
                    // Check if following column has cards
                    if (!(i < deckDropColumns.Length - 1 && deckDropColumns[i + 1].Cards.Count > 0)) {
                        _dropTransformCount = i;
                        ManagePreviousColumnCardTrigger(true);
                    }

                    break;
                }

                if (deckDropColumns[i] == column && i == 0 && deckDropColumns[i].Cards.Count > 0) {
                    // No need to check the condition in the comment above since this code is accessed only when others columns are empty
                    _dropTransformCount = i + 1;
                    ManagePreviousColumnCardTrigger(true);
                    break;
                }
            }
        }

        public void TurnDeck() {
            for (int i = deckDropColumns.Length - 1; i >= 0; i--) {
                var column = deckDropColumns[i];
                for (int j = column.Cards.Count - 1; j >= 0; j--) {
                    var card = column.Cards[j];
                    var destination = GameManager.GetCardDestinationPosition(transform, 0);
                    StartCoroutine(GameManager.MoveCardsOnBoard(card, destination, 500));
                    card.Flip(true);
                    card.transform.SetParent(transform);
                    card.TriggerActive = true;
                }

                column.Cards.RemoveAll(card => card != null);
            }

            _dropTransformCount = 0;

            GameManager.Instance.UpdateScore(-10000);
            GameManager.Instance.UpdateMove();
        }
    }
}