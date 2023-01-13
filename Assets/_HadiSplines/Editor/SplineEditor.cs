using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Hadi.Splines;

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
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var spline = (target as Spline);
            if(GUILayout.Button("Add Point"))
            {
                spline.AddPoint();
            }
        }

        private void OnSceneGUI()
        {
            var spline = (target as Spline);
            EditorGUI.BeginChangeCheck();
            List<Point> points = spline.GetPoints();
            foreach (Point point in points)
            {
                Handles.color = anchorColor;
                Handles.SphereHandleCap(0, point.anchor, Quaternion.identity, spline.ANCHOR_SIZE, EventType.Repaint);
                anchor = Handles.PositionHandle(point.anchor, Quaternion.identity);
                //rotation = Handles.RotationHandle(Quaternion.Euler(point.normal), point.anchor);
                //normal = rotation.eulerAngles;
                //Debug.Log($"Normal={normal}, Rotation={rotation}");
                Handles.color = controlPointColor;
                Handles.SphereHandleCap(0, point.controlPoint1, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                control1 = Handles.PositionHandle(point.controlPoint1, Quaternion.identity);
                if (point.mode == ControlMode.Mirrored)
                    Handles.color = disabledColor;
                else if (point.mode == ControlMode.Aligned)
                    Handles.color = alignedColor;
                Handles.SphereHandleCap(0, point.controlPoint2, Quaternion.identity, spline.CONTROL_SIZE, EventType.Repaint);
                control2 = Handles.PositionHandle(point.controlPoint2, Quaternion.identity);
                rotation = Handles.RotationHandle(point.rotation, point.anchor);
                //  Handles.Disc(point.rotation, point.anchor, Vector3.Cross(point.controlPoint1, Quaternion.Euler(point.rotation)), 1, true, 0);
                Handles.color = lineColor;
                Handles.DrawLine(point.anchor, point.controlPoint1, lineThickness);
                Handles.DrawLine(point.anchor, point.controlPoint2, lineThickness);
                point.Update(anchor, control1, control2, normal);
                point.rotation = rotation;
            }
            SplineData data = spline.SplineData;
            if(spline.DrawGizmos)
            {
                Handles.color = Color.white;
                for (int i = 0; i < data.Normals.Count; i++)
                {
                    Vector3 p = data.SegmentedPoints[i];
                    Handles.DrawLine(p, p + data.Normals[i] * 0.15f);
                }
                Handles.color = Color.green;
                for (int i = 0; i < data.Tangents.Count; i++)
                {
                    Vector3 p = data.SegmentedPoints[i];
                    Handles.DrawLine(p, p + data.Tangents[i] * 0.15f);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move point");
                //t.lookAtPoint = pos;
                //t.Update();
            }
        }
    }
}