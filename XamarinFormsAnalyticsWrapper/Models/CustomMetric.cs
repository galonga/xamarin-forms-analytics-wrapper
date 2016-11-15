using System;

namespace XamarinFormsAnalyticsWrapper.Models
{
    /// <summary>
    /// The metric index. Each custom metric has an associated index.
    /// There is a maximum of 20 custom metrics (200 for Premium accounts).
    /// The index suffix must be a positive integer greater than 0 (e.g. metric5).
    /// </summary>
    public class CustomMetric
    {
        public int MetricIndex { set; get; }

        public float MetricValue { set; get; }
    }
}
