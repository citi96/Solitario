using Managers;
using Undo.Moves;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Undo {
    public class Undo : MonoBehaviour {
        [SerializeField] private Button undoButton;

        private readonly DropOutStack<Move> _moves = new DropOutStack<Move>(3);

        private void Awake() {
            if (PlayerPrefs.GetInt("DrawThree") == 1) {
                gameObject.SetActive(false);
            }
        }

        private void Update() {
            undoButton.interactable = !GameManager.Instance.Waiting && !_moves.IsEmpty();
        }

        public void AddLastMove(Move move) {
            _moves.Push(move);
        }

        public void UndoMove() {
            if (!GameManager.Instance.Waiting) {
                StartCoroutine(GameManager.Instance.StartWaiting());
                _moves.Pop().Rollback();
            }
        }
    }
}