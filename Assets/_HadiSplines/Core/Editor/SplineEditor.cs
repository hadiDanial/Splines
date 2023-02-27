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
            EditorGUI.BeginChangeCheck();
            List<Point> points = spline.GetPoints();
            Vector3 origin = spline.GetOrigin();
            Handles.color = anchorColor;
            for (int i = 0; i < points.Count; i++)
            {
                if (Handles.Button(points[i].anchor, Quaternion.identity, spline.ANCHOR_SIZE, 0.2f, Handles.SphereHandleCap))
                {
                    pickedHandle = i;
                }
            }
            if (pickedHandle.HasValue)
            {
                bool changed = false;
                EditorGUI.BeginChangeCheck();
                Point point = points[pickedHandle.Value];
                Vector3 handlePos = point.anchor + origin;
                anchor = Handles.PositionHandle(handlePos, Quaternion.identity);
                rotation = Handles.RotationHandle(point.rotation, handlePos);
                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                    Undo.RecordObject(target, "Modified Anchor (Spline)");
                }

                EditorGUI.BeginChangeCheck();
                handlePos = point.controlPoint1 + origin;
                Handles.color = controlPointColor;
                Handles.SphereHandleCap(0, handlePos, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                control1 = Handles.PositionHandle(handlePos, Quaternion.identity);
                if (point.mode == ControlMode.Mirrored)
                    Handles.color = disabledColor;
                else if (point.mode == ControlMode.Aligned)
                    Handles.color = alignedColor;
                handlePos = point.controlPoint2 + origin;
                Handles.SphereHandleCap(0, handlePos, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                if (point.mode != ControlMode.Mirrored)
                    control2 = Handles.PositionHandle(handlePos, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                    Undo.RecordObject(target, "Moved Control Point (Spline)");
                }
                
                Handles.color = lineColor;
                Handles.DrawLine(point.anchor + origin, point.controlPoint1 + origin, lineThickness);
                Handles.DrawLine(point.anchor + origin, point.controlPoint2 + origin, lineThickness);
                if(changed)
                {
                    if (point.Update(anchor, control1, control2, rotation, origin, spline.SplineMode))
                        spline.GenerateSpline();
                }
            }

            SplineData data = spline.SplineData;
            if (spline.DrawGizmos)
            {
                Handles.color = Color.white;
                for (int i = 0; i < data.Normals.Count; i++)
                {
                    Vector3 p = data.Points[i] + origin;
                    Handles.DrawLine(p, p + data.Normals[i] * 0.15f);
                }
                Handles.color = Color.green;
                for (int i = 0; i < data.Tangents.Count; i++)
                {
                    Vector3 p = data.Points[i] + origin;
                    Handles.DrawLine(p, p + data.Tangents[i] * 0.15f);
                }
            }
        }
    }
}