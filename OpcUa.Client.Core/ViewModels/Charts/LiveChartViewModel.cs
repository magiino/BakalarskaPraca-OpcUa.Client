using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUa.Client.Core
{
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }

    public class LiveChartViewModel : BaseViewModel
    {
        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _selectedNode;
        private readonly Subscription _subscription;
        // TODO spravit globalny
        private readonly CartesianMapper<MeasureModel> _configuration = Mappers.Xy<MeasureModel>()
            .X(model => model.DateTime.Ticks)
            .Y(model => model.Value);

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public ObservableCollection<VariableLiveChartModel> Variables { get; set; } = new ObservableCollection<VariableLiveChartModel>();
        public VariableLiveChartModel SelectedVariable { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public double AxisMax { get; set; }
        public double AxisMin { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public LiveChartViewModel(UaClientApi uaClientApi)
        {
            _uaClientApi = uaClientApi;
            _subscription = _uaClientApi.Subscribe(2000, "LiveCharts");

            AddCommand = new RelayCommand(AddVariable);
            RemoveCommand = new RelayCommand(RemoveVariable);

            // Nastavenia grafu
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");
            AxisStep = TimeSpan.FromSeconds(10).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;
            SetAxisLimits(DateTime.Now);

            MessengerInstance.Register<SendSelectedRefNode>(
                this,
                msg => _selectedNode = msg.ReferenceNode);
        }

        private void AddVariable()
        {
            var r = new Random();
            var variable = new VariableLiveChartModel()
            {
                Name = _selectedNode.DisplayName.ToString(),
                NodeId = _selectedNode.NodeId.ToString()
            };

            SeriesCollection.Add(
                new LineSeries(_configuration)
                {
                    Title = variable.Name,
                    Values = new ChartValues<MeasureModel>(),
                    //{
                    //    new MeasureModel()
                    //    {
                    //        Value = 5,
                    //        DateTime = DateTime.Now,
                    //    },
                    //    new MeasureModel()
                    //    {
                    //        Value = 7,
                    //        DateTime = DateTime.Now.AddSeconds(5),
                    //    },
                    //    new MeasureModel()
                    //    {
                    //        Value = 10,
                    //        DateTime = DateTime.Now.AddSeconds(10),
                    //    }

                    //},
                    PointGeometrySize = 15,
                    PointGeometry = DefaultGeometries.Cross,
                    //PointForeground = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255),
                    //    (byte)r.Next(1, 255), (byte)r.Next(1, 233))),
                    //LineSmoothness = 1,
                    //StrokeThickness = 3,
                    //Stroke = Brushes.Aqua,
                    //Fill = Brushes.Transparent
                }
            );

            var item = _uaClientApi.NotificationMonitoredItem(variable.Name, variable.NodeId, null, 4);
            _uaClientApi.AddMonitoredItem(item, _subscription);
            item.Notification += Notification_MonitoredItem;

            Variables.Add(variable);
        }

        private void RemoveVariable()
        {
            SeriesCollection.RemoveAt(Variables.IndexOf(SelectedVariable));
            Variables.Remove(SelectedVariable);
            SelectedVariable = null;
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(10).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(120).Ticks; // and 8 seconds behind
        }

        #region Can use methods

        public bool AddNotificationCanUse()
        {
            if (_selectedNode == null)
                return false;
            else if (_selectedNode.NodeClass != NodeClass.Variable)
                return false;
            else return true;
        }

        public bool RemoveNotificationCanUse()
        {
            return SelectedVariable != null;
        }

        #endregion

        #region CallBack Methods

        /// <summary>
        /// Callback method for updating values of subscibed nodes
        /// </summary>
        /// <param name="monitoredItem"></param>
        /// <param name="e"></param>
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (!(e.NotificationValue is MonitoredItemNotification notification))
                return;


            var value = notification.Value;

            var variable = Variables.SingleOrDefault(x => x.Name == monitoredItem.DisplayName);
           
            if (variable == null) return;

            DateTime time = value.SourceTimestamp;
            if (value.SourceTimestamp < DateTime.Now)
                time = DateTime.Now;

            // OSetrit ak niekto da stringovu premennu
            var index = Variables.IndexOf(variable);
            SeriesCollection[index].Values.Add(new MeasureModel()
            {
                Value = Convert.ToDouble(value.Value),
                DateTime = time,
            });

            SetAxisLimits(DateTime.Now);

            if (SeriesCollection[index].Values.Count > 200)
                SeriesCollection[index].Values.RemoveAt(0);
        }

        #endregion
    }
}
