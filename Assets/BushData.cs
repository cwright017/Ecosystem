using UnityEngine;

[System.Serializable]
public struct Bush
{
    public GameObject prefab;
    [Range(0, 1)]
    public float probability;
    public Color color;
}

[System.Serializable]
public class BushData
{
    public int seed;
    public Bush[] bushes;
    public float colorVariation = 0.1f;
    public float sizeVariation = 0.1f;

}
