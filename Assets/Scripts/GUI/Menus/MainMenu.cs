using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUI.Menus {
    public class MainMenu : MonoBehaviour {
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