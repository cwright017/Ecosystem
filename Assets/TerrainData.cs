public static class TerrainData
{
    public static int width;
    public static int length;

    public static float[,] heightMap;

    public static bool[,] walkableTiles;
    public static bool[,] waterTiles;
    public static bool[,] shoreTiles;
    public static bool[,] edgeTiles;

    public static void Setup(int width, int length)
    {
        TerrainData.width = width;
        TerrainData.length = length;

        walkableTiles = new bool[width, length];
        waterTiles = new bool[width, length];
        shoreTiles = new bool[width, length];
        edgeTiles = new bool[width, length];
    }

    public static void AddTile(int x, int y)
    {
        //if (biomeInfo[0] == 0f)
        //{
        //}
    }

    public static bool IsWalkableTile(int x, int y)
    {
        return walkableTiles[x, y];
    }

    public static bool IsWaterTile(int x, int y)
    {
        return waterTiles[x, y];
    }

    public static bool IsShoreTile(int x, int y)
    {
        return shoreTiles[x, y];
    }

}