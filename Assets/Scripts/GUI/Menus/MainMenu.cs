using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUI.Menus {
    public class MainMenu : MonoBehaviour {
        private void Awake() {
            if (!PlayerPrefs.HasKey("UndoCount")) {
                PlayerPrefs.SetInt("HintsToggle", 0);
                PlayerPrefs.SetString("UndoCount", "3");
                PlayerPrefs.SetInt("BestScore", 0);
                PlayerPrefs.SetInt("BestScoreDrawThree", 0);
            }
        }

        public void OnSelectClassicMode() {
            LoadScene(0);
        }

        public void OnSelectDrawThreeMode() {
            LoadScene(1);
        }

        private static void LoadScene(int drawThree) {
            PlayerPrefs.SetInt("DrawThree", drawThree);
            SceneManager.LoadScene("GameScene");
        }
    }
}