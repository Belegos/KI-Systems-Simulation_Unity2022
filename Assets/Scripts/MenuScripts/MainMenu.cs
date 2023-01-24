using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelSelection1 = "WorldLandGeneration";
    public string levelSelection2 = "DungeonScene";
    public string levelSelection3 = "WaterScene";
    public string pauseMenu;
    public GameObject OptionSceneImg;
    public GameObject LevelSelectoionSceneImg;
    
    public void StartGame()
    {
        SceneManager.LoadScene(levelSelection1);
    }

    public void OpenOptions()
    {
        OptionSceneImg.SetActive(true);
    }

    public void CloseOptions()
    {
        OptionSceneImg.SetActive(false);
    }
    #region LevelSelection
    public void OpenLevelSelectoionSceneImg()
    {
        LevelSelectoionSceneImg.SetActive(true);
    }

    public void CloseLevelSelectoionSceneImg()
    {
        LevelSelectoionSceneImg.SetActive(false);
    }

    public void LoadLevelOne()
    {
        SceneManager.LoadScene(levelSelection1);
    }
    public void LoadLevelTwo()
    {
        SceneManager.LoadScene(levelSelection2);
    }
    public void LoadLevelThree()
    {
        SceneManager.LoadScene(levelSelection3);
    }
    #endregion
    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
