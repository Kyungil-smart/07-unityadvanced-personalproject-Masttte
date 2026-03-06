using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D GenerateDitherPattern()
    {
        int[,] bayer = new int[,] {
            {  0, 128,  32, 160 },
            { 192,  64, 224,  96 },
            {  48, 176,  16, 144 },
            { 240, 112, 208,  80 }
        };
        
        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;
        
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                float value = bayer[y, x] / 255f;
                tex.SetPixel(x, y, new Color(value, value, value, 1));
            }
        }
        
        tex.Apply();
        return tex;
    }
    
    public static Texture2D GenerateDiagonalPattern()
    {
        int size = 20;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isLine = ((x + y) % 10) < 2;
                Color col = isLine ? new Color(0, 0, 0, 0.1f) : Color.clear;
                tex.SetPixel(x, y, col);
            }
        }
        
        tex.Apply();
        return tex;
    }
    
    public static Texture2D GeneratePixelParticle()
    {
        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                bool isCenter = (x >= 1 && x <= 2 && y >= 1 && y <= 2);
                tex.SetPixel(x, y, isCenter ? Color.white : Color.clear);
            }
        }
        
        tex.Apply();
        return tex;
    }
}
