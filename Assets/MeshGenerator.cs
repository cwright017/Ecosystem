using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public TreeData treeData;
    public BushData bushData;
    public NoiseSettings noiseSettings;

    public bool centre = true;

    public Biome water;
    public Biome sand;
    public Biome grass;

    GameObject landHolder;
    GameObject waterHolder;

    MeshData landData;
    MeshData waterData;

    readonly float waterTileHeight = 0.2f;
    readonly float landTileHeight = 0f;

    readonly IDictionary<Sides, int[]> sideVertIndexByDir = new Dictionary<Sides, int[]>()
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

        landData.attach();
        waterData.attach();

        SpawnFolliage(treeData);
        SpawnFolliage(bushData);

        landData.navMeshSurface.BuildNavMesh();
    }

    void SetupBiomes()
    {
        BiomeData.biomes.Clear();

        BiomeData.biomes.Add(water);
        BiomeData.biomes.Add(sand);
        BiomeData.biomes.Add(grass);

    }

    void SetupMeshComponents()
    {
        
        landHolder = new GameObject("Terrain");

        waterHolder = new GameObject("Water");
        waterHolder.layer = LayerMask.NameToLayer("Water");

        // Parent under land
        waterHolder.transform.parent = landHolder.transform;

        landData = new MeshData(landHolder, true);
        waterData = new MeshData(waterHolder);
    }

    void SetupMaterial()
    {
        if (mat != null)
        {
            mat.SetColor("_Color", Color.white);

            landData.meshRenderer.sharedMaterial = mat;
            waterData.meshRenderer.sharedMaterial = mat;
        }
    }

    void SpawnFolliage(Folliage data)
    {
        if (data.elements.Length == 0) return;

        Array.Sort(data.elements, delegate (Element a, Element b)
        {
            return b.probability.CompareTo(a.probability);
        });

        System.Random spawnPrng = new System.Random(data.seed);

        for (int y = 0; y <= length - 1; y++)
        {
            for (int x = 0; x <= width - 1; x++)
            {
                if (TerrainData.IsWalkableTile(x, y))
                {
                    for (int i = 0; i < data.elements.Length; i++)
                    {
                        if (spawnPrng.NextDouble() < data.elements[i].probability)
                        {
                            GameObject element = Instantiate(data.elements[i].prefab, TerrainData.tileCentres[x, y], Quaternion.Euler(0, 0, 0));
                            MeshRenderer elementMesh = element.GetComponent<MeshRenderer>();

                            // Color
                            Color minCol = data.elements[i].color;
                            Color maxCol = new Color
                            {
                                r = minCol.r + ((float)spawnPrng.NextDouble() * 2 - 1) * data.colorVariation,
                                g = minCol.r + ((float)spawnPrng.NextDouble() * 2 - 1) * data.colorVariation,
                                b = minCol.r + ((float)spawnPrng.NextDouble() * 2 - 1) * data.colorVariation
                            };

                            Color color = Color.Lerp(minCol, maxCol, (float)spawnPrng.NextDouble());

                            elementMesh.material.color = color;

                            // Scale
                            Vector3 scale = Vector3.one * (1 + ((float)spawnPrng.NextDouble() * 2 - 1) * treeData.sizeVariation);
                            element.transform.localScale = scale;

                            // Group under terrain
                            element.transform.parent = landHolder.transform;

                            // Add NavMesh
                            if (data.isObstical)
                            {
                                NavMeshObstacle navMeshObstacle = element.AddComponent<NavMeshObstacle>();
                                navMeshObstacle.carving = true;
                            }

                            // Mark as unwalkable
                            TerrainData.walkableTiles[x, y] = false;
                            break;
                        }
                    }
                }
            }
        }
    }

    Vector3[] AddTop(int x, int y)
    {
        float minW = (centre) ? -width / 2f : 0;
        float minH = (centre) ? -length / 2f : 0;

        bool isWaterTile = TerrainData.IsWaterTile(x, y);

        MeshData meshData = isWaterTile ? waterData : landData;

        float depth = isWaterTile ? -waterTileHeight : landTileHeight;

        // Top 
        Vector3 a = new Vector3(minW + x, depth, minH + y + 1);
        Vector3 b = a + Vector3.right;
        Vector3 c = a + Vector3.back;
        Vector3 d = c + Vector3.right;

        Vector3[] topVerts = { a, b, c, d };

        AddFace(topVerts, x, y, meshData);

        TerrainData.tileCentres[x, y] = a + new Vector3(0.5f, 0, -0.5f);

        return topVerts;
    }

    void AddSide(Sides side, Vector3[] topVerts, int x, int y)
    {
        bool isWaterTile = TerrainData.IsWaterTile(x, y);

        MeshData meshData = isWaterTile ? waterData : landData;

        float depth = isWaterTile ? waterTileHeight : waterTileHeight * 2;

        int[] i = sideVertIndexByDir[side];
        Vector3 a = topVerts[i[0]];
        Vector3 b = a + Vector3.down * depth;
        Vector3 c = topVerts[i[1]];
        Vector3 d = c + Vector3.down * depth;

        Vector3[] sideVerts = { a, b, c, d };

        AddFace(sideVerts, x, y, meshData);
        
    }

    void AddFace(Vector3[] sideVerts, int x, int y, MeshData meshData)
    {
        int vi = meshData.verts.Count;

        Color[] startCols = { water.startCol, sand.startCol, grass.startCol };
        Color[] endCols = { water.endCol, sand.endCol, grass.endCol };

        meshData.verts.AddRange(sideVerts);

        BiomeInfo biomeInfo = BiomeData.GetBiomeInfo(x, y);

        Color color = Color.Lerp(startCols[biomeInfo.biomeIndex], endCols[biomeInfo.biomeIndex], biomeInfo.biomeDistance);

        meshData.colors.AddRange(new[] { color, color, color, color });

        meshData.tris.Add(vi);
        meshData.tris.Add(vi + 1);
        meshData.tris.Add(vi + 2);

        meshData.tris.Add(vi + 2);
        meshData.tris.Add(vi + 1);
        meshData.tris.Add(vi + 3);
    }

}
