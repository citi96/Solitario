using Cards;
using Columns;
using Managers;

namespace Undo.Moves {
    public class FromDeckDropColumnMove : FromColumnMove {
        public FromDeckDropColumnMove(Column fromColumn, Column toColumn, CardObject[] cards) : base(fromColumn, toColumn, cards) { }

        protected override void RollbackMove() {
            base.RollbackMove();
            DeckManager.Instance.UndoDeckDropColumn(fromColumn);
        }
    }
}