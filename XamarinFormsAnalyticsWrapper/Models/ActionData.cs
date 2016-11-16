using System;

namespace XamarinFormsAnalyticsWrapper.Models
{
    public class ActionData
    {
        /// <summary>
        /// The transaction ID (e.g.T1234). *Required if the action type is purchase or refund.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The store or affiliation from which this transaction occurred (e.g.Google Store).
        /// </summary>
        /// <value>The affiliation.</value>
        public string Affiliation { get; set; }

        /// <summary>
        /// Specifies the total revenue or grand total associated with the transaction (e.g. 11.99). This value may include shipping, tax costs, or other adjustments to total revenue that you want to include as part of your revenue calculations. Note: if revenue is not set, its value will be automatically calculated using the product quantity and price fields of all products in the same hit.
        /// </summary>
        /// <value>The revenue.</value>
        public double Revenue { get; set; }

        /// <summary>
        /// The total tax associated with the transaction.
        /// </summary>
        /// <value>The tax.</value>
        public double Tax { get; set; }

        /// <summary>
        /// The shipping cost associated with the transaction.
        /// </summary>
        /// <value>The shipping.</value>
        public double Shipping { get; set; }

        /// <summary>
        /// The transaction coupon redeemed with the transaction.
        /// </summary>
        /// <value>The coupon.</value>
        public string Coupon { get; set; }

        /// <summary>
        /// The list that the associated products belong to. Optional.
        /// </summary>
        /// <value>The list.</value>
        public string List { get; set; }

        /// <summary>
        /// A number representing a step in the checkout process. Optional on checkout actions.
        /// </summary>
        /// <value>The step.</value>
        public int Step { get; set; }

        /// <summary>
        /// Additional field for checkout and checkout_option actions that can describe option information on the checkout page, like selected payment method.
        /// </summary>
        /// <value>The option.</value>
        public string Option { get; set; }
    }
}
