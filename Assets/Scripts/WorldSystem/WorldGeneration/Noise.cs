using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode { SingleMode, EndlessMode };
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode) // Generates a 2D array of floats for a noise map
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        #region SeedRandomInV2Array
        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float freuqency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }
        #endregion

        #region DivideByZeroCheck
        if (scale <= 0)
        {
            scale = 0.0001f; //clap to a minimum value to prevent divide by zero error
        }
        #endregion
        float maxSingleNoiseHeight = float.MinValue;
        float minSingleNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                 amplitude = 1;
                 freuqency = 1;
                float noiseHeight = 0;
                #region OctaveLoop  //loop that defines the detail range of the octaves
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octavesOffsets[i].x) / scale * freuqency ;
                    float sampleY = (y - halfHeight + octavesOffsets[i].y) / scale * freuqency ;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance; // decreases each octave
                    freuqency *= lacunarity; // increases each octave
                }
                #endregion
                if (noiseHeight > maxSingleNoiseHeight)
                {
                    maxSingleNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minSingleNoiseHeight)
                {
                    minSingleNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if(normalizeMode == NormalizeMode.SingleMode)
                {
                noiseMap[x, y] = Mathf.InverseLerp(minSingleNoiseHeight, maxSingleNoiseHeight, noiseMap[x, y]);
                //returns value between 0&1, if noiseMap==minSingleNoiseHeight => returns 0, if == to mxNoiseHeight returns 1, if half way between return 0.5
                //Long story short: normalize the noiseMap
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1)/(maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
