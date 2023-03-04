using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [CreateAssetMenu(fileName = "Line Renderer Settings", menuName = "Splines/Renderers/Line Settings")]
    public class SplineLineRendererSettings : RendererSettings
    {
        public bool useWidthCurve = true;
        // TODO: Custom editor, hide start/end width and show curve or vice versa based on bool
        [SerializeField, Range(0, 2)]
        private float startWidth = 0.25f;
        [SerializeField, Range(0, 2)]
        private float endWidth = 0.25f;
        [SerializeField]
        private AnimationCurve widthCurve = AnimationCurve.Constant(0, 1, 1);
        [SerializeField]
        private Color startColor = Color.white;
        [SerializeField]
        private Color endColor = Color.white;
        [SerializeField]
        private LineAlignment lineAlignment = LineAlignment.View;

        public float StartWidth { get => startWidth; }
        public float EndWidth { get => endWidth;}
        public AnimationCurve WidthCurve { get => widthCurve; }
        public Color StartColor { get => startColor; }
        public Color EndColor { get => endColor;  }
        public LineAlignment LineAlignment { get => lineAlignment; }

        protected override void SetRendererType()
        {
            rendererType = SplineRendererType.LineRenderer;
        }        
    }
}
