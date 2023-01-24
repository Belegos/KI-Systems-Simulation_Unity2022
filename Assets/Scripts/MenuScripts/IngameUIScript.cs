using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class IngameUIScript : MonoBehaviour
{
    [FormerlySerializedAs("WaterScene")] [SerializeField] private string waterScene;
    [FormerlySerializedAs("MenuScene")] [SerializeField] private string menuScene;
    [FormerlySerializedAs("DungeonScene")] [SerializeField] private string dungeonScene;
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(menuScene);
    }
}