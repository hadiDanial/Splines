using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hadi.Splines
{
    public class SplineSettingsEditorUtility
    {
        [MenuItem("Splines/Refresh Splines")]
        public static void RefreshSplines()
        {
            Spline[] splines = GameObject.FindObjectsOfType<Spline>();
            for (int i = 0; i < splines.Length; i++)
            {
                splines[i].Refresh();
            }
        }
        public static void RefreshSplines(SplineRendererType type)
        {
            Spline[] splines = GameObject.FindObjectsOfType<Spline>();
            for (int i = 0; i < splines.Length; i++)
            {
                if (splines[i].RendererType == type)
                    splines[i].Refresh();
            }
        }
    }
}
