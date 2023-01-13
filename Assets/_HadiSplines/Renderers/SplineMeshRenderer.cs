using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SplineMeshRenderer : MonoBehaviour, ISplineRenderer
    {
        [SerializeField, Range(3,50)]
        private int verticalResolution = 10;
        [SerializeField, Range(0.01f, 3f)] 
        private float radius = 0.25f;

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

        private void Awake()
        {
            mesh = new Mesh();
            mesh.name = "Spline";
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter= GetComponent<MeshFilter>();
            Setup();
        }

        public void Setup()
        {
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
            int numPoints = splineData.SegmentedPoints.Count * verticalResolution;
            vertices = new Vector3[numPoints];
            float angle;
            float angleDelta = 360f / verticalResolution;//(2 * Mathf.PI / verticalResolution);
            for (int i = 0; i < splineData.SegmentedPoints.Count; i++)
            {
                angle = 0;
                Vector3 cross = Vector3.Cross(splineData.Normals[i], splineData.Tangents[i]).normalized * radius;
                for (int j = 0; j < verticalResolution; j++)
                {
                    float x = Mathf.Cos(angle) * radius, y = Mathf.Sin(angle) * radius;
                    Vector3 pos;//= Quaternion.Euler(new Vector3(0, y, x)) * cross;
                    Quaternion rot = Quaternion.AngleAxis(angle, cross);
                    //pos = rot * Vector3.forward;//splineData.Normals[i];
                    pos = rot * new Vector3(0, y, x);
                    vertices[i * verticalResolution + j] = pos + splineData.SegmentedPoints[i];
                    angle += (angleDelta);
                }
            }
            triangles = new int[numPoints * 6];
            for (int i = 0, triangleIndex = 0; i < splineData.SegmentedPoints.Count - 1; i++)
            {
                for (int j = 0; j < verticalResolution - 1; j++, triangleIndex += 6)
                {
                    int vertexIndex = i * verticalResolution + j;
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 3] = triangles[triangleIndex + 2] = vertexIndex + verticalResolution + 1;
                    triangles[triangleIndex + 4] = triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 5] = vertexIndex + verticalResolution + 2;
                    if (triangles[triangleIndex + 5] == numPoints) triangles[triangleIndex + 5] = 0;
                }
            }
            Setup();
        }

        public void SetPoint(int index, Vector3 point)
        {
            throw new System.NotImplementedException();
        }

        public void SetPointCount(int count)
        {
            return;
        }

        public void SetClosedShape(bool closed)
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

        private void OnDrawGizmos()
        {
            if (vertices == null || vertices.Length == 0) return;
            Gizmos.color = Color.red;
            foreach (var item in vertices)
            {
                Gizmos.DrawSphere(item, 0.01f);
            }
            Gizmos.color = Color.black;
            for (int i = 0; i < triangles.Length; i+=3)
            {
                Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i + 1]]);
                Gizmos.DrawLine(vertices[triangles[i+ 1]], vertices[triangles[i + 2]]);
                Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i + 2]]);
            }
        }
    }

}
