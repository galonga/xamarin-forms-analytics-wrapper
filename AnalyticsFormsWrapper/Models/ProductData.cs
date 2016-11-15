using System;

namespace AnalyticsFormsWrapper.Models
{
    public class ProductData
    {
        /// <summary>
        /// The product ID or SKU (e.g. P67890). *Either this field or name must be set.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { set; get; }

        /// <summary>
        /// The name of the product (e.g.Android T-Shirt). *Either this field or id must be set.
        /// </summary>
        /// <value>The name.</value>
        public string Name { set; get; }

        /// <summary>
        /// The brand associated with the product (e.g.Google).
        /// </summary>
        /// <value>The brand.</value>
        public string Brand { set; get; }

        /// <summary>
        /// The category to which the product belongs (e.g.Apparel). Use / as a delimiter to specify up to 5-levels of hierarchy (e.g.Apparel/Men/T-Shirts).
        /// </summary>
        /// <value>The category.</value>
        public string Category { set; get; }

        /// <summary>
        /// The variant of the product (e.g. Black).
        /// </summary>
        /// <value>The variant.</value>
        public string Variant { set; get; }

        /// <summary>
        /// The price of a product (e.g. 29.20).
        /// </summary>
        /// <value>The price.</value>
        public double Price { set; get; }

        /// <summary>
        /// The quantity of a product (e.g. 2).
        /// </summary>
        /// <value>The quantity.</value>
        public int Quantity { set; get; }

        /// <summary>
        /// The coupon code associated with a product (e.g. SUMMER_SALE13).
        /// </summary>
        /// <value>The coupon.</value>
        public string Coupon { set; get; }

        /// <summary>
        /// The product's position in a list or collection (e.g. 2).
        /// </summary>
        public int Position { set; get; }
    }
}
