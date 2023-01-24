using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DungeonGenerator : MonoBehaviour
{
    [FormerlySerializedAs("_level")] [SerializeField] private int level = 1;
    [FormerlySerializedAs("_levlelMultiplier")] [SerializeField][Range(2.6f, 5.0f)] private float levlelMultiplier = 2.6f;
    private enum ERoomTypes
    {
        Empty = -1,
        Normal,
        Start
    }
    private const int MapWidth = 9;
    private const int MapHight = 8;
    ERoomTypes[,] _map;

    private void Awake()
    {
        GenerateMap(level);
        Debug.LogWarning(LogMap(_map));
    }

    private void GenerateMap(int level)
    {
        _map = new ERoomTypes[MapWidth, MapHight];

        for (int y = 0; y < MapHight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                _map[x, y] = ERoomTypes.Empty; //sets all rooms to empty to be save, there is no error
            }
        }
        int neededRoomCount = Random.Range(0, 2) + 5 + (int)(level * levlelMultiplier);

        int maxInterations = 3000;//prevents infinite loops
        int currentIterations = 0;

        bool isMapValid = false;
        while (!isMapValid && currentIterations < maxInterations)
        {
            //Generate new Level
            ERoomTypes[,] mapToValidate = GenerateLevelToValidate(_map, neededRoomCount); //tmp ERoomtype to modify for future usage

            //Validate new Level
            isMapValid = ValidateMap(mapToValidate, neededRoomCount);
            if (isMapValid)
            {
                isMapValid = true;
                _map = mapToValidate;
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
    private ERoomTypes[,] GenerateLevelToValidate(ERoomTypes[,] baseLevel, int numRoomsToGenerate)
    {
        Queue<Vector2Int> positionsToExpand = new Queue<Vector2Int>(); //coordinates of rooms
        Vector2Int midPos = new Vector2Int(MapWidth / 2, MapHight / 2); //middle of level
        baseLevel[midPos.x, midPos.y] = ERoomTypes.Start;
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
                if (toCheck.x >= 0 && toCheck.x < MapWidth && toCheck.y >= 0 && toCheck.y < MapHight) //checks if in bounds of array
                {
                    //Rule 1: if enough rooms => continue to next room
                    if (currRoomCount >= numRoomsToGenerate) { continue; }
                    //Rule 2: if there is already a room => continue to next room
                    if (baseLevel[toCheck.x, toCheck.y] != ERoomTypes.Empty) { continue; }
                    //Rule 3: coinflip rnd chance to skip room => continue to next room
                    float rndPercent = Random.Range(0f, 1f);
                    if (rndPercent <= 0.5f) { continue; }

                    //Rule 4: if more than one neihgbour room is at position => continue to next Room
                    int neighbourCount = GetNeighbourCount(baseLevel, toCheck);
                    if (neighbourCount < 1) { continue; }

                    //Generate Room

                    baseLevel[toCheck.x, toCheck.y] = ERoomTypes.Normal;
                    positionsToExpand.Enqueue(toCheck);
                    currRoomCount++;
                }
            }
        }

        return baseLevel;
    }
    private bool ValidateMap(ERoomTypes[,] levelToValidate, int neededRoomCount)
    {
        int count = 0;
        for (int y = 0; y < MapHight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (levelToValidate[x, y] != ERoomTypes.Empty)
                {
                    count++;
                }
            }
        }
        return count >= neededRoomCount;//even if more rooms were created than needed, the map is still valid
    }
    private int GetNeighbourCount(ERoomTypes[,] level, Vector2Int positionToCheck)
    {
        int count = 0;

        Vector2Int[] positionsToCheck = new Vector2Int[]
        {
            positionToCheck + Vector2Int.up,
            positionToCheck + Vector2Int.right,
            positionToCheck + Vector2Int.down,
            positionToCheck + Vector2Int.left
        };

        for (int neighbourPosIdx = 0; neighbourPosIdx < positionsToCheck.Length; neighbourPosIdx++)
        {
            Vector2Int toCheck = positionsToCheck[neighbourPosIdx];

            if (toCheck.x >= 0 && toCheck.x < MapWidth && toCheck.y >= 0 && toCheck.y < MapHight)
            {
                if (level[toCheck.x, toCheck.y] != ERoomTypes.Empty)
                {
                    count++;
                }
            }
        }

        return count;
    }


    private string LogMap(ERoomTypes[,] map) //generate a output string, to better see roomtypes
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);

        string output = "";
        for (int y = 0; y < mapHeight; y++)
        {
            output += "|";
            for (int x = 0; x < mapWidth; x++)
            {
                int num = (int)map[x, y];
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
