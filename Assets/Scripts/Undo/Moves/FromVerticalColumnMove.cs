using System.Collections.Generic;
using Cards;
using Columns;

namespace Undo.Moves {
    public class FromVerticalColumnMove : FromColumnMove {
        private readonly bool _hasTopCardFlipped;
        private new readonly VerticalColumn _fromColumn;

        public FromVerticalColumnMove(VerticalColumn fromColumn, Column toColumn, CardObject[] cards, bool hasTopCardFlipped,
            List<bool> cardsHasBeenInVerticalColumn) : base(fromColumn, toColumn, cards, cardsHasBeenInVerticalColumn) {
            _fromColumn = fromColumn;
            _hasTopCardFlipped = hasTopCardFlipped;
        }


        protected override void RollbackMove() {
            _fromColumn.FlipLastCard(!_hasTopCardFlipped, false);
            base.RollbackMove();
        }
    }
}