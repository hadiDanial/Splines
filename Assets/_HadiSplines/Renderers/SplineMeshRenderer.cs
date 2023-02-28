using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        private bool isClosed;
        Vector3 origin;
        [Header("DEBUG")]
        [SerializeField] private bool drawVertexIndices = false;
        private Material material;

        private float currentRadius;
        private int currentVerticalResolution;

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
            mesh.uv = uvs;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
        }

        public void SetData(SplineData splineData)
        {
            this.splineData = splineData;
            origin = (splineData.useWorldSpace ? splineData.objectTransform.position : Vector3.zero);
            currentRadius = radius;
            currentVerticalResolution = verticalResolution;
            GenerateMesh();
        }

        private void GenerateMesh(bool onlyGenerateVertices = false)
        {
            int numPoints = splineData.Points.Count * currentVerticalResolution;
            GenerateVertices(numPoints);
            if (!onlyGenerateVertices)
            {
                GenerateTriangles(numPoints);

            }
            InitializeMesh();
        }

        private void GenerateVertices(int numPoints)
        {
            vertices = new Vector3[numPoints];
            float angle;
            float angleDelta = 360f / currentVerticalResolution;
            float t;
            for (int i = 0; i < splineData.Points.Count; i++)
            {
                angle = 0;
                t = (float)i / splineData.Points.Count;
                for (int j = 0; j < currentVerticalResolution; j++)
                {
                    Vector3 pos;
                    Quaternion rot = Quaternion.AngleAxis(angle, splineData.Tangents[i]);
                    pos = (rot * splineData.Normals[i]).normalized;
                    if (useAnimationCurveForRadius)
                        pos *= radiusOverSpline.Evaluate(t) * currentRadius;
                    else pos *= currentRadius;
                    pos = pos + splineData.Points[i];
                    vertices[i * currentVerticalResolution + j] = splineData.useWorldSpace ? pos : transform.TransformPoint(pos);
                    angle += (angleDelta);
                }
            }
        }

        private void GenerateTriangles(int numPoints)
        {
            triangles = new int[(numPoints - 1) * 6];
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
            if (currentRadius != radius)
            {
                currentRadius = radius;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    GenerateMesh();
                };
#endif
            }
            else if (currentVerticalResolution != verticalResolution)
            {
                currentVerticalResolution = verticalResolution;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    GenerateMesh();
                };
#endif
            }
        }

        private void OnDrawGizmos()
        {
            if (vertices == null || vertices.Length == 0) return;
            Gizmos.color = Color.red;
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(transform.TransformSplinePoint(vertices[i], splineData.useWorldSpace), 0.01f);
                if (drawVertexIndices)
                    Handles.Label(transform.TransformSplinePoint(vertices[i], splineData.useWorldSpace) + Vector3.right * 0.05f, i + "", style);
            }

            Gizmos.color = Color.black;
            if (triangles != null)
            {
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i]], splineData.useWorldSpace),
                                    transform.TransformSplinePoint(vertices[triangles[i + 1]], splineData.useWorldSpace));
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i + 1]], splineData.useWorldSpace),
                                    transform.TransformSplinePoint(vertices[triangles[i + 2]], splineData.useWorldSpace));
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i]], splineData.useWorldSpace),
                                    transform.TransformSplinePoint(vertices[triangles[i + 2]], splineData.useWorldSpace));
                }
            }
        }
    }

}
