using UnityEditor;

namespace Hadi.Splines
{
#if UNITY_EDITOR
    [CustomEditor(typeof(FreeMovement))]
    public class FreeMovementEditor : SplineMoverEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void DisplayMovementInfo(SplineMover splineMover)
        {
            EditorGUILayout.LabelField("Is Moving " + (splineMover.MoveForwards ? "Forwards" : "Backwards") + $": { splineMover.IsMoving}");

        }
    }
#endif
}