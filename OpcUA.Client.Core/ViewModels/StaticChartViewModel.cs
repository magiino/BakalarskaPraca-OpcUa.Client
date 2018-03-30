using System;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using OpcUa.Client.Core;

namespace OpcUA.Client
{
    public class StaticChartViewModel : BaseViewModel
    {
        private readonly DataContext _dataContext;

        public SeriesCollection Series { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public double AxisMax { get; set; }
        public double AxisMin { get; set; }

        public StaticChartViewModel(DataContext dataContext)
        {
            _dataContext = dataContext;

            var config = Mappers.Xy<DateTimePoint>()
                .X(model => (double)model.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);                                                 //use the value property as Y

            Series = new SeriesCollection(config);

            LoadData();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;
        }


        private void LoadData()
        {
            var data = new LineSeries()
            {
                Title = "test",
                PointGeometrySize = 10,
                Values = new ChartValues<DateTimePoint>()
            };
            /*
            var variableModel = _dataContext.Variables.FirstOrDefault(x => x.Id == 1);
            if (variableModel == null) return;
            foreach (var record in variableModel.Records)
            {
                var type = TypeInfo.GetSystemType(variableModel.DataType, -1);
                var value = (Int16) Convert.ChangeType(record.Value, type);
                data.Values.Add(new ChartModel<Int16>
                {
                    DateTime = record.ArchiveTime,
                    Value = value
                });
            }
            Series.Add(data);
            
            SetAxisLimits(variableModel.Records.Last().ArchiveTime, variableModel.Records.First().ArchiveTime);
            */
        }

        private void SetAxisLimits(DateTime firstRecord, DateTime lastRecord)
        {
            AxisMax = lastRecord.Ticks; // lets force the axis to be 1 second ahead
            AxisMin = firstRecord.Ticks; // and 8 seconds behind
        }
    }
}
