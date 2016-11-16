using System;
using XamarinFormsAnalyticsWrapper.Models;
using Foundation;
using Google.Analytics;

namespace XamarinFormsAnalyticsWrapper.iOS.Mappper
{
    public class AnalyticsProductMapper
    {
        public EcommerceProduct mapProduct (ProductData p)
        {
            var product = new EcommerceProduct();

            if (p.Brand != null) {
                product.SetBrand (p.Brand);
            } else {
                product.SetBrand ("none");
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
                product.SetPosition (new NSNumber (p.Position));
            }
            if (p.Price != 0) {
                product.SetPrice (new NSNumber (p.Price));
            }
            if (p.Quantity != 0) {
                product.SetQuantity (new NSNumber (p.Quantity));
            }
            if (p.Variant != null) {
                product.SetVariant (p.Variant);
            }

            return product;
        }
    }
}
