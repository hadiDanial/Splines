using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    public class TubeMeshRenderer : SplineMeshRenderer
    {
        [Header("Tube Mesh Renderer")]

        private float currentRadius = 0.25f;
        private TubeMeshRendererSettings settings;
        public float Radius { get => (settings != null) ? settings.Radius : currentRadius; set => currentRadius = value; }

        public override void SetData(SplineData splineData)
        {
            currentRadius = Radius;
            base.SetData(splineData);
        }

        protected override int GetTriangleCount(int numPoints)
        {
            return (numPoints - currentMeshResolution) * 6;
        }

        protected override void GenerateVertices(int numPoints)
        {
            base.GenerateVertices(numPoints);
            float angle;
            float angleDelta = 360f / currentMeshResolution;
            float t;
            float radiusValue = (splineData.useObjectTransform ? (currentRadius * defaultScaleMagnitude) / splineData.objectTransform.localScale.magnitude : currentRadius);
            bool useObjectSpace = splineData.useObjectTransform;
            for (int i = 0; i < splineData.Points.Count; i++)
            {
                bool noRadius = false;
                if (capSides && (i == 0 || i == splineData.Points.Count - 1))
                {
                    noRadius = true;
                }
                angle = 0;
                t = (float)i / splineData.Points.Count;
                for (int j = 0; j < currentMeshResolution; j++)
                {
                    Vector3 pos;
                    Quaternion rot = Quaternion.AngleAxis(angle, splineData.Tangents[i]);
                    rot = splineData.useObjectTransform ? transform.rotation * rot : rot;
                    if (noRadius)
                        pos = Vector3.zero;
                    else
                    {

                        pos = (rot * splineData.Normals[i]).normalized;
                        if (useAnimationCurve)
                        {
                            pos *= widthOverSplineAnimationCurve.Evaluate(t) * currentRadius;
                        }
                        else pos *= currentRadius;
                    }
                    pos = transform.InverseTransformSplinePoint(pos + splineData.Points[i], useObjectSpace);
                    vertices[i * currentMeshResolution + j] = pos;
                    angle += (angleDelta);
                }
            }
        }

        protected override void GenerateTriangles(int numPoints)
        {
            base.GenerateTriangles(numPoints);
            for (int i = 0, triangleIndex = 0; i < splineData.Points.Count - 1; i++)
            {
                int vertexIndex, startIndex;
                for (int j = 0; j < currentMeshResolution - 1; j++, triangleIndex += 6)
                {
                    vertexIndex = i * currentMeshResolution + j;
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + currentMeshResolution;
                    triangles[triangleIndex + 3] = triangles[triangleIndex + 2];
                    triangles[triangleIndex + 4] = triangles[triangleIndex + 1];
                    triangles[triangleIndex + 5] = triangles[triangleIndex + 3] + 1;
                }
                // Last index connects back to start
                vertexIndex = i * currentMeshResolution + currentMeshResolution - 1;
                startIndex = i * currentMeshResolution;
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = startIndex;
                triangles[triangleIndex + 2] = vertexIndex + currentMeshResolution;
                triangles[triangleIndex + 3] = triangles[triangleIndex + 2];
                triangles[triangleIndex + 4] = triangles[triangleIndex + 1];
                triangles[triangleIndex + 5] = startIndex + currentMeshResolution;
                triangleIndex += 6;
            }
        }

        protected override void GenerateUVs()
        {
            base.GenerateUVs();
            for (int j = 0; j < currentMeshResolution; j++)
            {
                float verticalT = ((float)j / (currentMeshResolution - 1));
                for (int i = 0; i < splineData.Points.Count; i++)
                {
                    float horizontalT, t = ((float)i / (splineData.Points.Count - 1)); ;
                    switch (settings.UVGenerationType)
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
                    uvs[i * currentMeshResolution + j] = new Vector2(horizontalT, verticalT);
                }
            }
        }

        protected override string GetDefaultName()
        {
            return "Tube Spline";
        }

        protected override void OnValidate()
        {
            if (currentRadius != Radius)
            {
                currentRadius = Radius;
            }
            base.OnValidate();
        }

        protected override void SetSettings(RendererSettings settings)
        {
            this.settings = (TubeMeshRendererSettings)settings;
            meshRenderer.sharedMaterial = settings?.Material;
        }
    }
}
