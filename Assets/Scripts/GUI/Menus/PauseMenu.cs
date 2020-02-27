using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUI.Menus {
    public class PauseMenu : MonoBehaviour {
        public void NewGame() {
            SceneManager.LoadScene("GameScene");
        }

        public void MainMenu() {
            SceneManager.LoadScene("MenuScene");
        }
    }
}