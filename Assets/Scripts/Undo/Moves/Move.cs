using Cards;
using Columns;
using Managers;

namespace Undo.Moves {
    public abstract class Move {
        protected readonly Column toColumn;
        protected readonly CardObject[] cards;
        protected readonly GameManager gm;

        protected Move(Column toColumn, CardObject[] cards) {
            this.toColumn = toColumn;
            this.cards = cards;
            gm = GameManager.Instance;
        }

        public void Rollback() {
            if (toColumn != null) {
                toColumn.RemoveCards(cards, false);
            }

            RollbackMove();
        }

        protected abstract void RollbackMove();
    }
}