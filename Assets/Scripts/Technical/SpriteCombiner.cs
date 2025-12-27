using UnityEngine;

public class SpriteCombiner : MonoBehaviour
{
    public int textureWidth = 512;
    public int textureHeight = 512;

    public static SpriteCombiner Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Combines multiple sprites into one stacked Sprite for UI (Canvas Button)
    /// </summary>
    /// <param name="sprites">Array of sprites to combine</param>
    /// <param name="overlapPercent">Fraction of sprite height to overlap (0–1)</param>
    /// <param name="spacing">Optional extra spacing in pixels</param>
    /// <returns>Combined Sprite</returns>
    public Sprite CombineSpritesToSprite(Sprite[] sprites, float overlapPercent = 0.6f, int spacing = 0)
    {
        Texture2D finalTex = new Texture2D(
            textureWidth,
            textureHeight,
            TextureFormat.ARGB32,
            false
        );

        // Clear texture to transparent
        Color32[] clear = new Color32[textureWidth * textureHeight];
        for (int i = 0; i < clear.Length; i++)
            clear[i] = new Color32(0, 0, 0, 0);

        finalTex.SetPixels32(clear);

        // Find the widest sprite for horizontal centering
        int maxWidth = 0;
        foreach (var s in sprites)
            if (s != null)
                maxWidth = Mathf.Max(maxWidth, (int)s.textureRect.width);

        int currentY = 0; // vertical stack starting from bottom

        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i];
            if (sprite == null) continue;

            Rect r = sprite.textureRect;
            Color[] pixels = sprite.texture.GetPixels(
                (int)r.x,
                (int)r.y,
                (int)r.width,
                (int)r.height
            );

            // Center horizontally
            int startX = (textureWidth - (int)r.width) / 2;
            int startY = currentY;

            // Copy pixels into final texture
            for (int y = 0; y < r.height; y++)
            {
                for (int x = 0; x < r.width; x++)
                {
                    Color c = pixels[y * (int)r.width + x];
                    if (c.a <= 0f) continue;

                    int fx = startX + x;
                    int fy = startY + y;

                    if (fx < 0 || fx >= textureWidth || fy < 0 || fy >= textureHeight)
                        continue;

                    finalTex.SetPixel(fx, fy, c);
                }
            }

            // Calculate vertical step with overlap
            int overlap = Mathf.RoundToInt(r.height * overlapPercent);
            currentY += (int)r.height - overlap + spacing;
        }

        finalTex.Apply();

        // Create Sprite from the combined texture
        return Sprite.Create(
            finalTex,
            new Rect(0, 0, finalTex.width, finalTex.height),
            new Vector2(0.5f, 0.5f),
            100f // pixels per unit
        );
    }
}
