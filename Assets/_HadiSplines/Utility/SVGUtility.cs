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
        public const float splineToSVGScale = 100;
        public const float svgToSplineScale = 0.01f;

        private const string extension = ".svg";
#if UNITY_EDITOR
        [MenuItem("Splines/SVGs/Read File")]
        public static Element ReadSVG()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids == null || guids.Length == 0) return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            if (path == null || !path.ToLower().EndsWith(extension)) return null;

            return ReadSVG(File.ReadAllText(path));
        }

        public static Element ReadSVG(string svgText)
        {
            Element element = SVGFileParser.ReadSVG(svgText);
            //Debug.Log(element.ToString());
            return element;
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
            if (splineMode == null)
                splineMode = spline.SplineMode;
            bool useObjectTransform = spline.UseObjectTransform;
            SplineMode mode = splineMode.Value;
            Transform transform = spline.transform;

            for (int i = 0; i < points.Count - 1; i++)
            {
                vectors.Add(PositionTo2DVector(transform.TransformSplinePoint(points[i].anchor, useObjectTransform), mode));
                vectors.Add(PositionTo2DVector(transform.TransformSplinePoint(points[i].GetControlPoint2(), useObjectTransform), mode));
                vectors.Add(PositionTo2DVector(transform.TransformSplinePoint(points[i + 1].GetControlPoint1(), useObjectTransform), mode));
            }
            vectors.Add(PositionTo2DVector(transform.TransformSplinePoint(points[points.Count - 1].anchor, useObjectTransform), mode));

            if (spline.IsClosedSpline)
            {
                vectors.Add(PositionTo2DVector(transform.TransformSplinePoint(points[points.Count - 1].GetControlPoint2(), useObjectTransform), mode));
                vectors.Add(PositionTo2DVector(transform.TransformSplinePoint(points[0].GetControlPoint1(), useObjectTransform), mode));
                vectors.Add(PositionTo2DVector(transform.TransformSplinePoint(points[0].anchor, useObjectTransform), mode));
            }

            SVG svg = Path.CreatePathFromPoints(vectors, PositionTo2DVector(spline.transform.position, splineMode.Value), !spline.UseObjectTransform, spline.IsClosedSpline);
            //PrintVectorList(vectors);
            string folderPath = $"{Application.dataPath}/Exported SVGs";
            if (!Directory.Exists(folderPath))
                CreateFolder(folderPath);
            File.WriteAllText($"{folderPath}/{spline.gameObject.name}_{splineMode.Value}_{DateTime.Now.ToString("yyyy_dd_M-HH_mm_ss")}.svg", svg.ElementToSVGTag());
            AssetDatabase.Refresh();
        }

        private static void PrintVectorList(List<Vector2> vectors)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in vectors)
            {
                stringBuilder.Append(item.ToString());
                stringBuilder.Append(' ');
            }
            Debug.Log(stringBuilder.ToString());
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
