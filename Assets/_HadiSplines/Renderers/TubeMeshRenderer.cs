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
            bool useObjectTransform = splineData.useObjectTransform;
            for (int i = 0; i < splineData.SegmentedPoints.Count; i++)
            {
                bool hasZeroRadius = false;
                if (capSides && (i == 0 || i == splineData.SegmentedPoints.Count - 1))
                {
                    hasZeroRadius = true;
                }
                angle = 0;
                t = (float)i / splineData.SegmentedPoints.Count;
                for (int j = 0; j < currentMeshResolution; j++)
                {
                    Vector3 rotatedPosition;
                    Quaternion rotationByAngle = Quaternion.AngleAxis(angle, splineData.Tangents[i]) * transform.rotation;
                    if (hasZeroRadius)
                        rotatedPosition = Vector3.zero;
                    else
                    {

                        rotatedPosition = (rotationByAngle * splineData.Normals[i]).normalized;
                        if (useAnimationCurve)
                        {
                            rotatedPosition *= widthOverSplineAnimationCurve.Evaluate(t) * currentRadius;
                        }
                        else rotatedPosition *= currentRadius;
                    }
                    vertices[i * currentMeshResolution + j] = transform.InverseTransformSplinePoint(rotatedPosition + splineData.SegmentedPoints[i], useObjectTransform) ;
                    angle += (angleDelta);
                }
            }
        }

        protected override void GenerateTriangles(int numPoints)
        {
            base.GenerateTriangles(numPoints);
            for (int i = 0, triangleIndex = 0; i < splineData.SegmentedPoints.Count - 1; i++)
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
                for (int i = 0; i < splineData.SegmentedPoints.Count; i++)
                {
                    float horizontalT, t = ((float)i / (splineData.SegmentedPoints.Count - 1)); ;
                    switch (settings.UVGenerationType)
                    {
                        case UVGenerationType.Mesh:
                            {
                                horizontalT = t;
                                break;
                            }
                        case UVGenerationType.Segment:
                            {
                                float initT = t * splineData.SegmentedPoints.Count;
                                int segmentIndex = Mathf.FloorToInt(initT);
                                float nextT = (segmentIndex + 1f) / splineData.SegmentedPoints.Count;
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
