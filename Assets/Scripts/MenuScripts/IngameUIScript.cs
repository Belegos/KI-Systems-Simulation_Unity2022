using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameUIScript : MonoBehaviour
{
    [SerializeField] private string WaterScene;
    [SerializeField] private string MenuScene;
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(MenuScene);
    }
}