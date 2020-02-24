using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Columns;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Managers {
    public class GameManager : MonoBehaviour {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform deckTransform;
        [SerializeField] private Column[] columns;
        [SerializeField] private GameObject cardGameObject;

        private Dictionary<CardEnum.CardSuit, CardEnum.SuitColor> _suitColors;
        private readonly Card[] _deck = new Card[13 * 4];
        public static GameManager Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
        }

        private void Start() {
            var scaler = canvas.gameObject.GetComponent<CanvasScaler>();
            float canvasScale = GetCanvasScale(Screen.currentResolution.width, Screen.currentResolution.height, scaler.referenceResolution, 0.5f);
            canvas.transform.localScale *= canvasScale;
            AssignSuitColors();
            MakeDeck();
            ShuffleDeck();
            StartCoroutine(InstantiateAndMoveCardsToColumns());
        }

        private static float GetCanvasScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight) {
            return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
                   Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
        }

        private void InstantiateCardsLeft(int lastDeckIndex) {
            for (int i = lastDeckIndex; i < _deck.Length; i++) {
                InstantiateCard(lastDeckIndex);
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
                    StartCoroutine(MoveToColumn(cardObject, k));
                    yield return new WaitForSeconds(0.2f);
                }

                cardsInColumn++;
            }

            InstantiateCardsLeft(j);
        }

        private IEnumerator MoveToColumn(CardObject cardObject, int columnIndex) {
            var column = columns[columnIndex];
            cardObject.transform.SetParent(column.transform);

            var cardInColumnPosition = GetCardInColumnPosition(column);

            while (Vector3.Distance(cardObject.transform.position, cardInColumnPosition) >= 1f) {
                cardObject.transform.position = Vector3.MoveTowards(cardObject.transform.position, cardInColumnPosition, Time.deltaTime * 1000);
                yield return new WaitForSeconds(0.02f);
            }

            IsLastCard(cardObject, columnIndex, column);
        }

        private static Vector3 GetCardInColumnPosition(Column column) {
            var columnTransform = column.transform;
            float spacing = column.VerticalLayoutGroup.spacing;
            int childInGroup = columnTransform.childCount;
            var columnLocalPosition = columnTransform.localPosition;

            var cardInColumnPosition =
                columnTransform.TransformPoint(new Vector3(columnLocalPosition.x, columnLocalPosition.y - spacing * (childInGroup - 1), columnLocalPosition.z));
            cardInColumnPosition = new Vector3(columnTransform.position.x, cardInColumnPosition.y, cardInColumnPosition.z);
            return cardInColumnPosition;
        }

        private static void IsLastCard(CardObject cardObject, int columnIndex, Column column) {
            if (column.transform.childCount != columnIndex + 1 || column.transform.GetChild(columnIndex) != cardObject.transform) return;
            column.VerticalLayoutGroup.enabled = true;
            cardObject.Turn();
        }

        private void ShuffleDeck() {
            var random = new Random();

            for (int i = _deck.Length - 1; i > 0; i--) {
                int j = random.Next(i);
                var temp = _deck[i];
                _deck[i] = _deck[j];
                _deck[j] = temp;
            }
        }

        private void MakeDeck() {
            int i = 0;
            foreach (var suit in _suitColors.Keys) {
                for (int j = 1; j < 14; j++) {
                    _deck[i++] = new Card(j, suit, _suitColors[suit]);
                }
            }
        }

        private void AssignSuitColors() {
            int i = 0;
            _suitColors = new Dictionary<CardEnum.CardSuit, CardEnum.SuitColor>();

            foreach (CardEnum.CardSuit suit in Enum.GetValues(typeof(CardEnum.CardSuit))) {
                _suitColors.Add(suit, (CardEnum.SuitColor) Enum.GetValues(typeof(CardEnum.SuitColor)).GetValue(i % 2));
                i++;
            }
        }
    }
}