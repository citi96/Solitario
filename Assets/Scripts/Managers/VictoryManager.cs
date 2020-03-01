using System.Linq;
using Columns;
using TMPro;
using UnityEngine;

namespace Managers {
    public class VictoryManager : MonoBehaviour {
        [SerializeField] private StandardColumn[] suitColumn;
        [SerializeField] private GameObject victoryScreenContent;
        [SerializeField] private TMP_Text scoreField;
        [SerializeField] private TMP_Text bestScoreField;

        private bool _gameFinished;

        private static int BestScore {
            get => PlayerPrefs.GetInt("DrawThree") == 0 ? PlayerPrefs.GetInt("BestScore") : PlayerPrefs.GetInt("BestScoreDrawThree");
            set => PlayerPrefs.SetInt(PlayerPrefs.GetInt("DrawThree") == 0 ? "BestScore" : "BestScoreDrawThree", value);
        }

        private void Update() {
            if (suitColumn.FirstOrDefault(c => !c.IsComplete) == default && !_gameFinished) {
                VictoryScreen();
                _gameFinished = true;
            }
        }

        private void VictoryScreen() {
            victoryScreenContent.SetActive(true);

            int bestScore = BestScore;
            int score = GameManager.Instance.GetScore();

            scoreField.text = score.ToString();

            if (score > bestScore) {
                bestScoreField.text = "This is your new best score!";
                BestScore = score;
            } else if (score == bestScore) {
                bestScoreField.text = "That is equal to your best score!";
            } else {
                bestScoreField.text = $"Your best score is: {bestScore}";
            }
        }
    }
}