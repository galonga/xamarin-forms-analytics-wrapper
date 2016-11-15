using System;
namespace AnalyticsFormsWrapper.Services
{
    public interface IAnalyticsService
    {
        bool OptOut { set; get; }
        double DispatchPeriod { set; }
        string ClientId { get; }
        bool EnableAdvertisingFeatures { set; }
        void ManualDispatch ();
    }
}
