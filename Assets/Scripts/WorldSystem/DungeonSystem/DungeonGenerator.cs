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
        Debug.LogWarning(LogMap(map));
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

        int maxInterations = 3000;//prevents infinite loops
        int currentIterations = 0;

        bool isMapValid = false;
        while (!isMapValid && currentIterations < maxInterations)
        {
            //Generate new Level
            ERoomTypes[,] mapToValidate = GenerateLevelToValidate(map, neededRoomCount); //tmp ERoomtype to modify for future usage

            //Validate new Level
            isMapValid = ValidateMap(mapToValidate, neededRoomCount);
            if (isMapValid)
            {
                isMapValid = true;
                map = mapToValidate;
            }
            currentIterations++;
        }
#if (UNITY_EDITOR)
        if (currentIterations >= maxInterations)
        {
            Debug.LogError("Max Iterations reached");
        }
        
#endif

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
            for (int checkPosIdx = 0; checkPosIdx < positionsToCheck.Length; checkPosIdx++)
            {
                Vector2Int toCheck = positionsToCheck[checkPosIdx];

                //check if in bounds
                if (toCheck.x >= 0 && toCheck.x < MAP_WIDTH && toCheck.y >= 0 && toCheck.y < MAP_HIGHT) //checks if in bounds of array
                {
                    //Rule 1: if enough rooms => continue to next room
                    if (currRoomCount >= _numRoomsToGenerate) { continue; }
                    //Rule 2: if there is already a room => continue to next room
                    if (_baseLevel[toCheck.x, toCheck.y] != ERoomTypes.empty) { continue; }
                    //Rule 3: coinflip rnd chance to skip room => continue to next room
                    float rndPercent = Random.Range(0f, 1f);
                    if (rndPercent <= 0.5f) { continue; }

                    //Rule 4: if more than one neihgbour room is at position => continue to next Room
                    int neighbourCount = GetNeighbourCount(_baseLevel, toCheck);
                    if (neighbourCount < 1) { continue; }

                    //Generate Room

                    _baseLevel[toCheck.x, toCheck.y] = ERoomTypes.normal;
                    positionsToExpand.Enqueue(toCheck);
                    currRoomCount++;
                }
            }
        }

        return _baseLevel;
    }
    private bool ValidateMap(ERoomTypes[,] _levelToValidate, int _neededRoomCount)
    {
        int count = 0;
        for (int y = 0; y < MAP_HIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                if (_levelToValidate[x, y] != ERoomTypes.empty)
                {
                    count++;
                }
            }
        }
        return count >= _neededRoomCount;//even if more rooms were created than needed, the map is still valid
    }
    private int GetNeighbourCount(ERoomTypes[,] _level, Vector2Int _positionToCheck)
    {
        int count = 0;

        Vector2Int[] positionsToCheck = new Vector2Int[]
        {
            _positionToCheck + Vector2Int.up,
            _positionToCheck + Vector2Int.right,
            _positionToCheck + Vector2Int.down,
            _positionToCheck + Vector2Int.left
        };

        for (int neighbourPosIdx = 0; neighbourPosIdx < positionsToCheck.Length; neighbourPosIdx++)
        {
            Vector2Int toCheck = positionsToCheck[neighbourPosIdx];

            if (toCheck.x >= 0 && toCheck.x < MAP_WIDTH && toCheck.y >= 0 && toCheck.y < MAP_HIGHT)
            {
                if (_level[toCheck.x, toCheck.y] != ERoomTypes.empty)
                {
                    count++;
                }
            }
        }

        return count;
    }


    private string LogMap(ERoomTypes[,] _map) //generate a output string, to better see roomtypes
    {
        int mapWidth = _map.GetLength(0);
        int mapHeight = _map.GetLength(1);

        string output = "";
        for (int y = 0; y < mapHeight; y++)
        {
            output += "|";
            for (int x = 0; x < mapWidth; x++)
            {
                int num = (int)_map[x, y];
                if (num >= 0)
                {
                    output += " ";
                }
                output += num + "|";
            }
            output += "\n";
        }
        return output;
    }
}
