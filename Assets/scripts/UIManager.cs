using UnityEngine;
using UnityEngine.SceneManagement;  

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;

    public void onEnterPausePress() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // detiene el juego
    }
        public void onResumePress() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // reanuda el juego
    }

    public void onGameExitPress() {
        Application.Quit();
    }
}
