using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) // Generates a 2D array of floats for a noise map
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        #region SeedRandomInV2Array
        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);
        }
        #endregion

        #region DivideByZeroCheck
        if (scale <= 0)
        {
            scale = 0.0001f; //clap to a minimum value to prevent divide by zero error
        }
        #endregion
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float freuqency = 1;
                float noiseHeight = 0;
                #region OctaveLoop  //loop that defines the detail range of the octaves
                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = (x - halfWidth) / scale * freuqency + octavesOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * freuqency + octavesOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance; // decreases each octave
                    freuqency *= lacunarity; // increases each octave
                }
                #endregion
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                //returns value between 0&1, if noiseMap==minNoiseHeight => returns 0, if == to mxNoiseHeight returns 1, if half way between return 0.5
                //Long story short: normalize the noiseMap
            }
        }

        return noiseMap;
    }
}
