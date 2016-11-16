using System;

namespace XamarinFormsAnalyticsWrapper.Enums
{
    public enum ProductActions
    {
        none = 0,
        /// <summary>
        /// A click on a product or product link for one or more products.
        /// </summary>
        click = 1,
        /// <summary>
        /// A view of product details.
        /// </summary>
        detail = 2,
        /// <summary>
        /// Adding one or more products to a shopping cart.
        /// </summary>
        add = 3,
        /// <summary>
        /// Remove one or more products from a shopping cart.
        /// </summary>
        remove = 4,
        /// <summary>
        /// Initiating the checkout process for one or more products.
        /// </summary>
        checkout = 5,
        /// <summary>
        /// Sending the option value for a given checkout step.
        /// </summary>
        checkout_option = 6,
        /// <summary>
        /// The sale of one or more products.
        /// </summary>
        purchase = 6,
        /// <summary>
        /// The refund of one or more products.
        /// </summary>
        refund = 7,
        /// <summary>
        /// A click on an internal promotion.
        /// </summary>
        promo_click = 8,
    }
}
