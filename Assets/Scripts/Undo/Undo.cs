using System;
using Managers;
using Undo.Moves;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Undo {
    public class Undo : MonoBehaviour {
        [SerializeField] private Button undoButton;

        private readonly DropOutStack<Move> _moves = new DropOutStack<Move>(Convert.ToInt32(PlayerPrefs.GetString("UndoCount")));

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
                StartCoroutine(GameManager.Instance.StartWaiting(0.1f));
                _moves.Pop().Rollback();
            }
        }
    }
}