using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hadi.Splines
{
#if UNITY_EDITOR
    [CustomEditor(typeof(RendererSettings))]
    public class RendererSettingsEditor : Editor
    {
        
    }
#endif
}
