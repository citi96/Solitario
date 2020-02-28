using Cards;
using Columns;
using Managers;

namespace Undo.Moves {
    public class DeckMove : Move {
        public DeckMove(Column toColumn, CardObject[] cards) : base(toColumn, cards) { }

        public override void Rollback() {
            RollbackMove();
        }

        protected override void RollbackMove() {
            DeckManager.Instance.UndoCardPickedFromDeck(cards[0]);
        }
    }
}