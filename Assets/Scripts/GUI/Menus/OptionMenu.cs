using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.Menus {
    public class OptionMenu : MonoBehaviour {
        [SerializeField] private TMP_Text undoCount;
        [SerializeField] private TMP_Text bestScore;
        [SerializeField] private TMP_Text bestScoreDrawThree;
        [SerializeField] private Slider undoSlider;
        [SerializeField] private Toggle hintsToggle;

        private void OnEnable() {
            if (undoCount != null) {
                undoCount.text = PlayerPrefs.GetString("UndoCount");
                undoSlider.value = Convert.ToInt32(undoCount.text);
            }

            hintsToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("HintsToggle"));
            BestScoreSetup();
        }

        private void BestScoreSetup() {
            bestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
            bestScoreDrawThree.text = PlayerPrefs.GetInt("BestScoreDrawThree").ToString();
        }

        public void Toggle() {
            PlayerPrefs.SetInt("HintsToggle", Convert.ToInt32(hintsToggle.isOn));
        }

        public void ChangeSliderValue() {
            undoCount.text = undoSlider.value.ToString(CultureInfo.InvariantCulture);
            PlayerPrefs.SetString("UndoCount", undoCount.text);
        }
    }
}