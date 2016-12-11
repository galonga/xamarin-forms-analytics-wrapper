using GoogleAnalytics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamarinFormsAnalyticsWrapper.Models;

namespace XamarinFormsAnalyticsWrapper.UWP.Mapper
{
    class AnalyticsProductMapper
    {
        const long inMicrosMultiply = 1000000;

        public TransactionItem mapProduct(ProductData p, string transactionid, string currencyCode)
        {
            var product = new TransactionItem();

            if (p.Category != null)
                product.Category = p.Category;

            if (p.Id != null)
                product.SKU = p.Id;

            if (p.Name != null)
                product.Name = p.Name;

            if (p.Price != 0)
                product.PriceInMicros = Convert.ToInt64(p.Price * inMicrosMultiply);

            if (p.Quantity != 0)
                product.Quantity = Convert.ToInt64(p.Quantity);

            if (!string.IsNullOrEmpty(transactionid))
                product.TransactionId = transactionid;

            if (!string.IsNullOrEmpty(currencyCode))
                product.CurrencyCode = currencyCode;
                        
            return product;
        }
    }
}
