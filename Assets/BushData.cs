using UnityEngine;

[System.Serializable]
public struct Element
{
    public GameObject prefab;
    [Range(0, 1)]
    public float probability;
    public Color color;
}

[System.Serializable]
public class Folliage
{
    public int seed;
    public Element[] elements;
    public float colorVariation = 0.1f;
    public float sizeVariation = 0.1f;
    public bool isObstical = false;
}

public class BushData : Folliage
{
}
