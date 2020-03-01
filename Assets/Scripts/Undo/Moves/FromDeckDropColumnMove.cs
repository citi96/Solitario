using System.Collections.Generic;
using Cards;
using Columns;
using Managers;

namespace Undo.Moves {
    public class FromDeckDropColumnMove : FromColumnMove {
        public FromDeckDropColumnMove(Column fromColumn, Column toColumn, CardObject[] cards, List<bool> cardsHasBeenInVerticalColumn) : base(fromColumn,
            toColumn, cards, cardsHasBeenInVerticalColumn) { }

        protected override void RollbackMove() {
            base.RollbackMove();
            DeckManager.Instance.UndoDeckDropColumn(fromColumn);
        }
    }
}