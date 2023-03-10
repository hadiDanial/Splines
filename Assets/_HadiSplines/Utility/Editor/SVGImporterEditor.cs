using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

namespace Hadi.Splines
{
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
            if (GUILayout.Button("Load From Selected File"))
            {
                Undo.RegisterFullObjectHierarchyUndo(importer.gameObject, "Import SVG");
                importer.SetElement(SVGUtility.ReadSVG());
            }
            base.OnInspectorGUI();
            if (GUILayout.Button("Load From Text"))
            {
                Undo.RegisterFullObjectHierarchyUndo(importer.gameObject, "Import SVG");
                importer.SetElement(SVGUtility.ReadSVG(importer.SvgCode));
            }
        }
    }
}
