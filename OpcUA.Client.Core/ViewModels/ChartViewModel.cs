using System;
using System.Linq;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using Opc.Ua;
using OpcUA.Client.Core;

namespace OpcUA.Client
{
    public class ChartViewModel : BaseViewModel
    {
        private readonly DataContext _dataContext;
        public ChartValues<DateTimePoint> Values { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public long AxisMin { get; set; }
        public long AxisMax { get; set; }


        public ICommand StopCommand { get; set; }

        public ChartViewModel(DataContext dataContext)
        {
            _dataContext = dataContext;
            var mapper = Mappers.Xy<DateTimePoint>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            Charting.For<DateTimePoint>(mapper);
            //the values property will store our values array
            Values = new ChartValues<DateTimePoint>();

            Read();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("dd MMM H:mm:ss");
        }

        private void Read()
        {
            var variable = _dataContext.Variables.FirstOrDefault(x => x.Id == 1);
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
            AxisMax = lastRecord.Ticks + TimeSpan.FromMinutes(5).Ticks;
            AxisMin = firstRecord.Ticks - TimeSpan.FromMinutes(5).Ticks;
        }
    }
}
