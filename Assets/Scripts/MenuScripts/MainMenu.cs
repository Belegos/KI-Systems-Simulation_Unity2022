using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelSelection;
    public string pauseMenu;
    public GameObject OptionSceneImg;

    public void StartGame()
    {
        SceneManager.LoadScene(levelSelection);
    }

    public void OpenOptions()
    {
        OptionSceneImg.SetActive(true);
    }

    public void CloseOptions()
    {
        OptionSceneImg.SetActive(false);
    }
    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
