using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hadi.Splines
{
    [ExecuteInEditMode]
    public abstract class SplineMeshRenderer : MonoBehaviour, ISplineRenderer
    {
        [Header("Spline Mesh Renderer")]
        [SerializeField, Range(3, 30)]
        // TODO: Move curves, resolution to settings
        protected int meshResolution = 10;
        [SerializeField]
        protected bool useAnimationCurve = true;
        [SerializeField]
        protected AnimationCurve widthOverSplineAnimationCurve = AnimationCurve.Constant(0, 1, 1);
        [SerializeField]
        protected AnimationCurve heightOverSplineAnimationCurve = AnimationCurve.Constant(0, 1, 1);
        [SerializeField]
        protected bool capSides = true;

        protected SplineData splineData;

        [SerializeField]
        protected Vector3[] vertices;
        [SerializeField]
        protected int[] triangles;
        [SerializeField] 
        protected Vector3[] normals;
        [SerializeField]
        protected Vector2[] uvs;

        [Header("DEBUG")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private bool drawVertexIndices;
        [SerializeField] private bool drawNormals;
        protected RendererSettings rendererSettings;

        protected bool isClosed;
        protected int currentMeshResolution;

        protected Mesh mesh;
        protected MeshFilter meshFilter;
        protected MeshRenderer meshRenderer;
        protected AnimationCurve previousWidthOverSpline, previousHeightOverSpline;

#if UNITY_EDITOR
        private bool editorDelay;
#endif
        

        private void Awake()
        {
            SetupMesh();
            InitializeMesh();
        }

        public void Setup()
        {
            InitializeMesh();            
        }

        protected abstract void SetSettings(RendererSettings settings);


        /// <summary>
        /// Setup the mesh - Add/Get MeshFilter and MeshRenderer components, and setup a new mesh.
        /// </summary>
        private void SetupMesh()
        {
            mesh = new Mesh();
            mesh.name = GetDefaultName();
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        /// <summary>
        /// Initialize the mesh with the generated vertices, triangles and UVs.
        /// </summary>
        protected virtual void InitializeMesh()
        {
            if (mesh == null)
                SetupMesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateUVDistributionMetrics();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => { if(meshFilter != null) meshFilter.mesh = mesh; };
#else
            meshFilter.mesh = mesh;
#endif
            }
      

        public virtual void SetData(SplineData splineData)
        {
            this.splineData = splineData;
            rendererSettings = splineData.settings;
            SetSettings(splineData.settings);
            currentMeshResolution = meshResolution;
            if (meshRenderer != null)
                meshRenderer.sharedMaterial = rendererSettings?.Material;
            GenerateMesh();
        }

        /// <summary>
        /// Generate the mesh
        /// </summary>
        protected virtual void GenerateMesh()
        {
            int numPoints = GetVertexCount();
            GenerateVertices(numPoints);
            GenerateTriangles(GetTriangleCount(numPoints));
            GenerateUVs();

            InitializeMesh();
        }

        protected virtual int GetVertexCount()
        {
            return splineData.SegmentedPoints.Count * currentMeshResolution;
        }

        /// <summary>
        /// Generate the mesh vertices
        /// </summary>
        /// <param name="numPoints">Number of vertices</param>
        protected virtual void GenerateVertices(int numPoints)
        {
            vertices = new Vector3[numPoints];
            normals = new Vector3[numPoints];
        }

        /// <summary>
        /// Generate the triangles that define the faces of the mesh.
        /// </summary>
        /// <param name="numPoints">Number of triangle points</param>
        protected virtual void GenerateTriangles(int numPoints)
        {
            triangles = new int[numPoints];
        }

        /// <summary>
        /// Calculate the length of the triangles array.
        /// </summary>
        /// <param name="numPoints">Number of vertices</param>
        /// <returns></returns>
        protected abstract int GetTriangleCount(int numPoints);

        /// <summary>
        /// Generate UVs
        /// </summary>
        protected virtual void GenerateUVs()
        {
            uvs = new Vector2[vertices.Length];           
        }

        /// <summary>
        /// Default Mesh name
        /// </summary>
        protected abstract string GetDefaultName();


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

        public SplineRendererType GetRendererType()
        {
            return SplineRendererType.TubeMeshRenderer;
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


        protected virtual void OnValidate()
        {
            if (!widthOverSplineAnimationCurve.Equals(previousWidthOverSpline))
            {
                previousWidthOverSpline = widthOverSplineAnimationCurve;
            }
            if (!heightOverSplineAnimationCurve.Equals(previousHeightOverSpline))
            {
                previousHeightOverSpline = heightOverSplineAnimationCurve;
            }            
            if (currentMeshResolution != meshResolution)
            {
                currentMeshResolution = meshResolution;
            }
            if(splineData != null && splineData.Spline != null)
                splineData.Spline.GenerateSpline();
#if UNITY_EDITOR
            if (!editorDelay)
            {
                editorDelay = true;
                UnityEditor.EditorApplication.delayCall += ClearAndRegenerate;
            }
#endif
        }

        private void ClearAndRegenerate()
        {
            Clear();
            
            GenerateMesh();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if(editorDelay)
                UnityEditor.EditorApplication.delayCall -=  ClearAndRegenerate;
#endif
        }
#if UNITY_EDITOR
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
                Gizmos.color = Color.red;
                Vector3 transformSplinePoint = transform.TransformSplinePoint(vertices[i], splineData.useObjectTransform);
                Gizmos.DrawSphere(transformSplinePoint, 0.01f);
                if (drawVertexIndices)
                    Handles.Label(transformSplinePoint + Vector3.one * 0.005f, i + "", style);
                Gizmos.color = Color.white;
                if(drawNormals)
                    Gizmos.DrawLine(transformSplinePoint, transformSplinePoint + normals[i] * 0.1f);
            }

            Gizmos.color = Color.black;
            if (triangles != null)
            {
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i]], splineData.UseObjectTransform),
                                    transform.TransformSplinePoint(vertices[triangles[i + 1]], splineData.UseObjectTransform));
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i + 1]], splineData.UseObjectTransform),
                                    transform.TransformSplinePoint(vertices[triangles[i + 2]], splineData.UseObjectTransform));
                    Gizmos.DrawLine(transform.TransformSplinePoint(vertices[triangles[i]], splineData.UseObjectTransform),
                                    transform.TransformSplinePoint(vertices[triangles[i + 2]], splineData.UseObjectTransform));
                }
            }
        }
  #endif
  }

}
