using System;
using XamarinFormsAnalyticsWrapper.Models;
using Android.Gms.Analytics.Ecommerce;

namespace AnalyticsFormsWrapper.Droid.Mapper
{
    public class AnalyticsProductMapper
    {
        public Product mapProduct (ProductData p)
        {
            var product = new Product();

            if (p.Brand != null) {
                product.SetBrand (p.Brand);
            }
            if (p.Category != null) {
                product.SetCategory (p.Category);
            }
            if (p.Coupon != null) {
                product.SetCouponCode (p.Coupon);
            }
            if (p.Id != null) {
                product.SetId (p.Id);
            }
            if (p.Name != null) {
                product.SetName (p.Name);
            }
            if (p.Position != 0) {
                product.SetPosition (p.Position);
            }
            if (p.Price != 0) {
                product.SetPrice (p.Price);
            }
            if (p.Quantity != 0) {
                product.SetQuantity (p.Quantity);
            }
            if (p.Variant != null) {
                product.SetVariant (p.Variant);
            }

            return product;
        }
    }
}
