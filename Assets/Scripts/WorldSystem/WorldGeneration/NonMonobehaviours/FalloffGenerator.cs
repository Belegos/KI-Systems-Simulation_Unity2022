using UnityEngine;

public static class FalloffGenerator 
{
    /// <summary>
    /// Generate a negative map of the noise map
    /// </summary>
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float xCoord = (float)i / size * 2 - 1; //transform -1 to 1 value 
                float yCoord = (float)j / size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(xCoord), Mathf.Abs(yCoord));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }
    /// <summary>
    /// modifies the falloffMap to be less strong at the center and more stronger on the edges
    /// Formula:
    /// f(x)= (x^a)/((x^a)+(b-b*x)^a)
    /// </summary>
    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a)); //Mathf.Pow(b-b*value,a) is equal to (b-b*x)^a
    }
}
