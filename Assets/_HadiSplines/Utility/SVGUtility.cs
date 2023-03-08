using SVGImporter.Elements;
using SVGImporter.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hadi.Splines
{
    public class SVGUtility
    {

        private const string extension = ".svg";
#if UNITY_EDITOR
        [MenuItem("Splines/SVGs/Read File")]
        public static void ReadSVG()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids == null || guids.Length == 0) return;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            if (path == null || !path.ToLower().EndsWith(extension)) return;


            Element element = SVGFileParser.ReadSVG(File.ReadAllText(path));

            Debug.Log(element.ToString());

        }
#endif
    }
}
