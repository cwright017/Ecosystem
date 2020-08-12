using System.Collections.Generic;
using UnityEngine;

public static class TerrainData
{
    public static int width;
    public static int length;

    public static float[,] heightMap;

    public static bool?[,] walkableTiles;
    public static bool?[,] waterTiles;
    public static bool?[,] shoreTiles;
    public static bool?[,] edgeTiles;

    public static void Setup(int width, int length)
    {
        TerrainData.width = width;
        TerrainData.length = length;

        walkableTiles = new bool?[width, length];
        waterTiles = new bool?[width, length];
        shoreTiles = new bool?[width, length];
        edgeTiles = new bool?[width, length];
    }

    public static void AddTile(int x, int y)
    {

        if(IsWaterTile(x,y))
        { 
            waterTiles[x, y] = true;

            walkableTiles[x, y] = false;
            shoreTiles[x, y] = false;
        } else
        {
            waterTiles[x, y] = false;
            walkableTiles[x, y] = true;
        }

        shoreTiles[x, y] = IsShoreTile(x, y);
        edgeTiles[x, y] = IsEdgeTile(x, y);
    }

    public static bool IsOutOfBounds(int x, int y)
    {
        if (x >= width || x < 0 || y >= length || y < 0)
        {
            return true;
        }

        return false;
    }

    public static bool IsWalkableTile(int x, int y)
    {
        if(IsOutOfBounds(x,y))
        {
            return false;
        }

        Vector2 biomeInfo = BiomeData.GetBiomeInfo(x, y);

        if (walkableTiles[x, y] != null)
        {
            return (bool)walkableTiles[x, y];
        }

        if (!IsWaterTile(x,y))
        {
            walkableTiles[x, y] = true;
            return true;
        }

        walkableTiles[x, y] = false;
        return false;
    }

    public static bool IsWaterTile(int x, int y)
    {
        if (IsOutOfBounds(x, y))
        {
            return false;
        }

        Vector2 biomeInfo = BiomeData.GetBiomeInfo(x, y);

        if (waterTiles[x, y] != null)
        {
            return (bool)waterTiles[x, y];
        }

        if (biomeInfo[0] == 0f)
        {
            waterTiles[x, y] = true;
            return true;
        }

        waterTiles[x, y] = false;
        return false;
    }

    public static bool IsShoreTile(int x, int y)
    {
        if (IsOutOfBounds(x, y))
        {
            return false;
        }

        Vector2 biomeInfo = BiomeData.GetBiomeInfo(x, y);

        if (shoreTiles[x, y] != null)
        {
            return (bool)shoreTiles[x, y];
        }

        if (IsWaterTile(x, y))
        {
            shoreTiles[x, y] = false;
            return false;
        }

        if(IsWaterTile(x + 1, y) || IsWaterTile(x, y + 1) || IsWaterTile(x - 1, y) || IsWaterTile(x, y - 1))
        {
            shoreTiles[x, y] = true;
            return true;
        }

        shoreTiles[x, y] = false;
        return false;
    }

    public static bool IsEdgeTile(int x, int y)
    {
        if (IsOutOfBounds(x, y))
        {
            return false;
        }

        if (edgeTiles[x, y] != null)
        {
            return (bool) edgeTiles[x, y];
        }

        if (x == 0 || x == width - 1 || y == 0 || y == length - 1)
        {
            edgeTiles[x, y] = true;
            return true;
        }

        edgeTiles[x, y] = false;
        return false;
    }

}