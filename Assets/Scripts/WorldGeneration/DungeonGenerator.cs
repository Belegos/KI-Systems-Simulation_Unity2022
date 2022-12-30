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
        normal,
        start
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
        Queue<Vector2Int> positionsToExpand = new Queue<Vector2Int>(); //coordinates of rooms
        Vector2Int midPos = new Vector2Int(MAP_WIDTH / 2, MAP_HIGHT / 2); //middle of level
        _baseLevel[midPos.x, midPos.y] = ERoomTypes.start;
        positionsToExpand.Enqueue(midPos);//addes midPos to Queue
        int currRoomCount = 1;

        while (positionsToExpand.Count > 0)
        {
            Vector2Int currPosToExpamd = positionsToExpand.Dequeue(); //dequeue's the first position of rooms and saves in local var
            Vector2Int[] positionsToCheck = new Vector2Int[]
            {
                currPosToExpamd + Vector2Int.up,
                currPosToExpamd + Vector2Int.right,
                currPosToExpamd + Vector2Int.down,
                currPosToExpamd + Vector2Int.left
            };
            for (int checkPosIdc = 0; checkPosIdc < positionsToCheck.Length; checkPosIdc++)
            {
                Vector2Int toCheck = positionsToCheck[checkPosIdc];

                //check if in bounds
                if (toCheck.x >= 0 && toCheck.x < MAP_WIDTH && toCheck.y >= 0 && toCheck.y < MAP_HIGHT) //checks if in bounds of array
                {
                    //Rule 1: if enough rooms => continue to next room

                    //Rule 2:

                    //Rule 3:

                }
            }
        }

        return _baseLevel;
    }
}
