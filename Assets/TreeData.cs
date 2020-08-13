using UnityEngine;

[System.Serializable]
public struct Tree
{
    public GameObject prefab;
    [Range(0, 1)]
    public float probability;
    public Color color;
}

[System.Serializable]
public class TreeData
{
    public int seed;
    public Tree[] trees;
    public float colorVariation = 0.1f;
    public float sizeVariation = 0.1f;

}
