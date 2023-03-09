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

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var closedSplineProp = property.FindPropertyRelative("closedSpline");
            var useObjectTransformProp = property.FindPropertyRelative("useObjectTransform");
            var splineModeProp = property.FindPropertyRelative("splineMode");
            var splineTypeProp = property.FindPropertyRelative("splineType");
            var endOfSplineInstructionProp = property.FindPropertyRelative("EndOfSplineInstruction");
            var segmentsPerCurveProp = property.FindPropertyRelative("segmentsPerCurve");
            var newPointDistanceProp = property.FindPropertyRelative("newPointDistance");
            var automaticRotationProp = property.FindPropertyRelative("automaticPointRotations");


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
            float lineWidth = EditorGUIUtility.currentViewWidth - padding * 2f;

            Rect useObjectTransformRect = new Rect(padding, startYPos + lineHeight, lineWidth, lineHeight);
            Rect closedSplineRect = new Rect(padding, startYPos, lineWidth, lineHeight);
            Rect splineModeRect = new Rect(padding, startYPos + 2 * lineHeight, lineWidth, lineHeight);
            Rect splineTypeRect = new Rect(padding, startYPos + 3 * lineHeight, lineWidth, lineHeight);
            Rect endOfSplineInstructionRect = new Rect(padding, startYPos + 4 * lineHeight, lineWidth, lineHeight);
            Rect segmentsPerCurveRect = new Rect(padding, startYPos + 5 * lineHeight, lineWidth, lineHeight);
            Rect newPointDistanceRect = new Rect(padding, startYPos + 6 * lineHeight, lineWidth, lineHeight);
            Rect automaticRotationRect = new Rect(padding, startYPos + 7 * lineHeight, lineWidth, lineHeight);

            GUIContent closedSpline = new GUIContent("Closed?");
            closedSpline.tooltip = closedSplineProp.tooltip;
            closedSplineRect = EditorGUI.PrefixLabel(closedSplineRect, GUIUtility.GetControlID(FocusType.Passive), closedSpline);
            //closedSplineRect.width = halfWidth;

            EditorGUI.PropertyField(closedSplineRect, closedSplineProp, GUIContent.none);
            EditorGUI.PropertyField(useObjectTransformRect, useObjectTransformProp);

            EditorGUI.PropertyField(splineModeRect, splineModeProp);
            EditorGUI.PropertyField(splineTypeRect, splineTypeProp);
            EditorGUI.PropertyField(endOfSplineInstructionRect, endOfSplineInstructionProp);
            EditorGUI.PropertyField(segmentsPerCurveRect, segmentsPerCurveProp);
            EditorGUI.PropertyField(newPointDistanceRect, newPointDistanceProp);

            GUIContent automaticRotation = new GUIContent("Auto Point Rotation");
            automaticRotation.tooltip = automaticRotationProp.tooltip;
            automaticRotationRect = EditorGUI.PrefixLabel(automaticRotationRect, GUIUtility.GetControlID(FocusType.Passive), automaticRotation);
            automaticRotationRect.width = 50;
            EditorGUI.PropertyField(automaticRotationRect, automaticRotationProp, GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUILayout.Separator();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 9;
        }
    }
}
