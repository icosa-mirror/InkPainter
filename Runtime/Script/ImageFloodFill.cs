using System.Collections.Generic;
using UnityEngine;

namespace Es.InkPainter
{
    public static class ImageFloodFill
    {
        public static void FillFromPoint(Texture texture, Color color, Vector2Int point, float threshold = 0f,
            Texture mask = null, float maskThreshold = 0.05f)
        {
            FillFromPoints(texture, color, new Vector2Int[] { point }, threshold, mask, maskThreshold);
        }

        public static void FillFromCorners(Texture texture, Color color, float threshold = 0f,
            Texture mask = null, float maskThreshold = 0.05f)
        {
            var points = new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(texture.width - 1, 0),
                new Vector2Int(0, texture.height - 1),
                new Vector2Int(texture.width - 1, texture.height - 1)
            };
            FillFromPoints(texture, color, points, threshold, mask, maskThreshold);
        }

        public static void FillFromPoints(Texture texture, Color color, Vector2Int[] points, float threshold = 0f,
            Texture mask = null, float maskThreshold = 0.05f)
        {
            Texture2D texture2d;
            if (texture is Texture2D)
            {
                texture2d = (Texture2D)texture;
            }
            else if (texture is RenderTexture)
            {
                texture2d = RenderTextureToTexture2D((RenderTexture)texture);
            }
            else
            {
                Debug.LogError("Unsupported texture type: " + texture.GetType());
                return;
            }

            Texture2D mask2d = null;
            if (mask != null)
            {
                if (mask is Texture2D)
                {
                    mask2d = (Texture2D)mask;
                }
                else if (mask is RenderTexture)
                {
                    mask2d = RenderTextureToTexture2D((RenderTexture)mask);
                }
                else
                {
                    Debug.LogError("Unsupported mask type: " + mask.GetType());
                    return;
                }
            }

            Color[] pixelsLinear = texture2d.GetPixels();
            int width = texture2d.width;
            int height = texture2d.height;

            Color[] maskLinear = null;
            if (mask2d != null)
            {
                maskLinear = mask2d.GetPixels();
            }

            foreach (Vector2Int point in points)
            {
                FillPixels(pixelsLinear, point, width, height, color, threshold, maskLinear, maskThreshold);
            }

            texture2d.SetPixels(pixelsLinear);
            texture2d.Apply();

            // Apply the result to the original texture
            if (texture is RenderTexture)
            {
                Texture2DToRenderTexture(texture2d, (RenderTexture)texture);
            }
        }

        static void FillPixels(Color[] pixels, Vector2Int startPoint, int width, int height, Color color,
            float threshold, Color[] maskPixels, float maskThreshold)
        {
            bool[] pixelsHandled = new bool[width * height];
            Color originColor = pixels[startPoint.y * width + startPoint.x];
            var stack = new Stack<Vector2Int>();
            stack.Push(startPoint);

            while (stack.Count > 0)
            {
                Vector2Int point = stack.Pop();
                int index = point.y * width + point.x;

                if (point.x >= 0 && point.x < width && point.y >= 0 && point.y < height && !pixelsHandled[index])
                {
                    pixelsHandled[index] = true;

                    float maskLuma = 1f;
                    if (maskPixels != null)
                    {
                        // TODO
                        // Not strictly the same as luma
                        maskLuma = maskPixels[index].grayscale;
                    }
                    if (ColorDistance(pixels[index], originColor) <= threshold && maskLuma >= maskThreshold)
                    {
                        pixels[index] = color;

                        stack.Push(new Vector2Int(point.x - 1, point.y));
                        stack.Push(new Vector2Int(point.x + 1, point.y));
                        stack.Push(new Vector2Int(point.x, point.y - 1));
                        stack.Push(new Vector2Int(point.x, point.y + 1));
                    }
                }
            }
        }

        static float ColorDistance(Color color1, Color color2)
        {
            return Mathf.Sqrt(
                Mathf.Pow(color1.r - color2.r, 2) +
                Mathf.Pow(color1.g - color2.g, 2) +
                Mathf.Pow(color1.b - color2.b, 2)
            );
        }

        static Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = currentRT;
            return texture;
        }

        static void Texture2DToRenderTexture(Texture2D texture, RenderTexture renderTexture)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Graphics.Blit(texture, renderTexture);
            RenderTexture.active = currentRT;
        }
    }
}
