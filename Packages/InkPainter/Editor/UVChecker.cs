using System.Text;
using UnityEditor;
using UnityEngine;

namespace Es.Editor.Window
{
    /// <summary>
    /// Editor window to check UV.
    /// </summary>
    public class UVChecker : EditorWindow
    {
        private GameObject targetGameObject;
        private MeshFilter targetMeshFilter;
        private Texture2D tex;

        [MenuItem("Window/InkPainter/UVChecker")]
        private static void Open()
        {
            GetWindow<UVChecker>();
        }

        private void OnGUI()
        {
            targetGameObject =
                EditorGUILayout.ObjectField("TargetMesh", targetGameObject, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("Execute"))
            {
                targetMeshFilter = targetGameObject.GetComponent<MeshFilter>();
                tex = new Texture2D(256, 256);
                var mesh = targetMeshFilter.sharedMesh;
                DrawUV(mesh);
            }

            if (tex != null) EditorGUI.DrawPreviewTexture(new Rect(10, 50, tex.width, tex.height), tex);
        }

        private void DrawUV(Mesh mesh)
        {
            var uvs = mesh.uv;
            var tri = mesh.triangles;

            for (var i_base = 0; i_base < tri.Length; i_base += 3)
            {
                var i_1 = i_base;
                var i_2 = i_base + 1;
                var i_3 = i_base + 2;

                var uv1 = uvs[tri[i_1]];
                var uv2 = uvs[tri[i_2]];
                var uv3 = uvs[tri[i_3]];

                DrawLine(uv1, uv2);
                DrawLine(uv2, uv3);
                DrawLine(uv3, uv1);
            }

            tex.Apply(false);

            UVLog(uvs);
        }

        private void UVLog(Vector2[] uvs)
        {
            var sb = new StringBuilder();
            foreach (var uv in uvs) sb.AppendLine(uv.ToString());

            Debug.Log(sb.ToString());
        }

        private void DrawLine(Vector2 from, Vector2 to)
        {
            var x0 = Mathf.RoundToInt(from.x * tex.width);
            var y0 = Mathf.RoundToInt(from.y * tex.height);
            var x1 = Mathf.RoundToInt(to.x * tex.width);
            var y1 = Mathf.RoundToInt(to.y * tex.height);

            DrawLine(x0, y0, x1, y1, Color.red);
        }

        private void DrawLine(int x0, int y0, int x1, int y1, Color col)
        {
            var dy = y1 - y0;
            var dx = x1 - x0;
            int stepx, stepy;

            if (dy < 0)
            {
                dy = -dy;
                stepy = -1;
            }
            else
            {
                stepy = 1;
            }

            if (dx < 0)
            {
                dx = -dx;
                stepx = -1;
            }
            else
            {
                stepx = 1;
            }

            dy <<= 1;
            dx <<= 1;

            float fraction = 0;

            tex.SetPixel(x0, y0, col);
            if (dx > dy)
            {
                fraction = dy - (dx >> 1);
                while (Mathf.Abs(x0 - x1) > 1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }

                    x0 += stepx;
                    fraction += dy;
                    tex.SetPixel(x0, y0, col);
                }
            }
            else
            {
                fraction = dx - (dy >> 1);
                while (Mathf.Abs(y0 - y1) > 1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }

                    y0 += stepy;
                    fraction += dx;
                    tex.SetPixel(x0, y0, col);
                }
            }
        }
    }
}