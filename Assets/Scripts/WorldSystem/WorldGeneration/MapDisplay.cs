using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0); //first dimension of the array
        int hight = noiseMap.GetLength(1); //second dimension of the array

        Texture2D texture = new Texture2D(width, hight);

        Color[] colorMap = new Color[width * hight];

        for (int y = 0; y < hight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]); 
                //(y* width + x) == index of the 2D array in a one dimentional array, getting a color betweem black and white
                //percentage between minimum of noisemap.x and maximum noisemap.y
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture; //sharedMaterial set equal to the new texture out of runtime

        textureRenderer.transform.localScale = new Vector3(width, 1, hight); //scale the texture to the size of the plane / map
    }
}
