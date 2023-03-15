using SVGImporter.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Hadi.Splines
{
    [ExecuteInEditMode]
    public class Spline : MonoBehaviour
    {
        [SerializeField]
        private SplineSettings splineSettings;

        [Header("Renderer") ,SerializeField]        
        private RendererSettings rendererSettings;
        private SplineRendererType rendererType;


        [Header("DEBUG")]
        [SerializeField]
        private bool drawGizmos = true;
        [SerializeField, Range(0.01f, 0.5f)]
        private float controlSize = 0.15f;
        [Range(0.01F, 0.5F), SerializeField]
        private float anchorSize = 0.075f;
        [SerializeField, Tooltip("Should the spline be reset when entering play mode?")] 
        private bool resetSplineOnPlay = false;
        [SerializeField, Tooltip("Should the transform be reset when the spline is reset?")] 
        private bool resetTransformOnSplineReset = true;
        [SerializeField]
        private SplineShapes shapeOnReset = SplineShapes.DefaultShape;
        [SerializeField]
        private SplineData splineData;

        private bool prevUseObjectTransform = false;        
        private Vector3 previousPosition;
        private Quaternion previousRotation;
        private Vector3 previousLocalScale;
        private ISplineRenderer splineRenderer;
        private RendererSettings currentSettings;
        private Element element;

        public float ANCHOR_SIZE { get => anchorSize; }
        public float CONTROL_SIZE { get => controlSize; }
        public SplineData SplineData { get => splineData; set => splineData = value; }
        public bool DrawGizmos { get => drawGizmos; }
        public bool UseObjectTransform { get => splineSettings.useObjectTransform; set => splineSettings.useObjectTransform = value; }
        public SplineMode SplineMode { get => splineSettings.splineMode; private set => splineSettings.splineMode = value; }
        public float Length { get => SplineData.Length; }
        public bool IsClosedSpline { get => splineSettings.closedSpline; set => splineSettings.closedSpline = value; }
        public SplineRendererType RendererType { get => rendererType; }
        public SplineSettings SplineSettings { get => splineSettings; set => splineSettings = value; }
        public RendererSettings RendererSettings { get => rendererSettings; set => rendererSettings = value; }

        private void Awake()
        {
            if(SplineData == null)
            {
                SplineData = new SplineData();
            }
            if(splineSettings == null)
            {
                splineSettings = new SplineSettings();
            }
            SplineData.objectTransform = transform;
            SplineData.useObjectTransform = splineSettings.useObjectTransform;
            splineRenderer = GetComponent<ISplineRenderer>();
            rendererType = (splineRenderer == null) ? SplineRendererType.None : splineRenderer.GetRendererType();
        }

        private void Start()
        {
            if (resetSplineOnPlay || splineData.Points.Count == 0)
                ResetSpline();
            else
                Refresh();
        }

        [ExecuteInEditMode]
        private void Update()
        {
#if UNITY_EDITOR
            CheckTransformChanged();
#endif
        }

        [ContextMenu("Add Point")]
        public void AddPoint()
        {
            Vector3 tangent, position;
            if (SplineData.Points.Count > 1)
            {
                Point lastPoint = SplineData.Points[SplineData.Points.Count - 1];
                tangent = lastPoint.relativeControlPoint2.normalized;
                position = lastPoint.anchor;
            }
            else
            {
                tangent = Vector3.right;
                position = Vector3.zero;
            }
            Vector3 newPointPosition = transform.TransformSplinePoint( position + tangent * splineSettings.newPointDistance, UseObjectTransform);
            Point newPoint = new Point(transform.InverseTransformSplinePoint(newPointPosition, UseObjectTransform), -tangent);
            newPoint.SetAutoRotation();
            SplineData.Points.Add(newPoint);
            GenerateSpline();
        }

        public void AddPointAtIndex(int index)
        {
            // Get point position by interpolating between point at index, index+1
            // Same for tangents, lerp, but give them half the magnitude
            // Tangents of the original points should be halved as well?
        }

        public void ResetSpline()
        {
            SplineData.Points.Clear();
            SplineData.Points = SplineShapesUtility.CreateShape(shapeOnReset, IsClosedSpline);

            if(resetTransformOnSplineReset)
            {
                transform.SetPositionAndRotation(Vector2.zero, Quaternion.identity);
                transform.localScale = Vector3.one;
            }
            Refresh();
        }

        [ContextMenu("Setup Renderer")]
        protected void SetRendererType()
        {
            if (rendererSettings == null)
                Debug.LogWarning($"No settings for spline renderer @{gameObject.name}!");
            else
            {
                splineRenderer = RendererSettings.GetRenderer(rendererSettings, gameObject, splineData);
                rendererType = splineRenderer.GetRendererType();
            }
        }

        /// <summary>
        /// Setup the spline based on the curves list.
        /// </summary>
        [ContextMenu("Generate")]
        public virtual void GenerateSpline()
        {
            int pointsCount = SplineData.Points.Count;
            if (pointsCount < 2)
            {
                Debug.LogError("Cannot create a spline without at least 2 points! " + gameObject.name);
                return;
            }
            SavePreviousTransform();

            SplineData.Clear();
            SplineData.settings = currentSettings;
            splineRenderer?.Clear();
            for (int i = 0; i < pointsCount - 1; i++)
            {
                CalculateCurve(SplineData.Points[i], SplineData.Points[i + 1], i);
            }
            if (splineSettings.closedSpline)
            {
                splineRenderer?.SetClosedSpline(true);
                CloseSpline();
            }
            else
                splineRenderer?.SetClosedSpline(false);

            SplineData.CalculateLength();
            SplineData.numPoints = pointsCount;
            if(splineRenderer != null)
                splineRenderer?.SetData(SplineData);
        }

        private void SavePreviousTransform()
        {
            previousPosition = transform.position;
            previousRotation = transform.rotation;
            previousLocalScale = transform.localScale;
        }

        /// <summary>
        /// Close the spline by connecting the last point with the first point.
        /// </summary>
        protected virtual void CloseSpline()
        {
            if (SplineData.Points.Count < 2)
            {
                Debug.LogError("Cannot close a spline with less than two points! " + gameObject.name);
                splineSettings.closedSpline = false;
                return;
            }
            CalculateCurve(SplineData.Points[SplineData.Points.Count - 1], SplineData.Points[0], SplineData.Points.Count - 1, true);
        }

        /// <summary>
        /// Calculates a cubic bezier curve between two points using polynomial coefficients, split into segments, and saves the results in <code>SplineData.</code>
        /// </summary>
        /// <param name="P1">First point.</param>
        /// <param name="P2">Second point.</param>
        /// <param name="index">Index of the curve in the spline.</param>
        protected virtual void CalculateCurve(Point P1, Point P2, int index, bool isClosingCurve = false)
        {
            int totalSegments = splineSettings.segmentsPerCurve;// * POINT_COUNT_PER_CURVE;

            // By caching these, we reduce the amount of computations needed. Same result as DeCasteljau's, but more efficient.
            // See `The Continuity of Splines` by Freya Holmer, @6:10
            Vector3 P1anchor = transform.TransformSplinePoint(P1.anchor, UseObjectTransform),
                    P1controlPoint1 = transform.TransformSplinePoint(P1.GetControlPoint1(), UseObjectTransform), 
                    P1controlPoint2 = transform.TransformSplinePoint(P1.GetControlPoint2(), UseObjectTransform);
            Vector3 P2anchor = transform.TransformSplinePoint(P2.anchor, UseObjectTransform),
                    P2controlPoint1 = transform.TransformSplinePoint(P2.GetControlPoint1(), UseObjectTransform), 
                    P2controlPoint2 = transform.TransformSplinePoint(P1.GetControlPoint2(), UseObjectTransform);

            Vector3 factor0 = P1anchor;
            Vector3 factor1 = -3 * P1anchor + 3 * P1controlPoint2;
            Vector3 factor2 = 3 * P1anchor - 6 * P1controlPoint2 + 3 *P2controlPoint1;
            Vector3 factor3 = -P1anchor + 3 * P1controlPoint2 - 3 * P2controlPoint1 + P2anchor;

            int start = index * splineSettings.segmentsPerCurve, end = start + totalSegments;
            for (int i = start; i < end; i++)
            {
                float t = ((float)i % totalSegments) / (end - start);
                CalculatePoint(P1, P2, factor0, factor1, factor2, factor3, t);
            }
            // t = 1
            if (!isClosingCurve && index == SplineData.Points.Count - 2)
            {
                CalculatePoint(P1, P2, factor0, factor1, factor2, factor3, 1);
            }
            if (isClosingCurve)
            { 
                CalculatePoint(P1, P2, factor0, factor1, factor2, factor3, 1);
            }
        }

        /// <summary>
        /// Calculate SplineData between points P1 and P2 at t-percent.
        /// </summary>
        /// <param name="P1">First point</param>
        /// <param name="P2">Second point</param>
        /// <param name="factor0">First factor in a polynomial representation of the curve (*1).</param>
        /// <param name="factor1">Second factor in a polynomial representation of the curve (*t).</param>
        /// <param name="factor2">Third factor in a polynomial representation of the curve (*t^2).</param>
        /// <param name="factor3">Fourth factor in a polynomial representation of the curve (*t^3).</param>
        /// <param name="t">Percentage along the curve.</param>
        private void CalculatePoint(Point P1, Point P2, Vector3 factor0, Vector3 factor1, Vector3 factor2, Vector3 factor3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            Vector3 P = factor0 + t * factor1 + t2 * factor2 + t3 * factor3;
            SplineData.SegmentedPoints.Add(P);
            P = factor1 + 2 * t * factor2 + 3 * t2 * factor3;
            SplineData.Tangents.Add(P);
            if(P1.relativeControlPoint2 != Vector3.zero && P2.relativeControlPoint1 != Vector3.zero)
                P = Quaternion.Slerp(P1.rotation, P2.rotation, t) * Vector3.up;
            else 
                P = Quaternion.Slerp(P1.rotation, P2.rotation, 0.5f) * Vector3.up;
            SplineData.Normals.Add(P.normalized);
        }

        /// <summary>
        /// Calculates a cubic bezier using DeCasteljau's method
        /// </summary>
        /// <param name="p0">Control Point of first anchor</param>
        /// <param name="p1">Anchor 1</param>
        /// <param name="p2">Control Point of second anchor</param>
        /// <param name="p3">Anchor 2</param>
        /// <param name="t">Time</param>
        /// <returns></returns>
        protected virtual Vector3 DeCasteljau(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 a, b, c, d, e;
            a = Vector3.Lerp(p0, p1, t);
            b = Vector3.Lerp(p1, p2, t);
            c = Vector3.Lerp(p2, p3, t);
            d = Vector3.Lerp(a, b, t);
            e = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(d, e, t);
        }


        public List<Point> GetPoints()
        {
            return SplineData.Points;
        }

        /// <summary>
        /// Get a position along the spline.
        /// </summary>
        /// <param name="t">A number between 0 and 1 representing the location along the spline.</param>
        /// <returns></returns>
        public Vector3 GetPosition(float t)
        {
            SplineSegment segment = GetSplineSegment(t);
            return SplineUtility.LerpPosition(SplineData, segment);
        }

        /// <summary>
        /// Get the normal at a position along the spline.
        /// </summary>
        /// <param name="t">A number between 0 and 1 representing the location along the spline.</param>
        /// <returns></returns>
        public Vector3 GetNormal(float t)
        {
            SplineSegment segment = GetSplineSegment(t);
            return SplineUtility.LerpNormal(SplineData, segment);
        }



        /// <summary>
        /// Get the tangent direction along the spline (normalized).
        /// </summary>
        /// <param name="t">A number between 0 and 1 representing the location along the spline.</param>
        /// <returns></returns>
        public Vector3 GetTangent(float t)
        {
            return GetVelocity(t).normalized;
        }
        /// <summary>
        /// Get the curve velocity along the spline (the tangent).
        /// </summary>
        /// <param name="t">A number between 0 and 1 representing the location along the spline.</param>
        /// <returns></returns>
        public Vector3 GetVelocity(float t)
        {
            SplineSegment segment = GetSplineSegment(t);
            return SplineUtility.LerpVelocity(SplineData, segment);
        }

        /// <summary>
        /// Gets all the data at time t.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>SplineDataAtPoint that contains position, normal, tangent and velocity data.</returns>
        public SplineDataAtPoint GetDataAtPoint(float t)
        {
            SplineSegment segment = GetSplineSegment(t);

            bool looped = splineSettings.closedSpline && segment.pointIndex >= splineData.SegmentedPoints.Count - 1;
            //print($"index: {segment.pointIndex} / {splineData.Points.Count}  looped={looped}");
            SplineDataAtPoint data = SplineUtility.GetDataAtSegment(SplineData, segment, looped);

            return data;
        }

        /// <summary>
        /// Get the appropriate spline segment (index and time) for a percentage along the spline.
        /// </summary>
        /// <param name="percentageAlongSpline"></param>
        /// <returns></returns>
        private SplineSegment GetSplineSegment(float percentageAlongSpline)
        {
            percentageAlongSpline = CalculateEndOfSplineInstruction(percentageAlongSpline);
            int numSegments = splineData.SegmentedPoints.Count;
            float percentPerSegment = 1f / numSegments;
            int segmentIndex = Mathf.FloorToInt(percentageAlongSpline * numSegments);
            float t = (percentageAlongSpline - segmentIndex * percentPerSegment) * numSegments;
            return new SplineSegment(t, segmentIndex);
        }

        private float CalculateEndOfSplineInstruction(float value, float maxValue = 1f)
        {
            if (value < maxValue) value += maxValue;
            switch (splineSettings.EndOfSplineInstruction)
            {
                case EndOfSplineInstruction.End:
                    if (value > maxValue) value = maxValue;
                    break;
                case EndOfSplineInstruction.Loop:
                    value = Mathf.Repeat(value, maxValue);
                    break;
                case EndOfSplineInstruction.PingPong:
                    value = Mathf.PingPong(value, maxValue);
                    break;
                default:
                    break;
            }

            return value;
        }

        internal SplineDataAtPoint GetDataAtDistance(float distance, int index = 0, bool moveForwards = false)
        {
            float maxDistance = splineData.Length, initialDistance = distance;
            float t = 0;
            int count = splineData.CumulativeLengthAtPoint.Count;
            bool found = false;
            distance = CalculateEndOfSplineInstruction(distance, maxDistance);
            int numSegments = splineData.SegmentedPoints.Count + (splineSettings.closedSpline ? 1 : 0);
            float percentPerSegment = 1f / numSegments;
            if(distance == 0 || distance < splineData.CumulativeLengthAtPoint[0]) return GetDataAtPoint(0);         
            int i = index;
            while (!found)
            {
                if (splineData.CumulativeLengthAtPoint[i] < distance && splineData.CumulativeLengthAtPoint[i + 1] > distance)
                {
                    float currentDistance = splineData.CumulativeLengthAtPoint[i + 1] - splineData.CumulativeLengthAtPoint[i];
                    float localT = ((distance - splineData.CumulativeLengthAtPoint[i]) / currentDistance);
                    t = ((float)i / count) + Mathf.Lerp(0, percentPerSegment, localT);
                    found = true;
                }
                if (found) i = count;
                if (moveForwards)
                    i = (i + 1) % (count - 1);
                else
                {

                    i--;
                    if (i == -1) i = count - 1;
                }
            }


            return GetDataAtPoint(t);
        }

        private void OnDrawGizmos()
        {
            if (SplineData.Points.Count == 0) ResetSpline();
            CheckTransformChanged();
        }

        private void CheckTransformChanged()
        {
            if (transform.position != previousPosition || transform.rotation != previousRotation || transform.localScale != previousLocalScale)
            {                
                GenerateSpline();
            }
        }

        public Vector3 GetPointTangent(int value)
        {
            return GetTangent(((float)value) / SplineData.Points.Count);
        }

        public void ResetPointRotations()
        {
            SetPointRotations(Quaternion.identity);
        }
        public void SetPointRotations(Quaternion rotation)
        {
            foreach (Point point in SplineData.Points)
            {
                point.rotation = rotation;
            }
            GenerateSpline();
        }

        public void Refresh()
        {
            splineData.settings = rendererSettings;
            splineData.useObjectTransform = splineSettings.useObjectTransform;
            SetRendererType();
            GenerateSpline();
        }
        protected void OnValidate()
        {
            if (SplineData == null)
            {
                SplineData = new SplineData();
                ResetSpline();
            }
            if (SplineData.Points.Count == 0) return;
            if (splineRenderer == null) splineRenderer = GetComponent<ISplineRenderer>();
            foreach (Point point in SplineData.Points)
            {
                point.Refresh(SplineMode);
                //if (splineSettings.automaticPointRotations)
                //    point.SetAutoRotation(); 
            }
            bool generate = true;
            if (currentSettings != rendererSettings || splineRenderer == null)
            {
                currentSettings = rendererSettings;
                generate = false;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (rendererSettings != null && rendererSettings.RendererType != RendererType)
                    {
                        splineRenderer?.Destroy();
                        splineRenderer = null;
                    }
                    if (currentSettings != null)
                    {
                        Refresh();
                    }

                };
#endif
            }
            if (prevUseObjectTransform != splineSettings.useObjectTransform)
            {
                prevUseObjectTransform = splineSettings.useObjectTransform;
                SplineData.useObjectTransform = splineSettings.useObjectTransform;
            }
#if UNITY_EDITOR
            if(generate)
                GenerateSpline();
#endif
        }

        internal void SetByElement(Element element, List<Point> points, RendererSettings defaultRendererSettings)
        {
            this.element = element;
            splineData.Points = points;
            rendererSettings = defaultRendererSettings;
            currentSettings = rendererSettings;
            splineData.settings = rendererSettings;
            if (points.Count < 2)
                ResetSpline();
            else
                Refresh();
        }
    }
}
