using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameUIScript : MonoBehaviour
{
    [SerializeField] private string WaterScene;
    [SerializeField] private string MenuScene;
    [SerializeField] private string DungeonScene;
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(MenuScene);
    }
}