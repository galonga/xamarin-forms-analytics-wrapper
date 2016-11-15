using System;
using System.Collections.Generic;
using AnalyticsFormsWrapper.Enums;
using AnalyticsFormsWrapper.Models;

namespace AnalyticsFormsWrapper.Services
{
    public interface IAnalyticsService
    {
        bool OptOut { set; get; }
        double DispatchPeriod { set; }
        string ClientId { get; }
        bool EnableAdvertisingFeatures { set; }
        void ManualDispatch ();
        void TrackScreen (string screenName, string campaignUrl, List<ProductData> products, ProductActions productAction, ActionData actionData, PromotionData promotion, CustomDimension customDimension, CustomMetric customMetric);
        void TrackEvent (EventData eventData, CustomDimension customDimension, CustomMetric customMetric);
        void TrackException (string ExceptionMessage, bool isFatalException);
        void TrackSocial (string socialNetworkName, string socialAction, string socialTarget);
        void TrackTiming (string categroy, string label, long value, string variable);
        void SetCurrencyCode (string currencyCode);
        void TrackUserId (string userid);
    }
}
