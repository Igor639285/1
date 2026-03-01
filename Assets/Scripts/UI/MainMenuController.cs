using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OpenLevels()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings: здесь можно подключить меню графики/звука/управления.");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
