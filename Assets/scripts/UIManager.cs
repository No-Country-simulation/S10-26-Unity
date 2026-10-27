using UnityEngine;
using UnityEngine.SceneManagement;  

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;

    public UIManager()
    {
    }

    public void onEnterPausePress() {
        pauseMenu.SetActive(true);
    }

    public void onExitPausePress() {
        pauseMenu.SetActive(false);
    }

    public void onGameExitPress() {
        Application.Quit();
    }
}
