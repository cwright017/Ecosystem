using System;
using System.Collections.Generic;
using UnityEngine;

//Vector3.up
//Vector3(0, 1, 0)

//Vector3.down
//Vector3(0, -1, 0)

//Vector3.left
//Vector3(-1, 0, 0)

//Vector3.right
//Vector3(1, 0, 0)

//Vector3.forward
//Vector3(0, 0, 1)

//Vector3.back
//Vector3(0, 0, -1)

[ExecuteInEditMode]
public class MeshGenerator : MonoBehaviour
{
    enum Sides { Up, Down, Left, Right };

    public int width;
    public int length;

    public Material mat;

    public NoiseSettings noiseSettings;

    public bool centre = true;

    public Biome water;
    public Biome sand;
    public Biome grass;

    GameObject holder;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    MeshData meshData;

    float waterTileHeight = 0.2f;
    float landTileHeight = 0f;

    IDictionary<Sides, int[]> sideVertIndexByDir = new Dictionary<Sides, int[]>()
    {
        { Sides.Up, new int[] { 0, 1 } },
        { Sides.Down, new int[] { 3, 2 } },
        { Sides.Left, new int[] { 2, 0 } },
        { Sides.Right, new int[] { 1, 3 } },
    };

    public void Generate()
    {
        SetupMeshComponents();

        SetupBiomes();

        SetupMaterial();

        TerrainData.Setup(width, length);

        TerrainData.heightMap = HeightmapGenerator.Generate(noiseSettings, width, length, true);

        for (int y = 0; y <= length - 1; y++)
        {
            for (int x = 0; x <= width - 1; x++)
            {

                TerrainData.AddTile(x, y);

                //Top
                Vector3[] topVerts = AddTop(x, y);

                // Sides

                bool isWaterTile = TerrainData.IsWaterTile(x,y);

                if (x == 0 || (TerrainData.IsWaterTile(x-1, y) && !isWaterTile))
                {
                    AddSide(Sides.Left, topVerts, x, y);
                }

                if (x == width - 1 || (TerrainData.IsWaterTile(x + 1, y) && !isWaterTile))
                {
                    AddSide(Sides.Right, topVerts, x, y);
                }

                if (y == 0 || (TerrainData.IsWaterTile(x, y - 1) && !isWaterTile))
                {
                    AddSide(Sides.Down, topVerts, x, y);
                }

                if (y == length - 1 || (TerrainData.IsWaterTile(x, y + 1) && !isWaterTile))
                {
                    AddSide(Sides.Up, topVerts, x, y);
                }

            }
        }

        meshData.attach(mesh);
    }

    void SetupBiomes()
    {
        BiomeData.biomes.Add(water);
        BiomeData.biomes.Add(sand);
        BiomeData.biomes.Add(grass);

    }

    void SetupMeshComponents()
    {
        holder = new GameObject("Terrain");

        meshFilter = holder.AddComponent<MeshFilter>();

        meshRenderer = holder.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        meshData = new MeshData();

        if (meshFilter.sharedMesh == null)
        {
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshFilter.sharedMesh = mesh;
        }
        else
        {
            mesh = meshFilter.sharedMesh;
            mesh.Clear();
        }
    }

    void SetupMaterial()
    {
        if (mat != null)
        {
            mat.SetColor("_Color", Color.white);

            meshRenderer.sharedMaterial = mat;
        }
    }

    Vector3[] AddTop(int x, int y)
    {
        float minW = (centre) ? -width / 2f : 0;
        float minH = (centre) ? -length / 2f : 0;

        bool isWaterTile = TerrainData.IsWaterTile(x, y);

        float depth = isWaterTile ? -waterTileHeight : landTileHeight;

        // Top 
        Vector3 a = new Vector3(minW + x, depth, minH + y + 1);
        Vector3 b = a + Vector3.right;
        Vector3 c = a + Vector3.back;
        Vector3 d = c + Vector3.right;

        Vector3[] topVerts = { a, b, c, d };

        AddFace(topVerts, x, y, depth);

        return topVerts;
    }

    void AddSide(Sides side, Vector3[] topVerts, int x, int y)
    {
        bool isWaterTile = TerrainData.IsWaterTile(x, y);

        float depth = isWaterTile ? waterTileHeight : waterTileHeight * 2;

        int[] i = sideVertIndexByDir[side];
        Vector3 a = topVerts[i[0]];
        Vector3 b = a + Vector3.down * depth;
        Vector3 c = topVerts[i[1]];
        Vector3 d = c + Vector3.down * depth;

        Vector3[] sideVerts = { a, b, c, d };
        AddFace(sideVerts, x, y, depth);
        
    }

    void AddFace(Vector3[] sideVerts, int x, int y, float depth)
    {
        int vi = meshData.verts.Count;

        Color[] startCols = { water.startCol, sand.startCol, grass.startCol };
        Color[] endCols = { water.endCol, sand.endCol, grass.endCol };

        meshData.verts.AddRange(sideVerts);

        Vector2 uv = BiomeData.GetBiomeInfo(x, y);

        Color color = Color.Lerp(startCols[(int)uv.x], endCols[(int)uv.x], uv.y);

        meshData.colors.AddRange(new[] { color, color, color, color });

        meshData.tris.Add(vi);
        meshData.tris.Add(vi + 1);
        meshData.tris.Add(vi + 2);

        meshData.tris.Add(vi + 2);
        meshData.tris.Add(vi + 1);
        meshData.tris.Add(vi + 3);
    }

}
