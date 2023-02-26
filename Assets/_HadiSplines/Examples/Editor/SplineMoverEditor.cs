using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Hadi.Splines
{
    [CustomEditor(typeof(SplineMover))]
    public class SplineMoverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var splineMover = (target as SplineMover);
            serializedObject.Update();

            DisplayMovementInfo(splineMover);
            EditorGUILayout.LabelField($"Current Value: {splineMover.currentValue}", $"Last Index: {splineMover.lastIndex}");
            splineMover.spline = EditorGUILayout.ObjectField("Spline", splineMover.spline, typeof(Spline), true) as Spline;

            splineMover.movementMode = (SplineMovementMode)EditorGUILayout.EnumPopup("Movement Mode", splineMover.movementMode);
            if (splineMover.movementMode == SplineMovementMode.Time)
            {
                EditorGUILayout.BeginVertical();
                splineMover.timeValueEasing = EditorGUILayout.CurveField("Easing Curve", splineMover.timeValueEasing);
                splineMover.timePerLoop = Mathf.Max(0.1f, EditorGUILayout.FloatField("Time", splineMover.timePerLoop));
                EditorGUILayout.EndVertical();
            }
            else if (splineMover.movementMode == SplineMovementMode.Distance)
            {
                splineMover.movementSpeed = EditorGUILayout.FloatField("Movement Speed", splineMover.movementSpeed);
            }
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DisplayMovementInfo(SplineMover splineMover)
        {
            EditorGUILayout.LabelField($"Is Moving: {splineMover.IsMoving}");
        }
    }
}
