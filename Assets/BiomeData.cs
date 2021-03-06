﻿using System;
using System.Collections.Generic;
using UnityEngine;

public struct BiomeInfo
{
    public int biomeIndex { get; }
    public float biomeDistance { get; }

    public BiomeInfo(int biomeIndex, float biomeDistance)
    {
        this.biomeIndex = biomeIndex;
        this.biomeDistance = biomeDistance;
    }
}

public static class BiomeData
{
    public static List<Biome> biomes = new List<Biome>();

    public static BiomeInfo GetBiomeInfo(int x, int y)
    {
        float height = TerrainData.heightMap[x, y];

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
        BiomeInfo biomeInfo = new BiomeInfo(biomeIndex, sampleT);
        return biomeInfo;
    }
}
