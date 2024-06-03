using UnityEngine;

namespace Es.InkPainter.Sample
{
    public class MousePainter : MonoBehaviour, ITextureBrushController
    {
        /// <summary>
        /// Types of methods used to paint.
        /// </summary>
        [System.Serializable]
        private enum UseMethodType
        {
            RaycastHitInfo,
            WorldPoint,
            NearestSurfacePoint,
            DirectUV
        }

        [SerializeField] private Brush brush;

        [SerializeField] private UseMethodType useMethodType = UseMethodType.RaycastHitInfo;

        [SerializeField] private bool erase = false;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) brush.ResetSpacingCalculation();
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var success = true;
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    var paintObject = hitInfo.transform.GetComponent<InkCanvas>();
                    if (paintObject != null)
                        switch (useMethodType)
                        {
                            case UseMethodType.RaycastHitInfo:
                                success = erase ? paintObject.Erase(brush, hitInfo) : paintObject.Paint(brush, hitInfo);
                                break;

                            case UseMethodType.WorldPoint:
                                success = erase
                                    ? paintObject.Erase(brush, hitInfo.point)
                                    : paintObject.Paint(brush, hitInfo.point);
                                break;

                            case UseMethodType.NearestSurfacePoint:
                                success = erase
                                    ? paintObject.EraseNearestTriangleSurface(brush, hitInfo.point)
                                    : paintObject.PaintNearestTriangleSurface(brush, hitInfo.point);
                                break;

                            case UseMethodType.DirectUV:
                                if (!(hitInfo.collider is MeshCollider))
                                    Debug.LogWarning("Raycast may be unexpected if you do not use MeshCollider.");
                                success = erase
                                    ? paintObject.EraseUVDirect(brush, hitInfo.textureCoord)
                                    : paintObject.PaintUVDirect(brush, hitInfo.textureCoord);
                                break;
                        }
                }
            }
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Reset"))
                foreach (var canvas in FindObjectsOfType<InkCanvas>())
                    canvas.ResetPaint();
        }

        public void SetBrushTexture(RenderTexture brushTexture)
        {
            brush.BrushTexture = brushTexture;
        }

    }
}