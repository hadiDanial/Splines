using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        Vector3 normal;
        private int? pickedHandle;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var spline = (target as Spline);
            if (GUILayout.Button("Add Point"))
            {
                spline.AddPoint();
            }
            if (GUILayout.Button("Reset Spline"))
            {
                spline.ResetSpline();
            }
        }

        private void OnSceneGUI()
        {
            var spline = (target as Spline);
            Vector3 origin = spline.transform.TransformSplinePoint(spline.transform.position, spline.UseWorldSpace);
            EditorGUI.BeginChangeCheck();
            List<Point> points = spline.GetPoints();
            Handles.color = anchorColor;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 handlePos = spline.UseWorldSpace ? points[i].anchor : spline.transform.TransformPoint(points[i].anchor);
                if (Handles.Button(handlePos, Quaternion.identity, spline.ANCHOR_SIZE, 0.2f, Handles.SphereHandleCap))
                {
                    pickedHandle = i;
                }
            }
            if (pickedHandle.HasValue)
            {
                bool changed = false;
                EditorGUI.BeginChangeCheck();
                Point point = points[pickedHandle.Value];
                Vector3 handlePos = spline.UseWorldSpace ? point.anchor : spline.transform.TransformPoint(point.anchor);
                Quaternion rotation = Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : spline.transform.rotation;
                
                anchor = Handles.PositionHandle(handlePos, rotation);
                rotation = Handles.RotationHandle(point.rotation, handlePos);
                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                    Undo.RecordObject(target, "Modified Anchor (Spline)");
                }

                EditorGUI.BeginChangeCheck();
                Vector3 c1Pos = spline.UseWorldSpace ? point.controlPoint1 : spline.transform.TransformPoint(point.controlPoint1);
                Handles.color = controlPointColor;
                Handles.SphereHandleCap(0, c1Pos, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                control1 = Handles.PositionHandle(c1Pos, rotation);
                if (point.mode == ControlMode.Mirrored)
                    Handles.color = disabledColor;
                else if (point.mode == ControlMode.Aligned)
                    Handles.color = alignedColor;
                Vector3 c2Pos = spline.UseWorldSpace ? point.controlPoint2 : spline.transform.TransformPoint(point.controlPoint2);
                Handles.SphereHandleCap(0, c2Pos, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                if (point.mode != ControlMode.Mirrored)
                    control2 = Handles.PositionHandle(c2Pos, rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                    Undo.RecordObject(target, "Moved Control Point (Spline)");
                }
                
                Handles.color = lineColor;
                Handles.DrawLine(handlePos, c1Pos, lineThickness);
                Handles.DrawLine(handlePos, c2Pos, lineThickness);
                if(changed)
                {
                    //if(!spline.UseWorldSpace)
                    //{
                    //    anchor = spline.transform.InverseTransformPoint(anchor);
                    //    control1 = spline.transform.InverseTransformPoint(control1);
                    //    control2 = spline.transform.InverseTransformPoint(control2);
                    //}
                    if (point.Update(anchor, control1, control2, rotation, origin, spline.SplineMode))
                        spline.GenerateSpline();
                }
            }

            SplineData data = spline.SplineData;
            if (spline.DrawGizmos)
            {
                bool UseWorldSpace = spline.UseWorldSpace;
                for (int i = 0; i < data.Points.Count; i++)
                {
                    Handles.color = Color.white;
                    Vector3 p = spline.transform.TransformSplinePoint(data.Points[i], spline.UseWorldSpace);
                    Handles.DrawLine(p, p + data.Normals[i] * 0.15f);
                    Handles.color = Color.green;
                    Handles.DrawLine(p, p + data.Tangents[i] * 0.15f);
                    Handles.color = Color.red;
                    if (i < data.Points.Count - 1)
                        Handles.DrawLine(p, spline.transform.TransformSplinePoint(data.Points[i + 1], spline.UseWorldSpace));
                }                
            }
        }
    }
}