using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Opc.Ua;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class ZoomChartCodeBehind : UserControl
    {
        private ZoomingOptions _zoomingMode;

        public ZoomChartCodeBehind()
        {
            InitializeComponent();

            var gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(33, 148, 241), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = GetData(),
                    Fill = Brushes.Transparent,
                    StrokeThickness = 1,
                    PointGeometrySize = 15,
                    
                }
            };

            ZoomingMode = ZoomingOptions.X;

            XFormatter = value => new DateTime((long)value).ToString("dd MMM H:mm:ss");
            YFormatter = val => val.ToString("C");

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public ZoomingOptions ZoomingMode
        {
            get { return _zoomingMode; }
            set
            {
                _zoomingMode = value;
                OnPropertyChanged();
            }
        }

        private void ToogleZoomingMode(object sender, RoutedEventArgs e)
        {
            switch (ZoomingMode)
            {
                case ZoomingOptions.None:
                    ZoomingMode = ZoomingOptions.X;
                    break;
                case ZoomingOptions.X:
                    ZoomingMode = ZoomingOptions.Y;
                    break;
                case ZoomingOptions.Y:
                    ZoomingMode = ZoomingOptions.Xy;
                    break;
                case ZoomingOptions.Xy:
                    ZoomingMode = ZoomingOptions.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ChartValues<DateTimePoint> GetData()
        {
            var values = new ChartValues<DateTimePoint>();

            var variable = IoC.UnitOfWork.Variables.SingleOrDefault(x => x.Id == 1);
            if (variable == null) return null;
            foreach (var record in variable.Records)
            {
                var type = TypeInfo.GetSystemType(variable.DataType, -1);
                var value = (short)Convert.ChangeType(record.Value, type);
                values.Add(new DateTimePoint
                {
                    DateTime = record.ArchiveTime,
                    Value = value,  
                });
            }

            return values;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            X.MinValue = double.NaN;
            X.MaxValue = double.NaN;
            Y.MinValue = double.NaN;
            Y.MaxValue = double.NaN;
        }
    }
}