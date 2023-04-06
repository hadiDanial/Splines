using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Hadi.Splines
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SVGImporter))]

    public class SVGImporterEditor : Editor
    {
        private SVGImporter importer;

        private void OnEnable()
        {
            importer = target as SVGImporter;
        }
        public override void OnInspectorGUI()
        {
            // if (GUILayout.Button("Load From Selected File"))
            // {
            //     Undo.RegisterFullObjectHierarchyUndo(importer.gameObject, "Import SVG");
            //     importer.SetElement(SVGUtility.ReadSVG());
            // }
            if (GUILayout.Button("Load From Text"))
            {
                Undo.RegisterFullObjectHierarchyUndo(importer.gameObject, "Import SVG");
                importer.SetElement(SVGUtility.ReadSVG(importer.SvgCode));
            }
            if (GUILayout.Button("Delete Children and Reset"))
            {
                Undo.RegisterFullObjectHierarchyUndo(importer.gameObject, "Reset SVG Importer");
                Transform importerTransform = importer.transform;
                for (int i = importerTransform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(importerTransform.GetChild(i).gameObject);
                }
                importerTransform.position = Vector3.zero;
                importerTransform.localScale = Vector3.one;
                importerTransform.rotation = Quaternion.identity;
            }
            base.OnInspectorGUI();
        }
    }
#endif
}
