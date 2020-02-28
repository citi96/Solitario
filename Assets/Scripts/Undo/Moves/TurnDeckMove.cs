using Managers;

namespace Undo.Moves {
    public class TurnDeckMove : Move {
        public TurnDeckMove() : base(null, null) { }

        protected override void RollbackMove() {
            DeckManager.Instance.UndoTurnDeck();
        }
    }
}