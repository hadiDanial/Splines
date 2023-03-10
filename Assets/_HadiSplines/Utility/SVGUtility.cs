using SVGImporter.Elements;
using SVGImporter.Elements.Containers;
using SVGImporter.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Path = SVGImporter.Elements.Path;
using Vector2 = SVGImporter.Utility.Vector2;

namespace Hadi.Splines
{
    public class SVGUtility
    {
        private const float splineToSVGScale = 100;
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

        [MenuItem("Splines/SVGs/Spline to SVG")]
        public static void SplineToSVG()
        {
            GameObject gameObject = Selection.activeGameObject;
            if (gameObject == null) return;

            Spline spline = gameObject.GetComponent<Spline>();
            if (spline == null) return;
            SplineToSVG(spline);
        }

        public static void SplineToSVG(Spline spline, SplineMode? splineMode = null)
        {
            List<Point> points = spline.GetPoints();
            List<Vector2> vectors = new List<Vector2>();
            if(splineMode == null)
                 splineMode = spline.SplineMode;
            for (int i = 0; i < points.Count - 1; i++)
            {
                vectors.Add(PositionTo2DVector(points[i].anchor, splineMode.Value));
                vectors.Add(PositionTo2DVector(points[i].GetControlPoint2(), splineMode.Value));
                vectors.Add(PositionTo2DVector(points[i + 1].GetControlPoint1(), splineMode.Value));
            }
            vectors.Add(PositionTo2DVector(points[points.Count - 1].anchor, splineMode.Value));
            if (spline.IsClosedSpline)
            {
                vectors.Add(PositionTo2DVector(points[points.Count - 1].GetControlPoint2(), splineMode.Value));
                vectors.Add(PositionTo2DVector(points[0].GetControlPoint1(), splineMode.Value));
                vectors.Add(PositionTo2DVector(points[0].anchor, splineMode.Value));
            }

            SVG svg = Path.CreatePathFromPoints(vectors, PositionTo2DVector(spline.transform.position, splineMode.Value), !spline.UseObjectTransform, spline.IsClosedSpline);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in vectors)
            {
                stringBuilder.Append(item.ToString());
                stringBuilder.Append(' ');
            }

            string folderPath = $"{Application.dataPath}/Exported SVGs";
            if (!Directory.Exists(folderPath))
                CreateFolder(folderPath);
            File.WriteAllText($"{folderPath}/{spline.gameObject.name}_{splineMode.Value}_{DateTime.Now.ToString("yyyy_dd_M-HH_mm_ss")}.svg", svg.ElementToSVGTag());
            AssetDatabase.Refresh();
        }

        private static void CreateFolder(string folderPath)
        {            
            Directory.CreateDirectory(folderPath);
        }

        private static Vector2 PositionTo2DVector(Vector3 vector3, SplineMode splineMode)
        {
            Vector2 point;
            switch (splineMode)
            {
                case SplineMode.XZ:
                    point = new Vector2(vector3.x, -vector3.z);
                    break;
                case SplineMode.YZ:
                    point = new Vector2(vector3.z, -vector3.y);
                    break;
                case SplineMode.Full3D:
                case SplineMode.XY:
                default:
                    point = new Vector2(vector3.x, -vector3.y);
                    break;
            }
            return new Vector2(point.x * splineToSVGScale, point.y * splineToSVGScale);
        }
#endif
    }
}
