using System;
using System.Linq;
using Cards;
using Columns;
using Undo.Moves;
using UnityEngine;

namespace Managers {
    public class DeckManager : MonoBehaviour {
        [SerializeField] private Column[] deckDropColumns;

        private bool _isDrawThree;
        private int _dropTransformCount;
        private GameManager _gm;

        public static DeckManager Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start() {
            _gm = GameManager.Instance;
            _isDrawThree = Convert.ToBoolean(PlayerPrefs.GetInt("DrawThree"));
        }

        public void PickCardFromDeckAfterClick(CardObject card) {
            _gm = GameManager.Instance;
            if (!_gm.Waiting) {
                StartCoroutine(_gm.StartWaiting(0.2f));

                if (_isDrawThree)
                    PickCardFromDeckDrawThree(card);
                else
                    PickCardFromDeckClassic(card);

                _gm.UpdateMove();
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
            var destination = _gm.GetCardDestinationPosition(deckDrop.transform, 0);

            StartCoroutine(_gm.MoveCardsOnBoard(card, destination, 500));
            GameManager.Instance.AddMoveToUndo(new DeckMove(deckDrop, new[] {card}));
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
                var destination = _gm.GetCardDestinationPosition(deckDropColumns[toIndex].transform, 0);
                deckDropColumns[toIndex].AddCards(new[] {card});
                deckDropColumns[formIndex].RemoveCards(new[] {card});
                StartCoroutine(_gm.MoveCardsOnBoard(card, destination, speed));
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
            if (!_gm.Waiting) {
                StartCoroutine(_gm.StartWaiting(1f));
                _gm.AddMoveToUndo(new TurnDeckMove());
                for (int i = deckDropColumns.Length - 1; i >= 0; i--) {
                    var column = deckDropColumns[i];
                    for (int j = column.Cards.Count - 1; j >= 0; j--) {
                        var card = column.Cards[j];
                        var destination = _gm.GetCardDestinationPosition(transform, 0);
                        StartCoroutine(_gm.MoveCardsOnBoard(card, destination, 500));
                        card.Flip(true);
                        card.transform.SetParent(transform);
                        card.TriggerActive = true;
                    }

                    column.Cards.RemoveAll(card => card != null);
                }

                _dropTransformCount = 0;

                _gm.UpdateScore(-10000);
                _gm.UpdateMove();
            }
        }

        public void UndoDeckDropColumn(Column column) {
            for (int i = 0; i < deckDropColumns.Length; i++) {
                if (deckDropColumns[i] == column) {
                    _dropTransformCount = i;
                    break;
                }
            }

            ManagePreviousColumnCardTrigger(false);
            _dropTransformCount++;
        }

        public void UndoCardPickedFromDeck(CardObject card) {
            if (_isDrawThree) {
                UndoCardPickedFromDeckDrawThree(card);
            }
            else {
                UndoCardPickedFromDeckClassic(card);
            }
        }

        private void UndoCardPickedFromDeckDrawThree(CardObject card) {
            foreach (var dropColumn in deckDropColumns) {
                RestoreDeckAfterUndo(dropColumn.Cards[dropColumn.Cards.Count - 1]);
            }

            if (deckDropColumns[0].Cards.Count > 1) {
                for (int i = deckDropColumns.Length; i > 0; i--) {
                    var dropColumn = deckDropColumns[0];
                    deckDropColumns[i].RollbackCard(new[] {dropColumn.transform.GetChild(dropColumn.transform.childCount - 1).GetComponent<CardObject>()});
                }
            }
        }

        private void UndoCardPickedFromDeckClassic(CardObject card) {
            if (deckDropColumns[deckDropColumns.Length - 1].transform.childCount > 0 && deckDropColumns[0].transform.childCount > 1) {
                MoveCardsToNextColumn();
                _dropTransformCount++;
            }

            RestoreDeckAfterUndo(card);
        }

        private void MoveCardsToNextColumn() {
            for (int i = 0; i < deckDropColumns.Length - 1; i++) {
                var dropColumn = deckDropColumns[i];
                var cardToMove = i == 0
                    ? dropColumn.transform.GetChild(dropColumn.transform.childCount - 1).GetComponent<CardObject>()
                    : dropColumn.transform.GetChild(0).GetComponent<CardObject>();

                deckDropColumns[i + 1].RollbackCard(new[] {cardToMove});
            }
        }

        private void RestoreDeckAfterUndo(CardObject card) {
            Transform cardTransform;
            (cardTransform = card.transform).SetParent(transform);
            cardTransform.position = transform.position;
            card.Flip(true, false);
        }

        public void UndoTurnDeck() {
            var firstColumn = deckDropColumns[0];
            var cards = transform.Cast<Transform>().Select(child => child.GetComponent<CardObject>()).Reverse().ToArray();
            firstColumn.RollbackCard(cards);

            foreach (var card in cards) {
                card.Flip(false, false);
                card.TriggerActive = false;
            }

            var lastCard = firstColumn.transform.GetChild(firstColumn.Cards.Count - 1).GetComponent<CardObject>();
            lastCard.TriggerActive = true;
            for (int i = deckDropColumns.Length - 1; i > 0; i--) {
                lastCard = firstColumn.transform.GetChild(firstColumn.Cards.Count - 1).GetComponent<CardObject>();
                deckDropColumns[i].RollbackCard(new[] {lastCard});
                firstColumn.Cards.Remove(lastCard);
            }

            _dropTransformCount = deckDropColumns.Length - 1;
        }
    }
}