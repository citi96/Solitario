using System;
using TMPro;
using UnityEngine;
using Utility;

namespace GUI {
    public class TopGui : MonoBehaviour {
        [SerializeField] private TMP_Text movesText;
        [SerializeField] private TMP_Text scoreText;

        private DropOutStack<string> _lastScores;
        private string _lastScore = "0";

        private void Awake() {
            _lastScores = new DropOutStack<string>(Convert.ToInt32(PlayerPrefs.GetString("UndoCount")));
        }

        public int ScoreText {
            get => Convert.ToInt32(scoreText.text);
            set {
                int score = Convert.ToInt32(scoreText.text);
                score += value;
                score = score < 0 ? 0 : score;
                scoreText.text = score.ToString();
                _lastScores.Push(_lastScore);
                _lastScore = scoreText.text;
            }
        }

        public void IncrementMoves() {
            int move = Convert.ToInt32(movesText.text);
            movesText.text = (++move).ToString();
        }

        public void UndoScore() {
            scoreText.text = _lastScores.Pop();
            _lastScore = scoreText.text;
        }
    }
}