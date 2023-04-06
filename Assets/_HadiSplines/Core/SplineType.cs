namespace Hadi.Splines
{
    public enum SplineType
    {
        /// <summary>
        /// No control points or interpolation, straight lines.
        /// </summary>
        Linear,
        /// <summary>
        /// Shared control point between points.
        /// </summary>
        Quadratic,
        /// <summary>
        /// Each point has its own control points, and each curve has two control points.
        /// </summary>
        Cubic
    }
}
