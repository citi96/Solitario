using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Columns;
using Events;
using GUI;
using Undo.Moves;
using UnityEngine;
using Random = System.Random;

namespace Managers {
    public delegate void CallBack(CardObject cardObject, VerticalColumn column);

    public class GameManager : MonoBehaviour {
        [SerializeField] private GameObject cardGameObject;
        [SerializeField] private VerticalColumn[] columns;
        [SerializeField] private Transform deckTransform;
        [SerializeField] private DraggedCards draggedCards;
        [SerializeField] private TopGui topGui;
        [SerializeField] private Undo.Undo undo;

        [SerializeField] private GameEvent enablePauseButtonEvent;
        [SerializeField] private GameEvent enableCardTriggerEvent;

        private readonly Card[] _allCards = new Card[13 * 4];
        private Dictionary<CardEnum.CardSuit, CardEnum.SuitColor> _suitColors;
        private bool _waiting;

        public static GameManager Instance { get; private set; }

        public DraggedCards DraggedCards => draggedCards;

        public bool Waiting => _waiting;

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
            for (int i = lastDeckIndex; i < _allCards.Length; i++) {
                InstantiateCard(lastDeckIndex++).IsInDeck = true;
            }
        }

        private CardObject InstantiateCard(int index) {
            var cardObject = Instantiate(cardGameObject, deckTransform).GetComponent<CardObject>();
            cardObject.Card = _allCards[index];
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

            enablePauseButtonEvent.Raise();
            InstantiateCardsLeft(j);
            enableCardTriggerEvent.Raise();
        }

        private void MoveToColumn(CardObject cardObject, int columnIndex) {
            var column = columns[columnIndex];
            column.AddCoveredCards(cardObject);

            var cardInColumnPosition = GetCardDestinationPosition(column.transform, column.Spacing);

            StartCoroutine(MoveCardsOnBoard(cardObject, cardInColumnPosition, 1000, IsLastCard, column));
        }

        public Vector3 GetCardDestinationPosition(Transform parentTransform, float spacing, int otherChild = 0) {
            int childInGroup = parentTransform.childCount;
            var parentLocalPosition = parentTransform.localPosition;

            var cardInColumnPosition =
                parentTransform.TransformPoint(new Vector3(parentLocalPosition.x, parentLocalPosition.y - spacing * (childInGroup - 1 + otherChild),
                    parentLocalPosition.z));
            cardInColumnPosition = new Vector3(parentTransform.position.x, cardInColumnPosition.y, cardInColumnPosition.z);
            return cardInColumnPosition;
        }

        private static void IsLastCard(CardObject cardObject, VerticalColumn column) {
            if (!column.IsInitPhaseFinished(cardObject)) return;
            column.LayoutGroupEnabled = true;
            cardObject.Flip(false);
        }

        private void ShuffleDeck() {
            var random = new Random();

            for (int i = _allCards.Length - 1; i > 0; i--) {
                int j = random.Next(i);
                var temp = _allCards[i];
                _allCards[i] = _allCards[j];
                _allCards[j] = temp;
            }
        }

        private void MakeDeck() {
            int i = 0;
            foreach (var suit in _suitColors.Keys)
                for (int j = 1; j < 14; j++)
                    _allCards[i++] = new Card(j, suit, _suitColors[suit]);
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
            foreach (var column in columns) column.FlipLastCard();
        }


        public static IEnumerator MoveCardsOnBoard(CardObject card, Vector3 destination, float speed = 1000, CallBack callBack = null,
            VerticalColumn column = null) {
            while (Vector3.Distance(card.transform.position, destination) >= 5) {
                card.transform.position = Vector3.MoveTowards(card.transform.position, destination, Time.deltaTime * speed);
                yield return new WaitForSeconds(0.02f);
            }

            card.transform.position = destination;
            callBack?.Invoke(card, column);
        }

        public void UpdateScore(int score) {
            topGui.ScoreText = score;
        }

        public void UpdateMove() {
            topGui.IncrementMoves();
        }

        public IEnumerator StartWaiting(float seconds = 0.5f) {
            _waiting = true;
            yield return new WaitForSeconds(seconds);
            _waiting = false;
        }

        public void AddMoveToUndo(Move move) {
            undo.AddLastMove(move);
        }

        public int GetScore() {
            return topGui.ScoreText;
        }

        public void UndoScore() {
            topGui.UndoScore();
        }
    }
}