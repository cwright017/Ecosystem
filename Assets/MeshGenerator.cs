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

    List<Biome> biomes;

    IDictionary<Sides, int[]> sideVertIndexByDir = new Dictionary<Sides, int[]>()
    {
        { Sides.Up, new int[] { 0, 1 } },
        { Sides.Down, new int[] { 3, 2 } },
        { Sides.Left, new int[] { 2, 0 } },
        { Sides.Right, new int[] { 1, 3 } },
    };

    float[,] heightMap;

    public void Generate()
    {
        SetupMeshComponents();

        SetupBiomes();

        heightMap = HeightmapGenerator.Generate(noiseSettings, width, length, true);

        for (int l = 0; l <= length - 1; l++)
        {
            for (int w = 0; w <= width - 1; w++)
            {

                //Top
                Vector3[] topVerts = AddTop(w, l);

                // Sides

                bool isWaterTile = IsWaterTile(w, l);

                if (w == 0 || (IsWaterTile(w-1, l) && !isWaterTile))
                {
                    AddSide(Sides.Left, topVerts, w, l);
                }

                if (w == width - 1 || (IsWaterTile(w + 1, l) && !isWaterTile))
                {
                    AddSide(Sides.Right, topVerts, w, l);
                }

                if (l == 0 || (IsWaterTile(w, l - 1) && !isWaterTile))
                {
                    AddSide(Sides.Down, topVerts, w, l);
                }

                if (l == length - 1 || (IsWaterTile(w, l + 1) && !isWaterTile))
                {
                    AddSide(Sides.Up, topVerts, w, l);
                }

            }
        }

        meshData.attach(mesh);

        meshRenderer.sharedMaterial = mat;

        UpdateColours();
    }

    bool IsWaterTile(int w, int l)
    {
        Vector2 uv = GetBiomeInfo(heightMap[w, l]);

        bool isWaterTile = uv.x == 0f;

        return isWaterTile;
    }

    void SetupBiomes()
    {
        biomes = new List<Biome>();

        biomes.Add(water);
        biomes.Add(sand);
        biomes.Add(grass);

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

    Vector3[] AddTop(int w, int l)
    {
        float minW = (centre) ? -width / 2f : 0;
        float minH = (centre) ? -length / 2f : 0;

        bool isWaterTile = IsWaterTile(w, l);

        float depth = isWaterTile ? -waterTileHeight : landTileHeight;

        // Top 
        Vector3 a = new Vector3(minW + w, depth, minH + l + 1);
        Vector3 b = a + Vector3.right;
        Vector3 c = a + Vector3.back;
        Vector3 d = c + Vector3.right;

        Vector3[] topVerts = { a, b, c, d };

        AddFace(topVerts, w, l, depth);

        return topVerts;
    }

    void AddSide(Sides side, Vector3[] topVerts, int w, int l)
    {
        bool isWaterTile = IsWaterTile(w, l);

        float depth = isWaterTile ? waterTileHeight : waterTileHeight * 2;

        int[] i = sideVertIndexByDir[side];
        Vector3 a = topVerts[i[0]];
        Vector3 b = a + Vector3.down * depth;
        Vector3 c = topVerts[i[1]];
        Vector3 d = c + Vector3.down * depth;

        Vector3[] sideVerts = { a, b, c, d };
        AddFace(sideVerts, w, l, depth);
        
    }

    void AddFace(Vector3[] sideVerts, int w, int l, float depth)
    {
        int vi = meshData.verts.Count;

        Color[] startCols = { water.startCol, sand.startCol, grass.startCol };
        Color[] endCols = { water.endCol, sand.endCol, grass.endCol };

        meshData.verts.AddRange(sideVerts);

        Vector2 uv = GetBiomeInfo(heightMap[w, l]);

        Color color = Color.Lerp(startCols[(int)uv.x], endCols[(int)uv.x], uv.y);
        meshData.colors.AddRange(new[] { color, color, color, color });

        meshData.tris.Add(vi);
        meshData.tris.Add(vi + 1);
        meshData.tris.Add(vi + 2);

        meshData.tris.Add(vi + 2);
        meshData.tris.Add(vi + 1);
        meshData.tris.Add(vi + 3);
    }

    void UpdateColours()
    {
        if (mat != null)
        {
            mat.SetColor("_Color", Color.white);
        }
    }

    Vector2 GetBiomeInfo(float height)
    {
        // Find current biome
        int biomeIndex = 0;
        float biomeStartHeight = 0;

        for (int i = 0; i < biomes.Count; i++)
        {
            if (height <= biomes[i].height)
            {
                biomeIndex = i;
                break;
            }
            biomeStartHeight = biomes[i].height;
        }

        Biome biome = biomes[biomeIndex];
        float sampleT = Mathf.InverseLerp(biomeStartHeight, biome.height, height);
        sampleT = (int)(sampleT * biome.numSteps) / (float)Mathf.Max(biome.numSteps, 1);

        // UV stores x: biomeIndex and y: val between 0 and 1 for how close to prev/next biome
        Vector2 uv = new Vector2(biomeIndex, sampleT);
        return uv;
    }

}
