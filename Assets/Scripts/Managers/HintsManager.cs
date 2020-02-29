using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Columns;
using UnityEngine;
using UnityEngine.UI;

namespace Managers {
    public class HintsManager : MonoBehaviour {
        [SerializeField] private List<Column> otherColumns;
        [SerializeField] private Column[] suitColumn;
        [SerializeField] private Button hintButton;

        private void Awake() {
            hintButton.interactable = Convert.ToBoolean(PlayerPrefs.GetInt("HintsToggle"));
        }

        public void FindHint() {
            var gameManager = GameManager.Instance;

            if (!gameManager.Waiting) {
                StartCoroutine(gameManager.StartWaiting());

                if (SearchColumns(otherColumns, suitColumn, false))
                    return;
                SearchColumns(otherColumns, otherColumns, true);
            }
        }

        private bool SearchColumns(IEnumerable<Column> fromColumns, IEnumerable<Column> toColumns, bool first) {
            var toColumnsArray = toColumns.ToArray();

            foreach (var fromColumn in fromColumns) {
                var card = first ? fromColumn.GetFirstVisibleCard() : fromColumn.GetLastVisibleCard();
                if (card != null) {
                    foreach (var toColumn in toColumnsArray.Where(toColumn => toColumn.CanAddCard(card.Card))) {
                        // Condition to avoid infinite switches between two empty vertical column with cards starting with a K.
                        if (!fromColumn.CanHaveHiddenCard() || fromColumn.Cards.FirstOrDefault(c => !c.Card.IsVisible) != null || card.Card.Number != 13) {
                            GenerateHint(toColumn, fromColumn.PickCardsFromColumn(card).ToArray());
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void GenerateHint(Column toColumn, IEnumerable<CardObject> cards) {
            var hintCards = new List<CardObject>();
            InstantiateHintCards(hintCards, cards);

            for (int i = 0; i < hintCards.Count; i++) {
                var card = hintCards[i];
                var destination = GameManager.Instance.GetCardDestinationPosition(toColumn.transform, toColumn.Spacing, 1 + i);
                StartCoroutine(GameManager.Instance.MoveCardsOnBoard(card, destination, 500, DestroyHintCard));
            }
        }

        private void InstantiateHintCards(ICollection<CardObject> hintCards, IEnumerable<CardObject> cards) {
            foreach (var card in cards) {
                var hintCard = Instantiate(card, transform);
                hintCard.transform.position = card.transform.position;
                hintCards.Add(hintCard);
                MakeTransparent(hintCard);
            }
        }

        private static void MakeTransparent(Component card) {
            foreach (var image in card.GetComponentsInChildren<Image>()) {
                var color = image.color;
                image.color = new Color(color.r, color.g, color.b, 0.6f);
            }
        }

        private static void DestroyHintCard(CardObject card, VerticalColumn column) {
            Destroy(card.gameObject);
        }
    }
}