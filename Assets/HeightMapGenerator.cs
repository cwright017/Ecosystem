using UnityEngine;

public static class HeightmapGenerator
{
    public static float[,] Generate(NoiseSettings noiseSettings, int width, int length, bool normalize = true)
    {
        float[,] map = new float[width, length];
        System.Random prng = new System.Random(noiseSettings.seed);
        Noise noise = new Noise(noiseSettings.seed);

        Vector2[] offsets = new Vector2[noiseSettings.numLayers];

        for(int layer = 0; layer < noiseSettings.numLayers; layer++)
        {
            // use settings
            offsets[layer] = new Vector2((float) prng.NextDouble() * 2 - 1, (float) prng.NextDouble() * 2 - 1) * 10000;
        }

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        for (int l = 0; l < length; l++)
        {
            for (int w = 0; w < width; w++)
            {
                float frequency = noiseSettings.scale;
                float amplitude = 1;
                float height = 0;

                // Sum layers of noise
                for (int layer = 0; layer < noiseSettings.numLayers; layer++)
                {
                    double sampleX = (double) w / width * frequency + offsets[layer].x + noiseSettings.offset.x;
                    double sampleY = (double) l / length * frequency + offsets[layer].y + noiseSettings.offset.y;
                    height += (float) noise.Evaluate(sampleX, sampleY) * amplitude;
                    frequency *= noiseSettings.lacunarity;
                    amplitude *= noiseSettings.persistence;
                }
                map[w, l] = height;

                minHeight = Mathf.Min(minHeight, height);
                maxHeight = Mathf.Max(maxHeight, height);
            }
        }

        if (normalize)
        {
            for(int l = 0; l < length; l++)
            {
                for(int w = 0; w < width; w++)
                {
                    map[w, l] = Mathf.InverseLerp(minHeight, maxHeight, map[w, l]);
                }
            }
        }

        return map;
    }
}
