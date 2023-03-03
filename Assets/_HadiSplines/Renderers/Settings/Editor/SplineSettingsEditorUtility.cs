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
    }
}
