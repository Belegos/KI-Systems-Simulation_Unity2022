using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;//makes the texture more crisp
        texture.wrapMode = TextureWrapMode.Clamp;//fix the wrapping at the edges
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0); //first dimension of the array
        int hight = heightMap.GetLength(1); //second dimension of the array

        Color[] colorMap = new Color[width * hight];

        for (int y = 0; y < hight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                //(y* width + x) == index of the 2D array in a one dimentional array, getting a color betweem black and white
                //percentage between minimum of noisemap.x and maximum noisemap.y
            }
        }
        return TextureFromColorMap(colorMap, width, hight);
    }
}

