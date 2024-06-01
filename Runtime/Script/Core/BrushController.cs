using System;
using UnityEngine;

namespace Es.InkPainter
{
    public abstract class BrushController : MonoBehaviour
    {
        public abstract void SetBrushTexture(RenderTexture brushTexture);
    }
}