using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hadi.Splines
{
    [ExecuteInEditMode]
    public class SplineMeshRenderer : MonoBehaviour, ISplineRenderer
    {
        [SerializeField, Range(3, 50)]
        private int verticalResolution = 10;
        [SerializeField, Range(0.01f, 3f)]
        private float radius = 0.25f;
        [SerializeField]
        private bool useAnimationCurveForRadius = true;
        [SerializeField]
        private AnimationCurve radiusOverSpline = AnimationCurve.Constant(0, 1, 1);
        [SerializeField]
        private bool capSides = true;
        [SerializeField]
        private UVGenerationType UVGenerationType = UVGenerationType.Segment;
        private Mesh mesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private SplineData splineData;
        [SerializeField]
        private int[] triangles;
        [SerializeField]
        private Vector3[] vertices;
        [SerializeField]
        private Vector2[] uvs;

        [Header("DEBUG")]
        [SerializeField] private bool drawVertexIndices = false;
        [SerializeField] private bool drawGizmos;
        private Material material;

        private float currentRadius;
        private int currentVerticalResolution;
        private bool isClosed;
        private object previousRadiusOverSpline;
        private const float defaultScaleMagnitude = 1.73205080757f; // Sqrt(3)
        private void Awake()
        {
            SetupMesh();
            InitializeMesh();
        }

        private void SetupMesh()
        {
            mesh = new Mesh();
            mesh.name = "Spline";
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        public void Setup(Material material)
        {
            this.material = material;
            InitializeMesh();
            meshRenderer.sharedMaterial = material;
        }

        private void InitializeMesh()
        {
            if (mesh == null)
                SetupMesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateUVDistributionMetrics();
            meshFilter.mesh = mesh;
        }

        public void SetData(SplineData splineData)
        {
            this.splineData = splineData;
            currentRadius = radius;
            currentVerticalResolution = verticalResolution;
            GenerateMesh();
        }

        private void GenerateMesh(bool onlyGenerateVertices = false)
        {
            int numPoints = splineData.Points.Count * currentVerticalResolution;
            GenerateVertices(numPoints);
            GenerateTriangles(numPoints - currentVerticalResolution);
            GenerateUVs();
            
            InitializeMesh();
        }

        private void GenerateUVs()
        {
            uvs = new Vector2[vertices.Length];
            for (int j = 0; j < currentVerticalResolution; j++)
            {
                float verticalT = ((float)j / (currentVerticalResolution - 1));
                for (int i = 0; i < splineData.Points.Count; i++)
                {
                    float horizontalT, t = ((float)i / (splineData.Points.Count - 1)); ;
                    switch (UVGenerationType)
                    {
                        case UVGenerationType.Mesh:
                            {
                                horizontalT = t;
                                break;
                            }
                        case UVGenerationType.Segment:
                            {                                
                                float initT = t * splineData.Points.Count;
                                int segmentIndex = Mathf.FloorToInt(initT);
                                float nextT = (segmentIndex + 1f) / splineData.Points.Count;
                                horizontalT = (t - initT) / (nextT - initT);
                                break;
                            }
                        case UVGenerationType.Length:
                            {
                                horizontalT = t;
                                break;
                            }
                        default:
                            horizontalT = t;
                            break;
                    }
                    uvs[i * currentVerticalResolution + j] = new Vector2(horizontalT, verticalT);
                }
            }
        
        }

        private void GenerateVertices(int numPoints)
        {
            vertices = new Vector3[numPoints];
            float angle;
            float angleDelta = 360f / currentVerticalResolution;
            float t;
            float radiusValue = (splineData.useObjectTransform ? (currentRadius * defaultScaleMagnitude)/ splineData.objectTransform.localScale.magnitude : currentRadius);
            bool useObjectSpace = splineData.useObjectTransform;
            for (int i = 0; i < splineData.Points.Count; i++)
            {
                bool noRadius = false;
                if(capSides && (i == 0 || i == splineData.Points.Count - 1))
                {
                    noRadius = true;
                }
                angle = 0;
                t = (float)i / splineData.Points.Count;
                for (int j = 0; j < currentVerticalResolution; j++)
                {
                    Vector3 pos;
                    Quaternion rot = Quaternion.AngleAxis(angle, splineData.Tangents[i]);
                    rot = splineData.useObjectTransform ? transform.rotation * rot : rot;
                    if (noRadius)
                        pos = Vector3.zero;
                    else
                    {

                        pos = (rot * splineData.Normals[i]).normalized;
                        if (useAnimationCurveForRadius)
                        {
                            pos *= radiusOverSpline.Evaluate(t) * currentRadius;
                        }
                        else pos *= currentRadius;
                    }
                    pos = transform.InverseTransformSplinePoint(pos + splineData.Points[i], useObjectSpace);
                    vertices[i * currentVerticalResolution + j] = pos;
                    angle += (angleDelta);
                }
            }
        }

        private void GenerateTriangles(int numPoints)
        {
            triangles = new int[numPoints * 6];
            for (int i = 0, triangleIndex = 0; i < splineData.Points.Count - 1; i++)
            {
                int vertexIndex, startIndex;
                for (int j = 0; j < currentVerticalResolution - 1; j++, triangleIndex += 6)
                {
                    vertexIndex = i * currentVerticalResolution + j;
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + currentVerticalResolution;
                    triangles[triangleIndex + 3] = triangles[triangleIndex + 2];
                    triangles[triangleIndex + 4] = triangles[triangleIndex + 1];
                    triangles[triangleIndex + 5] = triangles[triangleIndex + 3] + 1;
                }
                // Last index connects back to start
                vertexIndex = i * currentVerticalResolution + currentVerticalResolution - 1;
                startIndex = i * currentVerticalResolution;
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = startIndex;
                triangles[triangleIndex + 2] = vertexIndex + currentVerticalResolution;
                triangles[triangleIndex + 3] = triangles[triangleIndex + 2];
                triangles[triangleIndex + 4] = triangles[triangleIndex + 1];
                triangles[triangleIndex + 5] = startIndex + currentVerticalResolution;
                triangleIndex += 6;
            }
        }

        public void SetClosedSpline(bool closed)
        {
            this.isClosed = closed;
        }

        public void SetFill(bool isFilled = true)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            if(mesh != null)
                mesh.Clear();
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (meshFilter != null)
                    DestroyImmediate(meshFilter);
                if (meshRenderer != null)
                    DestroyImmediate(meshRenderer);
                DestroyImmediate(this);
            };
#endif
        }
        public SplineRendererType GetRendererType()
        {
            return SplineRendererType.MeshRenderer;
        }

        private void OnValidate()
        {
            if (!radiusOverSpline.Equals(previousRadiusOverSpline))
            {
                previousRadiusOverSpline = radiusOverSpline;
            }
            if (currentRadius != radius)
            {
                currentRadius = radius;
            }
            if (currentVerticalResolution != verticalResolution)
            {
                currentVerticalResolution = verticalResolution;

            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                Clear();
                GenerateMesh();
            };
#endif
        }

        private void OnDrawGizmos()
        {
            if (vertices == null || vertices.Length == 0) return;
            if (!drawGizmos) return;
            Gizmos.color = Color.red;
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(transform.TransformSplinePoint(vertices[i], splineData.useObjectTransform), 0.01f);
                if (drawVertexIndices)
                    Handles.Label(transform.TransformSplinePoint(vertices[i], splineData.useObjectTransform) + Vector3.right * 0.05f, i + "", style);
            }

            Gizmos.color = Color.black;
            if (triangles != null)
            {
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i]], splineData.useObjectTransform),
                                    transform.TransformSplinePoint(vertices[triangles[i + 1]], splineData.useObjectTransform));
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i + 1]], splineData.useObjectTransform),
                                    transform.TransformSplinePoint(vertices[triangles[i + 2]], splineData.useObjectTransform));
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i]], splineData.useObjectTransform),
                                    transform.TransformSplinePoint(vertices[triangles[i + 2]], splineData.useObjectTransform));
                }
            }
        }
    }

}
