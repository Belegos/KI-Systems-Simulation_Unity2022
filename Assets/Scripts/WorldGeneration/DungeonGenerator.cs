using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int _level = 1;
    [SerializeField][Range(2.6f, 5.0f)] private float _levlelMultiplier = 2.6f;
    private enum ERoomTypes
    {
        empty = -1,
        normal
    }
    private const int MAP_WIDTH = 9;
    private const int MAP_HIGHT = 8;
    ERoomTypes[,] map;

    private void Awake()
    {
        GenerateMap(_level);
    }

    private void GenerateMap(int _level)
    {
        map = new ERoomTypes[MAP_WIDTH, MAP_HIGHT];

        for (int y = 0; y < MAP_HIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                map[x, y] = ERoomTypes.empty; //sets all rooms to empty to be save, there is no error
            }
        }
        int neededRoomCount = Random.Range(0, 2) + 5 + (int)(_level * _levlelMultiplier);

        bool isMapValid = false;
        while (!isMapValid)
        {
            //Generate new Level
            ERoomTypes[,] mapToValidate = GenerateLevelToValidate(map, neededRoomCount); //tmp ERoomtype to modify for future usage

            //Validate new Level
            bool isValid = true;
            if (isValid)
            {
                isMapValid = true;
                map = mapToValidate;
            }
        }

    }
    private ERoomTypes[,] GenerateLevelToValidate(ERoomTypes[,] _baseLevel, int _numRoomsToGenerate)
    {


        return _baseLevel;
    }
}
