using System.Collections.Generic;
using Cards;
using Columns;
using Managers;

namespace Undo.Moves {
    public class FromColumnMove : Move {
        private readonly List<bool> _cardsHasBeenInVerticalColumn;

        protected readonly Column fromColumn;

        public FromColumnMove(Column fromColumn, Column toColumn, CardObject[] cards, List<bool> cardsHasBeenInVerticalColumn) : base(toColumn, cards) {
            this.fromColumn = fromColumn;
            _cardsHasBeenInVerticalColumn = cardsHasBeenInVerticalColumn;
        }

        protected override void RollbackMove() {
            for (int i = 0; i < cards.Length; i++) {
                if (cards[i].Card.HasBeenInVerticalColumn != _cardsHasBeenInVerticalColumn[i]) {
                    cards[i].Card.HasBeenInVerticalColumn = _cardsHasBeenInVerticalColumn[i];
                    GameManager.Instance.UndoScore();
                }
            }

            fromColumn.RollbackCard(cards);
        }
    }
}