using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    public string levelSelection1 = "WorldLandGeneration";
    public string levelSelection2 = "DungeonScene";
    public string levelSelection3 = "WaterScene";
    public string pauseMenu;
    [FormerlySerializedAs("OptionSceneImg")] public GameObject optionSceneImg;
    [FormerlySerializedAs("LevelSelectoionSceneImg")] public GameObject levelSelectoionSceneImg;
    
    public void StartGame()
    {
        SceneManager.LoadScene(levelSelection1);
    }

    public void OpenOptions()
    {
        optionSceneImg.SetActive(true);
    }

    public void CloseOptions()
    {
        optionSceneImg.SetActive(false);
    }
    #region LevelSelection
    public void OpenLevelSelectoionSceneImg()
    {
        levelSelectoionSceneImg.SetActive(true);
    }

    public void CloseLevelSelectoionSceneImg()
    {
        levelSelectoionSceneImg.SetActive(false);
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
