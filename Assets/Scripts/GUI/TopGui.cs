using System;
using TMPro;
using UnityEngine;

namespace GUI {
    public class TopGui : MonoBehaviour {
        [SerializeField] private TMP_Text movesText;
        [SerializeField] private TMP_Text scoreText;

        public int ScoreText {
            set {
                int score = Convert.ToInt32(scoreText.text);
                score += value;
                score = score < 0 ? score = 0 : score;
                scoreText.text = (score).ToString();
            }
        }

        public void IncrementMoves() {
            int move = Convert.ToInt32(movesText.text);
            movesText.text = (++move).ToString();
        }
    }
}