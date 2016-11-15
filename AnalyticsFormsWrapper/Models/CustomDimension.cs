using System;

namespace AnalyticsFormsWrapper.Models
{
    /// <summary>
    /// The dimension index. Each custom dimension has an associated index.
    /// There is a maximum of 20 custom dimensions (200 for Premium accounts). 
    /// The index suffix must be a positive integer greater than 0 (e.g. dimension3).
    /// </summary>
    public class CustomDimension
    {
        public int DimensionIndex { set; get; }

        public string DimensionValue { set; get; }
    }
}
