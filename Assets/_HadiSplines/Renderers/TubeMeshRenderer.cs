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

        protected override int GetVertexCount()
        {
            return splineData.SegmentedPoints.Count * (currentMeshResolution + 1);
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
                bool hasZeroRadius = capSides && (i == 0 || i == splineData.SegmentedPoints.Count - 1);
                angle = 0;
                t = (float)i / splineData.SegmentedPoints.Count;
                for (int j = 0; j < currentMeshResolution; j++)
                {
                    Vector3 rotatedPosition;
                    Quaternion rotationByAngle;
                    if (j == currentMeshResolution)                    
                        rotationByAngle = transform.rotation;                                            
                    else
                        rotationByAngle = Quaternion.AngleAxis(angle, splineData.Tangents[i]) * transform.rotation;

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

                    var localScale = transform.localScale;
                    rotatedPosition = new Vector3(rotatedPosition.x * localScale.x * splineData.Scale[i].x,
                        rotatedPosition.y * localScale.y * splineData.Scale[i].y,
                        rotatedPosition.z * localScale.z * splineData.Scale[i].z);
                    int index = i * (currentMeshResolution + 1) + j;
                    vertices[index] = transform.InverseTransformSplinePoint(rotatedPosition + splineData.SegmentedPoints[i], useObjectTransform);
                    normals[index] = (vertices[index] - transform.InverseTransformSplinePoint(splineData.SegmentedPoints[i], useObjectTransform)).normalized;
                    //Debug.Log($"{i * (currentMeshResolution + 1) + j}: - {vertices[i * (currentMeshResolution + 1) + j]}");
                    angle += (angleDelta);
                }
                vertices[(i) * (currentMeshResolution + 1) + (currentMeshResolution)] = vertices[i * (currentMeshResolution + 1)];
                //Debug.Log($"{(i) * (currentMeshResolution + 1) + (currentMeshResolution)}: - {vertices[i * (currentMeshResolution + 1)]}");
            }
        }

        protected override void GenerateTriangles(int numPoints)
        {
            base.GenerateTriangles(numPoints);

            int triangleIndex = 0;
            for (int i = 0; i < splineData.SegmentedPoints.Count - 1; i++, triangleIndex += 6)
            {
                int vertexIndex, lastIndex;
                for (int j = 0; j < currentMeshResolution - 1; j++, triangleIndex += 6)
                {
                    vertexIndex = i * (currentMeshResolution + 1) + j;
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + currentMeshResolution + 1;
                    triangles[triangleIndex + 3] = triangles[triangleIndex + 2];
                    triangles[triangleIndex + 4] = triangles[triangleIndex + 1];
                    triangles[triangleIndex + 5] = triangles[triangleIndex + 3] + 1;
                }
                // Last index connects back to start
                lastIndex = i * (currentMeshResolution + 1) + currentMeshResolution;
                vertexIndex = i * (currentMeshResolution + 1) + currentMeshResolution - 1;
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = lastIndex;
                triangles[triangleIndex + 2] = vertexIndex + currentMeshResolution + 1;
                triangles[triangleIndex + 3] = triangles[triangleIndex + 2];
                triangles[triangleIndex + 4] = triangles[triangleIndex + 1];
                triangles[triangleIndex + 5] = lastIndex + currentMeshResolution + 1;
                triangleIndex += 6;
            }
        }

        protected override void GenerateUVs()
        {
            base.GenerateUVs();
            int segmentsPerCurve = splineData.Spline.SegmentsPerCurve, segmentIndex, segmentNum;
            float verticalT, horizontalT, t;
            for (int j = 0; j <= currentMeshResolution; j++)
            {

                verticalT = ((float)j / (currentMeshResolution));
                for (int i = 0; i < splineData.SegmentedPoints.Count; i++)
                {
                    switch (settings.UVGenerationType)
                    {
                        case UVGenerationType.Mesh:
                            {
                                horizontalT = ((float)i / (splineData.Points.Count - 1));
                                break;
                            }
                        case UVGenerationType.Segment:
                            {
                                segmentIndex = i % (segmentsPerCurve);
                                segmentNum = i / segmentsPerCurve;
                                horizontalT = ((float) segmentIndex / segmentsPerCurve) + segmentNum;
                                break;
                            }
                        case UVGenerationType.Length:
                            {
                                horizontalT = ((float)i / (splineData.Points.Count - 1));
                                break;
                            }
                        default:
                            horizontalT = ((float)i / (splineData.Points.Count - 1));
                            break;
                    }

                    uvs[i * (currentMeshResolution + 1) + j] = new Vector2(horizontalT, verticalT);
                    Debug.Log($"Index {i * (currentMeshResolution + 1) + j}: (i, j) = <{i}, {j}> | {uvs[i * (currentMeshResolution + 1) + j]}");
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
