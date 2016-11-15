using System;
namespace XamarinFormsAnalyticsWrapper.Models
{
    public class EventData
    {
        /// <summary>
        /// Required. Typically the object that was interacted with (e.g. 'Video')
        /// </summary>
        /// <value>The event category.</value>
        public string EventCategory { get; set; }

        /// <summary>
        /// Required. The type of interaction (e.g. 'play')
        /// </summary>
        /// <value>The event action.</value>
        public string EventAction { get; set; }

        /// <summary>
        /// Useful for categorizing events (e.g. 'Fall Campaign')
        /// </summary>
        /// <value>The event label.</value>
        public string EventLabel { get; set; }

        /// <summary>
        /// A numeric value associated with the event (e.g. 42)
        /// </summary>
        /// <value>The identifier.</value>
        public long Id { get; set; }
    }
}
