using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using OpcUa.Client.Core;

namespace OpcUA.Client
{
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }

    public class DemoChartViewModel : BaseViewModel
    {
        private double _trend;

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public bool IsReading { get; set; }
        public double AxisMax { get; set; }
        public double AxisMin { get; set; }

        public ICommand StopCommand { get; set; }

        public DemoChartViewModel(DataContext dataContext)
        {
            StopCommand = new RelayCommand(InjectStopOnClick);

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues = new ChartValues<MeasureModel>();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);

            //The next code simulates data changes every 300 ms
            IsReading = false;
        }

        private void InjectStopOnClick()
        {
            IsReading = !IsReading;
            if (IsReading) Task.Factory.StartNew(Read);
        }


        private void Read()
        {
            var r = new Random();

            while (IsReading)
            {
                Thread.Sleep(150);
                var now = DateTime.Now;

                _trend += r.Next(-8, 10);

                ChartValues.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = _trend
                });

                SetAxisLimits(now);

                //lets only use the last 150 values
                if (ChartValues.Count > 150) ChartValues.RemoveAt(0);
            }
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
        }

    }
}
