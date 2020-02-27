using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Columns;
using GUI;
using UnityEngine;

namespace Managers {
    public delegate void CallBack(CardObject cardObject, VerticalColumn column);

    public class GameManager : MonoBehaviour {
        private readonly Card[] _deck = new Card[13 * 4];

        private int _dropTransformCount = 0;

        private Dictionary<CardEnum.CardSuit, CardEnum.SuitColor> _suitColors;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject cardGameObject;
        [SerializeField] private VerticalColumn[] columns;
        [SerializeField] private Column[] deckDropColumns;
        [SerializeField] private Transform deckTransform;
        [SerializeField] private DraggedCards draggedCards;
        [SerializeField] private TopGui topGui;

        public static GameManager Instance { get; private set; }

        public DraggedCards DraggedCards => draggedCards;

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Start() {
            AssignSuitColors();
            MakeDeck();
            ShuffleDeck();
            StartCoroutine(InstantiateAndMoveCardsToColumns());
        }

        private void InstantiateCardsLeft(int lastDeckIndex) {
            for (int i = lastDeckIndex; i < _deck.Length; i++) {
                InstantiateCard(lastDeckIndex++).IsInDeck = true;
            }
        }

        private CardObject InstantiateCard(int index) {
            var cardObject = Instantiate(cardGameObject, deckTransform).GetComponent<CardObject>();
            cardObject.Card = _deck[index];
            return cardObject;
        }

        private IEnumerator InstantiateAndMoveCardsToColumns() {
            int cardsInColumn = 1;
            int j = 0;

            yield return new WaitForSeconds(0.5f); // Waiting for the initial lag spike.

            for (int k = 0; k < 7; k++) {
                for (int i = 0; i < cardsInColumn; i++) {
                    var cardObject = InstantiateCard(j++);
                    MoveToColumn(cardObject, k);
                    yield return new WaitForSeconds(0.2f);
                }

                cardsInColumn++;
            }

            InstantiateCardsLeft(j);
        }

        private void MoveToColumn(CardObject cardObject, int columnIndex) {
            var column = columns[columnIndex];
            column.AddCoveredCards(cardObject);

            var cardInColumnPosition = GetCardDestinationPosition(column.transform, column.Spacing);

            StartCoroutine(MoveCardsOnBoard(cardObject, cardInColumnPosition, 1000, IsLastCard, column));
        }

        public static Vector3 GetCardDestinationPosition(Transform parentTransform, float spacing) {
            int childInGroup = parentTransform.childCount;
            var parentLocalPosition = parentTransform.localPosition;

            var cardInColumnPosition =
                parentTransform.TransformPoint(new Vector3(parentLocalPosition.x, parentLocalPosition.y - spacing * (childInGroup - 1), parentLocalPosition.z));
            cardInColumnPosition = new Vector3(parentTransform.position.x, cardInColumnPosition.y, cardInColumnPosition.z);
            return cardInColumnPosition;
        }

        private static void IsLastCard(CardObject cardObject, VerticalColumn column) {
            if (!column.IsInitPhaseFinished(cardObject)) return;
            column.LayoutGroupEnabled = true;
            cardObject.Flip(false);
        }

        private void ShuffleDeck() {
            var random = new System.Random();

            for (int i = _deck.Length - 1; i > 0; i--) {
                int j = random.Next(i);
                var temp = _deck[i];
                _deck[i] = _deck[j];
                _deck[j] = temp;
            }
        }

        private void MakeDeck() {
            int i = 0;
            foreach (var suit in _suitColors.Keys)
                for (int j = 1; j < 14; j++)
                    _deck[i++] = new Card(j, suit, _suitColors[suit]);
        }

        private void AssignSuitColors() {
            int i = 0;
            _suitColors = new Dictionary<CardEnum.CardSuit, CardEnum.SuitColor>();

            foreach (CardEnum.CardSuit suit in Enum.GetValues(typeof(CardEnum.CardSuit))) {
                _suitColors.Add(suit, (CardEnum.SuitColor) Enum.GetValues(typeof(CardEnum.SuitColor)).GetValue(i % 2));
                i++;
            }
        }

        public void NotifyMoveToColumns() {
            foreach (var column in columns) column.TurnTopCard();
        }

        public void PickCardFromDeck(CardObject card) {
            UpdateMove();

            if (_dropTransformCount > 0 && _dropTransformCount % 3 == 0) {
                MoveDroppedCardsToNextColumn();
                _dropTransformCount = 2;
            }

            var deckDrop = deckDropColumns[_dropTransformCount];
            ManagePreviousColumnCardTrigger(false);
            deckDrop.AddCards(new[] {card});
            _dropTransformCount++;
            var destination = GetCardDestinationPosition(deckDrop.transform, 0);

            StartCoroutine(MoveCardsOnBoard(card, destination, 500));
        }

        private void ManagePreviousColumnCardTrigger(bool active) {
            if (_dropTransformCount > 0) {
                var deckDropColumn = deckDropColumns[_dropTransformCount - 1];
                var lastCardInColumn = deckDropColumn.Cards[deckDropColumn.Cards.Count - 1];
                lastCardInColumn.SetTriggerActive(active);
            }
            else if (deckDropColumns[0].Cards.Count > 0) {
                deckDropColumns[0].Cards[deckDropColumns[0].Cards.Count - 1].SetTriggerActive(active);
            }
        }

        public static IEnumerator MoveCardsOnBoard(CardObject card, Vector3 destination, float speed = 1000, CallBack callBack = null,
            VerticalColumn column = null) {
            while (Vector3.Distance(card.transform.position, destination) >= 1) {
                card.transform.position = Vector3.MoveTowards(card.transform.position, destination, Time.deltaTime * speed);
                yield return new WaitForSeconds(0.02f);
            }

            callBack?.Invoke(card, column);
        }

        private void MoveDroppedCardsToNextColumn() {
            for (int i = 1; i < deckDropColumns.Length; i++) {
                var card = deckDropColumns[i].transform.GetChild(0).GetComponent<CardObject>();
                var destination = GetCardDestinationPosition(deckDropColumns[i - 1].transform, 0);
                deckDropColumns[i - 1].AddCards(new[] {card});
                deckDropColumns[i].RemoveCards(new[] {card});
                StartCoroutine(MoveCardsOnBoard(card, destination, 50));
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
                    card.Flip(true);
                    card.transform.SetParent(deckTransform);
                    var destination = GetCardDestinationPosition(deckTransform, 0);
                    StartCoroutine(MoveCardsOnBoard(card, destination, 500));
                    card.SetTriggerActive(true);
                }

                column.Cards.RemoveAll(card => card != null);
            }

            _dropTransformCount = 0;

            UpdateScore(-10000);
            UpdateMove();
        }

        public void UpdateScore(int score) {
            topGui.ScoreText = score;
        }

        public void UpdateMove() {
            topGui.IncrementMoves();
        }
    }
}