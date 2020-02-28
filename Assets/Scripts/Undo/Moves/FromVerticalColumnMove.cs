using Cards;
using Columns;

namespace Undo.Moves {
    public class FromVerticalColumnMove : FromColumnMove {
        private readonly bool _hasTopCardFlipped;
        private new readonly VerticalColumn _fromColumn;

        public FromVerticalColumnMove(VerticalColumn fromColumn, Column toColumn, CardObject[] cards, bool hasTopCardFlipped) : base(fromColumn, toColumn,
            cards) {
            _fromColumn = fromColumn;
            _hasTopCardFlipped = hasTopCardFlipped;
        }

        protected override void RollbackMove() {
            _fromColumn.FlipLastCard(!_hasTopCardFlipped, false);
            base.RollbackMove();
        }
    }
}