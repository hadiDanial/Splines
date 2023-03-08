using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hadi.Splines.Editor
{
    [CustomPropertyDrawer(typeof(SplineSettings))]
    public class SplineSettingsPropertyDrawer : PropertyDrawer
    {
        private const int padding = 20;
        private static GUIStyle style;
        private static float lineHeight = EditorGUIUtility.singleLineHeight;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //EditorGUI.BeginChangeCheck();

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var closedSplineProp = property.FindPropertyRelative("closedSpline");
            var useObjectTransformProp = property.FindPropertyRelative("useObjectTransform");
            var splineModeProp = property.FindPropertyRelative("splineMode");
            var splineTypeProp = property.FindPropertyRelative("splineType");
            var endOfSplineInstructionProp = property.FindPropertyRelative("EndOfSplineInstruction");
            var segmentsPerCurveProp = property.FindPropertyRelative("segmentsPerCurve");
            var newPointDistanceProp = property.FindPropertyRelative("newPointDistance");


            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label);
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 15;
                style.fixedHeight = lineHeight * 2;
            }
            position.height += lineHeight;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, style);

            float startYPos = position.y + lineHeight * 1.5f;
            float lineWidth = EditorGUIUtility.currentViewWidth - 25;

            Rect useObjectTransformRect = new Rect(padding, startYPos, lineWidth/2, lineHeight);
            Rect closedSplineRect = new Rect(padding + lineWidth/2, startYPos, lineWidth/2, lineHeight);
            Rect splineModeRect = new Rect(padding, startYPos + lineHeight + EditorGUIUtility.standardVerticalSpacing, lineWidth, lineHeight);
            Rect splineTypeRect = new Rect(padding, startYPos + 2 * lineHeight + 2 * EditorGUIUtility.standardVerticalSpacing, lineWidth, lineHeight);
            Rect endOfSplineInstructionRect = new Rect(padding, startYPos + 3 * lineHeight + 3 * EditorGUIUtility.standardVerticalSpacing, lineWidth, lineHeight);
            Rect segmentsPerCurveRect = new Rect(padding, startYPos + 4 * lineHeight + 4 * EditorGUIUtility.standardVerticalSpacing, lineWidth, lineHeight);
            Rect newPointDistanceRect = new Rect(padding, startYPos + 5 * lineHeight + 5 * EditorGUIUtility.standardVerticalSpacing, lineWidth, lineHeight);

            EditorGUI.PropertyField(closedSplineRect, closedSplineProp);
            EditorGUI.PropertyField(useObjectTransformRect, useObjectTransformProp);
            EditorGUI.PropertyField(splineModeRect, splineModeProp);
            EditorGUI.PropertyField(splineTypeRect, splineTypeProp);
            EditorGUI.PropertyField(endOfSplineInstructionRect, endOfSplineInstructionProp);
            EditorGUI.PropertyField(segmentsPerCurveRect, segmentsPerCurveProp);
            EditorGUI.PropertyField(newPointDistanceRect, newPointDistanceProp);

            EditorGUI.indentLevel = indent;
            EditorGUILayout.Separator();
            EditorGUI.EndProperty();
            //if (EditorGUI.EndChangeCheck())
            //    Debug.Log("Change");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 8;
        }
    }
}
