using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Columns;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers {
    public class GameManager : MonoBehaviour {
        private readonly Card[] _deck = new Card[13 * 4];

        private Dictionary<CardEnum.CardSuit, CardEnum.SuitColor> _suitColors;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject cardGameObject;
        [SerializeField] private VerticalColumn[] columns;
        [SerializeField] private Transform deckTransform;
        [SerializeField] private DraggedCards draggedCards;
        [SerializeField] private int seed;
        public static GameManager Instance { get; private set; }


        public DraggedCards DraggedCards => draggedCards;

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            Random.seed = seed;
            Debug.Log(Random.seed);
        }

        private void Start() {
            AssignSuitColors();
            MakeDeck();
            ShuffleDeck();
            StartCoroutine(InstantiateAndMoveCardsToColumns());
        }

        private void InstantiateCardsLeft(int lastDeckIndex) {
            for (int i = lastDeckIndex; i < _deck.Length; i++) InstantiateCard(lastDeckIndex);
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
                    StartCoroutine(MoveToColumn(cardObject, k));
                    yield return new WaitForSeconds(0.2f);
                }

                cardsInColumn++;
            }

            InstantiateCardsLeft(j);
        }

        private IEnumerator MoveToColumn(CardObject cardObject, int columnIndex) {
            var column = columns[columnIndex];
            column.AddCoveredCards(cardObject);

            var cardInColumnPosition = GetCardDestinationPosition(column.transform, column.VerticalLayoutGroup.spacing);

            while (Vector3.Distance(cardObject.transform.position, cardInColumnPosition) >= 1f) {
                cardObject.transform.position = Vector3.MoveTowards(cardObject.transform.position, cardInColumnPosition, Time.deltaTime * 1000);
                yield return new WaitForSeconds(0.02f);
            }

            IsLastCard(cardObject, column);
        }

        public static Vector3 GetCardDestinationPosition(Transform parentTransform, float spacing) {
            int childInGroup = parentTransform.childCount;
            var columnLocalPosition = parentTransform.localPosition;

            var cardInColumnPosition =
                parentTransform.TransformPoint(new Vector3(columnLocalPosition.x, columnLocalPosition.y - spacing * (childInGroup - 1), columnLocalPosition.z));
            cardInColumnPosition = new Vector3(parentTransform.position.x, cardInColumnPosition.y, cardInColumnPosition.z);
            return cardInColumnPosition;
        }

        private static void IsLastCard(CardObject cardObject, VerticalColumn column) {
            if (!column.IsInitPhaseFinished(cardObject)) return;
            column.VerticalLayoutGroup.enabled = true;
            cardObject.Turn();
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
    }
}