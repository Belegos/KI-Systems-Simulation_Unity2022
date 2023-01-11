using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGenerator))]//defines what type of editor this is
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target; //Object the custom Editor is modifying
        if (DrawDefaultInspector()) 
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        } //Draws the default inspector
        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }
}
