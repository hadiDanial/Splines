using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.Handles;

namespace Hadi.Splines.Editor
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : UnityEditor.Editor
    {
        private static Color anchorColor = Color.blue;
        private static Color controlPointColor = Color.red;
        private static Color lineColor = Color.white;
        private static Color alignedColor = Color.yellow;
        private static Color disabledColor = Color.gray;
        private static float lineThickness = 3f;
            
        Vector3 anchor, control1, control2;
        Quaternion rotation;
        Vector3 scale;
        private int? pickedHandle;
        private Vector3 pointTangent;
        private Spline spline;
        private List<Point> points;
        private SplineMode exportMode = SplineMode.Full3D;
        private GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(80) };

        private void OnEnable()
        {
            Undo.undoRedoPerformed += Reset;
            Reset();
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Reset;
        }
        private void Reset()
        {
            spline = (target as Spline);
            points = spline.GetPoints();
            pickedHandle = null;
            exportMode = spline.SplineMode;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Add Point"))
            {
                Undo.RecordObject(spline, "Add Point To Spline");
                Reset();
                spline.AddPoint();
            }
            EditorGUILayout.BeginHorizontal();
            spline.ShapeOnReset = (SplineShapes)EditorGUILayout.EnumPopup(spline.ShapeOnReset, options);
            if (GUILayout.Button("Reset Spline"))
            {
                if (EditorUtility.DisplayDialog("Reset Spline", "Are you sure you want to the spline?", "Yes", "Cancel"))
                {
                    Undo.RecordObject(spline, "Reset Spline");
                    Reset();
                    spline.ResetSpline();
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Reset Point Rotations"))
            {
                if (EditorUtility.DisplayDialog("Reset Point Rotations", "Are you sure you want to reset all point rotations?", "Yes", "Cancel"))
                {
                    Undo.RecordObject(spline, "Reset Point Rotations");
                    spline.ResetPointRotations();
                    Reset();
                }
            }
            EditorGUILayout.BeginHorizontal();
            exportMode = (SplineMode)EditorGUILayout.EnumPopup(exportMode, options);
            if(GUILayout.Button("Export SVG"))
            {
                SVGUtility.SplineToSVG(spline, exportMode);
            }
            EditorGUILayout.EndHorizontal(); 
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            Vector3 origin = spline.UseObjectTransform ? spline.transform.position : Vector3.zero;
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : spline.transform.rotation;
            points = spline.GetPoints();
            Tools.hidden = true;
            DrawDefaultTransformHandle(spline, origin, handleRotation);
            DrawPointButtons(spline);
            DrawHandles(spline, handleRotation);
            DrawSplineDetails(spline);
        }

        private void DrawDefaultTransformHandle(Spline spline, Vector3 origin, Quaternion handleRotation)
        {
            if (spline.UseObjectTransform)
            {
                EditorGUI.BeginChangeCheck();
                Transform splineTransform = spline.transform;
                switch (Tools.current)
                {
                    case Tool.Move:
                        {
                            Vector3 position = Handles.PositionHandle(origin, handleRotation);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(splineTransform, "Changed Spline Transform (Position)");
                                splineTransform.position = position;
                                spline.GenerateSpline();
                            }
                            break;
                        }
                    case Tool.Rotate:
                        {
                            Quaternion rotation = Handles.RotationHandle(spline.transform.rotation, origin);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(splineTransform, "Changed Spline Transform (Rotation)");
                                splineTransform.rotation = rotation;
                                spline.GenerateSpline();
                            }
                            break;
                        }
                    case Tool.Scale:
                        {
                            Vector3 scale = Handles.ScaleHandle(spline.transform.localScale, origin, handleRotation);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(splineTransform, "Changed Spline Transform (Scale)");
                                splineTransform.localScale = scale;
                                spline.GenerateSpline();
                            }
                            break;
                        }
                }
            }
        }

        private static void DrawSplineDetails(Spline spline)
        {
            SplineData data = spline.SplineData;
            if (spline.DrawGizmos)
            {
                Transform splineTransform = spline.transform;
                for (int i = 0; i < data.SegmentedPoints.Count; i++)
                {
                    Handles.color = Color.white;
                    Vector3 p = data.SegmentedPoints[i];
                    Handles.DrawLine(p, p + data.Normals[i] * 0.15f);
                    Handles.color = Color.green;
                    Handles.DrawLine(p, p + data.Tangents[i] * 0.15f);
                    Handles.color = Color.red;
                    if (i < data.SegmentedPoints.Count - 1)
                        Handles.DrawLine(p, data.SegmentedPoints[i + 1]);
                }
            }
        }

        private void DrawHandles(Spline spline, Quaternion handleRotation)
        {
            if (pickedHandle.HasValue)
            {
                bool changed = false;
                EditorGUI.BeginChangeCheck();
                Point point = points[pickedHandle.Value];
                Vector3 handlePos = spline.transform.TransformSplinePoint(point.anchor, spline.UseObjectTransform);
                rotation = point.rotation;
                scale = point.scale;
                anchor = handlePos;
                switch (Tools.current)
                {                        
                    case Tool.Rotate:
                    rotation = Handles.RotationHandle(point.rotation, handlePos);
                        break;
                    case Tool.Scale:
                        scale = Handles.ScaleHandle(point.scale, handlePos, handleRotation);
                        scale = new Vector3(Mathf.Clamp(scale.x, 0.001f, scale.x),
                            Mathf.Clamp(scale.y, 0.001f, scale.y), Mathf.Clamp(scale.z, 0.001f, scale.z));
                        break;                 
                    case Tool.Move:
                    default:
                    anchor = Handles.PositionHandle(handlePos, handleRotation);
                        break;
                }
                //rotation = Handles.Disc(point.rotation, handlePos, pointTangent, 0.4f, false, 1);

                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                    if (spline.UseObjectTransform)
                    {
                        anchor = spline.transform.InverseTransformPoint(anchor);
                    }
                    Undo.RecordObject(spline, "Modified Anchor (Spline)");
                    point.UpdateAnchor(anchor, rotation, scale);
                }

                EditorGUI.BeginChangeCheck();
                Vector3 c1Pos = spline.transform.TransformSplinePoint(point.GetControlPoint1(), spline.UseObjectTransform);
                Handles.color = controlPointColor;
                Handles.SphereHandleCap(0, c1Pos, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                control1 = Handles.PositionHandle(c1Pos, handleRotation);
                if (point.mode == ControlMode.Mirrored)
                    Handles.color = disabledColor;
                else if (point.mode == ControlMode.Aligned)
                    Handles.color = alignedColor;
                Vector3 c2Pos = spline.transform.TransformSplinePoint(point.GetControlPoint2(), spline.UseObjectTransform);
                Handles.SphereHandleCap(0, c2Pos, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                switch (point.mode)
                {
                    case ControlMode.Aligned:
                        control2 = Handles.Slider(c2Pos, c2Pos - anchor, 0.65f, Handles.ArrowHandleCap, 0.1f);
                        break;
                    case ControlMode.Mirrored:
                        break;
                    case ControlMode.Automatic:
                    case ControlMode.Broken:
                    default:
                        control2 = Handles.PositionHandle(c2Pos, handleRotation);
                        break;
                }
                
                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                    if (spline.UseObjectTransform)
                    {
                        control1 = spline.transform.InverseTransformPoint(control1);
                        control2 = spline.transform.InverseTransformPoint(control2);
                    }
                    Undo.RecordObject(spline, "Moved Control Point (Spline)");
                    point.UpdateControlPoints(control1, control2);
                    if (spline.SplineSettings.automaticPointRotations)
                        point.SetAutoRotation();
                }

                Handles.color = lineColor;
                Handles.DrawLine(handlePos, c1Pos, lineThickness);
                Handles.DrawLine(handlePos, c2Pos, lineThickness);
                if (changed)
                {
                    pointTangent = spline.GetPointTangent(pickedHandle.Value);
                    point.Update(spline.SplineMode);
                    spline.GenerateSpline();
                }
            }
        }

        private void DrawPointButtons(Spline spline)
        {
            Handles.color = anchorColor;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 handlePos = spline.transform.TransformSplinePoint(points[i].anchor, spline.UseObjectTransform);
                if (pickedHandle.HasValue && pickedHandle.Value == i)
                {
                    Handles.SphereHandleCap(1, handlePos, Quaternion.identity, spline.ANCHOR_SIZE / 3, EventType.Repaint);
                }
                else
                {
                    if (Handles.Button(handlePos, Quaternion.identity, spline.ANCHOR_SIZE, spline.ANCHOR_SIZE, Handles.SphereHandleCap))
                    {
                        pickedHandle = i;
                        pointTangent = spline.GetPointTangent(pickedHandle.Value);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            Tools.hidden = false;
        }
    }
}