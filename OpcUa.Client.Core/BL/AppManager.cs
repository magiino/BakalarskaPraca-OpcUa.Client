using System;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;

namespace OpcUa.Client.Core
{
    public class AppManager
    {
        public int ProjectId { get; set; }
        public bool IsSaved { get; set; }
        public Action CloseAction { get; set; }

        public AppManager()
        {
            var dateTimePointCfg = Mappers.Xy<DateTimePoint>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);

            // Set up mapper for charts globally
            Charting.For<DateTimePoint>(dateTimePointCfg);

            var measureModelCfg = Mappers.Xy<MeasureModel>()
            .X(model => model.DateTime.Ticks)
            .Y(model => model.Value);
            Charting.For<MeasureModel>(measureModelCfg);


        }

        public void CloseApplication()
        {
            IoC.DisposeAll();
            CloseAction();
        }

        // TODO z tadeto vypisovat okná, aspon message dialogy
    }
}
