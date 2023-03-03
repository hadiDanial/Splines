using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hadi.Splines
{
    [CreateAssetMenu(fileName = "SplineRenderer Settings", menuName = "SplineRenderer Settings")]
    public abstract class RendererSettings : ScriptableObject
    {
        [SerializeField] private Material material;
        [SerializeField] private UVGenerationType uVGenerationType;

        internal UVGenerationType UVGenerationType { get => uVGenerationType; }
        internal Material Material { get => material; }
    }
}
