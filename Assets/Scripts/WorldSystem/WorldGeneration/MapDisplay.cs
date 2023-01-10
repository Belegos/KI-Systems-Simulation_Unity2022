using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture; //sharedMaterial set equal to the new texture out of runtime

        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height); //scale the texture to the size of the plane / map
    }
}
