using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class LiveChartViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _selectedNode;
        private readonly Subscription _subscription;
        #endregion

        #region Public Properties
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public ObservableCollection<VariableLiveChartModel> Variables { get; set; } = new ObservableCollection<VariableLiveChartModel>();
        public VariableLiveChartModel SelectedVariable { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public double AxisMax { get; set; }
        public double AxisMin { get; set; }
        #endregion

        #region Commands
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        #endregion

        #region Constructor
        public LiveChartViewModel(UaClientApi uaClientApi, Messenger messenger)
        {
            _uaClientApi = uaClientApi;

            _subscription = _uaClientApi.Subscribe(2000, "LiveCharts");

            AddCommand = new MixRelayCommand(AddVariable);
            RemoveCommand = new MixRelayCommand(RemoveVariable);

            // Nastavenia grafu
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");
            AxisStep = TimeSpan.FromSeconds(10).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;
            SetAxisLimits(DateTime.Now);

            messenger.Register<SendSelectedRefNode>(msg => _selectedNode = msg.ReferenceNode);
        }
        #endregion

        #region Command Methods
        private void AddVariable(object parameter)
        {
            if (Variables.Count > 4) return;

            var r = new Random();
            Brush brush = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255),
                (byte)r.Next(1, 255), (byte)r.Next(1, 233)));
            var color = new BrushConverter().ConvertToString(brush);

            var variable = new VariableLiveChartModel()
            {
                Color = color,
                Name = _selectedNode.DisplayName.ToString(),
                NodeId = _selectedNode.NodeId.ToString()
            };

            SeriesCollection.Add(
                new LineSeries()
                {
                    Title = variable.Name,
                    Values = new ChartValues<DateTimePoint>(),
                    PointGeometrySize = 15,
                    PointGeometry = DefaultGeometries.Circle,
                    Fill = Brushes.Transparent,
                    PointForeground = Brushes.White,
                    Stroke = brush,
                    StrokeThickness = 4
                }
            );

            var item = _uaClientApi.CreateMonitoredItem(variable.Name, variable.NodeId, 500, null, 4);
            _uaClientApi.AddMonitoredItem(item, _subscription);
            item.Notification += Notification_MonitoredItem;

            Variables.Add(variable);
        }

        private void RemoveVariable(object parameter)
        {
            SeriesCollection.RemoveAt(Variables.IndexOf(SelectedVariable));
            Variables.Remove(SelectedVariable);
            SelectedVariable = null;
        } 
        #endregion

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

        #region Private Methods
        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(2).Ticks;
            AxisMin = now.Ticks - TimeSpan.FromSeconds(60).Ticks;
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
            SeriesCollection[index].Values.Add(new DateTimePoint()
            {
                Value = Convert.ToDouble(value.Value),
                DateTime = time,
            });

            SetAxisLimits(DateTime.Now);

            if (SeriesCollection[index].Values.Count > 100)
                SeriesCollection[index].Values.RemoveAt(0);
        }
        #endregion
    }
}