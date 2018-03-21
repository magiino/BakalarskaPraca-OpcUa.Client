using System;
using System.Linq;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class StaticChartViewModel : BaseViewModel
    {
        private readonly DataContext _dataContext;

        public ChartValues<ChartModel<Int16>> Values { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public bool IsReading { get; set; }
        public double AxisMax { get; set; }
        public double AxisMin { get; set; }

        public StaticChartViewModel(DataContext dataContext)
        {
            _dataContext = dataContext;

            //the values property will store our values array
            Values = new ChartValues<ChartModel<Int16>>();
            Read();

            var mapper = Mappers.Xy<ChartModel<Int16>>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<ChartModel<Int16>>(mapper);

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(30).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            //AxisUnit = TimeSpan.TicksPerSecond;

            IsReading = false;
        }


        private void Read()
        {
            var r = new Random();

            var variable = _dataContext.Variables.FirstOrDefault(x => x.Id == 1);
            if (variable != null)
                foreach (var record in variable.Records)
                {
                    var type = TypeInfo.GetSystemType(variable.DataType, -1);
                    var value = (Int16)Convert.ChangeType(record.Value, type);
                    Values.Add(new ChartModel<Int16>
                    {
                        DateTime = record.ArchiveTime,
                        Value = value
                    });
                }


            SetAxisLimits(variable.Records.First().ArchiveTime, variable.Records.Last().ArchiveTime);

            /*
            while (IsReading)
            {
                Thread.Sleep(150);
                var now = DateTime.Now;

                _trend += r.Next(-8, 10);

                Values.Add(new ChartModel<Int16>
                {
                    DateTime = now,
                    Value = 5
                });

                SetAxisLimits(now);

                //lets only use the last 150 values
                if (ChartValues.Count > 150) ChartValues.RemoveAt(0);
            }
            */
        }

        private void SetAxisLimits(DateTime firstRecord, DateTime lastRecord)
        {
            AxisMax = firstRecord.Ticks; // lets force the axis to be 1 second ahead
            AxisMin = lastRecord.Ticks; // and 8 seconds behind
        }

    }
}
