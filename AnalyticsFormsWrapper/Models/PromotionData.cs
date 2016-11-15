using System;

namespace AnalyticsFormsWrapper.Models
{
    public class PromotionData
    {
        /// <summary>
        /// The promotion ID (e.g.PROMO_1234). *Either this field or name must be set.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { set; get; }

        /// <summary>
        /// The name of the promotion (e.g.Summer Sale). *Either this field or id must be set.
        /// </summary>
        /// <value>The name.</value>
        public string Name { set; get; }

        /// <summary>
        /// The creative associated with the promotion (e.g.summer_banner2).
        /// </summary>
        /// <value>The creative.</value>
        public string Creative { set; get; }

        /// <summary>
        /// The position of the creative (e.g.banner_slot_1).
        /// </summary>
        /// <value>The position.</value>
        public string Position { set; get; }
    }
}
