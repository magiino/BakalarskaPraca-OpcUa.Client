using System;
using System.Linq;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class ChartViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private DateTime _lastTime;

        public ChartValues<DateTimePoint> Values { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public long AxisMin { get; set; }
        public long AxisMax { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }


        public ICommand StopCommand { get; set; }

        public ChartViewModel(IUnitOfWork IunitOfWork)
        {
            _unitOfWork = IunitOfWork;

            var mapper = Mappers.Xy<DateTimePoint>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            Charting.For<DateTimePoint>(mapper);
            //the values property will store our values array
            Values = new ChartValues<DateTimePoint>();

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromHours(3).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            //AxisUnit = TimeSpan.TicksPerSecond;

            Read();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("dd MMM H:mm:ss");
        }

        private void Read()
        {
            var variable = _unitOfWork.Variables.SingleOrDefault(x => x.Id == 1);
            if (variable == null) return;
            foreach (var record in variable.Records)
            {
                var type = TypeInfo.GetSystemType(variable.DataType, -1);
                var value = (short)Convert.ChangeType(record.Value, type);
                Values.Add(new DateTimePoint
                {
                    DateTime = record.ArchiveTime,
                    Value = value
                });
            }

            SetAxisLimits(variable.Records.First().ArchiveTime, variable.Records.Last().ArchiveTime);
        }

        private void SetAxisLimits(DateTime firstRecord, DateTime lastRecord)
        {
            AxisMax = lastRecord.Ticks + TimeSpan.FromMinutes(1).Ticks;
            AxisMin = firstRecord.Ticks - TimeSpan.FromMinutes(2).Ticks;
            _lastTime = lastRecord;
        }
        private void MaxAxisLimit(DateTime newrRecord)
        {
            AxisMax = newrRecord.Ticks + TimeSpan.FromHours(2).Ticks;
            _lastTime = newrRecord;
        }
    }
}
