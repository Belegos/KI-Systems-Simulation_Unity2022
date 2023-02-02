using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimplePrefabSpawner : EditorWindow
{
    private string objectBaseName = "";
    private int objectCount = 1;
    private GameObject objectToSpawn;
    private Vector3 spawnPosition = Vector3.zero;
    private Vector3 spawnRotation = Vector3.zero;
    private Vector3 spawnScale = Vector3.one;

    private bool randomizeYRotation = false;
    private bool randomizeScale = false;
    private bool randomizePosition = false;

    [MenuItem("Tools/Simple Prefab Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SimplePrefabSpawner)); // Opens the window, GetWindows inherited from the class EditorWindow
    }

    private void OnGUI()
    {
        GUILayout.Label("Simple Prefab Spawner", EditorStyles.boldLabel); // Creates a label with the text "Simple Prefab Spawner" and the style "boldLabel"
        objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to spawn", objectToSpawn, typeof(GameObject), false); // Creates a field to select a prefab
        objectBaseName = EditorGUILayout.TextField("Base name", objectBaseName); // Creates a field to enter a base name
        objectCount = EditorGUILayout.IntField("Number of objects", objectCount); // Creates a field to enter the number of objects to spawn
        spawnPosition = EditorGUILayout.Vector3Field("Position", spawnPosition); // Creates a field to enter the position of the objects
        spawnRotation = EditorGUILayout.Vector3Field("Rotation", spawnRotation); // Creates a field to enter the rotation of the objects
        spawnScale = EditorGUILayout.Vector3Field("Scale", spawnScale); // Creates a field to enter the scale of the objects
        randomizeYRotation = EditorGUILayout.Toggle("Randomize Y Rotation", randomizeYRotation); // Creates a toggle to randomize the y rotation of the objects
        randomizeScale = EditorGUILayout.Toggle("Randomize Scale", randomizeScale); // Creates a toggle to randomize the scale of the objects
        randomizePosition = EditorGUILayout.Toggle("Randomize Position", randomizePosition); // Creates a toggle to randomize the position of the objects

        if (GUILayout.Button("Spawn")) // Creates a button to spawn the objects
        {
            SpawnObjects(randomizeYRotation, randomizeScale, randomizePosition);
        }
    }
    private void SpawnObjects(bool randomizeYRotation, bool randomizeScale = false, bool randomizePosition = false)
    {
        Collider[] SimpleColliderObj = new Collider[256]; //stores nonAlloc objects
        for (int i = 0; i < objectCount; i++)
        {
            GameObject spawnedObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject; // Instantiates the prefab
            spawnedObject.name = objectBaseName + i; // Sets the name of the object
            spawnedObject.transform.position = spawnPosition; // Sets the position of the object
            spawnedObject.transform.rotation = Quaternion.Euler(spawnRotation); // Sets the rotation of the object
            spawnedObject.transform.localScale = spawnScale; // Sets the scale of the object

            if (randomizeYRotation) // If the randomizeYRotation is true, randomize the y rotation
            {
                spawnedObject.transform.Rotate(0, Random.Range(0, 360), 0);
            }
            if (randomizeScale) // If the randomizeScale is true, randomize the scale
            {
                spawnedObject.transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f));
            }
            if (randomizePosition) // If the randomizePosition is true, randomize the position
            {
                Vector3 newPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                int foundItemsCount = Physics.OverlapSphereNonAlloc(newPosition, spawnedObject.GetComponent<Collider>().bounds.size.magnitude, SimpleColliderObj);
                if (foundItemsCount == 0) // check if there are no other objects at the random position
                {
                    spawnedObject.transform.position = newPosition;
                }
                else // if there are other objects at the random position, try again
                {
                    i--;
                }
            }
        }
    }
}
