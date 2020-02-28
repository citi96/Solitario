using Cards;
using Columns;

namespace Undo.Moves {
    public class FromColumnMove : Move {
        protected readonly Column fromColumn;

        public FromColumnMove(Column fromColumn, Column toColumn, CardObject[] cards) : base(toColumn, cards) {
            this.fromColumn = fromColumn;
        }

        protected override void RollbackMove() {
            fromColumn.RollbackCard(cards);
        }
    }
}